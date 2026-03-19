/*
 * using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ORM.Business;
using ORM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
*/
using Microsoft.AspNetCore.Http;
using ORM.Business;
using ORM.Models.Models;
using ORM.Shared;
using System;
using System.Text;
using System.Text.Json;

namespace APP.UI.WEB.BaseClasses
{
    public class OturumKontrol
    {
        private readonly IHttpContextAccessor _httpContextAccessor; // OZ
        private readonly ISession _session; // OZ

        public OturumKontrol(ISession _session)
        {
            this._session = _session;
        }

        public OturumKontrol(IHttpContextAccessor httpContextAccessor) // OZ
        {
            _httpContextAccessor = httpContextAccessor;
            _session = _httpContextAccessor.HttpContext.Session;
        }

        /// <summary>
        /// Şifre bu fonksiyona zaten MD5 olarak geliyor ise sifreMD5li = true olmalı
        /// </summary>
        /// <param name="kullaniciAd"></param>
        /// <param name="sifre"></param>
        /// <param name="sifreMD5li"></param>
        /// <param name="paramatre">Örneğin parametre="ldap" ise, şifre kontrolü yapılmaz</param>
        /// <returns></returns>
        public string KullaniciGiris(string kullaniciAd, string sifre)
        {
            try
            {
                users user = new UsersBusiness().UserLogin(kullaniciAd, sifre);
                if (user == null)
                {
                    return "-1";
                }
                else
                {
                    _session.SetString("AKTIF_KULLANICI", JsonSerializer.Serialize(user));
                    _session.SetString("AKTIF_KULLANICI_ID", user.id.ToString());
                    _session.SetString("AKTIF_KULLANICI_AD", user.user_name);
                    _session.SetInt32("AKTIF_KULLANICI_TUR", (Convert.ToInt32(user.user_type)));

                    return "1";
                }


            }
            catch (Exception ex)
            {
                return "Hata Oluştu: " + ex.Message;
            }
        }

        public bool KullaniciKontrol()
        {
            /// AKTIF_KULLANICI_ID session'ı açık mı?
            ///     - HAYIR: oturum açılmaz
            ///     - EVET : vw_kullanici tablosundan kullanıcı_id ve kullanici_ad a göre sorgula kayıt var mı?
            ///         - HAYIR: oturum açılmaz
            ///         - EVET : session'da ki zaman ile veritabanındaki zamanı karşılaştır, fark 10dk'dan az mı?
            ///             - HAYIR: oturum açılmaz
            ///             - EVET : oturum zamanını güncelle, oturumu aç 
            ///         
            Guid yonetici_id = Guid.Empty;
            string yonetici_kullanici_ad = "";

            try
            {
                try
                {
                    if (_session == null || _session.GetString("AKTIF_KULLANICI_ID") == null || _session.GetString("AKTIF_KULLANICI_ID") == Guid.Empty.ToString())
                    {
                        return false;
                    }
                    else
                    {
                        yonetici_id = Lib.isGuid(_session.GetString("AKTIF_KULLANICI_ID"));
                    }
                }
                catch
                {
                    return false;
                }

                if (yonetici_id == null || yonetici_id == Guid.Empty)
                {
                    return false;
                }

                yonetici_kullanici_ad = Lib.isString(_session.GetString("AKTIF_KULLANICI_AD"));

                users user = new UsersBusiness().GetUserById(yonetici_id, true);
                if (user == null)
                {
                    return false;
                }
                else
                {
                    if (DateTime.Compare(DateTime.Now, user.last_login_time) < 10)
                    {
                        user.last_login_time = DateTime.Now;
                        //Sabitler.AKTIF_KULLANICI = user;

                        new UsersBusiness().UpdateLastLoginTime(yonetici_id);

                        return true;
                    }
                    else
                        return false;
                }
                /*
                using (var db = new DataloggerEntities())
                {
                    clsKullanici kullanici = db.clsKullanici.FirstOrDefault(a => a.kullanici_id == yonetici_id);
                    if (kullanici == null)
                        return false;
                    else
                    {
                        if (DateTime.Compare(DateTime.Now, kullanici.oturum_acma_zaman) < 10)
                        {
                            kullanici.oturum_acma_zaman = DateTime.Now;
                            Sabitler.AKTIF_KULLANICI = kullanici;
                            db.SaveChanges();
                            return true;
                        }
                        else
                            return false;
                    }

                }
                */
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public void KullaniciCikis()
        {
            _session.Clear();
        }
    }
}
