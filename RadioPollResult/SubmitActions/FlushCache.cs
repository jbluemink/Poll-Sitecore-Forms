using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Eventing;
using Sitecore.Eventing.Remote;
using Sitecore.ExperienceForms.Models;
using Sitecore.ExperienceForms.Processing;
using Sitecore.ExperienceForms.Processing.Actions;
using Sitecore.Publishing;
using System;
using Sitecore.Caching;

namespace RadioPollResult.SubmitActions
{
    public class FlushCache : SubmitActionBase<string>
    {

        private IEventQueue defaultQueue;
        public FlushCache(ISubmitActionData submitActionData) : base(submitActionData)
        {
            this.defaultQueue = (IEventQueue)ServiceLocator.ServiceProvider.GetService<BaseEventQueueProvider>();
        }
        protected override bool TryParse(string value, out string target)
        {
            target = string.Empty;
            return true;
        }

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

            //clear Html cache (only current site, if used on other sites, adjust this)
            CacheManager.GetHtmlCache(Sitecore.Context.Site).Clear();
            //raise a (fake) publish end remote event, to clear cache other servers..
            PublishOptions publishoption = new PublishOptions(Sitecore.Context.Database, Sitecore.Context.Database, PublishMode.SingleItem, Sitecore.Context.Language, DateTime.Now);
            var publisher = new Publisher(publishoption);
            defaultQueue.QueueEvent<PublishEndRemoteEvent>(new PublishEndRemoteEvent(publisher));
            return true;

        }
      
    }
}

      