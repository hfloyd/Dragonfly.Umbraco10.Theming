namespace Dragonfly.Umbraco8Theming.PropertyEditors
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Http;
    using Umbraco.Core.IO;
    using Umbraco.Web.WebApi;

    public class CssOverridePropertyEditorApiController : UmbracoApiController
    {
        //  /Umbraco/Api/CssOverridePropertyEditorApi/GetFiles
        [HttpGet]
        public IEnumerable<string> GetFiles()
        {
            var dir = IOHelper.MapPath("~/Themes/~CssOverrides");
            //return Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
            var allFiles = Directory.GetFiles(dir).Select(x => new FileInfo(x).Name);
            
            //Add an empty option
            var listFiles = new List<string>();
            listFiles.Add("");
            listFiles.AddRange(allFiles);

            return listFiles;
        }
    }
  
}
