# 11 — DataTable Child Row (Satır Detayı)

## Genel Davranış
- Her satırın solunda `>` (chevron) ikonu bulunur
- Tıklanınca ikon aşağı döner (`▼`), child row açılır
- Tekrar tıklanınca kapanır
- **Ajax versiyonunda:** açılırken loading spinner gösterilir, veri gelince tablo render edilir
- **Model versiyonunda:** veri zaten sayfada vardır, gizli JSON olarak `data-` attribute'a gömülür

---

## VERSİYON 1 — Model İçinden (Razor, veri sayfada gömülü)

### View (.cshtml)

```cshtml
@model IEnumerable<InvoiceListViewModel>

<table class="table table-bordered table-hover" id="InvoiceTable"
       style="margin-top: 13px !important">
    <thead>
        <tr>
            <th></th>  {{!-- chevron kolonu --}}
            <th>Fatura No</th>
            <th>Tarih</th>
            <th>Tutar</th>
            <th>Durum</th>
            <th>İşlemler</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            {{!-- Child veriyi JSON olarak data attribute'a göm --}}
            var linesJson = System.Text.Json.JsonSerializer.Serialize(item.Lines);
            <tr data-lines="@linesJson">
                <td class="child-toggle">
                    <span class="svg-icon svg-icon-sm svg-icon-primary child-icon">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                             viewBox="0 0 24 24">
                            <path d="M8.59 16.59L13.17 12 8.59 7.41 10 6l6 6-6 6z"
                                  fill="currentColor"/>
                        </svg>
                    </span>
                </td>
                <td>@item.InvoiceNo</td>
                <td>@item.Date.ToString("dd-MM-yyyy")</td>
                <td>@item.Total.ToString("N2") ₺</td>
                <td>
                    <span class="label label-success label-pill label-inline">@item.Status</span>
                </td>
                <td>
                    <a asp-controller="Invoice" asp-action="Update"
                       asp-route-id="@item.Id"
                       class="btn btn-icon btn-warning btn-sm"
                       data-toggle="tooltip" title="Güncelle" data-theme="dark">
                        <i class="flaticon-edit"></i>
                    </a>
                </td>
            </tr>
        }
    </tbody>
</table>

@section Scripts {
    <script src="~/js/custom/invoice-list-page.js"></script>
}
```

### JavaScript (`invoice-list-page.js`)

```javascript
'use strict';
var InvoiceListPage = function () {

    var table;

    // Child row HTML oluştur — sade tablo versiyonu
    var buildChildTable = function (lines) {
        if (!lines || lines.length === 0) {
            return '<div class="p-4 text-muted">Satır bulunamadı.</div>';
        }

        var html = '<div class="p-4">' +
            '<table class="table table-sm table-bordered" style="background:#f9f9f9">' +
            '<thead class="thead-light">' +
            '<tr>' +
            '<th>Ürün</th>' +
            '<th>Miktar</th>' +
            '<th>Birim Fiyat</th>' +
            '<th>Toplam</th>' +
            '</tr>' +
            '</thead><tbody>';

        $.each(lines, function (i, line) {
            html += '<tr>' +
                '<td>' + line.productName + '</td>' +
                '<td>' + line.quantity + '</td>' +
                '<td>' + parseFloat(line.unitPrice).toFixed(2) + ' ₺</td>' +
                '<td>' + parseFloat(line.total).toFixed(2) + ' ₺</td>' +
                '</tr>';
        });

        html += '</tbody></table></div>';
        return html;
    };

    // Child row HTML oluştur — key-value detay versiyonu
    var buildChildKeyValue = function (lines) {
        var html = '<div class="p-4"><div class="row">';
        $.each(lines, function (i, line) {
            html += '<div class="col-md-4 mb-2">' +
                '<span class="text-danger font-weight-bold font-size-sm">' + line.label + ': </span>' +
                '<span class="text-muted font-size-sm">' + line.value + '</span>' +
                '</div>';
        });
        html += '</div></div>';
        return html;
    };

    var initDataTable = function () {
        table = $('#InvoiceTable').DataTable({
            responsive: false,
            columnDefs: [
                { orderable: false, targets: [0, -1] },  // chevron ve işlemler kolonu
                { width: '40px', targets: 0 }
            ]
        });

        // Chevron tıklama
        $('#InvoiceTable tbody').on('click', 'td.child-toggle', function () {
            var tr   = $(this).closest('tr');
            var row  = table.row(tr);
            var icon = tr.find('.child-icon');

            if (row.child.isShown()) {
                // Kapat
                row.child.hide();
                tr.removeClass('shown');
                icon.css('transform', 'rotate(0deg)');
            } else {
                // Aç — veriyi data attribute'dan oku
                var lines = tr.data('lines');
                row.child(buildChildTable(lines)).show();
                tr.addClass('shown');
                icon.css('transform', 'rotate(90deg)');
            }
        });
    };

    return {
        init: function () {
            initDataTable();
        }
    };
}();

window.addEventListener('load', function () {
    InvoiceListPage.init();
});
```

---

## VERSİYON 2 — Ajax ile (tıklanınca sunucudan çekilen)

### View (.cshtml)

