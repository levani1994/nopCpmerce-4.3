using System;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Models
{
    public class RequestBodyModel
    {
        public string Locale { get; set; }
        public Guid OrderId { get; set; }
        public decimal AmountValue { get; set; }
    }
}
