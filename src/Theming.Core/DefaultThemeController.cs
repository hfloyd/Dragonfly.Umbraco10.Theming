namespace Dragonfly.UmbracoTheming
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco.Extensions;

    /// <summary>
    /// Find the theme setting and retrieve the appropriate view
    /// </summary>
    public class DefaultThemeController : RenderController
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _AppSettingsConfig;
        private readonly ThemeHelperService _ThemeHelperService;        

        public DefaultThemeController(
            ILogger<DefaultThemeController> logger,
            IConfiguration AppSettingsConfig,
            ThemeHelperService themeHelperService,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor)
        : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _logger = logger;
            _AppSettingsConfig = AppSettingsConfig;
            _ThemeHelperService = themeHelperService;
        }

        // GET: Default
        public IActionResult Index(ContentModel model)
        {
            var themeProp = _AppSettingsConfig["Dragonfly.Theming.ThemePropertyAlias"];

            if (!string.IsNullOrEmpty(themeProp))
            {
                var currentTemplateName = model.Content.GetTemplateAlias();
                var rootNode = model.Content.AncestorOrSelf(1);
                if (rootNode != null)
                {
                    var siteTheme = rootNode.Value<string>(themeProp);
                    if (string.IsNullOrEmpty(siteTheme))
                    {
                        _logger.LogWarning($"Node '{rootNode.Name}' does not have a value for Theme picker property '{themeProp}'.");
                        return base.CurrentTemplate(model);
                    }
                    else
                    {
                        var templatePath =
                            _ThemeHelperService.GetFinalThemePath(siteTheme, Theming.PathType.View, currentTemplateName);
                        return View(templatePath, model);
                    }
                }
                else
                {
                    _logger.LogWarning($"DefaultThemeController: Root Node is NULL for node #{model.Content.Id}");
                    return base.CurrentTemplate(model);
                }
            }
            else
            {
                _logger.LogWarning($"Web.config AppSetting 'Dragonfly.Theming.ThemePropertyAlias' is not set.");
                return base.CurrentTemplate(model);
            }
        }
    }
}
