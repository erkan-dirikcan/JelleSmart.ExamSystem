# 03 — Auth Sayfaları

## Genel Kurallar
- Auth sayfaları `Layout = null` kullanır — standalone HTML dökümanıdır.
- Login + ForgotPassword **tek bir View**'da, toggle ile gösterilir.
- Model tipi: tuple `(LoginViewModel LoginModel, ForgetPasswordViewModel ForgetModel)`
- Controller: `UserController`

---

## Login.cshtml — Sayfa Yapısı

```cshtml
@model (LoginViewModel LoginModel, ForgetPasswordViewModel ForgetModel)
@{
    Layout = null;
    ViewData["Title"] = "Kullanıcı Girişi";
}
<!DOCTYPE html>
<html lang="tr">
<head>
    <title>{{Uygulama Adı}} | @ViewData["Title"]</title>
    <meta name="description" content="{{Uygulama Açıklaması}}" />
    <link rel="canonical" href="{{URL}}" />
    <link rel="stylesheet" href="https://fonts.googleapis.com/css?family=Poppins:300,400,500,600,700" />
    <link href="~/css/pages/login/login-1.css" rel="stylesheet" />
    <link href="~/plugins/global/plugins.bundle.css" rel="stylesheet" />
    <link href="~/plugins/custom/prismjs/prismjs.bundle.css" rel="stylesheet" />
    <link href="~/css/style.bundle.css" rel="stylesheet" />
    <link href="~/css/themes/layout/header/base/light.css" rel="stylesheet" />
    <link href="~/css/themes/layout/header/menu/light.css" rel="stylesheet" />
    <link href="~/css/themes/layout/brand/dark.css" rel="stylesheet" />  <!-- Login'de dark -->
    <link href="~/css/themes/layout/aside/dark.css" rel="stylesheet" />  <!-- Login'de dark -->
    <link rel="shortcut icon" href="{{Icon Path}}" />
</head>
<body id="kt_body" class="header-fixed header-mobile-fixed subheader-enabled subheader-fixed
  aside-enabled aside-fixed aside-minimize-hoverable page-loading">

<div class="d-flex flex-column flex-root">
    <!-- login-signin-on: başlangıçta login formu görünür -->
    <div class="login login-1 login-signin-on d-flex flex-column flex-lg-row flex-column-fluid bg-white"
         id="kt_login">

        <!-- SOL: Aside (renkli panel) -->
        <div class="login-aside d-flex flex-column flex-row-auto" style="background-color: #F2C98A;">
            <div class="d-flex flex-column-auto flex-column pt-lg-40 pt-15">
                <a href="#" class="text-center mb-10">
                    <img src="~/media/logos/LogoLogin.png" class="max-h-150px" alt="" />
                </a>
                <h3 class="font-weight-bolder text-center font-size-h4 font-size-h1-lg"
                    style="color: #986923;">
                    {{Uygulama Adı}}<br />{{Uygulama Açıklaması}}
                </h3>
            </div>
            <div class="aside-img d-flex flex-row-fluid bgi-no-repeat bgi-position-y-bottom bgi-position-x-center"
                 style="background-image: url(/media/svg/illustrations/login-visual-1.svg);"></div>
        </div>

        <!-- SAĞ: İçerik (formlar) -->
        <div class="login-content flex-row-fluid d-flex flex-column justify-content-center
                    position-relative overflow-hidden p-7 mx-auto">
            <div class="d-flex flex-column-fluid flex-center">

                <!-- LOGIN FORMU -->
                <div class="login-form login-signin">
                    <form id="kt_login_signin_form"
                          asp-action="Login" asp-controller="User"
                          asp-route-returnUrl="@Context.Request.Query["returnUrl"]"
                          method="post" novalidate="novalidate">
                        <div class="alert alert-danger" role="alert" asp-validation-summary="ModelOnly"></div>
                        <div class="pb-13 pt-lg-0 pt-5">
                            <h3 class="font-weight-bolder text-dark font-size-h4 font-size-h1-lg">
                                {{Uygulama Adı}}
                            </h3>
                            <p class="text-muted font-weight-bold font-size-h4">
                                Giriş yapmak için kullanıcı adı ve şifrenizi giriniz
                            </p>
                        </div>
                        <div class="form-group">
                            <label class="font-size-h6 font-weight-bolder text-dark">Kullanıcı Adı</label>
                            <input class="form-control form-control-solid h-auto py-6 px-6 rounded-lg"
                                   type="text" asp-for="LoginModel.Email" autocomplete="off" />
                        </div>
                        <div class="form-group">
                            <div class="d-flex justify-content-between mt-n5">
                                <label class="font-size-h6 font-weight-bolder text-dark pt-5">Şifre</label>
                            </div>
                            <input class="form-control form-control-solid h-auto py-6 px-6 rounded-lg"
                                   asp-for="LoginModel.Password" autocomplete="off" />
                        </div>
                        <div class="form-group d-flex flex-wrap justify-content-between align-items-center mt-3">
                            <label class="checkbox checkbox-outline m-0 text-muted">
                                <input type="checkbox" asp-for="LoginModel.RememberMe" />
                                <span></span>Beni Hatırla
                            </label>
                            <a href="javascript:;" id="kt_login_forgot" class="text-muted text-hover-primary">
                                Şifrenizi mi Unuttunuz ?
                            </a>
                        </div>
                        <div class="pb-lg-0 pb-5">
                            <button type="submit"
                                    class="btn btn-block btn-primary font-weight-bolder font-size-h6 px-8 py-4 my-3 mr-3">
                                Kullanıcı Girişi
                            </button>
                        </div>
                    </form>
                </div>

                <!-- FORGOT PASSWORD FORMU -->
                <div class="login-form login-forgot">
                    <form id="kt_login_forgot_form"
                          asp-controller="User" asp-action="ForgetPassword">
                        <div class="pb-13 pt-lg-0 pt-5">
                            <h3 class="font-weight-bolder text-dark font-size-h4 font-size-h1-lg">
                                Şifrenizi mi unuttunuz ?
                            </h3>
                            <p class="text-muted font-weight-bold font-size-h4">
                                Şifrenizi Sıfırlamak İçin E-Posta Adresinizi Giriniz.
                            </p>
                        </div>
                        <div class="form-group">
                            <input class="form-control form-control-solid h-auto py-6 px-6 rounded-lg font-size-h6"
                                   type="email" placeholder="E-Posta"
                                   asp-for="ForgetModel.Email" autocomplete="off" />
                        </div>
                        <div class="form-group d-flex flex-wrap pb-lg-0">
                            <button type="submit" id="kt_login_forgot_submit"
                                    class="btn btn-primary font-weight-bolder font-size-h6 px-8 py-4 my-3 mr-4">
                                Sıfırlama E-Postası Gönder
                            </button>
                            <button type="button" id="kt_login_forgot_cancel"
                                    class="btn btn-light-primary font-weight-bolder font-size-h6 px-8 py-4 my-3">
                                Vazgeç
                            </button>
                        </div>
                    </form>
                </div>

            </div>
            <!-- Footer -->
            <div class="d-flex justify-content-lg-start justify-content-center align-items-end py-7 py-lg-0">
                <div class="text-dark-50 font-size-lg font-weight-bolder mr-10">
                    <span class="mr-1">2025©</span>
                    <a href="{{URL}}" target="_blank" class="text-dark-75 text-hover-primary">{{Geliştirici}}</a>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Scripts (layout dışı, standalone) -->
<script>var KTAppSettings = { ... };</script>  <!-- _Layout'taki ile aynı config -->
<script src="~/plugins/global/plugins.bundle.js"></script>
<script src="~/plugins/custom/prismjs/prismjs.bundle.js"></script>
<script src="~/js/scripts.bundle.js"></script>
<script src="~/js/pages/custom/login/login-general.js"></script>
</body>
</html>
```

