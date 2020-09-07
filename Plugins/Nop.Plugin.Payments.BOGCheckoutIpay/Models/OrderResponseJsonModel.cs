using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Models
{
    public class OrderResponseJsonModel
    {
        public string payment_hash { get; set; }
        public string status { get; set; }
        public List<OrderResponseLinks> links { get; set; }

    }

    public class OrderResponseLinks
    {
        public string rel { get; set; }
        public string href { get; set; }
    }
}
