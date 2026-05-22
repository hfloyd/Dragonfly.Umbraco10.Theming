export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Dragonfly Umbraco 10 Theming Entrypoint",
    alias: "DragonflyUmbracoTheming.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
