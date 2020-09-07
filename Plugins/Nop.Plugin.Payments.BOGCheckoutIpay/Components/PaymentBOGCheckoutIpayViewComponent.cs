using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Components
{
    [ViewComponent(Name = "PaymentBOGCheckoutIpay")]
    public class PaymentBOGCheckoutIpayViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.BOGCheckoutIpay/Views/PaymentInfo.cshtml");
        }
    }
}
