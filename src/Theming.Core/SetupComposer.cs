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

            var logger = builder.BuilderLoggerFactory.CreateLogger("SetupComposer");

            builder.Services.AddScoped<DependencyLoader>();
            builder.Services.AddScoped<ThemeHelperService>();

            CheckAddThemeBootstrapper(builder);

            //builder.AddUmbracoOptions<Settings>();

        }

        /// <summary>
        /// If enabled in appSettings config,
        /// set the default RenderController to be the Theme Controller.
        /// This will allow us to set the theme templates automatically.
        /// </summary>
        public void CheckAddThemeBootstrapper(IUmbracoBuilder builder)
        {
            var options = new DragonflyThemingConfig();
            builder.Config.GetSection(DragonflyThemingConfig.DragonflyTheming).Bind(options);
  
            var useDefaultThemeController = options.EnableDefaultThemeController;
          
            if (useDefaultThemeController)
            {
                // Configure Umbraco Render Controller Type
                builder.Services.Configure<UmbracoRenderingDefaultsOptions>(c =>
                {
                    c.DefaultControllerType = typeof(DefaultThemeController);
                });
                var logger = builder.BuilderLoggerFactory.CreateLogger("SetupComposer");
                logger.Log(LogLevel.Information,"Dragonfly DefaultThemeController set");
            }
     
        }
    }

}