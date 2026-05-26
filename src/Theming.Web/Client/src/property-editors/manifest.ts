export const manifests: Array<UmbExtensionManifest> = [
    {
        type: 'propertyEditorUi',
        alias: 'DragonflyUmbracoTheming.PropertyEditorUi.ThemePicker',
        name: 'Dragonfly Theme Picker',
        element: () => import('./theme-picker.element.js'),
        elementName: 'dragonfly-theme-picker',
        meta: {
            label: 'Dragonfly Theme Picker',
            icon: 'icon-brush',
            group: 'common',
            propertyEditorSchemaAlias: 'Umbraco.Plain.String',
        },
    },
    {
        type: 'propertyEditorUi',
        alias: 'DragonflyUmbracoTheming.PropertyEditorUi.CssOverridePicker',
        name: 'Dragonfly CSS Override Picker',
        element: () => import('./css-override-picker.element.js'),
        elementName: 'dragonfly-css-override-picker',
        meta: {
            label: 'Dragonfly CSS Override Picker',
            icon: 'icon-css',
            group: 'common',
            propertyEditorSchemaAlias: 'Umbraco.Plain.String',
        },
    },
];
