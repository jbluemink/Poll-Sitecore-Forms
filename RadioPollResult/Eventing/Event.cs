using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Eventing;
using Sitecore.Events;
using Sitecore.Pipelines;
using System;

namespace RadioPollResult.Eventing
{
    public class Event
    {
        public void Process(PipelineArgs args)
        {
            Initialize();
        }
        public void Initialize()
        {
            var action = new Action<CustomFlushPartialCacheRemoteEvent>(RaiseRemoteEvent);
            EventManager.Subscribe<CustomFlushPartialCacheRemoteEvent>(action);

        }
        private static void RaiseRemoteEvent(CustomFlushPartialCacheRemoteEvent @event)
        {
            Assert.ArgumentNotNull((object)@event, nameof(@event));
            //at this point there is no Context, but most likely, it is a remote event, we can get the item from web database
            Item obj = Database.GetDatabase("web").GetItem(new ID(@event.ItemId));
            Sitecore.Events.Event.RaiseEvent("custom:flushpartialcache:remote", obj);
        }
    }
}