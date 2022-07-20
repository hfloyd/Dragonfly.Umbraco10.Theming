namespace Dragonfly.UmbracoTheming
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Web.Common.Attributes;
    using Umbraco.Cms.Web.BackOffice.Controllers;
    using Umbraco.Cms.Web.Common.Controllers;
    using Umbraco.Extensions;


    //  /Umbraco/backoffice/Dragonfly/ThemePropertyEditor/
    [PluginController("Dragonfly")]
    [IsBackOffice]
    public class ThemePropertyEditorController : UmbracoAuthorizedApiController
    {
        private readonly ILogger<ThemePropertyEditorController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ThemeHelperService _themeHelperService;

        public ThemePropertyEditorController(
            ILogger<ThemePropertyEditorController> logger,
            IWebHostEnvironment hostingEnvironment,
            ThemeHelperService themeHelperService
        )
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _themeHelperService = themeHelperService;
        }


        //  /Umbraco/backoffice/Dragonfly/ThemePropertyEditor/GetThemes
        [HttpGet]
        public IEnumerable<string> GetThemes()
        {
           // var themeDirName = _themeHelperService. //_themeHelperService.ThemingConfigOptions().ThemesRootFolder;
           var dir = _themeHelperService.GetAllThemesRoot(true);// _hostingEnvironment.MapPathWebRoot(themeDirName);

            var allDirs = Directory.GetDirectories(dir).Select(x => new DirectoryInfo(x).Name);
            allDirs = allDirs.Where(x => !x.StartsWith("~"));
            return allDirs;
        }

        #region Tests & Examples

        //  /Umbraco/backoffice/Dragonfly/ThemePropertyEditor/Test
        [HttpGet]
        public bool Test()
        {
            return true;
        }
        #endregion
    }

}
