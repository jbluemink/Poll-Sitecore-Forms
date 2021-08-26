using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Eventing;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using System;
using Sitecore.Data.Items;
using Sitecore.Data;
using Sitecore;
using Sitecore.Data.Eventing.Remote;

namespace RadioPollResult.SubmitActions
{
    public class FlushPartialHtmlCache : SubmitActionBase<FlushPartialHtmlCacheData>
    {

        private IEventQueue defaultQueue;
        public FlushPartialHtmlCache(ISubmitActionData submitActionData) : base(submitActionData)
        {
            this.defaultQueue = (IEventQueue)ServiceLocator.ServiceProvider.GetService<BaseEventQueueProvider>();
        }

        /// <summary>
        /// Executes the action with the specified <paramref name="data" />.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="formSubmitContext">The form submit context.</param>
        /// <returns><c>true</c> if the action is executed correctly; otherwise <c>false</c></returns>
        protected override bool Execute(FlushPartialHtmlCacheData data, FormSubmitContext formSubmitContext)
        {
            Assert.ArgumentNotNull(data, nameof(data));
            Assert.ArgumentNotNull(formSubmitContext, nameof(formSubmitContext));

            if (data.ReferenceId == null || data.ReferenceId == Guid.Empty)
            {
                return false;
            }

            Sitecore.Data.Items.Item obj = Context.Database.GetItem(new ID(data.ReferenceId.Value));
            ItemChanges changes = new ItemChanges(obj);
            //optional do dummy change
            //Sitecore.Data.Fields.Field dummychange = obj.Fields[0];
            //changes.SetFieldValue(dummychange, dummychange.Value);

            Sitecore.Events.Event.RaiseEvent("item:saved", (object)obj, (object)changes);

            //raise a (fake) item saved remote event, to clear cache other servers.
            defaultQueue.QueueEvent<SavedItemRemoteEvent>(new SavedItemRemoteEvent(obj, changes), true, false);
            return true;

        }

      
    }
}

      