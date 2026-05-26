import {
    LitElement,
    css,
    html,
    customElement,
    property,
    state,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import { DragonflyUmbracoThemingService } from '../api/index.js';

@customElement('dragonfly-theme-picker')
export class DragonflyThemePickerElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement
{
    @property({ type: String })
    public value = '';

    @state()
    private _themes: string[] = [];

    @state()
    private _loading = true;

    override async connectedCallback() {
        super.connectedCallback();
        await this.#loadThemes();
    }

    async #loadThemes() {
        const { data, error } = await DragonflyUmbracoThemingService.getThemes();

        if (error) {
            console.error('ThemePicker: failed to load themes', error);
        } else if (data) {
            this._themes = data;
        }
        this._loading = false;
    }

    #onChange(e: Event) {
        this.value = (e.target as HTMLElement & { value: string }).value;
        this.dispatchEvent(new UmbChangeEvent());
    }

    render() {
        if (this._loading) {
            return html`<uui-loader></uui-loader>`;
        }

        if (this._themes.length === 0) {
            return html`
                <p class="no-themes">
                    No themes found. Add a folder to the Themes directory to make it available here.
                </p>
            `;
        }

        const options = this._themes.map((theme) => ({
            name: theme,
            value: theme,
            selected: theme === this.value,
        }));

        return html`
            <uui-select
                .options=${options}
                @change=${this.#onChange}
            ></uui-select>
        `;
    }

    static styles = css`
        :host {
            display: block;
        }

        uui-select {
            width: 100%;
        }

        .no-themes {
            color: var(--uui-color-danger);
            margin: 0;
        }
    `;
}

export default DragonflyThemePickerElement;

declare global {
    interface HTMLElementTagNameMap {
        'dragonfly-theme-picker': DragonflyThemePickerElement;
    }
}
