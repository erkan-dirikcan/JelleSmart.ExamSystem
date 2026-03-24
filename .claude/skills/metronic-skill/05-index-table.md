# 05 — Index / Liste Sayfası (DataTable)

## Genel Yapı

Liste sayfaları `_Layout.cshtml`'i kullanır.
Sayfa her zaman şu hiyerarşiyle başlar:
```
.d-flex.flex-column-fluid
  └── .container-fluid
        └── .row
              └── .col-lg-12
                    └── .card.card-custom
                          ├── .card-header  ← başlık + toolbar (buton)
                          └── .card-body    ← alert mesajları + tablo
```

## Standart Index.cshtml Şablonu

```cshtml
@model IEnumerable<YourProject.Core.Models.ViewModels.EntityListViewModel>
@{
    ViewData["Title"] = "{{Sayfa Başlığı}}";
    ViewData["ActivePage"] = "{{MenuAdı}}";
    ViewData["MenuToggle"] = "{{UstMenuAdı}}";  // Tekil menüde bu satır olmaz

    var infoMessage  = TempData["InfoMessage"]  == null ? "" : TempData["InfoMessage"].ToString();
    var errorMessage = TempData["ErrorMessage"] == null ? "" : TempData["ErrorMessage"].ToString();
}

<div class="d-flex flex-column-fluid">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12">
                <div class="card card-custom">

                    <!-- CARD HEADER -->
                    <div class="card-header flex-wrap border-0 pt-6 pb-0">
                        <div class="card-title">
                            <h3 class="card-label">
                                {{Modül Adı}} Yönetimi
                                <span class="d-block text-muted pt-2 font-size-sm">{{Modül Adı}} Listesi</span>
                            </h3>
                        </div>
                        <div class="card-toolbar">
                            <!-- Yeni kayıt butonu — yetki kontrolü varsa @if sarıl -->
                            <a asp-action="Create" asp-controller="{{Controller}}"
                               class="btn btn-primary font-weight-bolder">
                                <span class="svg-icon svg-icon-md">
                                    <!-- SVG veya FontAwesome ikon -->
                                    <i class="flaticon2-plus"></i>
                                </span>
                                {{Modül Adı}} Ekle
                            </a>
                        </div>
                    </div>

                    <!-- CARD BODY -->
                    <div class="card-body">

                        <!-- TempData alert mesajları -->
                        @if (!string.IsNullOrEmpty(infoMessage))
                        {
                            <div class="alert alert-success" role="alert">
                                @infoMessage
                                <div class="alert-close">
                                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                        <span aria-hidden="true"><i class="ki ki-close"></i></span>
                                    </button>
                                </div>
                            </div>
                        }
                        @if (!string.IsNullOrEmpty(errorMessage))
                        {
                            <div class="alert alert-danger" role="alert">
                                @errorMessage
                                <div class="alert-close">
                                    <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                                        <span aria-hidden="true"><i class="ki ki-close"></i></span>
                                    </button>
                                </div>
                            </div>
                        }

                        <!-- DATATABLE -->
                        <table class="table table-bordered table-hover"
                               id="{{tabloId}}" style="margin-top: 13px !important">
                            <thead>
                                <tr>
                                    <th>{{Kolon 1}}</th>
                                    <th>{{Kolon 2}}</th>
                                    <th>Durum</th>
                                    <th>İşlemler</th>
                                </tr>
                            </thead>
                            <tbody>
                                @foreach (var item in Model)
                                {
                                    <tr>
                                        <td>@item.Field1</td>
                                        <td>@item.Field2</td>

                                        <!-- Durum — label badge pattern -->
                                        <td>
                                            @{
                                                var (statusClass, statusText) = item.Status switch
                                                {
                                                    StatusEnum.Active  => ("label-success", "Aktif"),
                                                    StatusEnum.Passive => ("label-danger",  "Pasif"),
                                                    StatusEnum.Deleted => ("label-danger",  "Silinmiş"),
                                                    _                  => ("label-default", "Bilinmiyor")
                                                };
                                            }
                                            <span class="label @statusClass label-pill label-inline mr-2">
                                                @statusText
                                            </span>
                                        </td>

                                        <!-- İşlemler -->
                                        <td>
                                            <!-- Düzenle -->
                                            <a asp-controller="{{Controller}}" asp-action="Update"
                                               asp-route-id="@item.Id"
                                               class="btn btn-icon btn-warning btn-sm"
                                               data-toggle="tooltip" title="Güncelle" data-theme="dark">
                                                <i class="flaticon-edit"></i>
                                            </a>

                                            <!-- Durum değiştir (Active/Passive toggle) -->
                                            @if (item.Status == StatusEnum.Active)
                                            {
                                                <button class="btn btn-icon btn-danger btn-sm change-status-bt"
                                                        data-name="@item.Name"
                                                        data-id="@item.Id"
                                                        data-status="@StatusEnum.Passive"
                                                        data-toggle="tooltip" title="Pasife Çek" data-theme="dark">
                                                    <i class="flaticon2-cancel"></i>
                                                </button>
                                            }
                                            else
                                            {
                                                <button class="btn btn-icon btn-success btn-sm change-status-bt"
                                                        data-name="@item.Name"
                                                        data-id="@item.Id"
                                                        data-status="@StatusEnum.Active"
                                                        data-toggle="tooltip" title="Aktif Et" data-theme="dark">
                                                    <i class="flaticon2-check-mark"></i>
                                                </button>
                                            }

                                            <!-- Silme butonu (data-id ile) -->
                                            <button class="btn btn-icon btn-danger btn-sm delete-bt"
                                                    data-id="@item.Id"
                                                    data-name="@item.Name"
                                                    data-toggle="tooltip" title="Sil" data-theme="dark">
                                                <i class="flaticon2-trash"></i>
                                            </button>
                                        </td>
                                    </tr>
                                }
                            </tbody>
                        </table>

                    </div><!-- /card-body -->
                </div><!-- /card -->
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/custom/{{sayfa-adi}}-page.js"></script>
}
```

