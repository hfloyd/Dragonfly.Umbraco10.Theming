namespace Dragonfly.UmbracoTheming
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco.Extensions;


    public class CssOverridePropertyEditorApiController : UmbracoApiController
    {
        private readonly ILogger<CssOverridePropertyEditorApiController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public CssOverridePropertyEditorApiController(
            ILogger<CssOverridePropertyEditorApiController> logger,
            IWebHostEnvironment hostingEnvironment
            )
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }
        
        //  /Umbraco/Api/CssOverridePropertyEditorApi/GetFiles
        [HttpGet]
        public IEnumerable<string> GetFiles()
        {
           var dir = _hostingEnvironment.MapPathWebRoot("Themes/~CssOverrides");
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
