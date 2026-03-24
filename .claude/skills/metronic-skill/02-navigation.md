# 02 — Navigasyon & ManageNavPages

## Genel Mantık

Menü aktif durumu iki farklı mekanizma ile kontrol edilir:

| Durum | ViewData Anahtarı | Değer | ManageNavPages Metodu |
|---|---|---|---|
| Tekil menü (üst menüsüz) | `ViewData["ActivePage"]` | `"MenuAdı"` | `PageMainNavClass` → `menu-item-active` |
| Alt menü (üst menü açık) | `ViewData["ActivePage"]` | `"AltMenuAdı"` | `PageMainNavClass` → `menu-item-active` |
| Üst menü (açık kalması) | `ViewData["MenuToggle"]` | `"UstMenuAdı"` | `PageMainToogleNavClass` → `menu-item-open` |

### View'da Kullanım

**Tekil menü sayfası:**
```csharp
@{
    ViewData["ActivePage"] = "TekilMenu";
    // MenuToggle set edilmez
}
```

**Alt menüye sahip sayfa:**
```csharp
@{
    ViewData["ActivePage"] = "AltMenu1";
    ViewData["MenuToggle"] = "UstMenu";
}
```

---

## ManageNavPages.cs

`Models/ManageNavPages.cs` konumunda tutulur.

```csharp
public class ManageNavPages
{
    // ─── Menü tanımları (string property) ───────────────────────────
    private static string UstMenu   => "UstMenu";
    private static string AltMenu1  => "AltMenu1";
    private static string AltMenu2  => "AltMenu2";
    private static string AltMenu3  => "AltMenu3";
    private static string TekilMenu => "TekilMenu";

    // ─── Public metotlar (View'da kullanılır) ────────────────────────
    public static string UstMenuNavClass(ViewContext vc)    => PageMainToogleNavClass(vc, UstMenu);
    public static string AltMenu1MenuNavClass(ViewContext vc) => PageMainNavClass(vc, AltMenu1);
    public static string AltMenu2MenuNavClass(ViewContext vc) => PageMainNavClass(vc, AltMenu2);
    public static string AltMenu3MenuNavClass(ViewContext vc) => PageMainNavClass(vc, AltMenu3);
    public static string TekilMenuNavClass(ViewContext vc)  => PageMainNavClass(vc, TekilMenu);

    // ─── Private yardımcılar ─────────────────────────────────────────
    private static string PageMainNavClass(ViewContext viewContext, string page)
    {
        var activePage = viewContext.ViewData["ActivePage"] as string
            ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase)
            ? "menu-item-active" : null;
    }

    private static string PageMainToogleNavClass(ViewContext viewContext, string page)
    {
        var menuToggle = viewContext.ViewData["MenuToggle"] as string
            ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(menuToggle, page, StringComparison.OrdinalIgnoreCase)
            ? "menu-item-open" : "";
    }
}
```

### Yeni Menü Ekleme Adımları

1. `ManageNavPages.cs`'e private property ekle:
```csharp
private static string YeniMenu => "YeniMenu";
```

2. Public metot ekle:
```csharp
public static string YeniMenuNavClass(ViewContext vc) => PageMainNavClass(vc, YeniMenu);
// Üst menüyse:
public static string YeniUstMenuNavClass(ViewContext vc) => PageMainToogleNavClass(vc, YeniMenu);
```

3. `_MainMenuPartial.cshtml`'e `<li>` ekle (aşağıya bak).

4. İlgili View'a `ViewData` satırlarını ekle.

---

## _MainMenuPartial.cshtml Yapısı

### Tekil Menü Öğesi (SVG ikon ile)
```html
<li class="menu-item @ManageNavPages.TekilMenuNavClass(ViewContext)" aria-haspopup="true">
    <a href="~/" class="menu-link">
        <span class="svg-icon svg-icon-primary svg-icon-2x">
            <!-- SVG ikonu buraya -->
        </span>
        <span class="menu-text">Tekil Menü</span>
    </a>
</li>
```

### Tekil Menü Öğesi (FontAwesome ikon ile)
```html
<li class="menu-item @ManageNavPages.YeniMenuNavClass(ViewContext)" aria-haspopup="true">
    <a asp-controller="Controller" asp-action="Index" class="menu-link">
        <i class="menu-icon fas fa-home"></i>
        <span class="menu-text">Menü Adı</span>
    </a>
</li>
```

### Üst Menü + Alt Menüler
```html
<li class="menu-item menu-item-submenu @ManageNavPages.UstMenuNavClass(ViewContext)"
    aria-haspopup="true" data-menu-toggle="hover">
    <a href="javascript:;" class="menu-link menu-toggle">
        <i class="menu-icon far fa-building"></i>
        <span class="menu-text">Üst Menü</span>
        <i class="menu-arrow"></i>
    </a>
    <div class="menu-submenu">
        <i class="menu-arrow"></i>
        <ul class="menu-subnav">
            <li class="menu-item @ManageNavPages.AltMenu1MenuNavClass(ViewContext)" aria-haspopup="true">
                <a class="menu-link" asp-controller="Controller" asp-action="Index">
                    <i class="menu-bullet menu-bullet-dot"><span></span></i>
                    <span class="menu-text">Alt Menü 1</span>
                </a>
            </li>
            <li class="menu-item @ManageNavPages.AltMenu2MenuNavClass(ViewContext)" aria-haspopup="true">
                <a class="menu-link" asp-controller="Controller2" asp-action="Index">
                    <i class="menu-bullet menu-bullet-dot"><span></span></i>
                    <span class="menu-text">Alt Menü 2</span>
                </a>
            </li>
        </ul>
    </div>
</li>
```

### Aside (Sidebar) Dış Wrapper Yapısı
```html
<div class="aside aside-left aside-fixed d-flex flex-column flex-row-auto" id="kt_aside">
    <!-- Brand (logo + toggle butonu) -->
    <div class="brand flex-column-auto" id="kt_brand">
        <a href="~/" class="brand-logo">
            <img alt="Logo" src="/media/logos/{{LogoDosyaAdı}}" />
        </a>
        <button class="brand-toggle btn btn-sm px-0" id="kt_aside_toggle">
            <!-- Çift ok SVG ikonu -->
        </button>
    </div>
    <!-- Menü wrapper -->
    <div class="aside-menu-wrapper flex-column-fluid" id="kt_aside_menu_wrapper">
        <div id="kt_aside_menu" class="aside-menu my-4"
             data-menu-vertical="1"
             data-menu-scroll="1"
             data-menu-dropdown-timeout="500">
            <ul class="menu-nav">
                <!-- menu-item'lar buraya -->
            </ul>
        </div>
    </div>
</div>
```