## Yetki Kontrolü ile Toolbar

`IAuthorizationService` inject ederek claim bazlı buton göster/gizle:

```cshtml
@using Microsoft.AspNetCore.Authorization
@inject IAuthorizationService AuthorizationService

@if (AuthorizationService.AuthorizeAsync(User, "ModuleName.Create").Result.Succeeded)
{
    <a asp-action="Create" asp-controller="Controller" class="btn btn-primary ...">Ekle</a>
}
```

## StatusEnum — Türkçe Karşılık Tablosu

```csharp
// Razor'da switch ile:
var durumText = item.Status switch
{
    StatusEnum.Active   => "Aktif",
    StatusEnum.Passive  => "Pasif",
    StatusEnum.Deleted  => "Silinmiş",
    StatusEnum.Locked   => "Kilitli",
    StatusEnum.Unlocked => "Kilit Açık",
    StatusEnum.Pending  => "Beklemede",
    _                   => ""
};
```

## Tab Panel İçinde Tablolar

Birden fazla sekme ile filtrelenmiş tablolar göstermek için:

```cshtml
<!-- Card toolbar'da nav pills -->
<div class="card-toolbar">
    <ul class="nav nav-pills nav-pills-sm nav-dark-75">
        <li class="nav-item">
            <a class="nav-link py-2 px-4 active" data-toggle="tab" href="#tab_1">Sekme 1</a>
        </li>
        <li class="nav-item">
            <a class="nav-link py-2 px-4" data-toggle="tab" href="#tab_2">Sekme 2</a>
        </li>
    </ul>
</div>

<!-- Card body'de tab içerikleri -->
<div class="tab-content mt-5">
    <div class="tab-pane fade show active" id="tab_1" role="tabpanel">
        <div class="table-responsive">
            <table class="table table-vertical-center border-bottom" id="table1">
                ...
            </table>
        </div>
    </div>
    <div class="tab-pane fade" id="tab_2" role="tabpanel">
        <div class="table-responsive">
            <table class="table table-vertical-center border-bottom" id="table2">
                ...
            </table>
        </div>
    </div>
</div>
```

## DataTable — JS Başlatma (JS dosyasında)

Detay: [docs/08-javascript.md](08-javascript.md)

```javascript
var initTable = function () {
    var table = $('#{{tabloId}}');
    table.DataTable({
        responsive: true,
        columnDefs: [
            { orderable: false, targets: -1 }  // Son kolon (işlemler) sıralanamaz
        ]
    });
};
```

## HTML.Raw Kullanımı (dikkat!)

`Html.Raw()` yalnızca trusted içerik için kullanılır:
```cshtml
<!-- Claim listesi gibi durumlar: -->
var claims = item.Claims.Count > 0
    ? string.Join("<br/>", item.Claims.Select(x => x.Name))
    : "";
<td>@Html.Raw(claims)</td>

<!-- Koşullu HTML string: -->
var titleHtml = isProject
    ? $"<span class=\"text-danger\">(Proje) - </span>{item.Title}"
    : item.Title;
<td>@Html.Raw(titleHtml)</td>
```
