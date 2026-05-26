const e = [
  {
    name: "Dragonfly Umbraco 10 Theming Entrypoint",
    alias: "DragonflyUmbracoTheming.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint-CK4R624y.js")
  }
], r = [
  {
    name: "Dragonfly Umbraco 10 Theming Dashboard",
    alias: "DragonflyUmbracoTheming.Dashboard",
    type: "dashboard",
    js: () => import("./dashboard.element-BN_9a9go.js"),
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
], a = [
  {
    type: "propertyEditorUi",
    alias: "DragonflyUmbracoTheming.PropertyEditorUi.ThemePicker",
    name: "Dragonfly Theme Picker",
    element: () => import("./theme-picker.element-BjVHDbJR.js"),
    elementName: "dragonfly-theme-picker",
    meta: {
      label: "Dragonfly Theme Picker",
      icon: "icon-brush",
      group: "common",
      propertyEditorSchemaAlias: "Umbraco.Plain.String"
    }
  },
  {
    type: "propertyEditorUi",
    alias: "DragonflyUmbracoTheming.PropertyEditorUi.CssOverridePicker",
    name: "Dragonfly CSS Override Picker",
    element: () => import("./css-override-picker.element-CXSV3N4J.js"),
    elementName: "dragonfly-css-override-picker",
    meta: {
      label: "Dragonfly CSS Override Picker",
      icon: "icon-css",
      group: "common",
      propertyEditorSchemaAlias: "Umbraco.Plain.String"
    }
  }
], o = [
  ...e,
  ...r,
  ...a
];
export {
  o as manifests
};
//# sourceMappingURL=dragonfly-umbraco-theming.js.map