### Login/Forgot Toggle JS (`login-general.js`)
```javascript
'use strict';
var KTLogin = function () {
    var _login;

    var _showForm = function (form) {
        var cls = 'login-' + form + '-on';
        var formId = 'kt_login_' + form + '_form';
        _login.removeClass('login-forgot-on');
        _login.removeClass('login-signin-on');
        _login.addClass(cls);
        KTUtil.animateClass(KTUtil.getById(formId), 'animate__animated animate__backInUp');
    };

    var _handleSignInForm = function () {
        $('#kt_login_forgot').on('click', function (e) {
            e.preventDefault();
            _showForm('forgot');
        });
    };

    var _handleForgotForm = function () {
        $('#kt_login_forgot_cancel').on('click', function (e) {
            e.preventDefault();
            _showForm('signin');
        });
    };

    return {
        init: function () {
            _login = $('#kt_login');
            _handleSignInForm();
            _handleForgotForm();
        }
    };
}();

jQuery(document).ready(function () {
    KTLogin.init();
});
```

---

## UserController — Auth Action'ları

### Model Binding — Tuple ile

Login sayfasının model'i tuple olduğu için `[Bind(Prefix = "Item1")]` ve `[Bind(Prefix = "Item2")]` kullanılır:

```csharp
[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Login(
    [Bind(Prefix = "Item1")] LoginViewModel loginModel,
    [Bind(Prefix = "Item2")] ForgetPasswordViewModel forgetModel,
    string returnUrl = "/")
{
    var model = (loginModel, forgetModel);
    // ...
}
```

