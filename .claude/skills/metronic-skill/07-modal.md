# 07 — Modal Bileşenleri

## Genel Kurallar

- Modal HTML'leri sayfanın **en altına**, `@section Scripts` bloğundan önce yerleştirilir.
- Her modal'ın unique bir `id`'si olur.
- İçeriği Ajax ile doldurulan modal'larda body boş bırakılır, JS ile doldurulur.
- Modal boyutları: `modal-sm` / (varsayılan) / `modal-lg` / `modal-xl`
- Scrollable modal için: `modal-dialog-scrollable`
- Ortalanmış modal için: `modal-dialog-centered`

## Temel Modal Şablonu

```html
<div class="modal fade" id="{{modal-id}}" tabindex="-1" role="dialog"
     aria-labelledby="{{modal-id}}-label" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered modal-xl modal-dialog-scrollable">
        <div class="modal-content">

            <!-- HEADER -->
            <div class="modal-header">
                <h5 class="modal-title" id="{{modal-id}}-label">{{Modal Başlığı}}</h5>
                <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                    <i aria-hidden="true" class="ki ki-close"></i>
                </button>
            </div>

            <!-- BODY -->
            <div class="modal-body">
                <!-- içerik buraya -->
            </div>

            <!-- FOOTER -->
            <div class="modal-footer">
                <button type="button" class="btn btn-light-primary font-weight-bold"
                        data-dismiss="modal">İptal</button>
                <button type="button" id="{{kaydet-bt-id}}"
                        class="btn btn-success font-weight-bold">Kaydet</button>
            </div>

        </div>
    </div>
</div>
```

## Modal Body — Form Alanları

Modal içindeki form alanları sayfadaki formlarla aynı pattern'i kullanır
**ancak** `<form>` etiketi olmadan, JS ile submit edilir:

```html
<div class="modal-body">
    <form class="form">  <!-- veya sadece <form class=""> -->
        <div class="form-group row">
            <label class="col-form-label text-right col-lg-2 col-sm-12">Alan Adı</label>
            <div class="col-lg-10 col-md-10 col-sm-12">
                <input class="form-control form-control-sm" id="modal_field_tb" type="text" />
            </div>
        </div>
        <div class="form-group row">
            <label class="col-form-label text-right col-lg-2 col-sm-12">Select</label>
            <div class="col-lg-10 col-md-10 col-sm-12">
                <select class="form-control select2" id="modal_select_sb"></select>
            </div>
        </div>
    </form>
</div>
```

## Modal Body — Kart İçinde Form (iç içe card)

Detay gerektiren büyük modal'larda form alanları card'larla gruplanır:

```html
<div class="modal-body">
    <div class="card card-custom gutter-b card-border border-primary">
        <div class="card-header">
            <h5 class="modal-title text-dark-50 mt-5">Bölüm Başlığı</h5>
        </div>
        <div class="card-body">
            <form class="">
                <!-- form alanları -->
            </form>
        </div>
    </div>
    <!-- ikinci kart -->
    <div class="card card-custom gutter-b card-border border-primary">
        ...
    </div>
</div>
```

## Modal Footer — Buton Kombinasyonları

### Standart (iptal + kaydet)
```html
<div class="modal-footer">
    <button type="button" class="btn btn-light-primary font-weight-bold"
            data-dismiss="modal">İptal</button>
    <button type="button" id="save_bt"
            class="btn btn-success font-weight-bold">Kaydet</button>
</div>
```

### Çoklu aksiyon butonu
```html
<div class="modal-footer">
    <button type="button" class="btn btn-light-primary font-weight-bold"
            data-dismiss="modal">İptal</button>
    <button type="button" id="start_bt"
            class="btn btn-primary font-weight-bold">Üzerine Al/Başla</button>
    <button type="button" id="assign_bt"
            class="btn btn-danger font-weight-bold">Kullanıcıya Ata</button>
    <button type="button" id="update_bt"
            class="btn btn-warning font-weight-bold">Güncelle</button>
</div>
```

## Modal İçinde Tablo (DataTable)

```html
<div class="modal-body">
    <div class="alert alert-custom alert-outline-2x alert-outline-danger fade show mb-5" role="alert">
        <div class="alert-icon"><i class="flaticon-danger"></i></div>
        <div class="alert-text">Uyarı mesajı buraya.</div>
        <div class="alert-close">
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true"><i class="ki ki-close"></i></span>
            </button>
        </div>
    </div>

    <table class="table table-bordered table-hover" id="ModalTable"
           style="margin-top: 13px !important">
        <thead>
            <tr>
                <th>Kolon 1</th>
                <th>Kolon 2</th>
            </tr>
        </thead>
        <tbody></tbody>
    </table>
</div>
```

## Modal Açma Yöntemleri

### Buton data attribute ile tetikleme (Bootstrap)
```html
<button data-toggle="modal" data-target="#{{modal-id}}"
        class="btn btn-primary">Aç</button>
```

### JS ile açma (data attribute + jQuery)
```html
<!-- Tetikleyici buton -->
<button class="btn btn-success btn-sm open-modal-bt"
        data-id="@item.Id"
        data-name="@item.Name">Detay</button>
```

```javascript
// JS'de:
$('.open-modal-bt').on('click', function () {
    var id = $(this).data('id');
    var name = $(this).data('name');
    // Modal başlığını güncelle
    $('#{{modal-id}}-label').text(name + ' Detayı');
    // Ajax ile doldur (opsiyonel)
    $.get('/Controller/GetDetail/' + id).done(function (result) {
        $('#modal_field_tb').val(result.data.field);
    });
    $('#{{modal-id}}').modal('show');
});
```

## ID Adlandırma Kuralları

Modal element ID'leri **prefix** sistemi ile çakışmayı önler:

| Prefix | Kullanım | Örnek |
|---|---|---|
| `crt_` | Create modal alanları | `crt_sup_firm_sb` |
| `upt_` | Update modal alanları | `upt_ticket_state` |
| `det_` | Detail/görüntüleme alanları | `det_sup_firm_sb` |
| `to_` | Take-on modal | `to_sup_description_tb` |

## Element ID Suffix Kuralları

| Suffix | Tip | Örnek |
|---|---|---|
| `_tb` | Text input / textarea | `crt_sup_notes_tb` |
| `_sb` | Select/dropdown | `crt_sup_firm_sb` |
| `_cb` | Checkbox | `crt_logo_cb` |
| `_bt` | Button | `save_bt` |
| `_lb` | Label | `det_sup_logo_exp_lb` |
| `_hdn` | Hidden input | `crt_sup_projectcode_hdn` |
| `_div` | Container div | `upt_sup_close_div` |
