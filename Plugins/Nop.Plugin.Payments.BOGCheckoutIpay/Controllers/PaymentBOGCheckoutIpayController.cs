using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.BOGCheckoutIpay.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using ILogger = Nop.Services.Logging.ILogger;

namespace Nop.Plugin.Payments.BOGCheckoutIpay.Controllers
{

    [Area(AreaNames.Admin)]
    public class PaymentBOGCheckoutIpayController : BasePaymentController
    {
        #region Fields

        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly OrderSettings _orderSettings;
        private readonly IOrderProcessingService _orderProcessingService;

        #endregion

        #region Ctor

        public PaymentBOGCheckoutIpayController(
            ISettingService settingService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            ILogger logger,
            IOrderService orderService,
            OrderSettings orderSettings,
            IOrderProcessingService orderProcessingService,
            INotificationService notificationService
            )
        {
            _settingService = settingService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _notificationService = notificationService;
            _logger = logger;
            _orderService = orderService;
            _orderSettings = orderSettings;
            _orderProcessingService = orderProcessingService;
        }

        #endregion

        #region Actions
        [AuthorizeAdmin]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;

            var bogCheckoutIpayPaymentSettings = _settingService.LoadSetting<BOGCheckoutIpayPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                AuthorizationUrl = bogCheckoutIpayPaymentSettings.AuthorizationUrl,
                CheckoutUrl = bogCheckoutIpayPaymentSettings.CheckoutUrl,
                ClientId = bogCheckoutIpayPaymentSettings.ClientId,
                SecretKey = bogCheckoutIpayPaymentSettings.SecretKey,
                RequestBodyInJson = bogCheckoutIpayPaymentSettings.RequestBodyInJson
            };

            return View("/Plugins/Payments.BOGCheckoutIpay/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [HttpPost]
        [AdminAntiForgery]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var bogCheckOutPaymentIpaySettings = _settingService.LoadSetting<BOGCheckoutIpayPaymentSettings>(storeScope);

            //save settings
            bogCheckOutPaymentIpaySettings.AuthorizationUrl = model.AuthorizationUrl;
            bogCheckOutPaymentIpaySettings.CheckoutUrl = model.CheckoutUrl;
            bogCheckOutPaymentIpaySettings.ClientId = model.ClientId;
            bogCheckOutPaymentIpaySettings.SecretKey = model.SecretKey;
            bogCheckOutPaymentIpaySettings.RequestBodyInJson = model.RequestBodyInJson;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(bogCheckOutPaymentIpaySettings, x => x.AuthorizationUrl, model.AuthorizationUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(bogCheckOutPaymentIpaySettings, x => x.CheckoutUrl, model.CheckoutUrl_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(bogCheckOutPaymentIpaySettings, x => x.ClientId, model.ClientId_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(bogCheckOutPaymentIpaySettings, x => x.SecretKey, model.SecretKey_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(bogCheckOutPaymentIpaySettings, x => x.RequestBodyInJson, model.RequestBodyInJson_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Complete(CheckoutCompleteModel checkout)
        {
            _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, "Parameters from IPay on callback", $"{"orderId= " + checkout.order_id + " " + "paymentHash= " + checkout.payment_hash + " " + "ipaypaymentId= " + checkout.ipay_payment_id + " " + "status= " + checkout.status + " " + "statusDescription= " + checkout.status_description + " " + "shopOrderId= " + checkout.shop_order_id + " " + "paymentMethod= " + checkout.payment_method + " " + "cardType= " + checkout.card_type + " " + "transactionId= " + checkout.transaction_id + " " + "pan= " + checkout.pan}", null);

            //load order by identifier (if provided)
            var order = _orderService.GetOrderByGuid(checkout.shop_order_id);

            //disable "order completed" page?
            if (_orderSettings.DisableOrderCompletedPage)
            {
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });
            }

            //mark order as paid in case of success
            if (checkout.status.Equals(BOGCheckoutIpayConstants.OrderResponse.SUCCESS_STATUS, StringComparison.InvariantCultureIgnoreCase))
            {
                _orderProcessingService.MarkOrderAsPaid(order);
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Information, "Payment was placed successfuly", $"{"orderId= " + checkout.order_id + " " + "paymentHash= " + checkout.payment_hash + " " + "ipaypaymentId= " + checkout.ipay_payment_id + " " + "status= " + checkout.status + " " + "statusDescription= " + checkout.status_description + " " + "shopOrderId= " + checkout.shop_order_id + " " + "paymentMethod= " + checkout.payment_method + " " + "cardType= " + checkout.card_type + " " + "transactionId= " + checkout.transaction_id + " " + "pan= " + checkout.pan}", null);
            }
            else if (checkout.status.Equals(BOGCheckoutIpayConstants.OrderResponse.ERROR_STATUS, StringComparison.InvariantCultureIgnoreCase))
            {
                _logger.InsertLog(Nop.Core.Domain.Logging.LogLevel.Error, "Error during ipay payment", $"{"orderId= " + checkout.order_id + " " + "paymentHash= " + checkout.payment_hash + " " + "ipaypaymentId= " + checkout.ipay_payment_id + " " + "status= " + checkout.status + " " + "statusDescription= " + checkout.status_description + " " + "shopOrderId= " + checkout.shop_order_id + " " + "paymentMethod= " + checkout.payment_method + " " + "cardType= " + checkout.card_type + " " + "transactionId= " + checkout.transaction_id + " " + "pan= " + checkout.pan}", null);
            }

            return Ok();
        }

        public ActionResult PaymentInfo()
        {
            return View("~/Plugins/Payments.BOGCheckoutIpay/Views/PaymentInfo.cshtml");
        }

        #endregion
    }
}
