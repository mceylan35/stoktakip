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
   
    public class BolumController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Index()
        {
            return View(db.Bolums.ToList());
        }



        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Yeni()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni([Bind(Include = "BolumId,BolumAdi")] Bolum bolum)
        {


            if (ModelState.IsValid)
            {
                var bolum1 = db.Bolums.FirstOrDefault(x => x.BolumAdi == bolum.BolumAdi);
                if (bolum1==null)
                {
                    db.Bolums.Add(bolum);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Bu Bölüm mevcut";
                }
                
            }

            return View(bolum);
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Duzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Bolum bolum = db.Bolums.Find(id);
            if (bolum == null)
            {
                return HttpNotFound();
            }
            return View(bolum);
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle([Bind(Include = "BolumId,BolumAdi")] Bolum bolum)
        {
            if (ModelState.IsValid)
            {
                var bolum1 = db.Bolums.FirstOrDefault(x => x.BolumAdi == bolum.BolumAdi);
                if (bolum1 == null)
                {
                    db.Entry(bolum).State = EntityState.Modified;
                   
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Bu Bölüm mevcut";
                }
                
              
            }
            return View(bolum);
        }

        public string Sil(int id)
        {
            var bolum = db.Bolums.FirstOrDefault(x => x.BolumId == id);
            db.Bolums.Remove(bolum);
            try
            {
                db.SaveChanges();
                return "başarılı";
            }
            catch (Exception e)
            {
                return "hata oluştu";
            }
           


        }
    }
}
