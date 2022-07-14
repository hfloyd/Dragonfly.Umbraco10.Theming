namespace Dragonfly.UmbracoTheming
{
    using Newtonsoft.Json;

    public class DragonflyThemingConfig
    {
        //As per https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#bind-hierarchical-configuration-data-using-the-options-pattern-1

        //AppSettings Config Section name
        public const string DragonflyTheming = "DragonflyTheming";

        [JsonProperty("EnableDefaultThemeController")]
        public bool EnableDefaultThemeController { get; set; } = false;

        [JsonProperty("ThemePickerPropertyAlias")]
        public string ThemePropertyAlias { get; set; } = "Theme";


        [JsonProperty("CssFilePickerPropertyAlias")]
        public string CssFilePickerPropertyAlias { get; set; } = "SiteCss";

        [JsonProperty("ThemesRootFolder")]
        public string ThemesRootFolder { get; set; } = "~/Themes";

        [JsonProperty("FallbackAssetsCssFolder")]
        public string FallbackAssetsCssFolder { get; set; } = "~/css";

        [JsonProperty("FallbackAssetsJsFolder")]
        public string FallbackAssetsJsFolder { get; set; } = "~/scripts";

        [JsonProperty("ThemedAssetsCssFolder")]
        public string ThemedAssetsCssFolder { get; set; } = "Css";

        [JsonProperty("ThemedAssetsJsFolder")]
        public string ThemedAssetsJsFolder { get; set; } = "Js";

    }

   
   
}
