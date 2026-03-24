# 08 — JavaScript Pattern'leri

## Dosya Konumu ve Adlandırma

- **Tüm sayfa JS dosyaları:** `wwwroot/js/custom/`
- **Adlandırma:** `{sayfa-adi}-page.js` → ör: `module-list-page.js`, `applicationlist.js`
- **View'a bağlama:** `@section Scripts { <script src="~/js/custom/{dosya}.js"></script> }`
- `PageUtilities.js` layout'ta global olarak yüklenir, sayfa JS'lerinden önce gelir.

## Temel Modül Pattern'i (Tüm dosyalar bu yapıyı kullanır)

```javascript
'use strict';
var {{SayfaModulAdi}} = function () {

    // Private fonksiyonlar
    var {{initFonksiyon1}} = function () {
        // ...
    };

    var {{initFonksiyon2}} = function () {
        // ...
    };

    // Public API
    return {
        init: function () {
            {{initFonksiyon1}}();
            {{initFonksiyon2}}();
        }
    };
}();

// Sayfa yüklenince başlat
window.addEventListener('load', function () {
    {{SayfaModulAdi}}.init();
});
// veya:
jQuery(document).ready(function () {
    {{SayfaModulAdi}}.init();
});
```

---

## DataTable Başlatma

```javascript
var initDataTable = function () {
    var table = $('#{{tabloId}}');
    table.DataTable({
        responsive: true,
        columnDefs: [
            { orderable: false, targets: -1 }  // Son kolon (İşlemler) sıralanamaz
        ]
    });
};
```

### DataTable — Ajax ile veri çekme (server-side)

```javascript
var initDataTable = function () {
    $('#{{tabloId}}').DataTable({
        processing: true,
        serverSide: true,
        ajax: {
            url: '/Controller/GetList',
            type: 'GET'
        },
        columns: [
            { data: 'field1' },
            { data: 'field2' },
            {
                data: null,
                render: function (data) {
                    return '<a href="/Controller/Update/' + data.id + '" class="btn btn-warning btn-sm">' +
                           '<i class="flaticon-edit"></i></a>';
                }
            }
        ]
    });
};
```

---

## SweetAlert2 — Silme Onayı

```javascript
var initDeleteButton = function () {
    $('.delete-bt').on('click', function () {
        var id   = $(this).data('id');
        var name = $(this).data('name');

        Swal.fire({
            title: name + ' İsimli Kayıt Siliniyor',
            text: 'Bunu Yapmak İstediğinizden Emin misiniz?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet, Sil!',
            cancelButtonText: 'Hayır, Vazgeçtim!',
            reverseButtons: true
        }).then(function (result) {
            if (result.value) {
                $.get('/Controller/Delete/' + id).done(function (result) {
                    if (result.isSuccess) {
                        toastr.success('Kayıt Başarıyla Silindi', result.message);
                    } else {
                        toastr.error('Bir Hata Oluştu', result.message);
                    }
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000);
                });
            } else if (result.dismiss === 'cancel') {
                Swal.fire('İptal Edildi', 'Silme işlemi kullanıcı tarafından iptal edildi', 'error');
            }
        });
    });
};
```

## SweetAlert2 — Durum Değiştirme (Active/Passive toggle)

```javascript
var initChangeStatus = function () {
    $('.change-status-bt').on('click', function () {
        var id     = $(this).data('id');        // veya data-moduleId gibi özel isimler
        var name   = $(this).data('name');
        var status = $(this).data('status');
        var title  = name + ' durumu değiştiriliyor';

        Swal.fire({
            title: title,
            text: 'Bunu Yapmak İstediğinizden Emin misiniz?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Evet!',
            cancelButtonText: 'Hayır, Vazgeçtim!',
            reverseButtons: true
        }).then(function (result) {
            if (result.value) {
                var model = { Id: id, Status: status };
                $.post('/Controller/ChangeStatus', model).done(function (result) {
                    if (result.isSuccess) {
                        toastr.success('Durum Güncellendi', result.message);
                    } else {
                        toastr.error('Bir Hata Oluştu', result.message);
                    }
                    setTimeout(function () { window.location.reload(); }, 2000);
                });
            }
        });
    });
};
```

