namespace Dragonfly.UmbracoTheming
{
    using Dragonfly.UmbracoServices;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Umbraco.Cms.Core.Cache;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Cms.Web.Common;
    using IHostingEnvironment = Umbraco.Cms.Core.Hosting.IHostingEnvironment;

    public class DependencyLoader
    {
        public IWebHostEnvironment HostingEnvironment { get; }
        public IHttpContextAccessor ContextAccessor { get; }
        public IUmbracoContextAccessor UmbracoContextAccessor { get; }
     
        public UmbracoHelper UmbHelper;
        public HttpContext Context;
        public ServiceContext Services;

        public AppCaches AppCaches;
        //public FileHelperService DragonflyFileHelperService { get; }

        public IConfiguration AppSettingsConfig;

        public DependencyLoader(
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor contextAccessor,
            IUmbracoContextAccessor umbracoContextAccessor,
            //FileHelperService fileHelperService,
            ServiceContext serviceContext,
            AppCaches appCaches,
            IConfiguration appSettingsConfig
           )
        {
            HostingEnvironment = hostingEnvironment;
            ContextAccessor = contextAccessor;
            UmbracoContextAccessor = umbracoContextAccessor;
            UmbHelper = contextAccessor.HttpContext.RequestServices.GetRequiredService<UmbracoHelper>();
            //DragonflyFileHelperService = fileHelperService;
            Context = contextAccessor.HttpContext;
            Services = serviceContext;
            AppCaches = appCaches;
            AppSettingsConfig = appSettingsConfig;
        }
    }
}
