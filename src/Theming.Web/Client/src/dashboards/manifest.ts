export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Dragonfly Umbraco 10 Theming Dashboard",
    alias: "DragonflyUmbracoTheming.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element.js"),
    meta: {
      label: "Example Dashboard",
      pathname: "example-dashboard",
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content",
      },
    ],
  },
];
