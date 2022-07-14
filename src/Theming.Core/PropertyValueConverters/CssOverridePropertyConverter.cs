namespace Dragonfly.UmbracoTheming
{
    using System;
    using Umbraco.Cms.Core.Models.PublishedContent;
    using Umbraco.Cms.Core.PropertyEditors;

    public class CssOverridePropertyConverter : IPropertyValueConverter
    {

        public bool IsConverter(IPublishedPropertyType propertyType)
        {
            return propertyType.EditorAlias.Equals("Dragonfly.Theming.CssOverridePicker"); 
        }

        public Type GetPropertyValueType(IPublishedPropertyType propertyType)
        {
            return typeof(string);
        }

        public PropertyCacheLevel GetPropertyCacheLevel(IPublishedPropertyType propertyType)
        {
            return PropertyCacheLevel.Element;
        }

        public bool? IsValue(object value, PropertyValueLevel level)
        {
            switch (level)
            {
                case PropertyValueLevel.Source:
                    return value != null && value is string;
                default:
                    throw new NotSupportedException($"Invalid level: {level}.");
            }
        }

        public object? ConvertSourceToIntermediate(IPublishedElement owner, IPublishedPropertyType propertyType, object? source, bool preview)
        {
            string returnVal = "";

            if (source == null)
            {
                return returnVal;
            }

            returnVal = source.ToString();

            return returnVal;
        }

        public object ConvertIntermediateToObject(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object? inter, bool preview)
        {
            if (inter == null)
            {
                return "";
            }

            if (inter is string)
            {
                return inter;
            }

            return "";
        }

        public object ConvertIntermediateToXPath(IPublishedElement owner, IPublishedPropertyType propertyType, PropertyCacheLevel referenceCacheLevel, object inter, bool preview)
        {
            if (inter == null)
            {
                return "";
            }

            return inter.ToString();
        }

    }
}
