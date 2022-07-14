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

    public class ThemePropertyEditorApiController : UmbracoApiController
    {
        private readonly ILogger<ThemePropertyEditorApiController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ThemePropertyEditorApiController(
            ILogger<ThemePropertyEditorApiController> logger,
            IWebHostEnvironment hostingEnvironment
        )
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        //  /Umbraco/Api/ThemePropertyEditorApi/GetThemes
        [HttpGet]
        public IEnumerable<string> GetThemes()
        {
            var dir = _hostingEnvironment.MapPathWebRoot("~/Themes");
            var allDirs =  Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
            allDirs = allDirs.Where(x => !x.StartsWith("~"));
            return allDirs;
        }
    }
  
}
