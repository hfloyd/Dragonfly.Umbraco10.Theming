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

@customElement('dragonfly-css-override-picker')
export class DragonflyCssOverridePickerElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement
{
    @property({ type: String })
    public value = '';

    @state()
    private _files: string[] = [];

    @state()
    private _loading = true;

    override async connectedCallback() {
        super.connectedCallback();
        await this.#loadFiles();
    }

    async #loadFiles() {
        const { data, error } = await DragonflyUmbracoThemingService.getUmbracoDragonflythemingApiV1CssOverrides();

        if (error) {
            console.error('CssOverridePicker: failed to load CSS overrides', error);
        } else if (data) {
            this._files = data;
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

        if (this._files.length === 0) {
            return html`
                <p class="no-files">
                    No CSS override files found. Add <code>.css</code> files to
                    <code>wwwroot/Themes/~CssOverrides/</code> to make them available here.
                </p>
            `;
        }

        const options = [
            { name: '(none)', value: '', selected: this.value === '' },
            ...this._files.map((file) => ({
                name: file,
                value: file,
                selected: file === this.value,
            })),
        ];

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

        .no-files {
            color: var(--uui-color-danger);
            margin: 0;
        }

        code {
            background: var(--uui-color-surface-emphasis);
            padding: 0.1em 0.35em;
            border-radius: 3px;
            font-size: 0.9em;
        }
    `;
}

export default DragonflyCssOverridePickerElement;

declare global {
    interface HTMLElementTagNameMap {
        'dragonfly-css-override-picker': DragonflyCssOverridePickerElement;
    }
}
