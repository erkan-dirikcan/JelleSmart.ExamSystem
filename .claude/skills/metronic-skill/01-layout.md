# 01 — Layout & Partial'lar

## _Layout.cshtml — Temel Yapı

Ana layout `Views/Shared/_Layout.cshtml` konumundadır.
Auth ve Error sayfaları bu layout'u KULLANMAZ (`Layout = null`).

### CSS Yükleme Sırası (head)
```html
<link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700" />
<link href="~/plugins/custom/fullcalendar/fullcalendar.bundle.css" rel="stylesheet" />
<link href="~/plugins/custom/datatables/datatables.bundle.css" rel="stylesheet" />
<link href="~/plugins/global/plugins.bundle.css" rel="stylesheet" />
<link href="~/plugins/custom/prismjs/prismjs.bundle.css" rel="stylesheet" />
<link href="~/css/style.bundle.css" rel="stylesheet" />
<!-- Tema renk seçimi: light veya dark -->
<link href="~/css/themes/layout/header/base/light.css" rel="stylesheet" />
<link href="~/css/themes/layout/header/menu/light.css" rel="stylesheet" />
<link href="~/css/themes/layout/brand/light.css" rel="stylesheet" />  <!-- dark.css de olabilir -->
<link href="~/css/themes/layout/aside/light.css" rel="stylesheet" />  <!-- dark.css de olabilir -->
<link rel="shortcut icon" href="{{Icon Path}}" />
```

### body class — Sayfa Özellikleri
```html
<body id="kt_body" class="page-loading-enabled page-loading header-fixed header-mobile-fixed
  aside-enabled aside-fixed aside-minimize-hoverable page-loading">
```

### Layout Gövde Yapısı
```
body
├── .page-loader (loader animasyonu — doğrudan layout içinde, partial değil)
├── _HeaderMobilePartial
└── .d-flex.flex-column.flex-root
    └── .d-flex.flex-row.flex-column-fluid.page
        ├── _MainMenuPartial (aside / sidebar)
        └── #kt_wrapper .wrapper
            ├── _UserPanelPartial  ← @if (User.Identity.IsAuthenticated)
            ├── _HeaderPartial     ← @if (User.Identity.IsAuthenticated)
            ├── _UserPanelPartial  ← tekrar (offcanvas için gerekli)
            ├── #kt_content .content
            │   └── @RenderBody()
            └── @if (IsAuthenticated)
                ├── _FooterPartial
                └── _ChatPartial (varsa)
```

### JS Yükleme Sırası (body sonu)
```html
<script>var KTAppSettings = { ... };</script>  <!-- Renk/font config — değiştirme! -->
<script src="~/plugins/global/plugins.bundle.js"></script>
<script src="~/plugins/custom/prismjs/prismjs.bundle.js"></script>
<script src="~/js/scripts.bundle.js"></script>
<script src="~/js/Custom/PageUtilities.js"></script>
<script src="~/js/pages/features/miscellaneous/toastr.js"></script>
<script src="~/plugins/custom/fullcalendar/fullcalendar.bundle.js"></script>
<script src="~/plugins/custom/datatables/datatables.bundle.js"></script>
@RenderSection("PartialScripts", false)
@RenderSection("Scripts", false)
```

> **Not:** Sayfa bazlı JS dosyaları `@section Scripts { <script src="..."></script> }` ile eklenir.

---

## Partial Dosyaları

### _FooterPartial.cshtml
```html
<div class="footer bg-white py-4 d-flex flex-lg-column" id="kt_footer">
    <div class="container-fluid d-flex flex-column flex-md-row align-items-center justify-content-between">
        <div class="text-dark order-2 order-md-1">
            <span class="text-muted font-weight-bold mr-2">2025&copy;</span>
            <a href="{{GeliştiriciURL}}" target="_blank" class="text-dark-75 text-hover-primary">{{Geliştirici}}</a>
            <span class="text-muted font-weight-bold mr-2"> tarafından desteklenmektedir</span>
        </div>
    </div>
</div>
```

### _LoaderPartial.cshtml
```html
<div class="page-loader page-loader-logo">
    <img alt="Logo" class="max-h-75px" src="~/media/logos{{LoaderImage}}" />
    <div class="spinner spinner-primary"></div>
</div>
```
> **Not:** Layout'ta bu partial `<partial name="..." />` ile değil, doğrudan inline HTML olarak kullanılmaktadır.

### _ScrollTopPartial.cshtml
```html
<div id="kt_scrolltop" class="scrolltop">
    <span class="svg-icon">
        <svg xmlns="http://www.w3.org/2000/svg" width="24px" height="24px" viewBox="0 0 24 24" version="1.1">
            <g stroke="none" stroke-width="1" fill="none" fill-rule="evenodd">
                <polygon points="0 0 24 0 24 24 0 24" />
                <rect fill="#000000" opacity="0.3" x="11" y="10" width="2" height="10" rx="1" />
                <path d="M6.70710678,12.7071068 C6.31658249,13.0976311 5.68341751,13.0976311 5.29289322,12.7071068
                  C4.90236893,12.3165825 4.90236893,11.6834175 5.29289322,11.2928932 L11.2928932,5.29289322
                  C11.6714722,4.91431428 12.2810586,4.90106866 12.6757246,5.26284586 L18.6757246,10.7628459
                  C19.0828436,11.1360383 19.1103465,11.7686056 18.7371541,12.1757246
                  C18.3639617,12.5828436 17.7313944,12.6103465 17.3242754,12.2371541
                  L12.0300757,7.38413782 L6.70710678,12.7071068 Z"
                  fill="#000000" fill-rule="nonzero" />
            </g>
        </svg>
    </span>
</div>
```
> **Not:** Layout'ta bu da inline HTML olarak kullanılmaktadır.

### _HeaderMobilePartial.cshtml
Mobil görünümde logo ve hamburger menü butonunu içerir.
`@if (User.Identity!.IsAuthenticated)` kontrolü ile menü butonları gösterilir/gizlenir.
Logo: `~/media/logos/{{LogoDosyaAdı}}`

### _HeaderPartial.cshtml
Üst bar. İçinde `<user-fullname>` ve `<user-picture-thumbnail>` TagHelper'ları kullanılır.
Detay: [docs/09-taghelpers.md](09-taghelpers.md)

### _UserPanelPartial.cshtml
Sağdan açılan offcanvas kullanıcı paneli.
`@inject UserManager<AppUser> UserManager` ile kullanıcı bilgileri çekilir.
Profil linki: `asp-controller="User" asp-action="UserProfile"`
Çıkış linki: `asp-controller="User" asp-action="Logout" asp-route-returnUrl="/Dashboard"`

### _MainMenuPartial.cshtml
Sidebar menü. Detay: [docs/02-navigation.md](02-navigation.md)