### Login GET
```csharp
[AllowAnonymous]
public IActionResult Login(string returnUrl = "/")
{
    var model = (new LoginViewModel(), new ForgetPasswordViewModel());
    return View(model);
}
```

### Login POST — Hata Senaryoları
```csharp
// Kullanıcı bulunamadı
ModelState.AddModelError(string.Empty, "Kullanıcı Bilgilerinizi kontrol ederek tekrar deneyiniz");

// Hesap kilitli (5 başarısız deneme sonrası 15dk)
ModelState.AddModelErrorList(new List<string> {
    "Hesabınız Kilitli, 15 dakika boyunca giriş yapamazsınız."
});

// Erişim yok
ModelState.AddModelErrorList(new List<string> { "Erişim Yetkiniz bulunmamaktadır." });

// 2FA gerekiyor (TODO alanı — her projede implement edilmeyebilir)
if (res.RequiresTwoFactor) { /* implement edilecek */ }
```

### Redirect Sonrası
```csharp
returnUrl = returnUrl ?? Url.Action("Index", "Dashboard");
return Redirect(returnUrl);
```

### ForgetPassword POST
```csharp
[AllowAnonymous]
[HttpPost]
public async Task<IActionResult> ForgetPassword(
    [Bind(Prefix = "Item1")] LoginViewModel loginModel,
    [Bind(Prefix = "Item2")] ForgetPasswordViewModel forgetModel)
{
    // Token üret, mail gönder, ForgetConfirm'e yönlendir
    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var resetUrl = "https://" + Request.Host.Value +
        Url.Action("ResetPassword", "User", new { userId = user.Id, token });
    await _emailService.SendResetPasswordEmailAsync(...);
    TempData["Eposta"] = forgetModel.Email;
    return RedirectToAction("ForgetConfirm", "User");
}
```

### ResetPassword
```csharp
// GET: TempData'dan userId ve token alır, View döner
[AllowAnonymous]
public IActionResult ResetPassword(string userId, string token)
{
    TempData["UserId"] = userId;
    TempData["Token"] = token;
    return View();
}

// POST: Şifre sıfırlama işlemi
[HttpPost, AllowAnonymous]
public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
{
    var userId = TempData["UserId"];
    var token = TempData["Token"];
    var res = await _userManager.ResetPasswordAsync(user, token.ToString(), model.Password);
    if (res.Succeeded) return RedirectToAction("Index", "Dashboard");
    // hata durumunda ModelState doldur
}
```

---

## StatusEnum Kontrolü (Login POST'ta)
```csharp
if (user.Status != StatusEnum.Active)
{
    switch (user.Status)
    {
        case StatusEnum.Passive:
        case StatusEnum.Pending:
            ModelState.AddModelError("", "Lütfen aktif bir kullanıcı ile giriş yapmayı deneyin.");
            return View(model);
        case StatusEnum.Deleted:
            ModelState.AddModelError("", "Kullanıcı bilgilerine ulaşılamadı.");
            return View(model);
    }
}
```