```cshtml
@model IEnumerable<InvoiceListViewModel>

<table class="table table-bordered table-hover" id="InvoiceTable"
       style="margin-top: 13px !important">
    <thead>
        <tr>
            <th></th>  {{!-- chevron kolonu --}}
            <th>Fatura No</th>
            <th>Tarih</th>
            <th>Tutar</th>
            <th>Durum</th>
            <th>İşlemler</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model)
        {
            <tr data-id="@item.Id">
                <td class="child-toggle">
                    <span class="svg-icon svg-icon-sm svg-icon-primary child-icon">
                        <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24"
                             viewBox="0 0 24 24">
                            <path d="M8.59 16.59L13.17 12 8.59 7.41 10 6l6 6-6 6z"
                                  fill="currentColor"/>
                        </svg>
                    </span>
                </td>
                <td>@item.InvoiceNo</td>
                <td>@item.Date.ToString("dd-MM-yyyy")</td>
                <td>@item.Total.ToString("N2") ₺</td>
                <td>
                    <span class="label label-success label-pill label-inline">@item.Status</span>
                </td>
                <td>
                    <a asp-controller="Invoice" asp-action="Update"
                       asp-route-id="@item.Id"
                       class="btn btn-icon btn-warning btn-sm"
                       data-toggle="tooltip" title="Güncelle" data-theme="dark">
                        <i class="flaticon-edit"></i>
                    </a>
                </td>
            </tr>
        }
    </tbody>
</table>

@section Scripts {
    <script src="~/js/custom/invoice-list-page.js"></script>
}
```

### JavaScript (`invoice-list-page.js`) — Ajax versiyonu

```javascript
'use strict';
var InvoiceListPage = function () {

    var table;

    // Loading satırı
    var loadingHtml =
        '<div class="p-4 d-flex align-items-center">' +
        '<div class="spinner spinner-primary spinner-sm mr-3"></div>' +
        '<span class="text-muted font-size-sm">Yükleniyor...</span>' +
        '</div>';

    // Child tablo oluştur
    var buildChildTable = function (lines) {
        if (!lines || lines.length === 0) {
            return '<div class="p-4 text-muted">Satır bulunamadı.</div>';
        }

        var html =
            '<div class="p-4">' +
            '<table class="table table-sm table-bordered" style="background:#f9f9f9">' +
            '<thead class="thead-light"><tr>' +
            '<th>Ürün</th><th>Miktar</th><th>Birim Fiyat</th><th>Toplam</th>' +
            '</tr></thead><tbody>';

        $.each(lines, function (i, line) {
            html += '<tr>' +
                '<td>' + line.productName + '</td>' +
                '<td>' + line.quantity + '</td>' +
                '<td>' + parseFloat(line.unitPrice).toFixed(2) + ' ₺</td>' +
                '<td>' + parseFloat(line.total).toFixed(2) + ' ₺</td>' +
                '</tr>';
        });

        html += '</tbody></table></div>';
        return html;
    };

    // Key-value detay oluştur
    var buildKeyValue = function (detail) {
        var html = '<div class="p-4"><div class="row">';
        $.each(detail, function (key, value) {
            html += '<div class="col-md-4 mb-2">' +
                '<span class="text-danger font-weight-bold font-size-sm">' + key + ': </span>' +
                '<span class="text-muted font-size-sm">' + value + '</span>' +
                '</div>';
        });
        html += '</div></div>';
        return html;
    };

    var initDataTable = function () {
        table = $('#InvoiceTable').DataTable({
            responsive: false,
            columnDefs: [
                { orderable: false, targets: [0, -1] },
                { width: '40px', targets: 0 }
            ]
        });

        // Chevron tıklama
        $('#InvoiceTable tbody').on('click', 'td.child-toggle', function () {
            var tr   = $(this).closest('tr');
            var row  = table.row(tr);
            var icon = tr.find('.child-icon');
            var id   = tr.data('id');

            if (row.child.isShown()) {
                // Kapat
                row.child.hide();
                tr.removeClass('shown');
                icon.css('transform', 'rotate(0deg)');
                return;
            }

            // Loading göster, hemen aç
            row.child(loadingHtml).show();
            tr.addClass('shown');
            icon.css('transform', 'rotate(90deg)');

            // Ajax ile veri çek
            $.get('/Invoice/GetLines/' + id).done(function (result) {
                if (result.isSuccess) {
                    row.child(buildChildTable(result.data)).show();
                } else {
                    row.child(
                        '<div class="p-4 text-danger">' + result.message + '</div>'
                    ).show();
                }
            }).fail(function () {
                row.child(
                    '<div class="p-4 text-danger">Veri yüklenirken hata oluştu.</div>'
                ).show();
            });
        });
    };

    return {
        init: function () {
            initDataTable();
        }
    };
}();

window.addEventListener('load', function () {
    InvoiceListPage.init();
});
```

### Controller — Ajax Endpoint

```csharp
[HttpGet]
public async Task<IActionResult> GetLines(Guid id)
{
    var lines = await _invoiceService.GetLinesAsync(id);
    if (lines == null)
        return Json(new { isSuccess = false, message = "Satırlar bulunamadı." });

    return Json(new { isSuccess = true, data = lines });
}
```

---

## Chevron İkonu CSS (style.bundle içinde yoksa ekle)

```css
.child-icon {
    display: inline-block;
    transition: transform 0.2s ease;
    cursor: pointer;
    color: #3699FF;
}
.child-toggle {
    cursor: pointer;
    width: 40px;
    text-align: center;
}
/* Child row arka plan */
#TabloId tbody tr.shown + tr > td {
    background-color: #f3f6f9 !important;
    padding: 0 !important;
}
```

> CSS'i ilgili sayfanın `@section Scripts` bloğuna `<style>` etiketi içinde veya
> `wwwroot/css/custom.css` dosyasına ekle.

---

## Hangi Versiyonu Seçmeli?

| Durum | Versiyon |
|---|---|
| Satır sayısı azdır, veri zaten modelde var | Model (V1) |
| Tablo büyük, her satır için alt detay ağırdır | Ajax (V2) |
| Fatura satırları, sipariş kalemleri | Ajax (V2) — lazy load |
| Kullanıcı izinleri, basit detay | Model (V1) |
