using Sitecore.Diagnostics;
using Sitecore.Eventing;
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
            Sitecore.Events.Event.RaiseEvent("custom:flushpartialcache:remote", @event.ItemId);
        }
    }
}