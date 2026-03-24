# 10 — Dashboard & Partial Compose

## Dashboard Sayfası Yapısı

Dashboard sayfaları birden fazla partial'ı `@await Html.PartialAsync` ile birleştirir.
Sayfa kendisi ince kalır, iş mantığı partial'lara dağıtılır.

### Dashboard Index.cshtml Şablonu

```cshtml
@model (
    List<SupportListViewModel> supportList,
    List<UserInfoViewModel> users,
    List<DepartmentViewModel> departments,
    List<ChartViewModel> chartData,
    KpiViewModel kpis
)
@{
    ViewData["Title"] = "Dashboard";
    ViewData["ActivePage"] = "Dashboard";
}

<div class="content d-flex flex-column flex-column-fluid" id="kt_content">
    <div class="d-flex flex-column-fluid">
        <div class="container-fluid">

            <!-- KPI Kartları (üst bant) -->
            @await Html.PartialAsync("_DashboardKpiPartial", Model.kpis)

            <!-- Ana içerik satırı -->
            <div class="row">
                @await Html.PartialAsync("_ListPartial",
                    (Model.supportList, Model.users, Model.departments))
                @await Html.PartialAsync("_ChartPartial",
                    (Model.chartData))
            </div>

            <!-- Modal'lar (ayrı partial) -->
            @await Html.PartialAsync("_ModalsPartial")

        </div>
    </div>
</div>

@section Scripts {
    <script src="~/js/custom/list-page.js"></script>
    <script src="~/js/custom/modal-page.js"></script>
    <script src="~/js/custom/chart-page.js"></script>
}
```

---

## KPI Kartı Partial Şablonu

```cshtml
@model KpiViewModel

<div class="card card-custom gutter-b">
    <div class="card-body">
        <div class="d-flex align-items-center flex-wrap">

            <!-- KPI Item -->
            <div class="d-flex align-items-center flex-lg-fill mr-5 my-1">
                <span class="mr-4">
                    <i class="{{ikon-class}} icon-2x text-muted font-weight-bold"></i>
                </span>
                <div class="d-flex flex-column text-dark-75">
                    <span class="font-weight-bolder font-size-sm">{{KPI Adı}}</span>
                    <span class="font-weight-bolder font-size-h1">
                        <span class="font-weight-bold text-{{renk}}">@Model.{{KpiValue}}</span>
                    </span>
                </div>
            </div>

            <!-- Diğer KPI item'lar aynı şekilde tekrarlanır -->

        </div>
    </div>
</div>
```

**Renk seçenekleri:** `text-danger`, `text-primary`, `text-warning`, `text-success`, `text-info`

---

## Stat Widget Kartı (Grafik + Sayaç)

```cshtml
<div class="card card-custom card-stretch card-stretch-half gutter-b">
    <div class="card-body p-0">
        <!-- Sayaç -->
        <div class="d-flex align-items-center justify-content-between card-spacer flex-grow-1">
            <span class="symbol symbol-50 symbol-light-success mr-2">
                <span class="symbol-label">
                    <span class="svg-icon svg-icon-xl svg-icon-success">
                        <!-- SVG ikon -->
                    </span>
                </span>
            </span>
            <div class="d-flex flex-column text-right">
                <span class="text-dark-75 font-weight-bolder font-size-h3"
                      id="{{sayac-id}}">0</span>
                <span class="text-muted font-weight-bold mt-2">{{Sayaç Etiketi}}</span>
            </div>
        </div>
        <!-- Mini grafik alanı (ApexCharts) -->
        <div id="{{grafik-id}}" class="card-rounded-bottom"
             data-color="success" style="height: 150px"></div>
    </div>
</div>
```

---

## Partial — Veri Aktarma Yöntemleri

### Tuple model (birden fazla liste)
```csharp
// View'da:
@await Html.PartialAsync("_PartialName", (Model.list1, Model.list2))

// Partial'da:
@model (List<Type1> list1, List<Type2> list2)
// Kullanım:
@foreach (var item in Model.list1) { ... }
```

### Tek model
```csharp
@await Html.PartialAsync("_KpiPartial", Model.kpis)

// Partial'da:
@model KpiViewModel
```

### ViewBag/TempData ile
```cshtml
@{
    ViewBag.ExtraData = "değer";
}
@await Html.PartialAsync("_PartialName", Model)
// Partial içinde ViewBag.ExtraData ile erişilir
```

---

## Boş Durum Kartı (Empty State)

Veri yoksa gösterilecek placeholder kart:

```cshtml
@if (Model.Items.Count < 1)
{
    <div class="card card-custom bgi-no-repeat gutter-b card-stretch"
         style="background-color: #FFF4DE;
                background-image: url(/media/svg/patterns/taieri.svg);
                background-position: 0 calc(100% + 0.5rem);
                background-size: 100% auto;">
        <div class="card-body">
            <div class="p-4">
                <h3 class="font-weight-bolder my-7">Henüz Kayıt Bulunmuyor</h3>
                <p class="font-size-lg mb-7">
                    Bu alanda görüntülenecek<br />bir kayıt bulunmuyor.
                </p>
            </div>
        </div>
    </div>
}
else
{
    <!-- gerçek içerik -->
}
```

---

## Dashboard Controller Örneği

```csharp
public async Task<IActionResult> Index()
{
    var supportList  = await _supportService.GetListAsync();
    var users        = await _userService.GetCrmUsersAsync();
    var departments  = await _deptService.GetUserDepartmentsAsync(currentUserId);
    var chartData    = await _supportService.GetDailyChartAsync();
    var kpis         = await _supportService.GetKpiAsync(currentUserId);

    var model = (supportList, users, departments, chartData, kpis);
    return View(model);
}

// Ajax endpoint (JS'ten çağrılır):
[HttpGet]
public async Task<IActionResult> GetDashboardChartData()
{
    var data = await _service.GetChartDataAsync();
    return Json(new { errors = false, data });
}
```

---

## Popover ile Avatar Listesi (Kullanıcı listesi widget)

```cshtml
<div class="symbol-group symbol-hover mt-5">
    @foreach (var user in Model.Users)
    {
        @{
            var popoverContent = BuildTimelineHtml(user.Records);
        }
        <div class="symbol symbol-50 mr-3">
            <img src="/media/avatar/@user.Avatar"
                 data-toggle="popover"
                 title="@user.FullName"
                 data-html="true"
                 data-content="@popoverContent" />
        </div>
    }
</div>
```

Timeline HTML builder (Razor'da):
```csharp
@{
    var sb = new StringBuilder();
    foreach (var record in user.Records.OrderBy(x => x.Time))
    {
        sb.Append($@"
            <div class='timeline-item align-items-start'>
                <div class='timeline-label font-weight-bolder text-dark-75 font-size-lg'>
                    {record.Time}
                </div>
                <div class='timeline-badge'>
                    <i class='fa fa-genderless text-warning icon-xl'></i>
                </div>
                <div class='font-weight-mormal font-size-lg timeline-content text-muted pl-3'>
                    {record.Title}
                </div>
            </div>");
    }
    var popoverHtml = $"<div class='timeline timeline-6 mt-3'>{sb}</div>";
}
```
