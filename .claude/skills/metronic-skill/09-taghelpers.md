# 09 — TagHelper'lar

## Mevcut TagHelper'lar

| TagHelper | HTML Etiketi | Çıktı | Konum |
|---|---|---|---|
| `UserFullnameTagHelper` | `<user-fullname>` | Kullanıcı adı soyadı içeren `<span>` | `_HeaderPartial.cshtml` |
| `UserPictureThumbnailTagHelper` | `<user-picture-thumbnail>` | Avatar `<img>` etiketi | `_HeaderPartial.cshtml`, `_UserPanelPartial.cshtml` |

---

## UserFullnameTagHelper

```csharp
public class UserFullnameTagHelper : TagHelper
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserFullnameTagHelper(
        UserManager<AppUser> userManager,
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var user = _contextAccessor.HttpContext?.User;
        var userName = _userManager.GetUserAsync(user).Result;

        output.TagName = "span";
        output.Content.SetContent(userName.FirstName + " " + userName.LastName);
        output.AddClass("text-dark-50",      HtmlEncoder.Default);
        output.AddClass("font-weight-bolder", HtmlEncoder.Default);
        output.AddClass("font-size-base",    HtmlEncoder.Default);
        output.AddClass("d-none",            HtmlEncoder.Default);
        output.AddClass("d-md-inline",       HtmlEncoder.Default);
        output.AddClass("mr-3",              HtmlEncoder.Default);
    }
}
```

**Kullanım (`_HeaderPartial.cshtml`):**
```html
<user-fullname></user-fullname>
```

**Çıktı:**
```html
<span class="text-dark-50 font-weight-bolder font-size-base d-none d-md-inline mr-3">
    Ad Soyad
</span>
```

---

## UserPictureThumbnailTagHelper

```csharp
public class UserPictureThumbnailTagHelper : TagHelper
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserPictureThumbnailTagHelper(
        UserManager<AppUser> userManager,
        IHttpContextAccessor contextAccessor)
    {
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var user     = _contextAccessor.HttpContext?.User;
        var userInfo = _userManager.GetUserAsync(user).Result;

        output.TagName = "img";
        output.Attributes.SetAttribute("src",
            string.IsNullOrEmpty(userInfo.Avatar)
                ? "/avatars/DefaultNoPic.png"      // varsayılan avatar
                : $"/avatars/{userInfo.Avatar}");
        output.Attributes.SetAttribute("accept", "=\"image/png, image/jpeg\"");
        output.Attributes.SetAttribute("id", "AvatarImg");
        base.Process(context, output);
    }
}
```

**Kullanım:**
```html
<user-picture-thumbnail></user-picture-thumbnail>
```

**Çıktı:**
```html
<img src="/avatars/kullanici-uuid.jpg" id="AvatarImg" />
<!-- Avatar yoksa: -->
<img src="/avatars/DefaultNoPic.png" id="AvatarImg" />
```

**Avatar dosya konumu:** `wwwroot/avatars/`

---

## TagHelper Kayıt (`_ViewImports.cshtml`)

```cshtml
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@addTagHelper *, YourProject.WebUI   ← kendi TagHelper namespace'in
```

## DI Kayıt (`Program.cs`)

```csharp
builder.Services.AddHttpContextAccessor();
// UserManager Identity kurulumundan gelir, ayrıca kaydetmeye gerek yok.
```

---

## Yeni TagHelper Oluşturma Şablonu

```csharp
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

[HtmlTargetElement("tag-adi")]
public class YeniTagHelper : TagHelper
{
    // Inject gereken servisler constructor'a eklenir
    public YeniTagHelper() { }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span"; // veya "div", "img" vs.
        output.Content.SetContent("İçerik");
        output.AddClass("css-class", HtmlEncoder.Default);
    }

    // Async versiyon:
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "div";
        var content = await output.GetChildContentAsync();
        output.Content.SetHtmlContent(content.GetContent());
    }
}
```
