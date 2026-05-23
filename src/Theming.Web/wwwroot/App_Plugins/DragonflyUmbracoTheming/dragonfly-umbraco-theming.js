const a = [
  {
    name: "Dragonfly Umbraco 10 Theming Entrypoint",
    alias: "DragonflyUmbracoTheming.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-CK4R624y.js")
  }
], n = [
  {
    name: "Dragonfly Umbraco 10 Theming Dashboard",
    alias: "DragonflyUmbracoTheming.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element-DUiJrqFu.js"),
    meta: {
      label: "Example Dashboard",
      pathname: "example-dashboard"
    },
    conditions: [
      {
        alias: "Umb.Condition.SectionAlias",
        match: "Umb.Section.Content"
      }
    ]
  }
], o = [
  ...a,
  ...n
];
export {
  o as manifests
};
//# sourceMappingURL=dragonfly-umbraco-theming.js.map
