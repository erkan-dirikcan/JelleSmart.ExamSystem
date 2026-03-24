# 06 — Form Sayfası (Create / Update)

## Genel Yapı

Form sayfaları `_Layout.cshtml`'i kullanır.
`<form>` etiketi doğrudan `.col-lg-12` içini sarar:

```
.d-flex.flex-column-fluid
  └── .container-fluid
        └── .row
              └── .col-lg-12
                    └── <form>
                          └── .card.card-custom.card-stretch.gutter-b
                                ├── .card-header   ← başlık
                                ├── .card-body     ← form alanları
                                └── .card-footer   ← submit + iptal butonları
```

## Standart Create/Update Şablonu

```cshtml
@model YourProject.Core.ViewModels.CreateEntityViewModel
@{
    ViewData["Title"] = "{{Modül Adı}} Ekle";
    ViewData["ActivePage"] = "{{MenuAdı}}";
    ViewData["MenuToggle"] = "{{UstMenuAdı}}";
}

<div class="d-flex flex-column-fluid">
    <div class="container-fluid">
        <div class="row">
            <div class="col-lg-12">
                <form name="{{EntityName}}Form" id="{{EntityName}}Form"
                      asp-action="Create" asp-controller="{{Controller}}" method="post">

                    <div class="card card-custom card-stretch gutter-b">

                        <!-- CARD HEADER -->
                        <div class="card-header border-0 py-5">
                            <h3 class="card-title align-items-start flex-column">
                                <span class="card-label font-weight-bolder text-dark">
                                    {{Modül Adı}} Yönetimi
                                </span>
                                <span class="text-muted mt-3 font-weight-bold font-size-sm">
                                    Yeni {{Modül Adı}} Ekle
                                </span>
                            </h3>
                        </div>

                        <!-- CARD BODY -->
                        <div class="card-body pt-0 pb-3">
                            <div class="tab-content">

                                <!-- Validation summary -->
                                <div asp-validation-summary="ModelOnly"></div>

                                <!-- FORM SATIRLARI -->
                                <!-- Tekli alan (tam genişlik) -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Alan Adı
                                    </label>
                                    <div class="col-lg-10 col-sm-12">
                                        <input class="form-control form-control-sm"
                                               asp-for="FieldName" type="text" />
                                        <span asp-validation-for="FieldName" class="text-danger"></span>
                                    </div>
                                </div>

                                <!-- İkili alan (yan yana) -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Alan 1
                                    </label>
                                    <div class="col-lg-4 col-sm-12">
                                        <input class="form-control form-control-sm"
                                               asp-for="Field1" type="text" />
                                        <span asp-validation-for="Field1" class="text-danger"></span>
                                    </div>
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Alan 2
                                    </label>
                                    <div class="col-lg-4 col-sm-12">
                                        <input class="form-control form-control-sm"
                                               asp-for="Field2" type="text" />
                                        <span asp-validation-for="Field2" class="text-danger"></span>
                                    </div>
                                </div>

                                <!-- Select (dropdown) -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Seçim
                                    </label>
                                    <div class="col-lg-4 col-sm-12">
                                        <select class="form-control form-control-sm select2"
                                                asp-for="SelectField"
                                                asp-items="@ViewBag.SelectList">
                                            <option value="">Seçiniz...</option>
                                        </select>
                                    </div>
                                </div>

                                <!-- Toggle switch (checkbox) -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Aktif mi?
                                    </label>
                                    <div class="col-1">
                                        <span class="switch switch-outline switch-sm switch-icon switch-success">
                                            <label>
                                                <input type="checkbox" asp-for="IsActive" />
                                                <span></span>
                                            </label>
                                        </span>
                                    </div>
                                </div>

                                <!-- Tarih seçici (datetimepicker) -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Tarih
                                    </label>
                                    <div class="col-lg-4 col-sm-12">
                                        <div class="input-group date" id="DateField_picker"
                                             data-target-input="nearest">
                                            <input type="text"
                                                   class="form-control datetimepicker-input"
                                                   id="DateField"
                                                   name="DateField"
                                                   value="@DateTime.Now.ToString("dd-MM-yyyy")"
                                                   data-target="#DateField_picker" />
                                            <div class="input-group-append"
                                                 data-target="#DateField_picker"
                                                 data-toggle="datetimepicker">
                                                <span class="input-group-text">
                                                    <i class="ki ki-calendar"></i>
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <!-- Textarea -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Açıklama
                                    </label>
                                    <div class="col-lg-10 col-sm-12">
                                        <textarea class="form-control" asp-for="Description"
                                                  rows="3"></textarea>
                                        <span asp-validation-for="Description" class="text-danger"></span>
                                    </div>
                                </div>

                                <!-- Sayısal input (TouchSpin) -->
                                <div class="form-group row">
                                    <label class="col-form-label text-right col-lg-2 col-sm-12">
                                        Sayı
                                    </label>
                                    <div class="col-lg-4 col-sm-12">
                                        <input asp-for="CountField"
                                               type="text"
                                               class="form-control bootstrap-touchspin-vertical-btn"
                                               value="0" placeholder="0" />
                                        <span asp-validation-for="CountField" class="text-danger"></span>
                                    </div>
                                </div>

                            </div>
                        </div><!-- /card-body -->

                        <!-- CARD FOOTER -->
                        <div class="card-footer">
                            <button type="submit" class="btn btn-primary mr-2">
                                {{Modül Adı}} Ekle
                            </button>
                            <a href="~/{{Controller}}" class="btn btn-secondary">Vazgeç</a>
                        </div>

                    </div><!-- /card -->
                </form>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/custom/create{{entityname}}page.js"></script>
}
```

