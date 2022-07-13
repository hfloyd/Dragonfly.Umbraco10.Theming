namespace Dragonfly.Umbraco8Theming.PropertyEditors
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Http;
    using Umbraco.Core.IO;
    using Umbraco.Web.WebApi;

    public class ThemePropertyEditorApiController : UmbracoApiController
    {
        //  /Umbraco/Api/ThemePropertyEditorApi/GetThemes
        [HttpGet]
        public IEnumerable<string> GetThemes()
        {
            var dir = IOHelper.MapPath("~/Themes");
            var allDirs =  Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
            allDirs = allDirs.Where(x => !x.StartsWith("~"));
            return allDirs;
        }
    }
  
}
