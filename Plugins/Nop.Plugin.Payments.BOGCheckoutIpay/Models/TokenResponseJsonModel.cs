using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Models
{
    public class TokenResponseJsonModel
    {
        public string access_token { get; set; }
    }
}
