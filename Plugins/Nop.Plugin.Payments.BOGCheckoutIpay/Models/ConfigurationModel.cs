using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BOGCheckoutIpay.Fields.AuthorizationUrl")]
        public string AuthorizationUrl { get; set; }
        public bool AuthorizationUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BOGCheckoutIpay.Fields.CheckoutUrl")]
        public string CheckoutUrl { get; set; }
        public bool CheckoutUrl_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BOGCheckoutIpay.Fields.ClientId")]
        public string ClientId { get; set; }
        public bool ClientId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BOGCheckoutIpay.Fields.SecretKey")]
        public string SecretKey { get; set; }
        public bool SecretKey_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.BOGCheckoutIpay.Fields.RequestBodyInJson")]
        public string RequestBodyInJson { get; set; }
        public bool RequestBodyInJson_OverrideForStore { get; set; }
    }
}
