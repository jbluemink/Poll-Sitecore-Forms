using Sitecore.Abstractions;
using Sitecore.Caching;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
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
            Item item = (Item)Sitecore.Events.Event.ExtractParameter(args, 0);
            if (item != null)
            {
                Flush(item);
            }
        }

        private void Flush(Item item)
        {
            var sites = GetRelevantSites(item.Database.Name);
            foreach (SiteInfo site in sites)
            {
                var partialcache = Sitecore.Caching.CacheManager.FindCacheByName<string>(site.Name + "[partial html]");
                if (partialcache != null) {
                    foreach (var key in partialcache.GetCacheKeys())
                    {
                        if (key.Contains(item.ID.ToString()))
                        {
                            partialcache.Remove(key);
                        }
                    }
                }
            }
        }

        private IEnumerable<SiteInfo> GetRelevantSites(string databaseName) => this._siteContextFactory.GetSites().Where<SiteInfo>((Func<SiteInfo, bool>)(site => site.CacheHtml && site.EnablePartialHtmlCaheClear && site.Database == databaseName));

    }
}