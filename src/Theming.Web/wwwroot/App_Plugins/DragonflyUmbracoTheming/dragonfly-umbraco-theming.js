import { UMB_AUTH_CONTEXT as xe } from "@umbraco-cms/backoffice/auth";
import { LitElement as X, html as w, css as G, state as E, customElement as Q, property as ie } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin as Y } from "@umbraco-cms/backoffice/element-api";
import { UMB_NOTIFICATION_CONTEXT as ke } from "@umbraco-cms/backoffice/notification";
import { UMB_CURRENT_USER_CONTEXT as Oe } from "@umbraco-cms/backoffice/current-user";
import { UmbChangeEvent as ce } from "@umbraco-cms/backoffice/event";
import { umbHttpClient as Pe } from "@umbraco-cms/backoffice/http-client";
const $e = [
  {
    name: "Dragonfly Umbraco 10 Theming Entrypoint",
    alias: "DragonflyUmbracoTheming.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => Promise.resolve().then(() => et)
  }
], Ue = [
  {
    name: "Dragonfly Umbraco 10 Theming Dashboard",
    alias: "DragonflyUmbracoTheming.Dashboard",
    type: "dashboard",
    js: () => Promise.resolve().then(() => nt),
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
], De = [
  {
    type: "propertyEditorUi",
    alias: "DragonflyUmbracoTheming.PropertyEditorUi.ThemePicker",
    name: "Dragonfly Theme Picker",
    element: () => Promise.resolve().then(() => dt),
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
    element: () => Promise.resolve().then(() => bt),
    elementName: "dragonfly-css-override-picker",
    meta: {
      label: "Dragonfly CSS Override Picker",
      icon: "icon-css",
      group: "common",
      propertyEditorSchemaAlias: "Umbraco.Plain.String"
    }
  }
], Tt = [
  ...$e,
  ...Ue,
  ...De
], je = {
  bodySerializer: (t) => JSON.stringify(
    t,
    (e, r) => typeof r == "bigint" ? r.toString() : r
  )
}, ze = ({
  onRequest: t,
  onSseError: e,
  onSseEvent: r,
  responseTransformer: s,
  responseValidator: a,
  sseDefaultRetryDelay: c,
  sseMaxRetryAttempts: n,
  sseMaxRetryDelay: i,
  sseSleepFn: l,
  url: d,
  ...o
}) => {
  let h;
  const P = l ?? ((u) => new Promise((m) => setTimeout(m, u)));
  return { stream: async function* () {
    let u = c ?? 3e3, m = 0;
    const T = o.signal ?? new AbortController().signal;
    for (; !T.aborted; ) {
      m++;
      const $ = o.headers instanceof Headers ? o.headers : new Headers(o.headers);
      h !== void 0 && $.set("Last-Event-ID", h);
      try {
        const x = {
          redirect: "follow",
          ...o,
          body: o.serializedBody,
          headers: $,
          signal: T
        };
        let b = new Request(d, x);
        t && (b = await t(d, x));
        const p = await (o.fetch ?? globalThis.fetch)(b);
        if (!p.ok)
          throw new Error(
            `SSE failed: ${p.status} ${p.statusText}`
          );
        if (!p.body) throw new Error("No body in SSE response");
        const g = p.body.pipeThrough(new TextDecoderStream()).getReader();
        let H = "";
        const K = () => {
          try {
            g.cancel();
          } catch {
          }
        };
        T.addEventListener("abort", K);
        try {
          for (; ; ) {
            const { done: Ce, value: Se } = await g.read();
            if (Ce) break;
            H += Se;
            const Z = H.split(`

`);
            H = Z.pop() ?? "";
            for (const Ee of Z) {
              const Te = Ee.split(`
`), N = [];
              let ee;
              for (const y of Te)
                if (y.startsWith("data:"))
                  N.push(y.replace(/^data:\s*/, ""));
                else if (y.startsWith("event:"))
                  ee = y.replace(/^event:\s*/, "");
                else if (y.startsWith("id:"))
                  h = y.replace(/^id:\s*/, "");
                else if (y.startsWith("retry:")) {
                  const re = Number.parseInt(
                    y.replace(/^retry:\s*/, ""),
                    10
                  );
                  Number.isNaN(re) || (u = re);
                }
              let k, te = !1;
              if (N.length) {
                const y = N.join(`
`);
                try {
                  k = JSON.parse(y), te = !0;
                } catch {
                  k = y;
                }
              }
              te && (a && await a(k), s && (k = await s(k))), r?.({
                data: k,
                event: ee,
                id: h,
                retry: u
              }), N.length && (yield k);
            }
          }
        } finally {
          T.removeEventListener("abort", K), g.releaseLock();
        }
        break;
      } catch (x) {
        if (e?.(x), n !== void 0 && m >= n)
          break;
        const b = Math.min(
          u * 2 ** (m - 1),
          i ?? 3e4
        );
        await P(b);
      }
    }
  }() };
}, Ae = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, Ne = (t) => {
  switch (t) {
    case "form":
      return ",";
    case "pipeDelimited":
      return "|";
    case "spaceDelimited":
      return "%20";
    default:
      return ",";
  }
}, We = (t) => {
  switch (t) {
    case "label":
      return ".";
    case "matrix":
      return ";";
    case "simple":
      return ",";
    default:
      return "&";
  }
}, le = ({
  allowReserved: t,
  explode: e,
  name: r,
  style: s,
  value: a
}) => {
  if (!e) {
    const i = (t ? a : a.map((l) => encodeURIComponent(l))).join(Ne(s));
    switch (s) {
      case "label":
        return `.${i}`;
      case "matrix":
        return `;${r}=${i}`;
      case "simple":
        return i;
      default:
        return `${r}=${i}`;
    }
  }
  const c = Ae(s), n = a.map((i) => s === "label" || s === "simple" ? t ? i : encodeURIComponent(i) : q({
    allowReserved: t,
    name: r,
    value: i
  })).join(c);
  return s === "label" || s === "matrix" ? c + n : n;
}, q = ({
  allowReserved: t,
  name: e,
  value: r
}) => {
  if (r == null)
    return "";
  if (typeof r == "object")
    throw new Error(
      "Deeply-nested arrays/objects aren’t supported. Provide your own `querySerializer()` to handle these."
    );
  return `${e}=${t ? r : encodeURIComponent(r)}`;
}, ue = ({
  allowReserved: t,
  explode: e,
  name: r,
  style: s,
  value: a,
  valueOnly: c
}) => {
  if (a instanceof Date)
    return c ? a.toISOString() : `${r}=${a.toISOString()}`;
  if (s !== "deepObject" && !e) {
    let l = [];
    Object.entries(a).forEach(([o, h]) => {
      l = [
        ...l,
        o,
        t ? h : encodeURIComponent(h)
      ];
    });
    const d = l.join(",");
    switch (s) {
      case "form":
        return `${r}=${d}`;
      case "label":
        return `.${d}`;
      case "matrix":
        return `;${r}=${d}`;
      default:
        return d;
    }
  }
  const n = We(s), i = Object.entries(a).map(
    ([l, d]) => q({
      allowReserved: t,
      name: s === "deepObject" ? `${r}[${l}]` : l,
      value: d
    })
  ).join(n);
  return s === "label" || s === "matrix" ? n + i : i;
}, Ie = /\{[^{}]+\}/g, Me = ({ path: t, url: e }) => {
  let r = e;
  const s = e.match(Ie);
  if (s)
    for (const a of s) {
      let c = !1, n = a.substring(1, a.length - 1), i = "simple";
      n.endsWith("*") && (c = !0, n = n.substring(0, n.length - 1)), n.startsWith(".") ? (n = n.substring(1), i = "label") : n.startsWith(";") && (n = n.substring(1), i = "matrix");
      const l = t[n];
      if (l == null)
        continue;
      if (Array.isArray(l)) {
        r = r.replace(
          a,
          le({ explode: c, name: n, style: i, value: l })
        );
        continue;
      }
      if (typeof l == "object") {
        r = r.replace(
          a,
          ue({
            explode: c,
            name: n,
            style: i,
            value: l,
            valueOnly: !0
          })
        );
        continue;
      }
      if (i === "matrix") {
        r = r.replace(
          a,
          `;${q({
            name: n,
            value: l
          })}`
        );
        continue;
      }
      const d = encodeURIComponent(
        i === "label" ? `.${l}` : l
      );
      r = r.replace(a, d);
    }
  return r;
}, qe = ({
  baseUrl: t,
  path: e,
  query: r,
  querySerializer: s,
  url: a
}) => {
  const c = a.startsWith("/") ? a : `/${a}`;
  let n = (t ?? "") + c;
  e && (n = Me({ path: e, url: n }));
  let i = r ? s(r) : "";
  return i.startsWith("?") && (i = i.substring(1)), i && (n += `?${i}`), n;
};
function Be(t) {
  const e = t.body !== void 0;
  if (e && t.bodySerializer)
    return "serializedBody" in t ? t.serializedBody !== void 0 && t.serializedBody !== "" ? t.serializedBody : null : t.body !== "" ? t.body : null;
  if (e)
    return t.body;
}
const Re = async (t, e) => {
  const r = typeof e == "function" ? await e(t) : e;
  if (r)
    return t.scheme === "bearer" ? `Bearer ${r}` : t.scheme === "basic" ? `Basic ${btoa(r)}` : r;
}, de = ({
  allowReserved: t,
  array: e,
  object: r
} = {}) => (a) => {
  const c = [];
  if (a && typeof a == "object")
    for (const n in a) {
      const i = a[n];
      if (i != null)
        if (Array.isArray(i)) {
          const l = le({
            allowReserved: t,
            explode: !0,
            name: n,
            style: "form",
            value: i,
            ...e
          });
          l && c.push(l);
        } else if (typeof i == "object") {
          const l = ue({
            allowReserved: t,
            explode: !0,
            name: n,
            style: "deepObject",
            value: i,
            ...r
          });
          l && c.push(l);
        } else {
          const l = q({
            allowReserved: t,
            name: n,
            value: i
          });
          l && c.push(l);
        }
    }
  return c.join("&");
}, He = (t) => {
  if (!t)
    return "stream";
  const e = t.split(";")[0]?.trim();
  if (e) {
    if (e.startsWith("application/json") || e.endsWith("+json"))
      return "json";
    if (e === "multipart/form-data")
      return "formData";
    if (["application/", "audio/", "image/", "video/"].some(
      (r) => e.startsWith(r)
    ))
      return "blob";
    if (e.startsWith("text/"))
      return "text";
  }
}, Fe = (t, e) => e ? !!(t.headers.has(e) || t.query?.[e] || t.headers.get("Cookie")?.includes(`${e}=`)) : !1, Le = async ({
  security: t,
  ...e
}) => {
  for (const r of t) {
    if (Fe(e, r.name))
      continue;
    const s = await Re(r, e.auth);
    if (!s)
      continue;
    const a = r.name ?? "Authorization";
    switch (r.in) {
      case "query":
        e.query || (e.query = {}), e.query[a] = s;
        break;
      case "cookie":
        e.headers.append("Cookie", `${a}=${s}`);
        break;
      default:
        e.headers.set(a, s);
        break;
    }
  }
}, ae = (t) => qe({
  baseUrl: t.baseUrl,
  path: t.path,
  query: t.query,
  querySerializer: typeof t.querySerializer == "function" ? t.querySerializer : de(t.querySerializer),
  url: t.url
}), se = (t, e) => {
  const r = { ...t, ...e };
  return r.baseUrl?.endsWith("/") && (r.baseUrl = r.baseUrl.substring(0, r.baseUrl.length - 1)), r.headers = he(t.headers, e.headers), r;
}, Ve = (t) => {
  const e = [];
  return t.forEach((r, s) => {
    e.push([s, r]);
  }), e;
}, he = (...t) => {
  const e = new Headers();
  for (const r of t) {
    if (!r)
      continue;
    const s = r instanceof Headers ? Ve(r) : Object.entries(r);
    for (const [a, c] of s)
      if (c === null)
        e.delete(a);
      else if (Array.isArray(c))
        for (const n of c)
          e.append(a, n);
      else c !== void 0 && e.set(
        a,
        typeof c == "object" ? JSON.stringify(c) : c
      );
  }
  return e;
};
class F {
  constructor() {
    this.fns = [];
  }
  clear() {
    this.fns = [];
  }
  eject(e) {
    const r = this.getInterceptorIndex(e);
    this.fns[r] && (this.fns[r] = null);
  }
  exists(e) {
    const r = this.getInterceptorIndex(e);
    return !!this.fns[r];
  }
  getInterceptorIndex(e) {
    return typeof e == "number" ? this.fns[e] ? e : -1 : this.fns.indexOf(e);
  }
  update(e, r) {
    const s = this.getInterceptorIndex(e);
    return this.fns[s] ? (this.fns[s] = r, e) : !1;
  }
  use(e) {
    return this.fns.push(e), this.fns.length - 1;
  }
}
const Je = () => ({
  error: new F(),
  request: new F(),
  response: new F()
}), Xe = de({
  allowReserved: !1,
  array: {
    explode: !0,
    style: "form"
  },
  object: {
    explode: !0,
    style: "deepObject"
  }
}), Ge = {
  "Content-Type": "application/json"
}, fe = (t = {}) => ({
  ...je,
  headers: Ge,
  parseAs: "auto",
  querySerializer: Xe,
  ...t
}), Qe = (t = {}) => {
  let e = se(fe(), t);
  const r = () => ({ ...e }), s = (d) => (e = se(e, d), r()), a = Je(), c = async (d) => {
    const o = {
      ...e,
      ...d,
      fetch: d.fetch ?? e.fetch ?? globalThis.fetch,
      headers: he(e.headers, d.headers),
      serializedBody: void 0
    };
    o.security && await Le({
      ...o,
      security: o.security
    }), o.requestValidator && await o.requestValidator(o), o.body !== void 0 && o.bodySerializer && (o.serializedBody = o.bodySerializer(o.body)), (o.body === void 0 || o.serializedBody === "") && o.headers.delete("Content-Type");
    const h = ae(o);
    return { opts: o, url: h };
  }, n = async (d) => {
    const { opts: o, url: h } = await c(d), P = {
      redirect: "follow",
      ...o,
      body: Be(o)
    };
    let v = new Request(h, P);
    for (const f of a.request.fns)
      f && (v = await f(v, o));
    const A = o.fetch;
    let u = await A(v);
    for (const f of a.response.fns)
      f && (u = await f(u, v, o));
    const m = {
      request: v,
      response: u
    };
    if (u.ok) {
      const f = (o.parseAs === "auto" ? He(u.headers.get("Content-Type")) : o.parseAs) ?? "json";
      if (u.status === 204 || u.headers.get("Content-Length") === "0") {
        let g;
        switch (f) {
          case "arrayBuffer":
          case "blob":
          case "text":
            g = await u[f]();
            break;
          case "formData":
            g = new FormData();
            break;
          case "stream":
            g = u.body;
            break;
          default:
            g = {};
            break;
        }
        return o.responseStyle === "data" ? g : {
          data: g,
          ...m
        };
      }
      let p;
      switch (f) {
        case "arrayBuffer":
        case "blob":
        case "formData":
        case "json":
        case "text":
          p = await u[f]();
          break;
        case "stream":
          return o.responseStyle === "data" ? u.body : {
            data: u.body,
            ...m
          };
      }
      return f === "json" && (o.responseValidator && await o.responseValidator(p), o.responseTransformer && (p = await o.responseTransformer(p))), o.responseStyle === "data" ? p : {
        data: p,
        ...m
      };
    }
    const T = await u.text();
    let $;
    try {
      $ = JSON.parse(T);
    } catch {
    }
    const x = $ ?? T;
    let b = x;
    for (const f of a.error.fns)
      f && (b = await f(x, u, v, o));
    if (b = b || {}, o.throwOnError)
      throw b;
    return o.responseStyle === "data" ? void 0 : {
      error: b,
      ...m
    };
  }, i = (d) => (o) => n({ ...o, method: d }), l = (d) => async (o) => {
    const { opts: h, url: P } = await c(o);
    return ze({
      ...h,
      body: h.body,
      headers: h.headers,
      method: d,
      onRequest: async (v, A) => {
        let u = new Request(v, A);
        for (const m of a.request.fns)
          m && (u = await m(u, h));
        return u;
      },
      url: P
    });
  };
  return {
    buildUrl: ae,
    connect: i("CONNECT"),
    delete: i("DELETE"),
    get: i("GET"),
    getConfig: r,
    head: i("HEAD"),
    interceptors: a,
    options: i("OPTIONS"),
    patch: i("PATCH"),
    post: i("POST"),
    put: i("PUT"),
    request: n,
    setConfig: s,
    sse: {
      connect: l("CONNECT"),
      delete: l("DELETE"),
      get: l("GET"),
      head: l("HEAD"),
      options: l("OPTIONS"),
      patch: l("PATCH"),
      post: l("POST"),
      put: l("PUT"),
      trace: l("TRACE")
    },
    trace: i("TRACE")
  };
}, Ye = (t) => ({
  ...t,
  ...Pe.getConfig()
}), O = Qe(Ye(fe({
  baseUrl: "https://localhost:44394"
}))), Ke = async (t, e) => {
  const r = await t.getContext(xe);
  if (!r) {
    console.warn("UMB_AUTH_CONTEXT not available — extension API client will not be authenticated");
    return;
  }
  r.configureClient(O), console.log("Hello from my extension 🎉");
}, Ze = (t, e) => {
  console.log("Goodbye from my extension 👋");
}, et = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  onInit: Ke,
  onUnload: Ze
}, Symbol.toStringTag, { value: "Module" }));
class j {
  static getCssOverrides(e) {
    return (e?.client ?? O).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dragonflytheming/api/v1/css-overrides",
      ...e
    });
  }
  static ping(e) {
    return (e?.client ?? O).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dragonflytheming/api/v1/ping",
      ...e
    });
  }
  static getThemes(e) {
    return (e?.client ?? O).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dragonflytheming/api/v1/themes",
      ...e
    });
  }
  static whatsMyName(e) {
    return (e?.client ?? O).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dragonflytheming/api/v1/whatsMyName",
      ...e
    });
  }
  static whatsTheTimeMrWolf(e) {
    return (e?.client ?? O).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dragonflytheming/api/v1/whatsTheTimeMrWolf",
      ...e
    });
  }
  static whoAmI(e) {
    return (e?.client ?? O).get({
      security: [
        {
          scheme: "bearer",
          type: "http"
        }
      ],
      url: "/umbraco/dragonflytheming/api/v1/whoAmI",
      ...e
    });
  }
}
var tt = Object.defineProperty, rt = Object.getOwnPropertyDescriptor, me = (t) => {
  throw TypeError(t);
}, z = (t, e, r, s) => {
  for (var a = s > 1 ? void 0 : s ? rt(e, r) : e, c = t.length - 1, n; c >= 0; c--)
    (n = t[c]) && (a = (s ? n(e, r, a) : n(a)) || a);
  return s && a && tt(e, r, a), a;
}, pe = (t, e, r) => e.has(t) || me("Cannot " + r), U = (t, e, r) => (pe(t, e, "read from private field"), r ? r.call(t) : e.get(t)), W = (t, e, r) => e.has(t) ? me("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), at = (t, e, r, s) => (pe(t, e, "write to private field"), e.set(t, r), r), D, L, V, J;
let _ = class extends Y(X) {
  constructor() {
    super(), this._yourName = "Press the button!", W(this, D), W(this, L, async (t) => {
      const e = t.target;
      e.state = "waiting";
      const { data: r, error: s } = await j.whoAmI();
      if (s) {
        e.state = "failed", console.error(s);
        return;
      }
      r !== void 0 && (this._serverUserData = r, e.state = "success"), U(this, D) && U(this, D).peek("warning", {
        data: {
          headline: `You are ${this._serverUserData?.name}`,
          message: `Your email is ${this._serverUserData?.email}`
        }
      });
    }), W(this, V, async (t) => {
      const e = t.target;
      e.state = "waiting";
      const { data: r, error: s } = await j.whatsTheTimeMrWolf();
      if (s) {
        e.state = "failed", console.error(s);
        return;
      }
      r !== void 0 && (this._timeFromMrWolf = new Date(r), e.state = "success");
    }), W(this, J, async (t) => {
      const e = t.target;
      e.state = "waiting";
      const { data: r, error: s } = await j.whatsMyName();
      if (s) {
        e.state = "failed", console.error(s);
        return;
      }
      this._yourName = r, e.state = "success";
    }), this.consumeContext(ke, (t) => {
      at(this, D, t);
    }), this.consumeContext(Oe, (t) => {
      this.observe(
        t?.currentUser,
        (e) => {
          this._contextCurrentUser = e;
        },
        "_contextCurrentUser"
      );
    });
  }
  render() {
    return w`
      <uui-box headline="Who am I?">
        <div slot="header">[Server]</div>
        <h2>
          <uui-icon name="icon-user"></uui-icon>${this._serverUserData?.email ? this._serverUserData.email : "Press the button!"}
        </h2>
        <ul>
          ${this._serverUserData?.groups.map(
      (t) => w`<li>${t.name}</li>`
    )}
        </ul>
        <uui-button
          color="default"
          look="primary"
          @click="${U(this, L)}"
        >
          Who am I?
        </uui-button>
        <p>
          This endpoint gets your current user from the server and displays your
          email and list of user groups. It also displays a Notification with
          your details.
        </p>
      </uui-box>

      <uui-box headline="What's my Name?">
        <div slot="header">[Server]</div>
        <h2><uui-icon name="icon-user"></uui-icon> ${this._yourName}</h2>
        <uui-button
          color="default"
          look="primary"
          @click="${U(this, J)}"
        >
          Whats my name?
        </uui-button>
        <p>
          This endpoint has a forced delay to show the button 'waiting' state
          for a few seconds before completing the request.
        </p>
      </uui-box>

      <uui-box headline="What's the Time?">
        <div slot="header">[Server]</div>
        <h2>
          <uui-icon name="icon-alarm-clock"></uui-icon> ${this._timeFromMrWolf ? this._timeFromMrWolf.toLocaleString() : "Press the button!"}
        </h2>
        <uui-button
          color="default"
          look="primary"
          @click="${U(this, V)}"
        >
          Whats the time Mr Wolf?
        </uui-button>
        <p>This endpoint gets the current date and time from the server.</p>
      </uui-box>

      <uui-box headline="Who am I?" class="wide">
        <div slot="header">[Context]</div>
        <p>Current user email: <b>${this._contextCurrentUser?.email}</b></p>
        <p>
          This is the JSON object available by consuming the
          'UMB_CURRENT_USER_CONTEXT' context:
        </p>
        <umb-code-block language="json" copy
          >${JSON.stringify(this._contextCurrentUser, null, 2)}</umb-code-block
        >
      </uui-box>
    `;
  }
};
D = /* @__PURE__ */ new WeakMap();
L = /* @__PURE__ */ new WeakMap();
V = /* @__PURE__ */ new WeakMap();
J = /* @__PURE__ */ new WeakMap();
_.styles = [
  G`
      :host {
        display: grid;
        gap: var(--uui-size-layout-1);
        padding: var(--uui-size-layout-1);
        grid-template-columns: 1fr 1fr 1fr;
      }

      uui-box {
        margin-bottom: var(--uui-size-layout-1);
      }

      h2 {
        margin-top: 0;
      }

      .wide {
        grid-column: span 3;
      }
    `
];
z([
  E()
], _.prototype, "_yourName", 2);
z([
  E()
], _.prototype, "_timeFromMrWolf", 2);
z([
  E()
], _.prototype, "_serverUserData", 2);
z([
  E()
], _.prototype, "_contextCurrentUser", 2);
_ = z([
  Q("example-dashboard")
], _);
const st = _, nt = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get ExampleDashboardElement() {
    return _;
  },
  default: st
}, Symbol.toStringTag, { value: "Module" }));
var ot = Object.defineProperty, it = Object.getOwnPropertyDescriptor, ye = (t) => {
  throw TypeError(t);
}, B = (t, e, r, s) => {
  for (var a = s > 1 ? void 0 : s ? it(e, r) : e, c = t.length - 1, n; c >= 0; c--)
    (n = t[c]) && (a = (s ? n(e, r, a) : n(a)) || a);
  return s && a && ot(e, r, a), a;
}, ct = (t, e, r) => e.has(t) || ye("Cannot " + r), lt = (t, e, r) => e.has(t) ? ye("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), ne = (t, e, r) => (ct(t, e, "access private method"), r), I, be, ge;
let C = class extends Y(X) {
  constructor() {
    super(...arguments), lt(this, I), this.value = "", this._themes = [], this._loading = !0;
  }
  async connectedCallback() {
    super.connectedCallback(), await ne(this, I, be).call(this);
  }
  render() {
    if (this._loading)
      return w`<uui-loader></uui-loader>`;
    if (this._themes.length === 0)
      return w`
                <p class="no-themes">
                    No themes found. Add a folder to the Themes directory to make it available here.
                </p>
            `;
    const t = this._themes.map((e) => ({
      name: e,
      value: e,
      selected: e === this.value
    }));
    return w`
            <uui-select
                .options=${t}
                @change=${ne(this, I, ge)}
            ></uui-select>
        `;
  }
};
I = /* @__PURE__ */ new WeakSet();
be = async function() {
  const { data: t, error: e } = await j.getThemes();
  e ? console.error("ThemePicker: failed to load themes", e) : t && (this._themes = t), this._loading = !1;
};
ge = function(t) {
  this.value = t.target.value, this.dispatchEvent(new ce());
};
C.styles = G`
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
B([
  ie({ type: String })
], C.prototype, "value", 2);
B([
  E()
], C.prototype, "_themes", 2);
B([
  E()
], C.prototype, "_loading", 2);
C = B([
  Q("dragonfly-theme-picker")
], C);
const ut = C, dt = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get DragonflyThemePickerElement() {
    return C;
  },
  default: ut
}, Symbol.toStringTag, { value: "Module" }));
var ht = Object.defineProperty, ft = Object.getOwnPropertyDescriptor, ve = (t) => {
  throw TypeError(t);
}, R = (t, e, r, s) => {
  for (var a = s > 1 ? void 0 : s ? ft(e, r) : e, c = t.length - 1, n; c >= 0; c--)
    (n = t[c]) && (a = (s ? n(e, r, a) : n(a)) || a);
  return s && a && ht(e, r, a), a;
}, mt = (t, e, r) => e.has(t) || ve("Cannot " + r), pt = (t, e, r) => e.has(t) ? ve("Cannot add the same private member more than once") : e instanceof WeakSet ? e.add(t) : e.set(t, r), oe = (t, e, r) => (mt(t, e, "access private method"), r), M, _e, we;
let S = class extends Y(X) {
  constructor() {
    super(...arguments), pt(this, M), this.value = "", this._files = [], this._loading = !0;
  }
  async connectedCallback() {
    super.connectedCallback(), await oe(this, M, _e).call(this);
  }
  render() {
    if (this._loading)
      return w`<uui-loader></uui-loader>`;
    if (this._files.length === 0)
      return w`
                <p class="no-files">
                    No CSS override files found. Add <code>.css</code> files to
                    <code>wwwroot/Themes/~CssOverrides/</code> to make them available here.
                </p>
            `;
    const t = [
      { name: "(none)", value: "", selected: this.value === "" },
      ...this._files.map((e) => ({
        name: e,
        value: e,
        selected: e === this.value
      }))
    ];
    return w`
            <uui-select
                .options=${t}
                @change=${oe(this, M, we)}
            ></uui-select>
        `;
  }
};
M = /* @__PURE__ */ new WeakSet();
_e = async function() {
  const { data: t, error: e } = await j.getCssOverrides();
  e ? console.error("CssOverridePicker: failed to load CSS overrides", e) : t && (this._files = t), this._loading = !1;
};
we = function(t) {
  this.value = t.target.value, this.dispatchEvent(new ce());
};
S.styles = G`
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
R([
  ie({ type: String })
], S.prototype, "value", 2);
R([
  E()
], S.prototype, "_files", 2);
R([
  E()
], S.prototype, "_loading", 2);
S = R([
  Q("dragonfly-css-override-picker")
], S);
const yt = S, bt = /* @__PURE__ */ Object.freeze(/* @__PURE__ */ Object.defineProperty({
  __proto__: null,
  get DragonflyCssOverridePickerElement() {
    return S;
  },
  default: yt
}, Symbol.toStringTag, { value: "Module" }));
export {
  Tt as manifests
};
//# sourceMappingURL=dragonfly-umbraco-theming.js.map
