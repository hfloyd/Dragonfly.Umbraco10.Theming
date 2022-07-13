namespace Dragonfly.Umbraco8Theming
{
    using System;
    using System.IO;
    using System.Web.Hosting;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration settings read from a config XML file
    /// </summary>
    [XmlRoot("ThemeSettings")]
    public class ThemeConfig
    {
        /// <summary>
        /// Cached instance of configuration
        /// </summary>
        private static ThemeConfig _config;

        public string ThemeName { get; set; }
        public string ConfigPath { get; set; }
        public DateTime ConfigTimestamp { get; set; }

        [XmlElement]
        public string Author { get; set; }

        [XmlElement]
        public string AuthorUrl { get; set; }

        [XmlElement]
        public string SourceUrl { get; set; }

        [XmlElement]
        public string CssFramework { get; set; }

        [XmlElement]
        public string GridRenderer { get; set; }

        [XmlElement]
        public string Description { get; set; }

        public static ThemeConfig Get(string SiteThemeName)
        {
            string path = GetConfigFilePath(SiteThemeName);
            var lastModified = System.IO.File.GetLastWriteTime(path);

            if (_config == null|| _config.ThemeName!=SiteThemeName ||(_config.ThemeName==SiteThemeName && lastModified > _config.ConfigTimestamp))
            {
                // If the file is not there => Create with defaults
                CreateIfNotExists(SiteThemeName);
                
                XmlSerializer serializer = new XmlSerializer(typeof(ThemeConfig));

                // Read from file
                try
                {
                    using (var reader = new StreamReader(path))
                    {
                        _config = (ThemeConfig)serializer.Deserialize(reader);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException("The format of 'Theme.config' is invalid. Details: " + ex.Message,ex.InnerException);
                }

                _config.ThemeName = SiteThemeName;
                _config.ConfigPath = path;
                _config.ConfigTimestamp = lastModified;
            }

            return _config;

        }

        public static void CreateIfNotExists(string SiteThemeName)
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

        private static string GetConfigFilePath(string SiteThemeName)
        {
            var themeRoot = ThemeHelper.GetThemePath(SiteThemeName);
            var configPath = themeRoot + "Theme.config";
            return HostingEnvironment.MapPath(configPath);
        }

        private static readonly ThemeConfig DefaultConfig = new ThemeConfig
        {
            Author = "Unspecified Author",
            AuthorUrl = "https://unknown",
            SourceUrl = "https://unknown",
            CssFramework = "Unspecified Framework",
            GridRenderer = "Bootstrap3",
            Description = "Theme description..."
        };
    }



}
