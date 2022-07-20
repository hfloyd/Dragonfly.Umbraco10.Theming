namespace Dragonfly.UmbracoTheming
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco.Extensions;

    /// <summary>
    /// Find the theme setting and retrieve the appropriate view
    /// </summary>
    public class DefaultThemeController : RenderController
    {
        private readonly ILogger _logger;
        private readonly ThemeHelperService _ThemeHelperService;

        public DefaultThemeController(
            ILogger<DefaultThemeController> logger,
            ThemeHelperService themeHelperService,
            ICompositeViewEngine compositeViewEngine,
            IUmbracoContextAccessor umbracoContextAccessor)
            : base(logger, compositeViewEngine, umbracoContextAccessor)
        {
            _logger = logger;
            _ThemeHelperService = themeHelperService;
        }

        // GET: Default
        public override IActionResult Index()
        {
            var themeProp = _ThemeHelperService.ThemePropertyAlias();

            if (!string.IsNullOrEmpty(themeProp))
            {
                var currentTemplateName = CurrentPage.GetTemplateAlias();

                //Set logic to get the node which has the Theme Picker on it
                var rootNode = CurrentPage.AncestorOrSelf(1);
                if (rootNode != null)
                {
                    var siteTheme = rootNode.Value<string>(themeProp);
                    if (string.IsNullOrEmpty(siteTheme))
                    {
                        _logger.LogWarning($"Node '{rootNode.Name}' does not have a value for Theme picker property '{themeProp}'.");
                        return base.CurrentTemplate(CurrentPage);
                    }
                    else
                    {
                        var templatePath =
                            _ThemeHelperService.GetFinalThemePath(siteTheme, Theming.PathType.View, currentTemplateName);
                        return View(templatePath, CurrentPage);
                    }
                }
                else
                {
                    _logger.LogWarning($"DefaultThemeController: Root Node is NULL for node #{CurrentPage.Id}");
                    return base.CurrentTemplate(CurrentPage);
                }
            }
            else
            {
                _logger.LogWarning($"AppSetting 'DragonflyTheming.ThemePickerPropertyAlias' is not set.");
                return base.CurrentTemplate(CurrentPage);
            }
        }
    }
}
