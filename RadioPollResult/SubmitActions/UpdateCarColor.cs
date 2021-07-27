using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;
using Sitecore.Diagnostics;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;

namespace RadioPollResult.SubmitActions
{
    public class UpdateCarColor : SubmitActionBase<string>
    {

        public UpdateCarColor(ISubmitActionData submitActionData) : base(submitActionData)
        {
        }
        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }

        /// <summary>
        /// Gets the current tracker.
        /// </summary>
        protected virtual ITracker CurrentTracker => Tracker.Current;

        /// <summary>
        /// Executes the action with the specified <paramref name="data" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="formSubmitContext">The form submit context.</param>
        /// <returns><c>true</c> if the action is executed correctly; otherwise <c>false</c></returns>
        protected override bool Execute(string data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(data, nameof(data));
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));

            var colorField = GetFieldById(new Guid("{AF30BD1C-BDFA-4B89-8767-52DC667758FC}"), formSubmitContext.Fields);

            if (colorField == null)
            {
                return false;
            }
            //  https://doc.sitecore.com/en/developers/101/sitecore-experience-platform/set-contact-facets-in-session.html


            SetColorInformation(GetValue(colorField));
            return true;

        }

        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <returns>The <see cref="IXdbContext"/> instance.</returns>
        protected virtual IXdbContext CreateClient()
        {
            return SitecoreXConnectClientConfiguration.GetClient();
        }

        /// <summary>
        /// Gets the field by <paramref name="id" />.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>The field with the specified <paramref name="id" />.</returns>
        private static IViewModel GetFieldById(Guid id, IList<IViewModel> fields)
        {
            return fields.FirstOrDefault(f => Guid.Parse(f.ItemId) == id);
        }

        /// <summary>
        /// Gets the <paramref name="field" /> value.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>The field value.</returns>
        private static string GetValue(object field)
        {
            var valueObject = field?.GetType().GetProperty("Value")?.GetValue(field, null);
            if (valueObject == null)
            {
                return string.Empty;
            }
            if (valueObject is IList list)
            {
                var resultlist = new List<string>();
                foreach (object obj in list)
                {
                    resultlist.Add(obj.ToString());
                }
                return string.Join(",", resultlist);
            }
            return field?.GetType().GetProperty("Value")?.GetValue(field, null)?.ToString() ?? string.Empty;
        }



        public void SetColorInformation(string color)
        {
            var manager = Sitecore.Configuration.Factory.CreateObject("tracking/contactManager", true) as Sitecore.Analytics.Tracking.ContactManager;

            if (Sitecore.Analytics.Tracker.Current.Contact.IsNew || !Sitecore.Analytics.Tracker.Current.Contact.Identifiers.Any())
            {
                SetColorInformationNewContact(color);
            }
            else
            {
                var anyIdentifier = Sitecore.Analytics.Tracker.Current.Contact.Identifiers.FirstOrDefault();

                // Get contact from xConnect, update and save the facet
                using (XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
                {
                    try
                    {
                        var expandOptions = new ContactExpandOptions(RadioPollResult.Preferences.DefaultFacetKey);

                        //Sitecore 10+
                        var executeOptions = new ContactExecutionOptions(expandOptions);

                        var contact = client.Get<Contact>(new IdentifiedContactReference(anyIdentifier.Source, anyIdentifier.Identifier), executeOptions);

                        if (contact != null)
                        {
                            var preferences = contact.GetFacet<Preferences>(Preferences.DefaultFacetKey);
                            if (preferences != null)
                            {
                                preferences.FavoritCarColor = color;

                                client.SetFacet<Preferences>(contact, Preferences.DefaultFacetKey, preferences);
                            }
                            else
                            {
                                client.SetFacet<Preferences>(contact, Preferences.DefaultFacetKey, new Preferences(color));
                            }

                            client.Submit();

                            // Remove contact data from shared session state - contact will be re-loaded
                            // during subsequent request with updated facets
                            manager.RemoveFromSession(Sitecore.Analytics.Tracker.Current.Contact.ContactId);
                            Sitecore.Analytics.Tracker.Current.Session.Contact = manager.LoadContact(Sitecore.Analytics.Tracker.Current.Contact.ContactId);
                        }
                    }
                    catch (XdbExecutionException ex)
                    {
                        // Manage conflicts / exceptions
                    }
                }
            }
        }

        public void SetColorInformationNewContact(string color)
        {
            if (Sitecore.Analytics.Tracker.Current.Contact.IsNew)
            {
                var manager = Sitecore.Configuration.Factory.CreateObject("tracking/contactManager", true) as Sitecore.Analytics.Tracking.ContactManager;

                if (manager != null)
                {
                    // Save contact to xConnect; at this point, a contact has an anonymous
                    // TRACKER IDENTIFIER, which follows a specific format. Do not use the contactId overload
                    // and make sure you set the ContactSaveMode as demonstrated
                    Sitecore.Analytics.Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;
                    manager.SaveContactToCollectionDb(Sitecore.Analytics.Tracker.Current.Contact);

                    // Now that the contact is saved, you can retrieve it using the tracker identifier
                    // NOTE: Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource is marked internal in 9.0 Initial and cannot be used. If you are using 9.0 Initial, pass "xDB.Tracker" in as a string.
                    var trackerIdentifier = new IdentifiedContactReference(Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource, Sitecore.Analytics.Tracker.Current.Contact.ContactId.ToString("N"));

                    // Get contact from xConnect, update and save the facet
                    using (XConnectClient client = Sitecore.XConnect.Client.Configuration.SitecoreXConnectClientConfiguration.GetClient())
                    {
                        try
                        {
                            var expandOptions = new ContactExpandOptions(RadioPollResult.Preferences.DefaultFacetKey);

                            //Sitecore 10+
                            var executeOptions = new ContactExecutionOptions(expandOptions);

                            var contact = client.Get<Contact>(trackerIdentifier, executeOptions);

                            if (contact != null)
                            {
                                var preferences = contact.GetFacet<Preferences>(Preferences.DefaultFacetKey);
                                if (preferences != null)
                                {
                                    preferences.FavoritCarColor = color;
                                    client.SetFacet<Preferences>(contact, Preferences.DefaultFacetKey, preferences);
                                }
                                else
                                {
                                    client.SetFacet<Preferences>(contact, Preferences.DefaultFacetKey, new Preferences(color));
                                }

                                client.Submit();

                                // Remove contact data from shared session state - contact will be re-loaded
                                // during subsequent request with updated facets
                                manager.RemoveFromSession(Sitecore.Analytics.Tracker.Current.Contact.ContactId);
                                Sitecore.Analytics.Tracker.Current.Session.Contact = manager.LoadContact(Sitecore.Analytics.Tracker.Current.Contact.ContactId);
                            }
                        }
                        catch (XdbExecutionException ex)
                        {
                            // Manage conflicts / exceptions
                        }
                    }
                }
            }
        }  
    }
}

      