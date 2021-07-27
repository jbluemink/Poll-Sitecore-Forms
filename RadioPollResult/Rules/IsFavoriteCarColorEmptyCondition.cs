using System.Linq;
using Sitecore.Analytics;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;
 
namespace RadioPollResult.Rules
{
    public class IsFavoriteCarColorEmptyCondition<T> : WhenCondition<T> where T : RuleContext
    {

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.IsNotNull(Tracker.Current, "Tracker.Current is not initialized");
            Assert.IsNotNull(Tracker.Current.Session, "Tracker.Current.Session is not initialized");
            Assert.IsNotNull(Tracker.Current.Session.Interaction, "Tracker.Current.Session.Interaction is not initialized");

            //get value from xDB
            string userFaforiteCarColor = string.Empty;
            var xConnectFacets = Tracker.Current.Contact.GetFacet<Sitecore.Analytics.XConnect.Facets.IXConnectFacets>("XConnectFacets");
            if (xConnectFacets.Facets != null)
            {
                if (xConnectFacets.Facets.Where(f => f.Key == Preferences.DefaultFacetKey).FirstOrDefault().Value is Preferences preferenceFacet)
                {
                    userFaforiteCarColor = preferenceFacet.FavoritCarColor;
                }
            }
            return string.IsNullOrEmpty(userFaforiteCarColor);
        }
    }
}