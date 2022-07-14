#pragma warning disable 1591

namespace Dragonfly.UmbracoTheming
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Umbraco.Cms.Core.Composing;
    using Umbraco.Cms.Core.DependencyInjection;
    using Umbraco.Cms.Web.Website.Controllers;

    public class SetupComposer : IComposer
    {
        private readonly ILogger<SetupComposer> _logger;
        private readonly IConfiguration _AppSettingsConfig;
        public SetupComposer(
            ILogger<SetupComposer> logger,
            IConfiguration AppSettingsConfig
            )
        {
            _logger = logger;
            _AppSettingsConfig = AppSettingsConfig;
        }
        public void Compose(IUmbracoBuilder builder)
        {
            // builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddMvcCore().AddRazorViewEngine();
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();
            //builder.Services.AddSingleton<IRazorViewEngine>();
            //  builder.Services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            // builder.Services.AddScoped<IServiceProvider, ServiceProvider>();

            builder.Services.AddHttpContextAccessor();

            //  builder.Services.AddScoped<IViewRenderService, Dragonfly.NetHelperServices.ViewRenderService>();
            // builder.Services.AddScoped<Dragonfly.UmbracoServices.FileHelperService>();

            builder.Services.AddScoped<DependencyLoader>();
            builder.Services.AddScoped<ThemeHelperService>();
            CheckAddThemeBootstrapper(builder, _AppSettingsConfig);

            //builder.AddUmbracoOptions<Settings>();

        }

        /// <summary>
        /// If enabled in appSettings config,
        /// set the default RenderController to be the Theme Controller.
        /// This will allow us to set the theme templates automatically.
        /// </summary>
        public void CheckAddThemeBootstrapper(IUmbracoBuilder builder, IConfiguration AppSettingsConfig)
        {
            var useDefaultThemeController = AppSettingsConfig["Dragonfly.Theming.EnableDefaultThemeController"];
            if (useDefaultThemeController != null)
            {
                if (useDefaultThemeController.ToLower() == "true")
                {
                    // Configure Umbraco Render Controller Type
                    builder.Services.Configure<UmbracoRenderingDefaultsOptions>(c =>
                    {
                        c.DefaultControllerType = typeof(DefaultThemeController);
                    });
                    // builder.SetDefaultRenderMvcController(typeof(DefaultThemeController));
                    _logger.LogInformation("Default Controller set to 'DefaultThemeController'");
                }
            }
        }
    }

}