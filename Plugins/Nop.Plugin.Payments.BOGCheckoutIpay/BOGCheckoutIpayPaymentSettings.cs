using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Payments.BOGCheckoutIpay
{
    public class BOGCheckoutIpayPaymentSettings : ISettings
    {
        public string AuthorizationUrl { get; set; }
        public string CheckoutUrl { get; set; }
        public string ClientId { get; set; }
        public string SecretKey { get; set; }
        public string RequestBodyInJson { get; set; }
        public bool AdditionalFeePercentage { get; set; }
        public decimal AdditionalFee { get; set; }
    }
}
