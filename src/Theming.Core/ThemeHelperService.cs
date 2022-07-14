namespace Dragonfly.UmbracoTheming
{
    using System;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Extensions;


    public class ThemeHelperService
    {
        private readonly ILogger<ThemeHelperService> _logger;
        private readonly IConfiguration _AppSettingsConfig;
        private readonly IWebHostEnvironment _HostingEnvironment;

        /// <summary>
        /// Cached instance of configuration
        /// </summary>
        private ThemeConfig _config;


        //private readonly ThemeConfigurator _ThemeConfigurator;        
        public ThemeHelperService(
            ILogger<ThemeHelperService> logger,
            IConfiguration AppSettingsConfig,
            IWebHostEnvironment HostingEnvironment
        )
        {
            _logger = logger;
            _AppSettingsConfig = AppSettingsConfig;
            _HostingEnvironment = HostingEnvironment;
        }

        #region Public Properties/Methods

        public string ThemePropertyAlias()
        {
            var themeProp = _AppSettingsConfig["Dragonfly.Theming.ThemePropertyAlias"];

            if (!string.IsNullOrEmpty(themeProp))
            {
                return themeProp;
            }
            else
            {
                return "Theme"; //default
            }
        }

        /// <summary>
        /// Looks for the Theme property specified on the Site root node (Ancestor at Level 1)
        /// </summary>
        /// <param name="CurrentPage"></param>
        /// <returns></returns>
        public string GetSiteThemeName(IPublishedContent CurrentPage)
        {
            var themeProp = ThemePropertyAlias();

            if (!string.IsNullOrEmpty(themeProp))
            {
                return CurrentPage.AncestorOrSelf(1).Value<string>(themeProp);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Get the Configuration for the specified Theme
        /// </summary>
        /// <param name="ThemeName"></param>
        /// <returns></returns>
        public ThemeConfig GetThemeConfig(string ThemeName)
        {
            string path = GetConfigFilePath(ThemeName);
            var lastModified = System.IO.File.GetLastWriteTime(path);

            if (_config.ThemeName != ThemeName ||
                (_config.ThemeName == ThemeName && lastModified > _config.ConfigTimestamp))
            {
                SetThemeConfig(ThemeName);
            }

            return _config;
        }

        /// <summary>
        /// Determine if the Theme has a themed Umbraco Forms file
        /// </summary>
        /// <param name="SiteThemeName">Theme name</param>
        /// <param name="ViewPath">Final part of view path - ex: 'Form.cshtml' or 'Fieldtypes/FieldType.Text.cshtml'</param>
        /// <returns>True if file found</returns>
        public bool HasUmbracoFormsThemeFile(string SiteThemeName, string ViewPath = "")
        {
            var baseThemePath = string.Format("~/Themes/{0}", SiteThemeName);
            var themePath = string.Format("{0}/Views/Partials/Forms/Themes/default/{1}", baseThemePath, ViewPath);

            if (System.IO.File.Exists(_HostingEnvironment.MapPathWebRoot(themePath)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the final path to the requested type, based on the theme and file existence.
        /// </summary>
        /// <param name="SiteThemeName">Theme Name</param>
        /// <param name="PathType">Type of Path to return</param>
        /// <param name="FileName">Name of the View (without extension) or name of the config (with extension) (optional)</param>
        /// <param name="AlternateStandardPath">If the non-themed path is not standard, provide the full path here (optional)</param>
        /// <returns></returns>
        public string GetFinalThemePath(string SiteThemeName, Theming.PathType PathType, string FileName = "", string AlternateStandardPath = "")
        {
            if (SiteThemeName.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("Missing SiteThemeName parameter: No theme has been set for this website root, republish the root with a selected theme.");
            }

            var finalPath = "";
            var standardPath = "";
            var themePath = "";
            var isFolder = false;

            var baseThemePath = string.Format("~/Themes/{0}", SiteThemeName);

            switch (PathType)
            {
                case Theming.PathType.ThemeRoot:
                    standardPath = themePath;
                    themePath = string.Format("{0}/", baseThemePath);
                    isFolder = true;
                    break;

                case Theming.PathType.Configs:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Themes/~DefaultConfigs/{0}", FileName);
                    themePath = string.Format("{0}/Configs/{1}", baseThemePath, FileName);
                    break;

                case Theming.PathType.View:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/{0}.cshtml", FileName);
                    themePath = string.Format("{0}/Views/{1}.cshtml", baseThemePath, FileName);
                    break;

                case Theming.PathType.PartialView:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/Partials/{0}.cshtml", FileName);
                    themePath = string.Format("{0}/Views/Partials/{1}.cshtml", baseThemePath, FileName);
                    break;

                case Theming.PathType.FormsPartialsRoot:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/Partials/Forms/Themes/default/");
                    themePath = string.Format("{0}/Views/Partials/Forms/Themes/default/", baseThemePath);
                    isFolder = true;
                    break;

                case Theming.PathType.GridEditor:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/Partials/Grid/Editors/{0}.cshtml", FileName);
                    themePath = string.Format("{0}/Views/Partials/Grid/Editors/{1}.cshtml", baseThemePath, FileName);
                    break;

                default:
                    break;
            }

            if (isFolder & System.IO.Directory.Exists(_HostingEnvironment.MapPathWebRoot(themePath)))
            {
                finalPath = themePath;
            }
            else if (!isFolder & System.IO.File.Exists(_HostingEnvironment.MapPathWebRoot(themePath)))
            {
                finalPath = themePath;
            }
            else
            {
                finalPath = standardPath;
            }

            return finalPath;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.ThemeRoot
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <returns></returns>
        public string GetThemePath(string SiteThemeName)
        {
            var path = GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
            return path;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.View
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public string GetThemeViewPath(string SiteThemeName, string ViewName)
        {
            var path = GetFinalThemePath(SiteThemeName, Theming.PathType.View, ViewName);
            return path;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.Configs
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="ConfigFileName"></param>
        /// <returns></returns>
        public string GetThemedConfigFilePath(string SiteThemeName, string ConfigFileName)
        {
            var path = GetFinalThemePath(SiteThemeName, Theming.PathType.Configs, ConfigFileName);
            return path;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.PartialView
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="ViewName"></param>
        /// <returns></returns>
        public string GetThemePartialViewPath(string SiteThemeName, string ViewName)
        {
            var path = GetFinalThemePath(SiteThemeName, Theming.PathType.PartialView, ViewName);
            return path;
        }

        public string GetCssOverridePath(string CssOverrideFileName)
        {
            if (CssOverrideFileName.IsNullOrWhiteSpace())
            {
                return "";
            }
            else
            {
                var path = "/Themes/~CssOverrides/{0}";
                return string.Format(path, CssOverrideFileName);
            }

        }

        #endregion

        #region Private Methods

        private void SetThemeConfig(string SiteThemeName)
        {
            string path = GetConfigFilePath(SiteThemeName);
            var lastModified = System.IO.File.GetLastWriteTime(path);

            if (_config.ThemeName != SiteThemeName ||
                (_config.ThemeName == SiteThemeName && lastModified > _config.ConfigTimestamp))
            {
                // If the file is not there => Create with defaults
                CreateThemeConfigFileIfNotExists(SiteThemeName);

                XmlSerializer serializer = new XmlSerializer(typeof(ThemeConfig));

                // Read from file
                var newConfig = new ThemeConfig();
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        newConfig = (ThemeConfig)serializer.Deserialize(reader)!;
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException(
                        "The format of 'Theme.config' is invalid. Details: " + ex.Message, ex.InnerException);
                }

                newConfig.ThemeName = SiteThemeName;
                newConfig.ConfigPath = path;
                newConfig.ConfigTimestamp = lastModified;

                _config = newConfig;
            }

        }
        private void CreateThemeConfigFileIfNotExists(string SiteThemeName)
        {
            // Create from configuration file
            string path = GetConfigFilePath(SiteThemeName);

            // If it is not there yet, serialize our defaults to file
            if (!File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ThemeConfig));

                using (StreamWriter file = new StreamWriter(path))
                {
                    serializer.Serialize(file, DefaultConfig);
                }
            }
        }

        private string GetConfigFilePath(string SiteThemeName)
        {
            var themeRoot = GetThemePath(SiteThemeName);
            var configPath = themeRoot + "Theme.config";
            return _HostingEnvironment.MapPathWebRoot(configPath);
        }


        private readonly ThemeConfig DefaultConfig = new ThemeConfig
        {
            Author = "Unspecified Author",
            AuthorUrl = "https://unknown",
            SourceUrl = "https://unknown",
            CssFramework = "Unspecified Framework",
            GridRenderer = "Bootstrap3",
            Description = "Theme description..."
        };

        #endregion

    }

    public static class ThemingExtensions
    {
        #region Url Helpers

        /// <summary>
        /// Returns the url of a themed asset
        /// <example>In a View:
        /// <code>@Url.ThemedAsset(Model, "images/favicon.ico")</code>
        /// <em>NOTE: requires '@using ClientDependency.Core.Mvc'</em>
        /// </example>
        /// </summary>
        /// <param name="Url">UrlHelper (@Url.)</param>
        /// <param name="SiteThemeName"></param>
        /// <param name="RelativeAssetPath">Path to file inside [theme]/Assets/ folder</param>
        /// <returns></returns>
        public static string ThemedAsset(this UrlHelper Url, ThemeHelperService ThemeHelper, string SiteThemeName, string RelativeAssetPath)
        {
            var themeRoot = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
            //var absolutePath = VirtualPathUtility.ToAbsolute(themeRoot).EnsureEndsWith('/');
            var absolutePath = themeRoot.EnsureEndsWith('/');
            var virtualPath = absolutePath + "Assets/" + RelativeAssetPath;
            return virtualPath;
        }

        #endregion

        #region HTML Helpers

        /// <summary>
        /// Renders a partial view in the current theme
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SiteThemeName"></param>
        /// <param name="PartialName"></param>
        /// <param name="ViewModel"></param>
        /// <param name="ViewData"></param>
        /// <returns></returns>
        public static HtmlString ThemedPartial(this HtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName, string PartialName, object ViewModel, ViewDataDictionary ViewData = null)
        {
            //try
            //{
            var path = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.PartialView, PartialName);
            var htmlString = html.Partial(path, ViewModel, ViewData).ToHtmlString();
            return new HtmlString(htmlString);
            //}
            //catch (Exception ex)
            //{
            //    var msgTemplate = "ThemeHelper: Error rendering partial view '{PartialName}'";
            //    Current.Logger.Error(typeof(IHtmlString),ex, msgTemplate, PartialName);
            //    throw ex;
            //    return new HtmlString($"<span class=\"theme-error-msg\">Error rendering partial view '{PartialName}' - {ex.Message}</span>");
            //}
        }

        /// <summary>
        /// Renders a partial view in the current theme
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SiteThemeName"></param>
        /// <param name="PartialName"></param>
        /// <param name="ViewData"></param>
        /// <returns></returns>
        public static HtmlString ThemedPartial(this HtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName, string PartialName, ViewDataDictionary ViewData = null)
        {
            if (ViewData == null)
            {
                ViewData = html.ViewData;
            }
            //try
            //{
            var path = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.PartialView, PartialName);
            var htmlString = html.Partial(path, ViewData).ToHtmlString();
            return new HtmlString(htmlString);
            //}
            //catch (Exception ex)
            //{
            //    var msgTemplate = "ThemeHelper: Error rendering partial view '{PartialName}'";
            //    Current.Logger.Error(typeof(IHtmlString), ex, msgTemplate, PartialName);
            //    throw ex;
            //    return new HtmlString($"<span class=\"theme-error-msg\">Error rendering partial view '{PartialName}' - {ex.Message}</span>");
            //}

        }

        #endregion

        #region Smidge

        //public static HtmlHelper RequiresThemedCss(this HtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName, string FilePath)
        //{
        //    var themeRoot = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
        //    return html.RequiresCss(themeRoot + "Assets/css" + FilePath.EnsureStartsWith('/'));
        //}

        //public static HtmlHelper RequiresThemedJs(this HtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName, string FilePath)
        //{
        //    var themeRoot = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
        //    return html.RequiresJs(themeRoot + "Assets/js" + FilePath.EnsureStartsWith('/'));
        //}

        //public static HtmlHelper RequiresThemedCssFolder(this HtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName)
        //{
        //    var themeRoot = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
        //    return html.RequiresCssFolder(themeRoot + "Assets/css");
        //}

        //public static HtmlHelper RequiresThemedJsFolder(this HtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName)
        //{
        //    var themeRoot = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
        //    return html.RequiresJsFolder(themeRoot + "Assets/js");
        //}

        #endregion
    }

    public static class Theming
    {
        public enum PathType
        {
            ThemeRoot,
            View,
            PartialView,
            GridEditor,
            FormsPartialsRoot,
            Configs
        }
    }
}