---

## Toastr Bildirimleri

```javascript
// Başarı
toastr.success('İşlem Başarılı', 'Detay mesajı');

// Hata
toastr.error('Bir Hata Oluştu', 'Hata detayı');

// Uyarı
toastr.warning('Dikkat', 'Uyarı mesajı');

// Bilgi
toastr.info('Bilgi', 'Mesaj');
```

---

## Ajax ile Modal Doldurma

```javascript
var initDetailModal = function () {
    $('.detail-bt').on('click', function () {
        var id          = $(this).data('id');
        var module      = $(this).data('module');
        var isTransfer  = $(this).data('isTransferUser');

        $.get('/Controller/GetDetail/' + id).done(function (result) {
            if (result.isSuccess) {
                var data = result.data;
                // Label/span güncelleme:
                $('#det_field_sb').text(data.fieldName);
                // Input güncelleme:
                $('#det_input_tb').val(data.inputField);
                // Checkbox:
                $('#det_checkbox_cb').prop('checked', data.isActive);
                // Modal başlığı:
                $('#detail-modal-label').text(data.title + ' Detayı');
                // Koşullu buton göster/gizle:
                if (isTransfer) {
                    $('#det_assign_bt').show();
                } else {
                    $('#det_assign_bt').hide();
                }
                // Modal aç:
                $('#detail-modal').modal('show');
            } else {
                toastr.error('Veri yüklenemedi', result.message);
            }
        });
    });
};
```

## Ajax ile Form Submit (Modal içinden)

```javascript
var initSaveButton = function () {
    $('#save_bt').on('click', function () {
        var model = {
            FirmId:      $('#modal_firm_sb').val(),
            Description: $('#modal_desc_tb').val(),
            Date:        $('#modal_date_tb').val()
        };

        // Basit validasyon
        if (!model.FirmId) {
            toastr.warning('Firma seçilmedi', 'Lütfen firma seçiniz');
            return;
        }

        $.post('/Controller/Save', model).done(function (result) {
            if (result.isSuccess) {
                toastr.success('Kaydedildi', result.message);
                $('#save-modal').modal('hide');
                setTimeout(function () { window.location.reload(); }, 2000);
            } else {
                toastr.error('Hata', result.message);
            }
        });
    });
};
```

## Select2 — Bağımlı Dropdown (Cascade)

```javascript
var initCascadeSelects = function () {
    // Üst select değişince alt select'i doldur
    $('#parent_sb').on('change', function () {
        var parentId = $(this).val();
        $('#child_sb').empty().append('<option value="">Seçiniz...</option>');

        if (!parentId) return;

        $.get('/Controller/GetChildren/' + parentId).done(function (result) {
            $.each(result.data, function (i, item) {
                $('#child_sb').append(
                    $('<option>').val(item.id).text(item.name)
                );
            });
        });
    });
};
```

## ApexCharts — Ajax ile Grafik (Dashboard)

```javascript
var initChart = function () {
    $.get('/Controller/GetChartData').done(function (result) {
        if (!result.errors) {
            var element = document.getElementById('chartElementId');
            if (!element) return;

            var options = {
                series: [{ name: 'Veri', data: result.data.values }],
                chart: { type: 'area', height: 150, sparkline: { enabled: true } },
                stroke: { curve: 'smooth', width: 3, colors: ['#3699FF'] },
                xaxis: { categories: result.data.dates },
                tooltip: {
                    y: { formatter: function (val) { return val + ' kayıt'; } }
                },
                colors: [KTApp.getSettings()['colors']['theme']['light']['primary']],
            };

            var chart = new ApexCharts(element, options);
            chart.render();

            // Sayaç güncelleme
            $('#counter_element').text(result.data.totalCount);
        }
    });
};
```

## Controller — JSON Response Pattern

Controller'dan JS'e dönen standart response:

```csharp
// Başarılı:
return Json(new { isSuccess = true, message = "İşlem başarılı" });

// Başarısız:
return Json(new { isSuccess = false, message = "Hata oluştu" });

// Veri ile:
return Json(new { isSuccess = true, data = viewModel });

// Hata (errors field ile — ApexCharts pattern):
return Json(new { errors = false, data = chartData });
```
