# 12 — Response Tipleri, JsonResult, AntiForgery & Select2

## Response Sınıf Hiyerarşisi

```
ResponseDto                  → Veri dönmeyen işlemler (silme, durum değiştirme, vs.)
ResponseDto<T>               → Tek nesne dönen işlemler (detay, create sonrası)
ResponseListDto<T>           → Liste dönen işlemler (DataTable, dropdown listesi)
ErrorDto                     → Hata detayları (ResponseDto içinde Errors alanı)
```

> **Not:** Proje mimarisine göre `ResponseDto` yerine `Response` adı kullanılabilir.
> Skill bu yapıyı oluştururken değil, **kullanırken** bilmek için referans alır.

---

## ResponseDto — Kullanım Referansı

### Controller'da döndürme

```csharp
// Başarılı (veri yok — silme, durum değiştirme)
return Json(ResponseDto.Success(200, "Kayıt başarıyla silindi."));

// Başarılı (tek nesne)
return Json(ResponseDto<EntityViewModel>.SuccessData(200, "Kayıt getirildi.", viewModel));

// Başarılı (liste)
return Json(ResponseListDto<List<EntityViewModel>>.SuccessData(
    200, "Liste getirildi.", list,
    recordsTotal, recordsFiltered, recordsShow));

// Başarısız — tek hata mesajı
return Json(ResponseDto.Fail(400, "İşlem başarısız.", "Kayıt bulunamadı.", isShow: true));

// Başarısız — birden fazla hata
return Json(ResponseDto.Fail(400, "Doğrulama hatası.",
    new List<string> { "Ad boş olamaz.", "Email geçersiz." }, isShow: true));
```

### JsonResult — Dönüş Tipi

Tüm Ajax endpoint'leri `ActionResult` değil **`JsonResult`** döner:

```csharp
// ✅ Doğru
[HttpPost]
public JsonResult Delete(Guid id) { ... }

[HttpGet]
public JsonResult GetDetail(Guid id) { ... }

[HttpPost]
[ValidateAntiForgeryToken]
public JsonResult Save(CreateEntityViewModel model) { ... }

// ❌ Yanlış
public IActionResult Delete(Guid id) { ... }
public async Task<ActionResult> GetDetail(Guid id) { ... }
```

### Async JsonResult

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<JsonResult> Save(CreateEntityViewModel model)
{
    var result = await _service.CreateAsync(model);
    return Json(result);
}
```

---

## ResponseDto Alanları — JS Tarafında Kullanım

```javascript
$.post(url, data).done(function (result) {
    // result.isSuccess   → bool
    // result.message     → string
    // result.statusCode  → int (200, 400, vs.)
    // result.data        → T (ResponseDto<T> ise)
    // result.errors      → { errors: [...], isShow: bool }

    // result.recordsTotal     → int (ResponseListDto ise)
    // result.recordsFiltered  → int
    // result.recordsShow      → int

    if (result.isSuccess) {
        toastr.success(result.message);
    } else {
        toastr.error(result.message);
        // Hata detaylarını göster
        if (result.errors && result.errors.isShow && result.errors.errors.length > 0) {
            $.each(result.errors.errors, function (i, err) {
                toastr.warning(err);
            });
        }
    }
});
```

---

## AntiForgery Token

### Her Form Sayfasında Zorunlu

`<form>` etiketi içine mutlaka eklenir:

```cshtml
<form id="EntityForm" asp-action="Save" asp-controller="Controller" method="post">
    @Html.AntiForgeryToken()
    <!-- form alanları -->
</form>
```

### Ajax POST İsteklerinde Token Gönderme

#### Yöntem 1 — Her Ajax isteğine otomatik ekle (önerilen, `PageUtilities.js` içine)

```javascript
// PageUtilities.js içinde global setup
$.ajaxSetup({
    beforeSend: function (xhr) {
        xhr.setRequestHeader('RequestVerificationToken',
            $('input[name="__RequestVerificationToken"]').val());
    }
});
```

#### Yöntem 2 — Her istekte manuel ekle

```javascript
var token = $('input[name="__RequestVerificationToken"]').val();

$.ajax({
    url: '/Controller/Save',
    type: 'POST',
    data: {
        __RequestVerificationToken: token,
        field1: $('#field1').val(),
        field2: $('#field2').val()
    }
}).done(function (result) { ... });
```

#### Yöntem 3 — JSON body ile POST (header'dan gönder)

```javascript
$.ajax({
    url: '/Controller/Save',
    type: 'POST',
    contentType: 'application/json',
    headers: {
        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
    },
    data: JSON.stringify(model)
}).done(function (result) { ... });
```

### Controller Tarafı

```csharp
[HttpPost]
[ValidateAntiForgeryToken]   // ← zorunlu
public async Task<JsonResult> Save(CreateEntityViewModel model)
{
    ...
}
```

> **Not:** Modal içinden yapılan Ajax POST'larda token, modal'ın bulunduğu
> sayfadaki `__RequestVerificationToken` input'undan okunur.
> Modal partial ayrı bir sayfada render edilmediği için ekstra token gerekmez.

---

## Select2 — Standart Kullanım

### Tüm Kuralllar
- Projede **tüm `<select>` elementleri** `select2` ile init edilir
- Init her zaman ilgili sayfanın JS dosyasında yapılır
- `placeholder` her zaman set edilir
- Form sayfalarında `allowClear: true` eklenir

### Tekli Seçim

```html
<!-- View -->
<select class="form-control form-control-sm select2" asp-for="CategoryId"
        asp-items="@ViewBag.Categories">
    <option value="">Kategori Seçiniz...</option>