## Update Sayfası Farkları

Update sayfasında model `EditEntityViewModel` tipindedir ve id hidden field ile taşınır:

```cshtml
@model YourProject.Core.ViewModels.UpdateEntityViewModel
@{
    ViewData["Title"] = "{{Modül Adı}} Güncelle";
}

<form asp-action="Update" asp-controller="{{Controller}}" method="post">
    <input type="hidden" asp-for="Id" />
    <!-- Aynı form alanları, value'lar model'den gelir -->
    <div class="card-footer">
        <button type="submit" class="btn btn-warning mr-2">Güncelle</button>
        <a asp-action="Index" asp-controller="{{Controller}}" class="btn btn-secondary">Vazgeç</a>
    </div>
</form>
```

## Select2 Başlatma (JS'de)

Form sayfasındaki `select2` elementleri JS dosyasında init edilir:

```javascript
var initSelects = function () {
    $('#FieldName').select2({
        placeholder: 'Lütfen Seçiniz',
        allowClear: true
    });
    // Ajax ile dolan select2:
    $('#DynamicField').select2({
        placeholder: 'Arama yapın...',
        ajax: {
            url: '/Controller/GetList',
            dataType: 'json',
            processResults: function (data) {
                return {
                    results: data.map(function(item) {
                        return { id: item.id, text: item.name };
                    })
                };
            }
        }
    });
};
```

## Datetimepicker Başlatma (JS'de)

```javascript
var initDatePickers = function () {
    $('#DateField_picker').datetimepicker({
        format: 'DD-MM-YYYY'
    });
    // Saat ile birlikte:
    $('#DateTimePicker').datetimepicker({
        format: 'DD-MM-YYYY HH:mm'
    });
};
```

## Form Alanı Genişlik Referansı

| Kullanım | label class | input wrapper class |
|---|---|---|
| Tam genişlik | `col-lg-2` | `col-lg-10` |
| Yarım (yan yana 2 alan) | `col-lg-2` | `col-lg-4` |
| Üçte bir (yan yana 3 alan) | `col-lg-2` | `col-lg-2` |
| Switch toggle | `col-lg-2` | `col-1` |
