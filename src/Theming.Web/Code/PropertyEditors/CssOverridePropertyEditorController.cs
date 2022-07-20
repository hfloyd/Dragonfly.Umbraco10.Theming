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
    using Umbraco.Cms.Web.BackOffice.Controllers;
    using Umbraco.Cms.Web.Common.Attributes;

    //  /Umbraco/backoffice/Dragonfly/CssOverridePropertyEditor/
    [PluginController("Dragonfly")]
    [IsBackOffice]
    public class CssOverridePropertyEditorController : UmbracoAuthorizedApiController
    {
        private readonly ILogger<CssOverridePropertyEditorController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ThemeHelperService _themeHelperService;
        public CssOverridePropertyEditorController(
            ILogger<CssOverridePropertyEditorController> logger,
            IWebHostEnvironment hostingEnvironment,
            ThemeHelperService themeHelperService
            )
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _themeHelperService = themeHelperService;
        }

        //  /Umbraco/backoffice/Dragonfly/CssOverridePropertyEditor/GetFiles
        [HttpGet]
        public IEnumerable<string> GetFiles()
        {
            //var themeDirName = _themeHelperService.ThemingConfigOptions().ThemesRootFolder.EnsureEndsWith('/');
            var themesRoot = _themeHelperService.GetAllThemesRoot(true).EnsureEndsWith('/');
            var dir = $"{themesRoot}~CssOverrides";

            //return Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
            var allFiles = Directory.GetFiles(dir).Select(x => new FileInfo(x));
            var cssFiles = allFiles.Where(x => x.Extension == ".css");

            //Add an empty option
            var listFiles = new List<string>();
            listFiles.Add("");
            listFiles.AddRange(cssFiles.Select(x=> x.Name));

            return listFiles;
        }
        #region Tests & Examples

        //  /Umbraco/backoffice/Dragonfly/CssOverridePropertyEditor/Test
        [HttpGet]
        public bool Test()
        {
            return true;
        }
        #endregion

    }

}
