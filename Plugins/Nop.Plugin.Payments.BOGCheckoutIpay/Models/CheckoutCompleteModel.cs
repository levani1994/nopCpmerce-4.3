using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Models
{
    public class CheckoutCompleteModel
    {
        public string order_id { get; set; }
        public string payment_hash { get; set; }
        public int ipay_payment_id { get; set; }
        public string status { get; set; }
        public string status_description { get; set; }
        public Guid shop_order_id { get; set; }
        public string payment_method { get; set; }
        public string card_type { get; set; }
        public string transaction_id { get; set; }
        public string pan { get; set; }
    }
}
