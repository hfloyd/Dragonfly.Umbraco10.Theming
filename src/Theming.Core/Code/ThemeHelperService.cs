namespace Dragonfly.UmbracoTheming
{
    using System;
    using System.IO;
    using System.Xml.Serialization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Html;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core.Extensions;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Extensions;

    public class ThemeHelperService
    {
        private readonly ILogger<ThemeHelperService> _logger;
        private readonly IConfiguration _AppSettingsConfig;
        private readonly IWebHostEnvironment _HostingEnvironment;

        private readonly DragonflyThemingConfig _ConfigOptions;

        /// <summary>
        /// Cached instance of current Theme configuration
        /// </summary>
        private ThemeConfig? _ThemeConfig;


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

            _ConfigOptions = GetAppDataConfig();
        }

        #region Public Methods

        public DragonflyThemingConfig ThemingConfigOptions()
        {
            return _ConfigOptions;
        }

        /// <summary>
        /// Property Alias for Theme Picker property (from AppSettings)
        /// </summary>
        /// <returns></returns>
        public string ThemePropertyAlias()
        {
            var themeProp = _ConfigOptions.ThemePropertyAlias;
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
        /// Property Alias for CSS Picker property (from AppSettings)
        /// </summary>
        /// <returns></returns>
        public string CssPropertyAlias()
        {
            var cssProp = _ConfigOptions.CssFilePickerPropertyAlias;
            if (!string.IsNullOrEmpty(cssProp))
            {
                return cssProp;
            }
            else
            {
                return "SiteCss"; //default
            }
        }

        public string GetAllThemesViewsRoot(bool ReturnPhysicalPath = false)
        {

            var themesRootFolderName = _ConfigOptions.ThemesRootFolderName.EnsureEndsWith('/').EnsureStartsWith('/');
            var baseThemePath = themesRootFolderName;// $"{themesRootFolder}{SiteThemeName}/";

            //var themesRootPrefix = _ConfigOptions.ThemesAreInWwwRoot ? "wwwroot/" : "";

            //var virtualPath = $"{baseThemePath}";

            if (ReturnPhysicalPath)
            {
                var physicalPath = MapThemePath(baseThemePath, Theming.ThemeFileType.Views);
                return physicalPath;
            }
            else
            {
                return baseThemePath;
            }
        }

        public string GetAllThemesStaticFilesRoot(bool ReturnPhysicalPath = false)
        {

            var themesRootFolderName = _ConfigOptions.ThemesRootFolderName.EnsureEndsWith('/').EnsureStartsWith("/");
            var baseThemePath = themesRootFolderName;// $"{themesRootFolder}{SiteThemeName}/";

            //var themesRootPrefix = _ConfigOptions.ThemesAreInWwwRoot ? "wwwroot/" : "";

            //var virtualPath = $"{baseThemePath}";

            if (ReturnPhysicalPath)
            {
                var physicalPath = MapThemePath(baseThemePath, Theming.ThemeFileType.StaticFiles);
                return physicalPath;
            }
            else
            {
                return baseThemePath;
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
        public ThemeConfig? GetThemeConfig(string ThemeName)
        {
            string path = GetThemeConfigFilePath(ThemeName);
            var lastModified = System.IO.File.GetLastWriteTime(path);

            if (_ThemeConfig is null)
            {
                SetThemeConfig(ThemeName);
            }
            else if (_ThemeConfig.ThemeName != ThemeName ||
                    (_ThemeConfig.ThemeName == ThemeName && lastModified > _ThemeConfig.ConfigTimestamp))
            {
                SetThemeConfig(ThemeName);
            }

            return _ThemeConfig;
        }

        /// <summary>
        /// Returns the final path to the requested type, based on the theme and file existence.
        /// </summary>
        /// <param name="SiteThemeName">Theme Name</param>
        /// <param name="PathType">Type of Path to return</param>
        /// <param name="FileName">Name of the View (without extension) or name of the config (with extension) (optional)</param>
        /// <param name="AlternateStandardPath">If the non-themed path is not standard, provide the full path here (optional)</param>
        /// <returns></returns>
        public string GetFinalThemePath(string SiteThemeName, Theming.PathType PathType, string ViewOrFileName = "", string AlternateStandardPath = "")
        {
            var standardPath = "";
            var themePath = "";
            var isFolder = false;

            var fileType = ConvertPathTypeToFileType(PathType);
            var virtualThemesRoot = (fileType == Theming.ThemeFileType.Views ? GetAllThemesViewsRoot(false) : GetAllThemesStaticFilesRoot(false));
            var baseThemePath = $"{virtualThemesRoot}{SiteThemeName}";

            var viewPathAndNameNoExt = ViewOrFileName;
            var ext = Path.GetExtension(ViewOrFileName);
            if (ext != "")
            {
                viewPathAndNameNoExt = ViewOrFileName.Replace(ext, "");
            }

            switch (PathType)
            {
                case Theming.PathType.ThemeViewsRoot:
                    standardPath = "";
                    themePath = baseThemePath;
                    isFolder = true;
                    break;

                case Theming.PathType.Configs:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : $"{virtualThemesRoot}~DefaultConfigs/{ViewOrFileName}";
                    themePath = $"{baseThemePath}/Configs/{ViewOrFileName}";
                    break;

                case Theming.PathType.View:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : $"~/Views/{viewPathAndNameNoExt}.cshtml";
                    themePath = $"{baseThemePath}/Views/{viewPathAndNameNoExt}.cshtml";
                    break;

                case Theming.PathType.PartialView:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : $"~/Views/Partials/{viewPathAndNameNoExt}.cshtml";
                    themePath = $"{baseThemePath}/Views/Partials/{viewPathAndNameNoExt}.cshtml";
                    break;

                case Theming.PathType.FormsThemesRoot:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : $"~/Views/Partials/Forms/Themes/";
                    themePath = $"{baseThemePath}/Views/Partials/Forms/Themes/";
                    isFolder = true;
                    break;

                case Theming.PathType.GridEditor:
                    standardPath = AlternateStandardPath != "" ? AlternateStandardPath : $"~/Views/Partials/Grid/Editors/{viewPathAndNameNoExt}.cshtml";
                    themePath = $"{baseThemePath}/Views/Partials/Grid/Editors/{viewPathAndNameNoExt}.cshtml";
                    break;

                case Theming.PathType.ThemeStaticFilesRoot:
                    standardPath = "";
                    themePath = baseThemePath.EnsureEndsWith("/");
                    //themePath = $"{baseThemePath}/Views/Partials/Forms/Themes/";
                    isFolder = true;
                    break; 
                 
                default:
                    break;
            }

            var finalPath = "";
            // var physicalThemesRoot = (ConvertPathTypeToFileType(PathType) == Theming.ThemeFileType.Views ? GetAllThemesViewsRoot(true) : GetAllThemesStaticFilesRoot(true)).EnsureEndsWith('/');

            var mappedPath = MapThemePath(themePath, ConvertPathTypeToFileType(PathType));
            if (isFolder & System.IO.Directory.Exists(mappedPath))
            {
                finalPath = themePath;
            }
            else if (!isFolder & System.IO.File.Exists(mappedPath))
            {
                finalPath = themePath;
            }
            else
            {
                finalPath = standardPath;
            }

            return finalPath;
        }

        private Theming.ThemeFileType ConvertPathTypeToFileType(Theming.PathType PathType)
        {
            switch (PathType)
            {
                case Theming.PathType.ThemeViewsRoot:
                    return Theming.ThemeFileType.Views;
                    break;

                case Theming.PathType.ThemeStaticFilesRoot:
                    return Theming.ThemeFileType.StaticFiles;
                    break;

                case Theming.PathType.View:
                    return Theming.ThemeFileType.Views;
                    break;

                case Theming.PathType.PartialView:
                    return Theming.ThemeFileType.Views;
                    break;

                case Theming.PathType.GridEditor:
                    return Theming.ThemeFileType.Views;
                    break;

                case Theming.PathType.FormsThemesRoot:
                    return Theming.ThemeFileType.Views;
                    break;

                case Theming.PathType.Configs:
                    return Theming.ThemeFileType.StaticFiles;
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(PathType), PathType, null);
            }
        }

        /// <summary>
        /// Shortcut for 'GetFinalThemePath()' with PathType.ThemeRoot
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <returns></returns>
        public string GetThemePath(string SiteThemeName)
        {
            var path = GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeViewsRoot);
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
        public string GetThemeConfigsFolderPath(string SiteThemeName, string ConfigFileName)
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

        /// <summary>
        /// Returns a file from the '~CssOverrides' folder (Themes root as configured in the AppSettings)
        /// </summary>
        /// <param name="CssOverrideFileName">Name of the CSS file</param>
        /// <returns></returns>
        public string GetCssOverridePath(string CssOverrideFileName)
        {
            if (CssOverrideFileName.IsNullOrWhiteSpace())
            {
                return "";
            }
            else
            {
                var rootThemesPath = _ConfigOptions.ThemesRootFolderName.EnsureEndsWith('/');
                var path = $"{rootThemesPath}~CssOverrides/{CssOverrideFileName.EnsureEndsWith(".css")}";
                return path;
            }

        }


        /// <summary>
        /// Get the path to an Assets subfolder for the Theme
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="RelativeAssetFolderPath">Path to folder inside [theme]/Assets/ folder</param>
        /// <returns></returns>
        public string GetThemedAssetFolder(string SiteThemeName, string RelativeAssetFolderPath)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeStaticFilesRoot);
           // var absolutePath = themeRoot.EnsureEndsWith('/');
            var virtualPath = (themeRoot + "Assets/" + RelativeAssetFolderPath.EnsureEndsWith('/')).EnsureStartsWith("/");
            return virtualPath;
        }

        /// <summary>
        /// Returns a file from the Theme's Assets folder 
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="RelativeAssetPath">Path to file inside [theme]/Assets/ folder</param>
        /// <returns>String representing the file path</returns>
        public string GetThemedAssetFile(string SiteThemeName, string RelativeAssetPath)
        {
            var themeRoot = GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeStaticFilesRoot);
            // var absolutePath = themeRoot.EnsureEndsWith('/');
            var virtualPath = (themeRoot + "Assets/" + RelativeAssetPath).EnsureStartsWith("/");
            return virtualPath;
        }

        /// <summary>
        /// Get the path to the Css Assets folder for the Theme
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <returns></returns>
        public string GetThemedCssFolder(string SiteThemeName)
        {
            var themeFolderConfig = _ConfigOptions.ThemedAssetsCssFolder;
            var themedFolder = GetThemedAssetFolder(SiteThemeName, themeFolderConfig);
            return themedFolder;
        }

        /// <summary>
        /// Returns a themed CSS file, with optional fallback to default css folder
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="CssFileName"></param>
        /// <param name="UseFallback">Set to FALSE to NOT check for a fallback default file</param>
        /// <returns></returns>
        public string GetThemedCssFile(string SiteThemeName, string CssFileName, bool UseFallback = true)
        {
            //var themeFolderConfig = _ConfigOptions.ThemedAssetsCssFolder;
            var themedFolder = GetThemedCssFolder(SiteThemeName);
            var themedFilePath = $"{themedFolder}{CssFileName.EnsureEndsWith(".css")}";

            if (!UseFallback)
            {
                return themedFilePath;
            }
            else
            {
                //Check for file existence
                if (System.IO.File.Exists(MapThemePath(themedFilePath, Theming.ThemeFileType.StaticFiles)))
                {
                    return themedFilePath;
                }
                else
                {
                    var defaultFolderPath = _ConfigOptions.FallbackAssetsCssFolder;
                    var defaultFilePath = $"{GetNonThemedFolder(defaultFolderPath)}{CssFileName.EnsureEndsWith(".css")}";
                    return defaultFilePath;
                }
            }
        }

        /// <summary>
        /// Get the path to the JavaScript Assets folder for the Theme
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <returns></returns>
        public string GetThemedJsFolder(string SiteThemeName)
        {
            var themeFolderConfig = _ConfigOptions.ThemedAssetsJsFolder;
            var themedFolder = GetThemedAssetFolder(SiteThemeName, themeFolderConfig);
            return themedFolder;
        }

     

        /// <summary>
        /// Returns a themed JavaScript file, with optional fallback to default Js folder
        /// </summary>
        /// <param name="SiteThemeName"></param>
        /// <param name="JsFileName"></param>
        /// <param name="UseFallback">Set to FALSE to NOT check for a fallback default file</param>
        /// <returns></returns>
        public string GetThemedJsFile(string SiteThemeName, string JsFileName, bool UseFallback = true)
        {
            //var themeFolderConfig = _ConfigOptions.ThemedAssetsCssFolder;
            var themedFolder = GetThemedJsFolder(SiteThemeName);
            var themedFilePath = $"{themedFolder}{JsFileName.EnsureEndsWith(".js")}";

            if (!UseFallback)
            {
                return themedFilePath;
            }
            else
            {
                //Check for file existence
                if (System.IO.File.Exists(MapThemePath(themedFilePath, Theming.ThemeFileType.StaticFiles)))
                {
                    return themedFilePath;
                }
                else
                {
                    var defaultFolderPath = _ConfigOptions.FallbackAssetsJsFolder;
                    var defaultFilePath = $"{GetNonThemedFolder(defaultFolderPath)}{JsFileName.EnsureEndsWith(".js")}";
                    return defaultFilePath;
                }
            }

        }

        /// <summary>
        /// Determine if the Theme has a themed Umbraco Forms file
        /// </summary>
        /// <param name="SiteThemeName">Theme name</param>
        /// <param name="ViewPath">Final part of view path - ex: 'Form.cshtml' or 'Fieldtypes/FieldType.Text.cshtml'</param>
        /// <param name="FormsThemeName">Name of the Forms Theme ('default' if blank)</param>
        /// <returns>True if file found</returns>
        public bool HasUmbracoFormsThemeFile(string SiteThemeName, string ViewPath = "", string FormsThemeName = "default")
        {
            var viewFilePath = $"Views/Partials/Forms/Themes/{FormsThemeName}/{ViewPath}";
            var themePath = GetThemeViewPath(SiteThemeName, viewFilePath);

            if (System.IO.File.Exists(MapThemePath(themePath, Theming.ThemeFileType.StaticFiles)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        #endregion

        #region Private Methods

        private DragonflyThemingConfig GetAppDataConfig()
        {
            var options = new DragonflyThemingConfig();
            _AppSettingsConfig.GetSection(DragonflyThemingConfig.DragonflyTheming).Bind(options);

            return options;
        }

        //private string GetThemeRootPath(string SiteThemeName)
        //{
        //    if (SiteThemeName.IsNullOrWhiteSpace())
        //    {
        //        throw new InvalidOperationException("Missing SiteThemeName parameter: No theme has been set for this website root, republish the root with a selected theme.");
        //    }

        //    var themesRoot = _ConfigOptions.ThemesRootFolder.EnsureEndsWith('/');
        //    var baseThemePath = $"{themesRoot}{SiteThemeName}/";

        //    return baseThemePath;
        //}

        private void SetThemeConfig(string SiteThemeName)
        {
            // If the file is not there => Create with defaults
            CreateThemeConfigFileIfNotExists(SiteThemeName);

            string path = GetThemeConfigFilePath(SiteThemeName);
            var lastModified = System.IO.File.GetLastWriteTime(path);

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

            _ThemeConfig = newConfig;

        }
        private void CreateThemeConfigFileIfNotExists(string SiteThemeName)
        {
            // Create from configuration file
            string path = GetThemeConfigFilePath(SiteThemeName);

            // If it is not there yet, serialize our defaults to file
            if (!File.Exists(path))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ThemeConfig));

                using (StreamWriter file = new StreamWriter(path))
                {
                    serializer.Serialize(file, DefaultThemeConfig);
                }
            }
        }
        private string GetThemeConfigFilePath(string SiteThemeName)
        {
            var themeRoot = GetThemePath(SiteThemeName).EnsureEndsWith('/');
            var configPath = $"{themeRoot}Theme.config";
            return MapThemePath(configPath, Theming.ThemeFileType.StaticFiles);
        }


        private readonly ThemeConfig DefaultThemeConfig = new ThemeConfig
        {
            Author = "Unspecified Author",
            AuthorUrl = "https://unknown",
            SourceUrl = "https://unknown",
            CssFramework = "Unspecified Framework",
            GridRenderer = "Bootstrap3WithTheming",
            Description = "Theme description..."
        };

        private string GetNonThemedFolder(string FolderPath)
        {
            var absolutePath = FolderPath.EnsureEndsWith('/');
            var virtualPath = absolutePath;
            return virtualPath;
        }

        private string MapThemePath(string VirtualPath, Theming.ThemeFileType FileType)
        {
            switch (FileType)
            {
                case Theming.ThemeFileType.Views:
                    if (_ConfigOptions.ThemeViewsAreInWwwRoot)
                    {
                        return _HostingEnvironment.MapPathWebRoot(VirtualPath);
                    }
                    else
                    {
                        return _HostingEnvironment.MapPathContentRoot(VirtualPath);
                    }
                    break;
                case Theming.ThemeFileType.StaticFiles:
                    return _HostingEnvironment.MapPathWebRoot(VirtualPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(FileType), FileType, null);
            }


        }

        #endregion

    }

    public static class ThemingExtensions
    {
        #region Url Helpers

        /// <summary>
        /// Returns the url of a themed asset
        /// <example>In a View:
        /// <code>@Url.ThemedAsset(Model, "images/favicon.ico")</code>
        /// </example>
        /// </summary>
        /// <param name="Url">UrlHelper (@Url.)</param>
        /// <param name="ThemeHelper"></param>
        /// <param name="SiteThemeName"></param>
        /// <param name="RelativeAssetPath">Path to file inside [theme]/Assets/ folder</param>
        /// <returns></returns>
        public static string ThemedAsset(this IUrlHelper Url, ThemeHelperService ThemeHelper, string SiteThemeName, string RelativeAssetPath)
        {
            //var themeRoot = ThemeHelper.GetFinalThemePath(SiteThemeName, Theming.PathType.ThemeRoot);
            ////var absolutePath = VirtualPathUtility.ToAbsolute(themeRoot).EnsureEndsWith('/');
            //var absolutePath = themeRoot.EnsureEndsWith('/');
            //var virtualPath = absolutePath + "Assets/" + RelativeAssetPath;
            var virtualPath = ThemeHelper.GetThemedAssetFile(SiteThemeName, RelativeAssetPath);
            return virtualPath;
        }

        public static string GetCssOverridePath(this IUrlHelper Url, ThemeHelperService ThemeHelper,
            string CssOverrideFileName)
        {
            var path = ThemeHelper.GetCssOverridePath(CssOverrideFileName);

            return path;
        }

        public static string GetCssOverridePath(this IUrlHelper Url, ThemeHelperService ThemeHelper, IPublishedContent CurrentPage)
        {
            var cssPropAlias = ThemeHelper.CssPropertyAlias();

            if (!string.IsNullOrEmpty(cssPropAlias))
            {
                var cssPropValue = CurrentPage.AncestorOrSelf(1).Value<string>(cssPropAlias);

                var path = ThemeHelper.GetCssOverridePath(cssPropValue);

                return path;
            }
            else
            {
                return "";
            }
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
        public static HtmlString ThemedPartial(this IHtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName, string PartialName, object ViewModel, ViewDataDictionary ViewData = null)
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
        public static HtmlString ThemedPartial(this IHtmlHelper html, ThemeHelperService ThemeHelper, string SiteThemeName, string PartialName, ViewDataDictionary ViewData = null)
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

        #region Obsolete

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
            ThemeViewsRoot,
            View,
            PartialView,
            GridEditor,
            FormsThemesRoot,
            Configs,
            ThemeStaticFilesRoot
        }

        public enum ThemeFileType
        {
            Views,
            StaticFiles,
            Unknown
        }
    }
}
