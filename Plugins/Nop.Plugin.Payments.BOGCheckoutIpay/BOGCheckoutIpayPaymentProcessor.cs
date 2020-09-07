using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.BOGCheckoutIpay.Controllers;
using Nop.Plugin.Payments.BOGCheckoutIpay.Extensions;
using Nop.Plugin.Payments.BOGCheckoutIpay.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;


namespace Nop.Plugin.Payments.BOGCheckoutIpay
{
    public class BOGCheckoutIpayPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly BOGCheckoutIpayPaymentSettings _bogCheckoutIpayPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ILocalizationService _localizationService;
        private readonly IPaymentService _paymentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public BOGCheckoutIpayPaymentProcessor(BOGCheckoutIpayPaymentSettings bogCheckoutIpayPaymentSettings,
             ISettingService settingService,
             IOrderTotalCalculationService orderTotalCalculationService,
             ILocalizationService localizationService,
             IPaymentService paymentService,
             IHttpContextAccessor httpContextAccessor,
             IWebHelper webHelper,
             IWorkContext workContext,
             ILogger logger)
        {
            _bogCheckoutIpayPaymentSettings = bogCheckoutIpayPaymentSettings;
            _settingService = settingService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _localizationService = localizationService;
            _paymentService = paymentService;
            _httpContextAccessor = httpContextAccessor;
            _webHelper = webHelper;
            _workContext = workContext;
            _logger = logger;
        }

        #endregion

        #region Method

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //let's ensure that at least 1 minute passed after order is placed
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return false;

            return true;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var result = _paymentService.CalculateAdditionalFee(cart,
                _bogCheckoutIpayPaymentSettings.AdditionalFee, _bogCheckoutIpayPaymentSettings.AdditionalFeePercentage);

            return result;
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentBOGCheckoutIpay";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Payments.BOGCheckoutIpay.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentBOGCheckoutIpayController);
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentBOGCheckoutIpay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.BOGCheckoutIpay.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Hide Payment Method
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>Hide Payment Method</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //let's ensure that at least 1 minute passed after order is placed
            //if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
            return false;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            try
            {
                var token = GetAuthorizationToken(_bogCheckoutIpayPaymentSettings);

                var orderResponse = GetOrderUrl(token.access_token, postProcessPaymentRequest, _bogCheckoutIpayPaymentSettings);

                if (orderResponse.status.Equals(BOGCheckoutIpayConstants.OrderResponse.CREATED_STATUS, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (var item in orderResponse.links)
                    {
                        if (item.rel.Equals(BOGCheckoutIpayConstants.OrderResponse.LINKS_APPROVED_STATUS, StringComparison.InvariantCultureIgnoreCase))
                            _httpContextAccessor.HttpContext.Response.Redirect(item.href);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, $"Error with processing the payment with order - {postProcessPaymentRequest.Order.OrderGuid}", ex.Message, null);
                _httpContextAccessor.HttpContext.Response.Redirect($"{_webHelper.GetStoreLocation()}/checkout/failed");
            }
        }

        /// <summary>
        /// Get the Authorization token from Ipay
        /// </summary>
        /// <param name="settings"></param>
        /// <returns></returns>
        public TokenResponseJsonModel GetAuthorizationToken(BOGCheckoutIpayPaymentSettings settings)
        {
            RestClient client = new RestClient(settings.AuthorizationUrl)
            {
                Authenticator = new HttpBasicAuthenticator(settings.ClientId, settings.SecretKey),
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("grant_type", "client_credentials");
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);

            //return token if success
            if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<TokenResponseJsonModel>(response.Content);
            else
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, $"Unable to get token from Ipay: {response.StatusCode}", response.Content, null);

            return null;
        }

        /// <summary>
        /// Get an Ipay checkout url using token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="postProcessPaymentRequest"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        public OrderResponseJsonModel GetOrderUrl(string token, PostProcessPaymentRequest postProcessPaymentRequest, BOGCheckoutIpayPaymentSettings settings)
        {
            var requestBodyModel = new RequestBodyModel
            {
                Locale = _workContext.WorkingLanguage.UniqueSeoCode,
                OrderId = postProcessPaymentRequest.Order.OrderGuid,
                AmountValue = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2)// todo change this
            };

            // replace values from config property RequestBodyInJson
            string checkoutRequestData = TemplateParser<RequestBodyModel>.GetParsedTemplate(requestBodyModel,
                                                                                            _bogCheckoutIpayPaymentSettings.RequestBodyInJson);

            var client = new RestClient(settings.CheckoutUrl)
            {
                Timeout = -1
            };

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer " + token);
            request.AddParameter("application/json,application/json", checkoutRequestData, ParameterType.RequestBody);

            _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, $"Getting Ipay order url with params...", checkoutRequestData, null);

            var response = client.Execute(request);

            if (response.IsSuccessful && response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, $"Order url details with id: {postProcessPaymentRequest.Order.OrderGuid}", response.Content, null);
                return JsonConvert.DeserializeObject<OrderResponseJsonModel>(response.Content);
            }
            else
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, $"Unable to get order details from Ipay: {response.StatusCode}", response.Content, null);

            return null;
        }

        public override void Install()
        {
            //settings
            var settings = new BOGCheckoutIpayPaymentSettings()
            {
                AuthorizationUrl = "https://ipay.ge/opay/api/v1/oauth2/token",
                CheckoutUrl = "https://ipay.ge/opay/api/v1/checkout/orders"
            };

            _settingService.SaveSetting(settings);

            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payment.BOGCheckoutIpay.Fields.CertPath", "Cert Path");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payment.BOGCheckoutIpay.Fields.RedirectionTip", "You will be redirected to BOG checkout site to complete the order.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.Payment.BOGCheckoutIpay.PaymentMethodDescription", "Pay by BOG Ipay");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<BOGCheckoutIpayPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.Payment.BOGCheckoutIpay.Fields.UseSandbox");
            _localizationService.DeletePluginLocaleResource("Plugins.Payment.BOGCheckoutIpay.Fields.RedirectionTip");
            _localizationService.DeletePluginLocaleResource("Plugins.Payment.BOGCheckoutIpay.PaymentMethodDescription");

            base.Uninstall();
        }

        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            return new List<string>();
        }

        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        {
            return new ProcessPaymentRequest();
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentBogCheckoutIpay/Configure";
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentBOGCheckoutIpay";
        }

        #endregion

        #region Properies
        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Skip is supported
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public string PaymentMethodDescription
        {
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to BOG checkout site to complete the payment"
            get { return _localizationService.GetResource("Plugins.Payment.BOGCheckoutIpay.PaymentMethodDescription"); }
        }
        #endregion
    }
}
