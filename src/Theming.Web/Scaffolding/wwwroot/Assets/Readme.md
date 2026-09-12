This **/wwwroot/Assets/** folder is the fall-back location for any assets called in view files via
 `@ThemeHelper.GetThemedAssetFile("MY_THEME", "FILE")` ( or `@Url.ThemedAsset(ThemeHelper,"MY_THEME", "FILE")`)

So, for instance, if you had this:
 `@ThemeHelper.GetThemedAssetFile("MY_THEME", "img/logo.png")`

in a view file, it would first look for a themed version of the file at: */wwwroot/Themes/MY_THEME/Assets/img/logo.png*

if not found, it would then look at:
 */wwwroot/Assets/img/logo.png*

if still not found, it would result in a 404 on the asset.

NOTE: If you do not want the fall-back image checked, then add the parameter `UseFallback` with the value FALSE:

 `@ThemeHelper.GetThemedAssetFile("MY_THEME", "img/logo.png", false)`
