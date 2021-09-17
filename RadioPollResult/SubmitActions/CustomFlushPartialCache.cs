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
using RadioPollResult.Eventing;

namespace RadioPollResult.SubmitActions
{
    public class CustomFlushPartialCache : SubmitActionBase<FlushPartialHtmlCacheData>
    {

        private IEventQueue defaultQueue;
        public CustomFlushPartialCache(ISubmitActionData submitActionData) : base(submitActionData)
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

            Item item = Context.Database.GetItem(new ID(data.ReferenceId.Value));
           
            Sitecore.Events.Event.RaiseEvent("custom:flushpartialcache", (object)item);

            //raise a remote event, to clear cache other servers.
            defaultQueue.QueueEvent<CustomFlushPartialCacheRemoteEvent>(new CustomFlushPartialCacheRemoteEvent(item), true, false);
            return true;

        }

      
    }
}

      