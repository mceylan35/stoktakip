using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StokOtomasyanu;

namespace StokOtomasyanu.Controllers
{
    
    public class KullaniciController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Index()
        {
            var kullanicilars = db.Kullanicilars.Include(k => k.Bolum).Include(k => k.Roller);
            return View(kullanicilars.ToList());
        }

        public ActionResult Ara(string q)
        {
            var kullanicilars = db.Kullanicilars.Include(k => k.Bolum).Include(k => k.Roller);
            if (string.IsNullOrEmpty(q)==false)
            {
                kullanicilars = db.Kullanicilars.Where(i => i.KullaniciAdi.Contains(q));
            }

            return View(kullanicilars.ToList());

        }



        [Authorize(Roles = "Admin")]
        public ActionResult Yeni()
        {
            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi");
            ViewBag.RoleId = new SelectList(db.Rollers, "RoleId", "RolAdi");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni([Bind(Include = "KullaniciId,KullaniciAdi,Sifre,RoleId,BolumId,Ad,Soyad")] Kullanicilar kullanicilar)
        {

            if (ModelState.IsValid)
            {
                var kullanici = db.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullanicilar.KullaniciAdi);
                if (kullanici == null)
                {
                    db.Kullanicilars.Add(kullanicilar);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Kullanıcı Adı bulunmaktadır.";
                }

            }

            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi", kullanicilar.BolumId);
            ViewBag.RoleId = new SelectList(db.Rollers, "RoleId", "RolAdi", kullanicilar.RoleId);
            return View(kullanicilar);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Duzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kullanicilar kullanicilar = db.Kullanicilars.Find(id);
            if (kullanicilar == null)
            {
                return HttpNotFound();
            }
            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi", kullanicilar.BolumId);
            ViewBag.RoleId = new SelectList(db.Rollers, "RoleId", "RolAdi", kullanicilar.RoleId);

            var bk = db.Kullanicilars.FirstOrDefault(i => i.KullaniciAdi == "deneme");

            ViewBag.Konumx = bk.Ad;
            ViewBag.Konumy = bk.Soyad;

            return View(kullanicilar);
        }

        public ActionResult AlinanUrunler()
        {
            var alinanurunler = db.TblUrunAl.
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Duzenle([Bind(Include = "KullaniciId,KullaniciAdi,Sifre,RoleId,BolumId,Ad,Soyad")] Kullanicilar kullanicilar)
        {
            if (ModelState.IsValid)
            {
                var kullanici = db.Kullanicilars.FirstOrDefault(x => x.KullaniciAdi == kullanicilar.KullaniciAdi);
                if (kullanici == null)
                {
                    db.Entry(kullanicilar).State = EntityState.Modified;
                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Kullanıcı Adı bulunmaktadır.";
                }

                
            }
            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi", kullanicilar.BolumId);
            ViewBag.RoleId = new SelectList(db.Rollers, "RoleId", "RolAdi", kullanicilar.RoleId);
            return View(kullanicilar);
        }





        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string Sil(int id)
        {
           

            Kullanicilar kullanicilar = db.Kullanicilars.FirstOrDefault(x => x.KullaniciId == id);
            db.Kullanicilars.Remove(kullanicilar);

            db.SaveChanges();
            return "başarılı";




        }
    }
}