</select>
```

```javascript
// JS
$('#CategoryId').select2({
    placeholder: 'Kategori Seçiniz',
    allowClear: true
});
```

### Çoklu Seçim

```html
<!-- View — multiple attribute eklenir, asp-for liste tipine bağlanır -->
<select class="form-control select2-multiple" asp-for="SelectedIds"
        asp-items="@ViewBag.Items" multiple="multiple">
</select>
```

```javascript
// JS
$('.select2-multiple').select2({
    placeholder: 'Seçiniz...',
    allowClear: true
    // multiple özelliği HTML'den otomatik algılanır
});
```

```csharp
// ViewModel'de liste olarak tanımlanır
public List<Guid> SelectedIds { get; set; } = new();
```

### Ajax ile Dolan Select2 (Sunucudan arama)

```html
<select class="form-control select2" id="FirmId"></select>
```

```javascript
$('#FirmId').select2({
    placeholder: 'Firma Arayın...',
    allowClear: true,
    minimumInputLength: 2,
    ajax: {
        url: '/Firm/Search',
        dataType: 'json',
        delay: 300,
        data: function (params) {
            return { term: params.term };
        },
        processResults: function (result) {
            if (!result.isSuccess) return { results: [] };
            return {
                results: result.data.map(function (item) {
                    return { id: item.id, text: item.name };
                })
            };
        }
    }
});
```

```csharp
// Controller
[HttpGet]
public async Task<JsonResult> Search(string term)
{
    var list = await _firmService.SearchAsync(term);
    return Json(ResponseDto<List<FirmSelectViewModel>>
        .SuccessData(200, "OK", list));
}
```

### Modal İçinde Select2

Modal açıldıktan sonra select2 init edilmeli, önceden init edilirse dropdown
pozisyon sorunu yaşar:

```javascript
$('#modal-id').on('shown.bs.modal', function () {
    $('#modal_firm_sb').select2({
        placeholder: 'Firma Seçiniz',
        allowClear: true,
        dropdownParent: $('#modal-id')  // ← modal içinde açılması için zorunlu
    });
});
```

### Cascade (Bağımlı) Select2

```javascript
// Üst değişince alt'ı sıfırla ve doldur
$('#ParentSelect').on('change', function () {
    var parentId = $(this).val();
    var $child = $('#ChildSelect');

    $child.empty().append('<option value="">Seçiniz...</option>');
    if (!parentId) return;

    $.get('/Controller/GetChildren', { parentId: parentId }).done(function (result) {
        if (result.isSuccess) {
            $.each(result.data, function (i, item) {
                $child.append($('<option>').val(item.id).text(item.name));
            });
        }
    });
});
```

---

## Tam Ajax POST Şablonu (Token + JsonResult + ResponseDto)

```javascript
var initSaveButton = function () {
    $('#save_bt').on('click', function () {
        var token = $('input[name="__RequestVerificationToken"]').val();

        var model = {
            __RequestVerificationToken: token,
            Name:        $('#name_tb').val(),
            CategoryId:  $('#category_sb').val(),
            SelectedIds: $('#multi_sb').val(),  // select2 multiple → dizi döner
            IsActive:    $('#active_cb').is(':checked')
        };

        // Basit validasyon
        if (!model.Name) {
            toastr.warning('Ad alanı zorunludur.');
            return;
        }

        $.post('/Controller/Save', model).done(function (result) {
            if (result.isSuccess) {
                toastr.success(result.message);
                $('#save-modal').modal('hide');
                setTimeout(function () { window.location.reload(); }, 2000);
            } else {
                toastr.error(result.message);
                if (result.errors && result.errors.isShow) {
                    $.each(result.errors.errors, function (i, err) {
                        toastr.warning(err);
                    });
                }
            }
        }).fail(function () {
            toastr.error('Sunucu ile iletişim kurulamadı.');
        });
    });
};
```

---

## Hızlı Referans Tablosu

| Durum | Kullanılacak |
|---|---|
| Veri dönmeyen POST (sil, durum değiştir) | `ResponseDto.Success / Fail` |
| Tek nesne dönen GET/POST | `ResponseDto<T>.SuccessData / FailData` |
| Liste dönen GET | `ResponseListDto<T>.SuccessData / FailData` |
| Dönüş tipi | `JsonResult` (async: `Task<JsonResult>`) |
| POST endpoint | `[ValidateAntiForgeryToken]` zorunlu |
| Her select | `select2` ile init et |
| Modal içi select | `dropdownParent: $('#modal-id')` ekle |
| Çoklu seçim | `multiple="multiple"` + `List<T>` ViewModel |
