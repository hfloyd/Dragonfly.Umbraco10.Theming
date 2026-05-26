using Asp.Versioning;
using Dragonfly.UmbracoTheming;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;

namespace Dragonfly.UmbracoTheming.Controllers
{
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "DragonflyUmbracoTheming")]
    public class DragonflyThemingApiController : DragonflyThemingApiControllerBase
    {
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly DragonflyThemingConfig _themingConfig;

        public DragonflyThemingApiController(
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration)
        {
            _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            _hostEnvironment = hostEnvironment;
            _themingConfig = new DragonflyThemingConfig();
            configuration.GetSection(DragonflyThemingConfig.DragonflyTheming).Bind(_themingConfig);
        }

        [HttpGet("ping")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public string Ping() => "Pong";

        [HttpGet("whatsTheTimeMrWolf")]
        [ProducesResponseType(typeof(DateTime), 200)]
        public DateTime WhatsTheTimeMrWolf() => DateTime.Now;

        [HttpGet("whatsMyName")]
        [ProducesResponseType<string>(StatusCodes.Status200OK)]
        public string WhatsMyName()
        {
            // So we can see a long request in the dashboard with a spinning progress wheel
            Thread.Sleep(2000);

            var currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            return currentUser?.Name ?? "I have no idea who you are";
        }

        [HttpGet("whoAmI")]
        [ProducesResponseType<IUser>(StatusCodes.Status200OK)]
        public IUser? WhoAmI() => _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;

        [HttpGet("themes")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        public IEnumerable<string> GetThemes()
        {
            var basePath = _themingConfig.ThemeViewsAreInWwwRoot
                ? _hostEnvironment.WebRootPath
                : _hostEnvironment.ContentRootPath;

            var themesPath = Path.Combine(basePath, _themingConfig.ThemesRootFolderName);

            if (!Directory.Exists(themesPath))
                return [];

            return Directory.GetDirectories(themesPath)
                .Select(Path.GetFileName)
                .Where(name => name is not null && !name.StartsWith('~'))
                .OrderBy(name => name)
                .ToList()!;
        }

        [HttpGet("css-overrides")]
        [ProducesResponseType<IEnumerable<string>>(StatusCodes.Status200OK)]
        public IEnumerable<string> GetCssOverrides()
        {
            // CSS overrides are always static files in wwwroot, regardless of ThemeViewsAreInWwwRoot
            var overridesPath = Path.Combine(
                _hostEnvironment.WebRootPath,
                _themingConfig.ThemesRootFolderName,
                "~CssOverrides");

            if (!Directory.Exists(overridesPath))
                return [];

            return Directory.GetFiles(overridesPath, "*.css")
                .Select(Path.GetFileName)
                .Where(name => name is not null)
                .OrderBy(name => name)
                .ToList()!;
        }
    }
}
