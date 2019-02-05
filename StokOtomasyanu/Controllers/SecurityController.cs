using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StokOtomasyanu.Controllers
{

    public class SecurityController : Controller
    {
        StokTakipContext db = new StokTakipContext();
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(Kullanicilar k)
        {
            var kullaniciInDb = db.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == k.KullaniciAdi && x.Sifre == k.Sifre);
            if (kullaniciInDb != null)
            {
                FormsAuthentication.SetAuthCookie(kullaniciInDb.KullaniciAdi, false);
                return RedirectToAction("Index", "Urun");
            }
            else
            {
                ViewBag.Mesaj = "Kullanıcı Adı veya Şifre yanlış";
                return View();
            }


        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

    }
}