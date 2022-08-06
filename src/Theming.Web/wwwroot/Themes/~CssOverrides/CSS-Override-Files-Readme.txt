If you have a common Theme design, but want to just swap CSS files between sites, 
add the CSS files to this folder and add a property of type "CSS Overide Picker" 
to your Homepage document type.

You can then use:

<link rel="stylesheet" href="@Url.GetCssOverridePath(ThemeHelper, currentPage)" />

or

<link rel="stylesheet" href="@Url.GetCssOverridePath(ThemeHelper, theSiteCssStringValue)" />