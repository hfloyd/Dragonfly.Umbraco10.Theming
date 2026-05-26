# Dragonfly Umbraco 10 Theming

A theming system for Umbraco created by [Heather Floyd](https://www.HeatherFloyd.com).

> For a general explanation of the concept, see the article ["How to Create Multiple Unique Sites in One Installation Using Theming and the Umbraco Grid"](https://24days.in/umbraco-cms/2016/unique-sites-using-theming/).
> Please be aware that this article was published in 2016 before the code was further developed and put into GitHub. Also, the article is based on Umbraco 7, thus should really just be used for a general understanding - **this package includes all the code otherwise provided by the ZIP file mentioned in the article.**

## Instructions to Site Developers Using the Package

### Setup

Because this is a .Net Core site and Views are handled differently from static files, you will need to have two "Themes/[MyTheme]" folders - one next to the "Views" folder for holding Views and Partials, and one inside the wwwroot folder to hold all static theme-related files (css, js, images). Your Theme name should be the same for both "Themes" folders.

On your root Document Type, use the included "Theme Picker" Property Type to add a property for the site's chosen theme.

In appSettings.json add this section at the root-level (aka a sibling of 'Umbraco', not a child):

```
"DragonflyTheming": {
    "ThemeViewsAreInWwwRoot": false,
    "ThemesRootFolderName": "Themes",
    "ThemePickerPropertyAlias": "Theme",
    "CssFilePickerPropertyAlias": "SiteCss",
    "EnableDefaultThemeController": false,
    "ThemedAssetsCssFolder": "Css",
    "ThemedAssetsJsFolder": "Js",
    "FallbackAssetsFolder": "/Assets",
    "FallbackAssetsJsFolder": "/scripts",
    "FallbackAssetsCssFolder": "/css"
}
```

The Controller which runs for each page request needs to determine the Themed View file to render the page with, so if you already have custom controllers operating in your site, be sure to include something that will route the theme correctly. For example:

```
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
```

If you are not using any custom controllers, you can enable the 'DefaultThemeController' via the appSettings entries, which will handle theme routing for you automatically:

```
"DragonflyTheming": {
    ...
    "ThemePickerPropertyAlias": "Theme",
    "EnableDefaultThemeController": true,
    ...
}
```

## Individual Theme Configuration

You can store information about the Theme in an XML 'Theme.config' file. See the example config at "Themes/~CopyForNewTheme/Theme.config" for the format.

If the file doesn't exist in a Theme, a default one will be created automatically.

Example for a View:

```
var thisTheme = ThemeHelper.GetSiteThemeName(Model);
var themeConfiguration = ThemeHelper.GetThemeConfig(thisTheme);

@Html.GetGridHtml(Model, "GridPropertyAlias", themeConfiguration.GridRenderer)
```

This allows you to store the basic renderers all together outside of the Themes (in the standard root-level "/Views/Partials/grid/" folder) for reuse across Themes.

## Theme Conventions

Any folder in the "Themes" folder will have its name used as an available Theme, except for folders which start with a tilda (~), which will be ignored by the Theme Picker.

Any css file in the "Themes/~CssOverrides" folder will be available in the CSS Picker.

### Views

The Views folder in your Theme is where all customized View files should go. Just like in the main Umbraco-provided Views folder, you can have a "Partials" subfolder, etc. When a page is routed to its View, the Theme folder will be checked first, and if a matching file is not found there, it will default to the file in the primary Views folder (so make sure **all** your templates do have a file in the primary Views folder - even if it is blank).

### Assets

You can organize your assets folder however you like, but keep in mind that certain ThemeHelper functions rely upon knowing where the CSS and JS files are located. If you can standardize across all your themes, and make sure the AppSettings reflects your folder structure, that would be best. The config values represent the path once inside the 'Themes/MY_THEME/Assets/' folder:

```
"DragonflyTheming": {
    ...
    "ThemedAssetsCssFolder": "css",
    "ThemedAssetsJsFolder": "js"
    ...
}
```

If you have shared CSS/JS files (such as libraries, CSS frameworks, etc.) you can use web root relative folders (such as the Umbraco defaults "css" and "scripts" - which are available in the back-office) to hold those files, and reference them directly in your themed templates.

You can also have default fallback versions of certain CSS/JS files in the shared folders, if desired. Just make sure the config knows about your defaults:

```
"DragonflyTheming": {
    ...
    "FallbackAssetsFolder": "/Assets",
    "FallbackAssetsCssFolder": "/css",
    "FallbackAssetsJsFolder": "/scripts",
    ...
}
```

### Config Files

If you have theme-specific configuration files of whatever type, they can be added to a Theme folder in a subfolder named "Configs". You should also provide a default fall-back version of any config file you plan to call in a folder in the "Themes" root named "~DefaultConfigs".

You can get either the current theme's file, or the default, if none exists in the current theme in your custom code using `GetFinalThemePath()` with `Theming.PathType.Configs` (or use the shortcut - `GetThemeConfigsFolderPath()`).

## History of Project

Originally conceived and described for Umbraco v. 7 in 2016. I then created a proper Umbraco package for it for v. 8. I later ported it to .Net Core and Umbraco v. 10-13.

## Current Goals

Porting this to Umbraco v. 17. The back-office portions — the two Angular.js property editors (`ThemePicker` and `CssOverridePicker`) — need to be completely rewritten as Lit/TypeScript web components using the new Umbraco backoffice extension system (Vite + `umbraco-package.json` manifest).

Local copies of Umbraco source for reference:

- **Umbraco 17 CMS**: `C:\Users\Heather\WEBS\Code Projects\Umbraco-CMS\`
- **Umbraco UI Kit**: `C:\Users\Heather\WEBS\Code Projects\Umbraco.UI\`

## How the Theming System Works

### The Theme Concept

A **theme** is a named folder inside the root `Themes/` folder (any folder not prefixed with `~`). Each theme overrides the default site rendering and assets:

| Location                              | Purpose                                                                |
| ------------------------------------- | ---------------------------------------------------------------------- |
| `Themes/{ThemeName}/`                 | Razor views and partial views for this theme                           |
| `wwwroot/Themes/{ThemeName}/Assets/`  | Static files: CSS, images, JavaScript                                  |
| `wwwroot/Themes/{ThemeName}/Configs/` | JSON config files, accessed via `ThemeHelper.GetThemeConfigFilePath()` |

**Fallback chain at runtime:**

- Views → falls back to main `Views/` folder
- Assets → falls back to `wwwroot/Assets/`
- Config files → falls back to `wwwroot/Themes/~DefaultConfigs/`

### Special Folders (prefixed with `~`)

Tilde-prefixed folders inside `Themes/` are NOT treated as themes — they are utility folders:

| Folder                             | Purpose                                                       |
| ---------------------------------- | ------------------------------------------------------------- |
| `wwwroot/Themes/~DefaultConfigs/`  | Fallback config files when a theme doesn't have its own       |
| `wwwroot/Themes/~CssOverrides/`    | CSS files selectable by the CssOverridePicker property editor |
| `wwwroot/Themes/~CopyForNewTheme/` | Template to copy when creating a new theme                    |

### The Two Property Editors

**ThemePicker**

- Added to top-level "site" nodes in the Umbraco Content tree
- Scans the `Themes/` folder for non-tilde folders and presents them as a list
- The selected theme name drives view and asset resolution for all pages under that site node

**CssOverridePicker**

- Added to to top-level "site" nodes or any content nodes where a CSS override is needed
- Lists CSS files found in `wwwroot/Themes/~CssOverrides/`
- Allows editors to apply a specific CSS override on top of the active theme (based on how the theme view files are coded)

Both property editors have corresponding **Property Value Converters** in `Theming.Core`:

- `ThemePropertyConverter`
- `CssOverridePropertyConverter`

## Project Structure

```
src/
├── Theming.Core/               # C# models and property value converters (NuGet: Dragonfly.Umbraco10.Theming.Core)
│   └── Code/
│       ├── Models/             # DragonflyThemingConfig, ThemeConfig, DragonflyThemingPackage
│       ├── PropertyValueConverters/  # ThemePropertyConverter, CssOverridePropertyConverter
│       └── PropertyEditors-old/     # Legacy Angular.js editors (reference only, NOT compiled)
├── Theming.Web/                # Backoffice API + TypeScript frontend (NuGet: Dragonfly.Umbraco10.Theming.Web)
│   ├── Client/                 # Vite/TypeScript project for Umbraco 17 backoffice extensions
│   │   └── src/
│   │       ├── api/            # Auto-generated OpenAPI TypeScript client
│   │       ├── dashboards/     # Custom dashboard element + manifest
│   │       └── entrypoints/    # Extension entry point + manifest
│   ├── Composers/              # DragonflyThemingApiComposer (Swagger/OpenAPI setup)
│   ├── Controllers/            # Backoffice API controllers
│   └── wwwroot/
│       └── Themes/             # Theme templates and special folders
└── UmbracoTheming.TestSite/    # Functional test/demo Umbraco 17 installation
```

## Build Commands

**Frontend** (run from `Theming.Web/Client/`):

```
npm run watch        # TypeScript + Vite dev watch (outputs to wwwroot/App_Plugins/DragonflyUmbracoTheming)
npm run build        # TypeScript + Vite production build
npm run generate-client  # Regenerate TypeScript API client from OpenAPI spec
```

**Backend**:

```
dotnet build         # Standard build
dotnet run           # Run the TestSite (from UmbracoTheming.TestSite/)
```

**NuGet Packaging**:

```
dotnet pack --configuration Release
```

The `Custom.targets` MSBuild file automatically copies the `.nupkg` to `LocalNuGetPackages/` and pushes to nuget.org on Release builds.

## Migration Notes (Angular.js → Umbraco 17 Backoffice)

The old property editors are preserved (but excluded from compilation) in `Theming.Core/Code/PropertyEditors-old/` for reference.

**What needs to be built for v17:**

- `ThemePicker` as a Lit/TypeScript `UmbPropertyEditorUiElement` — reads available theme folders, presents as a selectable list
- `CssOverridePicker` as a Lit/TypeScript `UmbPropertyEditorUiElement` — reads CSS files from `~CssOverrides/`, presents as a selectable list
- Both need a corresponding backoffice API endpoint (in `Theming.Web/Controllers/`) to return the available options, since the frontend cannot read the file system directly
- Register both property editor UIs via `umbraco-package.json` manifest

**Patterns to follow**: Use the `Umbraco-CMS-Backoffice-Skills` Claude plugin (installed) for correct Umbraco 17 backoffice extension patterns — invoke the relevant skill before writing any backoffice UI code. The existing `Theming.Web/Client/src/dashboards/` code shows the established pattern for this project.

The local Umbraco source (`C:\Users\Heather\WEBS\Code Projects\Umbraco-CMS\`) and UI Kit (`C:\Users\Heather\WEBS\Code Projects\Umbraco.UI\`) are available for **reference** (reading specific implementations) but are not the primary guide for patterns.
