using Microsoft.AspNetCore.Mvc;
using Nop.Services.Security;
using Nop.Plugin.Widgets.ShowAllCategory.Models;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Media;
using Nop.Services.Configuration;
using Nop.Core;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.ShowAllCategory.Controllers
{
    class WidgetsShowAllCategoriesController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsShowAllCategoriesController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var myWidgetSettings = _settingService.LoadSetting<ShowAllCategorySettings>(storeScope);

            var model = new ConfigurationModel
            {
                // configuration model settings here
            };

            if (storeScope > 0)
            {
                // override settings here based on store scope
            }

            return View("~/Plugins/Widgets.MyFirstNopWidget/Views/Configure.cshtml", model);
        }
    }
}
