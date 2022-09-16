# Dragonfly Umbraco 10 Theming #

 A theming system for Umbraco version 10 created by [Heather Floyd](https://www.HeatherFloyd.com).

> For a general explanation of the concept, see the article ["How to Create Multiple Unique Sites in One Installation Using Theming and the Umbraco Grid"](https://24days.in/umbraco-cms/2016/unique-sites-using-theming/). 
Please be aware that this article was published in 2016 before the code was further developed and put into GitHub. Also, the article is based on Umbraco 7, thus should really just be used for a general understanding - **this package includes all the code otherwise provided by the ZIP file mentioned in the article.** 

Credit goes to [Shannon Deminick's *Articulate* package](https://github.com/Shazwazza/Articulate) for inspiration and initial code.

**NOTE:** This project was ported from the [v8 version](https://github.com/hfloyd/Dragonfly.Umbraco8Theming). Please [report any issues](https://github.com/hfloyd/Dragonfly.Umbraco10.Theming/issues) you experience.

## Installation ##
Generally, you will want to install the .Web version of the package into your Umbraco project, which includes the App_Plugins, a setup of the Themes folder, and example Razor files:

[![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.Umbraco10.Theming.Web)](https://www.nuget.org/packages/Dragonfly.Umbraco10.Theming.Web/)

     PM>   Install-Package Dragonfly.Umbraco10.Theming.Web

There is also a .Core version available which only includes the DLL files. This can be used if you have an accompanying Class project which needs to access the ThemeHelperService:

[![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.Umbraco10.Theming.Core)](https://www.nuget.org/packages/Dragonfly.Umbraco10.Theming.Core/)

     PM>   Install-Package Dragonfly.Umbraco10.Theming.Core

## Resources ##
GitHub Repository: [https://github.com/hfloyd/Dragonfly.Umbraco10.Theming](https://github.com/hfloyd/Dragonfly.Umbraco10.Theming)


- [Dragonfly Theming for Umbraco 10](https://github.com/hfloyd/Dragonfly.Umbraco10.Theming) 
- [Dragonfly Theming for Umbraco 8](https://github.com/hfloyd/Dragonfly.Umbraco8Theming) 
- [Dragonfly Theming for Umbraco 7](https://github.com/hfloyd/Dragonfly.UmbracoTheming)

## Setup ##
Because this is a .Net Core site and Views are handled differently from static files, you will need to have two "Themes/[MyTheme]" folders - one next to the "Views" folder for holding Views and Partials, and one inside the wwwroot folder to hold all static theme-related files (css, js, images). Your Theme name should be the same in both "Themes" folders. 

On your root Document Type, use the included "Theme Picker" Property Type to add a property for the site's chosen theme. 


In appSettings.json add this section at the root-level (aka a sibling of 'Umbraco', not a child):

	"DragonflyTheming": {
        "ThemeViewsAreInWwwRoot": false,
        "ThemesRootFolderName": "Themes",
        "ThemePickerPropertyAlias": "Theme",
        "CssFilePickerPropertyAlias": "SiteCss",
        "EnableDefaultThemeController": false,
        "FallbackAssetsCssFolder": "css",
        "ThemedAssetsCssFolder": "Css",
        "FallbackAssetsJsFolder": "scripts",
        "ThemedAssetsJsFolder": "Js"
    }


In your Startup.cs file, add:

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
			...
			app.UseStaticFiles();        
        }

The Controller which runs for each page request needs to determine the Themed View file to render the page with, so if you already have custom controllers operating in your site, be sure to include something that will route the theme correctly. For example:

	namespace MySite
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
	    public class DefaultController : RenderController
	    {
		private readonly ILogger _logger;
		private readonly ThemeHelperService _ThemeHelperService;

		public DefaultController(
		    ILogger<DefaultController> logger,
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


If you are not using any custom controllers, you can enable the 'DefaultThemeController' via the appSettings entries, which will handle theme routing for you automatically:

    "DragonflyTheming": {
		...
    	"ThemePickerPropertyAlias": "Theme",
    	"EnableDefaultThemeController": true
		...
    }


## Individual Theme Configuration ##

You can store information about the Theme in an XML 'Theme.config' file. See the example config at "Themes/~CopyForNewTheme/Theme.config" for the format.

If the file doesn't exist in a Theme, a default one will be created automatically.

This is most useful for storing information about the GridRenderer used for the Theme - in the event that different Themes use different rendering files.

Example for a View:
    
    var thisTheme = ThemeHelper.GetSiteThemeName(Model);
    var themeConfiguration = ThemeHelper.GetThemeConfig(thisTheme);
    
    @Html.GetGridHtml(Model, "GridPropertyAlias", themeConfiguration.GridRenderer)

This allows you to store the basic renderers all together outside of the Themes (in the standard root-level "/Views/Partials/grid/" folder) for reuse across Themes.


## Theme Conventions ##

Any folder in the "Themes" folder will have its name used as an available Theme, except for folders which start with a tilda (~), which will be ignored by the Theme Picker.

Any css file in the "Themes/~CssOverrides" folder will be available in the CSS Picker.

### Views ###

The Views folder in your Theme is where all customized View files should go. Just like in the main Umbraco-provided Views folder, you can have a "Partials" subfolder, etc. When a page is routed to its View, the Theme folder will be checked first, and if a matching file is not found there, it will default to the file in the primary Views folder (so make sure **all** your templates do have a file in the primary Views folder - even if it is blank).

### Assets ###

You can organize your assets folder however you like, but keep in mind that certain ThemeHelper functions rely upon knowing where the CSS and JS files are located. If you can standardize across all your themes, and make sure the AppSettings reflects your folder structure, that would be best. The config values represent the path once inside the 'Themes/MY_THEME/Assets/' folder:

	"DragonflyTheming": {
		...
		"ThemedAssetsCssFolder": "css",
		"ThemedAssetsJsFolder": "js"
		...
	}

If you have shared CSS/JS files (such as libraries, CSS frameworks, etc.) you can use web root relative folders (such as the Umbraco defaults "css" and "scripts" - which are available in the back-office) to hold those files, and reference them directly in your themed templates. 

You can also have default fallback versions of certain CSS/JS files in the shared folders, if desired. Just make sure the config knows about your defaults:

	"DragonflyTheming": {
		...
		"FallbackAssetsCssFolder": "~/css",
		"FallbackAssetsJsFolder": "~/scripts",
		...
	}

### Config Files ###

If you have theme-specific configuration files of whatever type, they can be added to a Theme folder in a subfolder named "Configs". You should also provide a default fall-back version of any config file you plan to call in a folder in the "Themes" root named "~DefaultConfigs".

You can get either the current theme's file, or the default, if none exists in the current theme in your custom code using `GetFinalThemePath()` with `Theming.PathType.Configs` (or use the shortcut - `GetThemeConfigsFolderPath()`).

## Changes from v7/v8 Version to v10 Version ##

If you are updating an Umbraco site which was previously using Dragonfly Theming, there are a few things you might want to know that have changed.

In terms of the code itself, there was some refactoring as well as bringing it into line with ASP.Net Core / Umbraco 10 best-practices (specifically better use of IoC / Dependency Injection). I've also added many more configurations for things which were otherwise hard-coded (such as folder locations, etc.).

Some things you will need to be aware of while updating your Themes and any custom code for v10:

- Your single "Theme/MyTheme" folder will need to be separated into two - one for the View files and one for the static files. (See the **Setup** section, above for details.)
- The static "ThemeHelper" has been converted to a non-static "ThemeHelperService". You will need to inject it into your views like this: `@inject ThemeHelperService ThemeHelper` (Put this into "_ViewImports.cshtml" and you won't have to add it to every View manually.)
- The HtmlHelpers and UrlHelpers have been moved to their own static Extensions class, so they are available as before, except you will now need to pass in the ThemeHelperService. ex: `@Url.ThemedAsset(ThemeHelper, thisTheme, "images/favicon.ico")`
- The HtmlHelpers `RequiresThemedCss()`, `RequiresThemedJs()`, `RequiresThemedCssFolder()`, and `RequiresThemedJsFolder()` have been removed, since ClientDependency has been swapped out for Smidge. Take a look at the provided "_Master.cshtml" file (in the "Themes/~CopyForNewTheme/Views" folder) for example code using Smidge. You can also use whatever bundling framework you prefer, since now helpers are available to return themed file and folder paths.
- All the included example Razor files have been updated as well, so if you are confused about anything, take a look at the contents of the "Themes/~CopyForNewTheme" folder, as well as "/Views/Partials/Grid/Bootstrap3WithTheming.cshtml" and "/Views/MacroPartials/~ExampleThemedMacro.cshtml"


## Troubleshooting ##

### Static Theme files are throwing 404 errors ###

If your CSS, JavaScript, and other theme assets don't seem to be rendering, make sure they are getting included in your publishing. Also, double-check the **Setup** section, above, and make sure you have all proper configurations in place.

