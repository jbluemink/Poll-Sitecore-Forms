using Sitecore.Abstractions;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Extensions;
using Sitecore.Web;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RadioPollResult.Eventing
{
    public class PartialHtmlCacheClearSingleItem
    {
        private readonly BaseSiteContextFactory _siteContextFactory;

        public PartialHtmlCacheClearSingleItem(BaseSiteContextFactory siteContextFactory)
        {
            Assert.ArgumentNotNull((object)siteContextFactory, nameof(siteContextFactory));
            this._siteContextFactory = siteContextFactory;
        }

        public void OnItemEvent(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));
            Guid itemId = (Guid)Sitecore.Events.Event.ExtractParameter(args, 0);
            Flush(new ID(itemId));
        }

        private void Flush(ID itemId)
        {
            var sites = GetSitesWithPartialHtml();
            foreach (SiteInfo site in sites)
            {
                var partialcache = Sitecore.Caching.CacheManager.FindCacheByName<string>(site.Name + "[partial html]");
                if (partialcache != null)
                {
                    foreach (var key in partialcache.GetCacheKeys())
                    {
                        if (key.Contains(itemId.ToString()))
                        {
                            partialcache.Remove(key);
                        }
                    }
                }
            }
        }

        private IEnumerable<SiteInfo> GetSitesWithPartialHtml()
        {
            return this._siteContextFactory.GetSites().Where<SiteInfo>(x => x.CacheHtml && x.EnablePartialHtmlCaheClear);
        }
    }
}