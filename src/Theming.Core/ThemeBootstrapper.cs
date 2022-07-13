namespace Dragonfly.Umbraco8Theming
{
    using System.Web.Configuration;
    using Dragonfly.UmbracoTheming;
    using Umbraco.Core.Composing;
    using Umbraco.Web;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Set the default RenderMVCcontroller to be the Theme Controller. This will allow us to set the theme templates
    /// </summary>
    public class ThemeBootstrapper : IUserComposer
    {
        public void Compose(Composition composition)
        {
            var useDefaultThemeController = WebConfigurationManager.AppSettings["Dragonfly.EnableDefaultThemeController"];
            if (useDefaultThemeController != null)
            {
                if (useDefaultThemeController.ToLower() == "true")
                {
                    composition.SetDefaultRenderMvcController(typeof(DefaultThemeController));
                    Current.Logger.Info<ThemeBootstrapper>("Default Controller set to 'DefaultThemeController'");
                }
            }
        }

    }
}
