namespace Dragonfly.UmbracoTheming
{
    using System;
    using System.Xml.Serialization;


    /// <summary>
    /// Configuration settings read from a config file
    /// </summary>
    [XmlRoot("ThemeSettings")]    
    public class ThemeConfig
    {
    	 #region Properties
        public string ThemeName { get; set; } = "";
        public string ConfigPath { get; set; } = "";
        public DateTime ConfigTimestamp { get; set; }

        [XmlElement] 
        public string Author { get; set; } = "";

        [XmlElement] 
        public string AuthorUrl { get; set; } = "";

        [XmlElement] 
        public string SourceUrl { get; set; } = "";

        [XmlElement] 
        public string CssFramework { get; set; } = "";

        [XmlElement] 
        public string GridRenderer { get; set; } = "";

        [XmlElement] 
        public string Description { get; set; } = "";
  #endregion

    }
}
