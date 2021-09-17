using Sitecore.Data.Eventing.Remote;
using Sitecore.Data.Items;
using System.Runtime.Serialization;

namespace RadioPollResult.Eventing 
{

    [DataContract]
    public class CustomFlushPartialCacheRemoteEvent : ItemRemoteEventBase
    {
        public CustomFlushPartialCacheRemoteEvent(Item item) : base(item)
        {
        }
    }
}
