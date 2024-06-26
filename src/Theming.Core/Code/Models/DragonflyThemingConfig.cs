﻿namespace Dragonfly.UmbracoTheming
{
    using Newtonsoft.Json;

    public class DragonflyThemingConfig
    {
        //As per https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#bind-hierarchical-configuration-data-using-the-options-pattern-1

        //AppSettings Config Section name
        public const string DragonflyTheming = "DragonflyTheming";

        [JsonProperty("ThemeViewsAreInWwwRoot")]
        public bool ThemeViewsAreInWwwRoot { get; set; } = false;
        

        [JsonProperty("EnableDefaultThemeController")]
        public bool EnableDefaultThemeController { get; set; } = false;

        [JsonProperty("ThemePickerPropertyAlias")]
        public string ThemePickerPropertyAlias { get; set; } = "Theme";


        [JsonProperty("CssFilePickerPropertyAlias")]
        public string CssFilePickerPropertyAlias { get; set; } = "SiteCss";

        [JsonProperty("ThemesRootFolderName")]
        public string ThemesRootFolderName { get; set; } = "Themes";

        [JsonProperty("ThemedAssetsCssFolder")]
        public string ThemedAssetsCssFolder { get; set; } = "Css";

        [JsonProperty("ThemedAssetsJsFolder")]
        public string ThemedAssetsJsFolder { get; set; } = "Js";

        [JsonProperty("FallbackAssetsCssFolder")]
        public string FallbackAssetsCssFolder { get; set; } = "/css";

        [JsonProperty("FallbackAssetsJsFolder")]
        public string FallbackAssetsJsFolder { get; set; } = "/scripts";
        
        [JsonProperty("FallbackAssetsFolder")]
        public string FallbackAssetsFolder { get; set; } = "/Assets";
    }

   
   
}
