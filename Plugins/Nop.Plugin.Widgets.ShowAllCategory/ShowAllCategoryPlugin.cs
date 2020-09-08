using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.ShowAllCategory
{
    public class ShowAllCategoryPlugin : BasePlugin, IWidgetPlugin
    {
        public bool HideInWidgetList => false;

        #region fields
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        #endregion

        #region Ctor
        public ShowAllCategoryPlugin(ILocalizationService localizationService,
        IWebHelper webHelper,
        ISettingService settingService)
        {
            _localizationService = localizationService;
            _webHelper = webHelper;
            _settingService = settingService;
        }
        #endregion

        #region methods
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsShowAllCategory";
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HomepageTop };
        }

        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsShowAllCategory/Configure";
        }

        public override void Install()
        {
            // custom logic like adding settings, locale resources and database table(s) here

            base.Install();
        }

        public override void Uninstall()
        {
            // custom logic like removing settings, locale resources and database table(s) which was created during widget installation
            _settingService.DeleteSetting<ShowAllCategorySettings>();

            base.Uninstall();
        }

        #endregion
    }
}
