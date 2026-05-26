import { LitElement as _, html as c, css as v, property as g, state as p, customElement as y } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as E } from "@umbraco-cms/backoffice/element-api";
import { UmbChangeEvent as T } from "@umbraco-cms/backoffice/event";
import { D as w } from "./sdk.gen-__6zMVnj.js";
var C = Object.defineProperty, D = Object.getOwnPropertyDescriptor, u = (t) => {
  throw TypeError(t);
}, i = (t, e, a, s) => {
  for (var r = s > 1 ? void 0 : s ? D(e, a) : e, l = t.length - 1, h; l >= 0; l--)
    (h = t[l]) && (r = (s ? h(e, a, r) : h(r)) || r);
  return s && r && C(e, a, r), r;
}, P = (t, e, a) => e.has(t) || u("Cannot " + a), k = (t, e, a) => e.has(t) ? u("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, a), m = (t, e, a) => (P(t, e, "access private method"), a), n, d, f;
let o = class extends E(_) {
  constructor() {
    super(...arguments), k(this, n), this.value = "", this._themes = [], this._loading = !0;
  }
  async connectedCallback() {
    super.connectedCallback(), await m(this, n, d).call(this);
  }
  render() {
    if (this._loading)
      return c`<uui-loader></uui-loader>`;
    if (this._themes.length === 0)
      return c`
                <p class="no-themes">
                    No themes found. Add a folder to the Themes directory to make it available here.
                </p>
            `;
    const t = this._themes.map((e) => ({
      name: e,
      value: e,
      selected: e === this.value
    }));
    return c`
            <uui-select
                .options=${t}
                @change=${m(this, n, f)}
            ></uui-select>
        `;
  }
};
n = /* @__PURE__ */ new WeakSet();
d = async function() {
  const { data: t, error: e } = await w.getUmbracoDragonflythemingApiV1Themes();
  e ? console.error("ThemePicker: failed to load themes", e) : t && (this._themes = t), this._loading = !1;
};
f = function(t) {
  this.value = t.target.value, this.dispatchEvent(new T());
};
o.styles = v`
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
i([
  g({ type: String })
], o.prototype, "value", 2);
i([
  p()
], o.prototype, "_themes", 2);
i([
  p()
], o.prototype, "_loading", 2);
o = i([
  y("dragonfly-theme-picker")
], o);
const x = o;
export {
  o as DragonflyThemePickerElement,
  x as default
};
//# sourceMappingURL=theme-picker.element-BjVHDbJR.js.map
