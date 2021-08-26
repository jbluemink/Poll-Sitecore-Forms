using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Extensions;
using Sitecore.Globalization;
using Sitecore.Publishing;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RadioPollResult.Eventing
{
    /// <summary>PublishEndRemoteEvent class.</summary>
    [DataContract]
    public class FormFlushCacheRemoteEvent
    {

        public FormFlushCacheRemoteEvent(string message)
        {        
            this.Message = message; 
        }
       
        [DataMember]
        public string Message { get; protected set; }

    }
}
