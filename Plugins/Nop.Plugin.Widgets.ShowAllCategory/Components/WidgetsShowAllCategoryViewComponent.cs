using System;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Widgets.ShowAllCategory;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.GoogleAnalytics.Components
{
    [ViewComponent(Name = "WidgetsShowAllCategory")]
    public class WidgetsShowAllCategoryViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
        
           
            return View("~/Plugins/Widgets.GoogleAnalytics/Views/PublicInfo.cshtml");
        }
    }
}