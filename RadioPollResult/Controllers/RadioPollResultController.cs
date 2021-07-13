using RadioPollResult.Models;
using RadioPollResult.Services;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace RadioPollResult.Controllers
{
    public class RadioPollResultController : SitecoreController
    {

        public ActionResult PollResult()
        {
            var currentItem = GetDatasourceItem();

            var model = new RadioPollResultModel();
            model.Title = currentItem.Fields["Title"]?.Value;
            model.Text = currentItem.Fields["Text"]?.Value;

            Item radioField = ((Sitecore.Data.Fields.InternalLinkField)currentItem.Fields["Radio Field"])?.TargetItem;

            if (radioField != null)
            {
                var FormItem = GetFormItem(radioField);
                if (FormItem != null) {
                    var sqlpoll = new SqlPoll();
                    model.Result = sqlpoll.GetAggregatedPollValues(FormItem.ID.Guid, radioField.ID.Guid);
                }
            }

            return View("~/Views/RadioPollResult/RadioPollResult.cshtml", model);
        }

        public Item GetDatasourceItem()
        {
            var datasourceId = RenderingContext.Current.Rendering.DataSource;

            return ID.IsID(datasourceId) ? Context.Database.GetItem(datasourceId) : null;
        }


        private Item GetFormItem(Item field)
        {
            return field.Axes.GetAncestors().FirstOrDefault(x => x.TemplateName == "Form");
        }

    }
}