using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APP.UI.WEB.BaseClasses;
using APP.UI.WEB.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ORM.Business;
using ORM.Models.Models;
using ORM.Shared;

namespace APP.UI.WEB.Controllers
{
    public class LoginController : Controller
    {
        public bool _LDAP_ACTIVE = false;
        CurrentSession CurrentSession;
        private readonly ISession _session;
        private readonly IStringLocalizer<SharedResources> _localizer;
        public LoginController(IHttpContextAccessor session, IStringLocalizer<SharedResources> localizer)
        {
            _session = session.HttpContext.Session;
            CurrentSession = new CurrentSession(_session);
            _localizer = localizer;
        }
        public class KullaniciLoginModel
        {
            public string kullanici_ad { get; set; }
            public string sifre { get; set; }
            public bool beni_hatirla { get; set; } = false;
        }

        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            CultureInfo cultureInfo = new CultureInfo(culture);
            cultureInfo.NumberFormat.NumberDecimalSeparator = ",";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(cultureInfo)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }
        public IActionResult Index()
        {
            ViewBag._LDAP_ACTIVE = this._LDAP_ACTIVE;
            if (Request.Cookies["RememberMe_BaseProject"] != null)
            {
                // Çerez varsa, kullanıcı kimliği alınır
                var userId = Request.Cookies["RememberMe_BaseProject"];
                users user = new GenericBusiness<users>().GetById(Convert.ToInt32(userId));

                HttpContext.Session.SetString("AKTIF_KULLANICI_ID", user.id.ToString());
                HttpContext.Session.SetString("AKTIF_KULLANICI_AD", user.user_name);
                HttpContext.Session.SetInt32("AKTIF_KULLANICI_TUR", (Convert.ToInt32(user.user_type)));

                CreateOrUpdateRememberMeCookie(HttpContext.Session.GetString("AKTIF_KULLANICI_ID"), "BaseProject");

                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public IActionResult Logout()
        {
            new BaseClasses.OturumKontrol(this.HttpContext.Session).KullaniciCikis();
            if (Request.Cookies["RememberMe_BaseProject"] != null)
            {
                Response.Cookies.Delete("RememberMe_BaseProject");
            }

            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public async Task<ActionResult> IndexAsync(KullaniciLoginModel kullanici)
        {
            string cookieName = "appormcore0001_lc";
            ViewBag._LDAP_ACTIVE = this._LDAP_ACTIVE;

            if (String.IsNullOrEmpty(kullanici.kullanici_ad) || String.IsNullOrEmpty(kullanici.sifre) || kullanici.sifre == "ldap")
            {
                ViewBag.Mesaj = "Kullanıcı Adı veya Şifre Boş Bırakılamaz";
                return View();
            }

            else
            {
                string gelen = new BaseClasses.OturumKontrol(this.HttpContext.Session).KullaniciGiris(kullanici.kullanici_ad, kullanici.sifre);
                users _user = new users();
                //List<ORM.Models.tbl_kullanici> tbl_Kullanicis = new ORM.Business.GenericBusiness<ORM.Models.tbl_kullanici>().GetAllByCustomQuery("").ToList();


                if (gelen == "1") // Kullanıcı uygun, onaylanmış
                {
                    DeleteCookie(cookieName);
                    if (kullanici.beni_hatirla)
                    {
                        CreateOrUpdateRememberMeCookie(HttpContext.Session.GetString("AKTIF_KULLANICI_ID"), "BaseProject");
                        CreateCookie(cookieName, kullanici.kullanici_ad);
                    }


                    int gelenKullaniciId = 0; // Kullanıcının ID'Si alınmalı

                    //MyClassesX.DbOperations.LogEkle(MyClasses.Tur.IslemTurler.OturumAcildi, "", Kutuphane.NullIseBosVer(gelenKullaniciId), kullanici);
                    kullanici.sifre = Base.GenerateDoubleMD5Password(kullanici.sifre);
                    string w = $" AND {nameof(users.user_name)}='{kullanici.kullanici_ad}' and {nameof(users.password)}='{kullanici.sifre}'";
                    _user = new GenericBusiness<users>().GetAllByCustomQuery(w).FirstOrDefault();
                    if (_user != null)
                    {
                        CurrentSession.userAuthInfo = _user;
                        string isAuth = await Operations.Authenticate(_user.user_name,_user.password);
                        HttpContext.Session.SetString("token", isAuth);
                    }
                    string p = ORM.Shared.Lib.isString(HttpContext.Request.Query["p"]);
                    if (p != "")
                    {
                        Response.Redirect(p);
                        //return 1;
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                        //return 1;
                    }
                }
                else if (gelen == "-1") // faydam kullanıcısı yok
                {
                    ViewBag.Mesaj = "MAKBUL - Geçersiz Kullanıcı Adı veya Şifre";

                    //MyClasses.DbOperations.LogEkle(MyClasses.Tur.IslemTurler.OturumGecersizKullaniciSifre, "", "", kullanici);
                }
                else
                {
                    // Exception oluşmuştur
                    ViewBag.Mesaj = "Bilinmeyen Durum: " + gelen;
                }
            }


            return View();
        }

        private void CreateOrUpdateRememberMeCookie(string userId, string applicationName)
        {
            var cookieName = "RememberMe_" + applicationName; // Uygulama adı ile çerez adı oluşturulur

            if (Request.Cookies[cookieName] != null)
            {
                // Çerez varsa, çerezin süresi güncellenir
                Response.Cookies.Delete(cookieName);
            }

            // Yeni çerez oluşturulur ve süresi güncellenir
            Response.Cookies.Append(cookieName, userId, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(2), // Çerez 30 gün boyunca saklanır
                HttpOnly = true // Tarafınızdan erişilemez
            });
        }


        private void CreateCookie(string name, string value)
        {
            //HttpCookie cookieVisitor = new HttpCookie(name, value);
            //// cookieVisitor.Expires = DateTime.Now.AddDays(2);
            //Response.Cookies.Add(cookieVisitor);

            CookieOptions option = new CookieOptions();

            //if (expireTime.HasValue)
            //    option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            //else
            //    option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(name, value, option);
        }
        private string GetCookie(string name)
        {
            //Böyle bir cookie mevcut mu kontrol ediyoruz
            //if (Request.Cookies.AllKeys.Contains(name))
            //{
            //    //böyle bir cookie varsa bize geri değeri döndürsün
            //    return Request.Cookies[name].Value;
            //}

            //string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies[name];

            //read cookie from Request object  
            string cookieValueFromReq = Request.Cookies[name];

            return cookieValueFromReq;
        }
        private void DeleteCookie(string name)
        {
            //Böyle bir cookie var mı kontrol ediyoruz
            //if (GetCookie(name) != null)
            //{
            //    //Varsa cookiemizi temizliyoruz
            //    Response.Cookies.Remove(name);
            //    //ya da 
            //    Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            //}

            Response.Cookies.Delete(name);
        }
    }
}