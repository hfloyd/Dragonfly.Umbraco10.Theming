import { LitElement as m, html as d, css as _, property as g, state as h, customElement as y } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as C } from "@umbraco-cms/backoffice/element-api";
import { UmbChangeEvent as w } from "@umbraco-cms/backoffice/event";
import { D as E } from "./sdk.gen-__6zMVnj.js";
var O = Object.defineProperty, b = Object.getOwnPropertyDescriptor, p = (r) => {
  throw TypeError(r);
}, n = (r, e, t, o) => {
  for (var s = o > 1 ? void 0 : o ? b(e, t) : e, l = r.length - 1, c; l >= 0; l--)
    (c = r[l]) && (s = (o ? c(e, t, s) : c(s)) || s);
  return o && s && O(e, t, s), s;
}, k = (r, e, t) => e.has(r) || p("Cannot " + t), D = (r, e, t) => e.has(r) ? p("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(r) : e.set(r, t), u = (r, e, t) => (k(r, e, "access private method"), t), i, f, v;
let a = class extends C(m) {
  constructor() {
    super(...arguments), D(this, i), this.value = "", this._files = [], this._loading = !0;
  }
  async connectedCallback() {
    super.connectedCallback(), await u(this, i, f).call(this);
  }
  render() {
    if (this._loading)
      return d`<uui-loader></uui-loader>`;
    if (this._files.length === 0)
      return d`
                <p class="no-files">
                    No CSS override files found. Add <code>.css</code> files to
                    <code>wwwroot/Themes/~CssOverrides/</code> to make them available here.
                </p>
            `;
    const r = [
      { name: "(none)", value: "", selected: this.value === "" },
      ...this._files.map((e) => ({
        name: e,
        value: e,
        selected: e === this.value
      }))
    ];
    return d`
            <uui-select
                .options=${r}
                @change=${u(this, i, v)}
            ></uui-select>
        `;
  }
};
i = /* @__PURE__ */ new WeakSet();
f = async function() {
  const { data: r, error: e } = await E.getUmbracoDragonflythemingApiV1CssOverrides();
  e ? console.error("CssOverridePicker: failed to load CSS overrides", e) : r && (this._files = r), this._loading = !1;
};
v = function(r) {
  this.value = r.target.value, this.dispatchEvent(new w());
};
a.styles = _`
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
n([
  g({ type: String })
], a.prototype, "value", 2);
n([
  h()
], a.prototype, "_files", 2);
n([
  h()
], a.prototype, "_loading", 2);
a = n([
  y("dragonfly-css-override-picker")
], a);
const A = a;
export {
  a as DragonflyCssOverridePickerElement,
  A as default
};
//# sourceMappingURL=css-override-picker.element-CXSV3N4J.js.map
