# 04 — Error Sayfası

## Genel Kurallar
- `Layout = null` — standalone HTML dökümanıdır.
- Hata mesajları `TempData["Error"]` ile `ResponseDto` tipinde taşınır.
- CSS: `~/css/pages/error/error-6.css`
- Arka plan: `/media/error/bg6.jpg`

## Error.cshtml

```cshtml
@using YourProject.Core.Dtos
@model YourProject.WebUI.Models.ErrorViewModel
@{
    ViewData["Title"] = "Hata";
    var errors = (ResponseDto)TempData["Error"]!;
    Layout = null;
}
<!DOCTYPE html>
<html lang="tr">
<head>
    <base href="~/">
    <meta charset="utf-8" />
    <title>{{Uygulama Adı}} | @ViewData["Title"]</title>
    <meta name="description" content="{{Uygulama Açıklaması}}" />
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700" />
    <link href="~/css/pages/error/error-6.css" rel="stylesheet" />
    <link href="~/plugins/global/plugins.bundle.css" rel="stylesheet" />
    <link href="~/css/style.bundle.css" rel="stylesheet" />
    <link href="~/css/themes/layout/header/base/light.css" rel="stylesheet" />
    <link href="~/css/themes/layout/header/menu/light.css" rel="stylesheet" />
    <link href="~/css/themes/layout/brand/dark.css" rel="stylesheet" />
    <link href="~/css/themes/layout/aside/dark.css" rel="stylesheet" />
    <link rel="shortcut icon" href="~/media/logos/favicon.ico" />
</head>
<body id="kt_body" class="header-fixed header-mobile-fixed subheader-enabled subheader-fixed
  aside-enabled aside-fixed aside-minimize-hoverable page-loading">
<div class="d-flex flex-column flex-root">
    <div class="error error-6 d-flex flex-row-fluid bgi-size-cover bgi-position-center"
         style="background-image: url(/media/error/bg6.jpg);">
        <div class="d-flex flex-column flex-row-fluid text-center">
            <h1 class="error-title font-weight-boldest text-white mb-12" style="margin-top: 12rem;">
                Oops...
            </h1>
            @if (errors != null)
            {
                <p class="display-4 font-weight-bold text-white">@errors.Message</p>
                if (errors.Errors.IsShow && errors.Errors.Errors.Count > 0)
                {
                    <ul>
                        @foreach (var item in errors.Errors.Errors)
                        {
                            <li class="display-4 font-weight-bold text-white">@item</li>
                        }
                    </ul>
                }
            }
        </div>
    </div>
</div>
<script>var KTAppSettings = { ... };</script>
<script src="~/plugins/global/plugins.bundle.js"></script>
<script src="~/plugins/custom/prismjs/prismjs.bundle.js"></script>
<script src="~/js/scripts.bundle.js"></script>
</body>
</html>
```

## ResponseDto Yapısı
```csharp
// TempData'ya koymak için:
TempData["Error"] = new ResponseDto
{
    Message = "Bir hata oluştu.",
    Errors = new ErrorDetailDto
    {
        IsShow = true,
        Errors = new List<string> { "Detay 1", "Detay 2" }
    }
};
return RedirectToAction("Error", "Home");
```

## ErrorViewModel
```csharp
// Models/ErrorViewModel.cs
public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
```
