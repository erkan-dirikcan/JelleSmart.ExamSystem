# Metronic 7.2.9 — ASP.NET Core MVC Skill

## Ne Zaman Kullan
Bu skill'i şu durumlarda kullan:
- Yeni bir MVC sayfası, View veya Partial oluşturulurken
- Layout, menü veya auth sayfalarına dokunulurken
- DataTable, Modal, Form, Dashboard bileşeni eklenirken
- JS dosyası (`/js/custom/`) oluşturulurken
- `ManageNavPages.cs`'e yeni menü eklenirken
- TagHelper kullanımı gerektiğinde

## Proje Yapısı

```
WebUI/
├── Controllers/
│   └── UserController.cs          ← Auth işlemleri (Login, Logout, ForgetPassword, ResetPassword)
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml                  ← Ana layout
│   │   ├── _FooterPartial.cshtml
│   │   ├── _HeaderMobilePartial.cshtml
│   │   ├── _HeaderPartial.cshtml
│   │   ├── _LoaderPartial.cshtml
│   │   ├── _MainMenuPartial.cshtml         ← Sidebar menü + ManageNavPages entegrasyonu
│   │   ├── _ScrollTopPartial.cshtml
│   │   └── _UserPanelPartial.cshtml        ← Offcanvas kullanıcı paneli
│   ├── User/
│   │   ├── Login.cshtml                    ← Login + ForgotPassword tek sayfada, Layout=null
│   │   ├── ResetPassword.cshtml
│   │   └── ForgetConfirm.cshtml
│   └── {Controller}/
│       ├── Index.cshtml                    ← DataTable liste sayfası
│       ├── Create.cshtml                   ← Form sayfası
│       └── Update.cshtml                   ← Form sayfası
├── TagHelpers/
│   ├── UserFullnameTagHelper.cs
│   └── UserPictureThumbnailTagHelper.cs
├── Models/
│   └── ManageNavPages.cs                   ← Menü aktif/pasif yönetimi
└── wwwroot/
    ├── assets/media/logos/                 ← Logo dosyaları
    ├── avatars/                            ← Kullanıcı avatar'ları
    ├── css/
    │   ├── style.bundle.css
    │   ├── themes/layout/header/base/light.css
    │   ├── themes/layout/header/menu/light.css
    │   ├── themes/layout/brand/light.css   ← veya dark.css
    │   ├── themes/layout/aside/light.css   ← veya dark.css
    │   └── pages/
    │       ├── login/login-1.css
    │       └── error/error-6.css
    ├── plugins/
    │   ├── global/plugins.bundle.css / .js
    │   ├── custom/datatables/datatables.bundle.css / .js
    │   ├── custom/fullcalendar/fullcalendar.bundle.css / .js
    │   └── custom/prismjs/prismjs.bundle.css / .js
    └── js/
        ├── scripts.bundle.js
        └── custom/                         ← Sayfa bazlı JS dosyaları buraya
            ├── PageUtilities.js
            └── {sayfa-adi}-page.js
```

## Kurulum Değişkenleri
`{{}}` ile işaretlenen alanlar her projede doldurulmalıdır:

| Değişken | Açıklama | Örnek |
|---|---|---|
| `{{Uygulama Adı}}` | `<title>` ve başlıklarda görünür | `Koala Portal` |
| `{{Uygulama Açıklaması}}` | Meta description ve login subtitle | `Şirket yönetim sistemi` |
| `{{URL}}` | Canonical URL | `https://portal.firma.com` |
| `{{Geliştirici}}` | Footer'da görünür | `Koala Yazılım` |
| `{{GeliştiriciURL}}` | Footer linki | `https://koala.com.tr` |
| `{{LogoDosyaAdı}}` | `/assets/media/logos/` altındaki dosya | `logo-light.png` |
| `{{LoaderImage}}` | Loader logo yolu | `/logo-loader.png` |
| `{{Icon Path}}` | Favicon yolu | `~/media/logos/favicon.ico` |

## Detaylı Dokümanlar
Her bileşen için ayrı doküman mevcuttur:

- [Layout & Partial'lar](docs/01-layout.md)
- [Navigasyon & ManageNavPages](docs/02-navigation.md)
- [Auth Sayfaları (Login/ForgotPassword/Reset)](docs/03-auth.md)
- [Error Sayfası](docs/04-error.md)
- [Index/Liste Sayfası (DataTable)](docs/05-index-table.md)
- [Form Sayfası (Create/Update)](docs/06-form.md)
- [Modal Bileşenleri](docs/07-modal.md)
- [JavaScript Pattern'leri](docs/08-javascript.md)
- [TagHelper'lar](docs/09-taghelpers.md)
- [Dashboard & Partial Compose](docs/10-dashboard.md)
- [DataTable Child Row (Satır Detayı)](docs/11-datatable-child-row.md)
- [Response Tipleri, JsonResult, AntiForgery & Select2](docs/12-response-select2-antiforgery.md)

## Hızlı Karar Rehberi

```
Yeni sayfa mı?
├── Auth sayfası → docs/03-auth.md (Layout=null, tam sayfa)
├── Error sayfası → docs/04-error.md (Layout=null, tam sayfa)
├── Liste sayfası → docs/05-index-table.md
├── Form sayfası → docs/06-form.md
└── Dashboard → docs/10-dashboard.md

Menüye yeni link mi?
└── docs/02-navigation.md

Modal eklenecek mi?
└── docs/07-modal.md

DataTable'da child row gerekiyor mu?
└── docs/11-datatable-child-row.md

JsonResult / Response tipi / AntiForgery / Select2 kuralları
└── docs/12-response-select2-antiforgery.md

JS dosyası yazılacak mı?
└── docs/08-javascript.md
```
