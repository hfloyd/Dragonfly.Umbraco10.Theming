namespace Dragonfly.UmbracoTheming
{
    using ClientDependency.Core.Mvc;
    using System;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using Umbraco.Core;
    using Umbraco.Web.Composing;
    using Umbraco.Core.IO;
    using Umbraco.Core.Models.PublishedContent;
    using Umbraco.Web;
    using Umbraco.Web.Logging;

    public static class ThemeHelper
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

        public static string ThemePropertyAlias()
        {
            var themeProp = WebConfigurationManager.AppSettings["Dragonfly.ThemePropertyAlias"];

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
        /// Looks for the Theme property specified ont he Site root node (Ancestor at Level 1)
        /// </summary>
        /// <param name="CurrentPage"></param>
        /// <returns></returns>
        public static string GetSiteThemeName(IPublishedContent CurrentPage)
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

        public static ThemeConfig GetThemeConfig(string ThemeName)
        {
            var config = ThemeConfig.Get(ThemeName);

            return config;
        }

        /// <summary>
        /// Determine if the Theme has a themed Umbraco Forms file
        /// </summary>
        /// <param name="SiteThemeName">Theme name</param>
        /// <param name="ViewPath">Final part of view path - ex: 'Form.cshtml' or 'Fieldtypes/FieldType.Text.cshtml'</param>
        /// <returns>True if file found</returns>
        public static bool HasUmbracoFormsThemeFile(string SiteThemeName, string ViewPath = "")
        {
            var baseThemePath = string.Format("~/Themes/{0}", SiteThemeName);
            var themePath = string.Format("{0}/Views/Partials/Forms/Themes/default/{1}", baseThemePath, ViewPath);

            if (System.IO.File.Exists(IOHelper.MapPath(themePath)))
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
        public static string GetFinalThemePath(string SiteThemeName, PathType PathType, string FileName = "", string AlternateStandardPath = "")
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
                case PathType.ThemeRoot:
                    standardPath = themePath;
                    themePath = string.Format("{0}/", baseThemePath);
                    isFolder = true;
                    break;

                case PathType.Configs:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Themes/~DefaultConfigs/{0}", FileName);
                    themePath = string.Format("{0}/Configs/{1}", baseThemePath, FileName);
                    break;

                case PathType.View:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/{0}.cshtml", FileName);
                    themePath = string.Format("{0}/Views/{1}.cshtml", baseThemePath, FileName);
                    break;

                case PathType.PartialView:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/Partials/{0}.cshtml", FileName);
                    themePath = string.Format("{0}/Views/Partials/{1}.cshtml", baseThemePath, FileName);
                    break;

                case PathType.FormsPartialsRoot:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/Partials/Forms/Themes/default/");
                    themePath = string.Format("{0}/Views/Partials/Forms/Themes/default/", baseThemePath);
                    isFolder = true;
                    break;

                case PathType.GridEditor:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : string.Format("~/Views/Partials/Grid/Editors/{0}.cshtml", FileName);
                    themePath = string.Format("{0}/Views/Partials/Grid/Editors/{1}.cshtml", baseThemePath, FileName);
                    break;

                default:
                    break;
            }

            if (isFolder & System.IO.Directory.Exists(IOHelper.MapPath(themePath)))
            {
                finalPath = themePath;
            }
            else if (!isFolder & System.IO.File.Exists(IOHelper.MapPath(themePath)))
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
        public static string GetThemePath(string SiteThemeName)
        {
            var path = GetFinalThemePath(SiteThemeName, PathType.ThemeRoot);
            return path;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.View
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        public static string GetThemeViewPath(string SiteThemeName, string ViewName)
        {
            var path = GetFinalThemePath(SiteThemeName, PathType.View, ViewName);
            return path;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.Configs
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="ConfigFileName"></param>
        /// <returns></returns>
        public static string GetThemedConfigFilePath(string SiteThemeName, string ConfigFileName)
        {
            var path = GetFinalThemePath(SiteThemeName, PathType.Configs, ConfigFileName);
            return path;
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.PartialView
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="ViewName"></param>
        /// <returns></returns>
        public static string GetThemePartialViewPath(string SiteThemeName, string ViewName)
        {
            var path = GetFinalThemePath(SiteThemeName, PathType.PartialView, ViewName);
            return path;
        }

        public static string GetCssOverridePath(string CssOverrideFileName)
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
        public static string ThemedAsset(this UrlHelper Url, string SiteThemeName, string RelativeAssetPath)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, PathType.ThemeRoot);
            var absolutePath = VirtualPathUtility.ToAbsolute(themeRoot).EnsureEndsWith('/');
            var virtualPath = absolutePath + "Assets/" + RelativeAssetPath;
            return virtualPath;
        }

        #region HTML Helpers

        public static HtmlHelper RequiresThemedCss(this HtmlHelper html, string SiteThemeName, string FilePath)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, PathType.ThemeRoot);
            return html.RequiresCss(themeRoot + "Assets/css" + FilePath.EnsureStartsWith('/'));
        }

        public static HtmlHelper RequiresThemedJs(this HtmlHelper html, string SiteThemeName, string FilePath)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, PathType.ThemeRoot);
            return html.RequiresJs(themeRoot + "Assets/js" + FilePath.EnsureStartsWith('/'));
        }

        public static HtmlHelper RequiresThemedCssFolder(this HtmlHelper html, string SiteThemeName)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, PathType.ThemeRoot);
            return html.RequiresCssFolder(themeRoot + "Assets/css");
        }

        public static HtmlHelper RequiresThemedJsFolder(this HtmlHelper html, string SiteThemeName)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, PathType.ThemeRoot);
            return html.RequiresJsFolder(themeRoot + "Assets/js");
        }

        /// <summary>
        /// Renders a partial view in the current theme
        /// </summary>
        /// <param name="html"></param>
        /// <param name="SiteThemeName"></param>
        /// <param name="PartialName"></param>
        /// <param name="ViewModel"></param>
        /// <param name="ViewData"></param>
        /// <returns></returns>
        public static IHtmlString ThemedPartial(this HtmlHelper html, string SiteThemeName, string PartialName, object ViewModel, ViewDataDictionary ViewData = null)
        {
            //try
            //{
                var path = GetFinalThemePath(SiteThemeName, PathType.PartialView, PartialName);
                return html.Partial(path, ViewModel, ViewData);
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
        public static IHtmlString ThemedPartial(this HtmlHelper html, string SiteThemeName, string PartialName, ViewDataDictionary ViewData = null)
        {
            if (ViewData == null)
            {
                ViewData = html.ViewData;
            }
            //try
            //{
                var path = GetFinalThemePath(SiteThemeName, PathType.PartialView, PartialName);
                return html.Partial(path, ViewData);
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
    }

}
