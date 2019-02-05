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
    [Authorize(Roles = "Admin")]
    public class KategoriController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

       
      
        public ActionResult Index()
        {
            return View(db.Kategorilers.ToList());
        }



      
        [Authorize(Roles = "Admin")]
        public ActionResult Yeni()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Yeni([Bind(Include = "KategoriId,KategoriAdi")] Kategoriler kategoriler)
        {
            if (ModelState.IsValid)
            {
                var kategori1 = db.Kategorilers.FirstOrDefault(x => x.KategoriAdi == kategoriler.KategoriAdi);
                if (kategori1==null)
                {
                    db.Entry(kategoriler).State = EntityState.Modified;
                   
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Bu ürün daha önce zimmetlendi.";
                }
               
            }

            return View(kategoriler);
        }

        
        [Authorize(Roles = "Admin")]
        public ActionResult Duzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kategoriler kategoriler = db.Kategorilers.Find(id);
            if (kategoriler == null)
            {
                return HttpNotFound();
            }
            return View(kategoriler);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Duzenle([Bind(Include = "KategoriId,KategoriAdi")] Kategoriler kategoriler)
        {
            if (ModelState.IsValid)
            {
                var kategori1 = db.Kategorilers.FirstOrDefault(x => x.KategoriAdi == kategoriler.KategoriAdi);
                if (kategori1 == null)
                {
                    db.Entry(kategoriler).State = EntityState.Modified;
                    db.Kategorilers.Add(kategoriler);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Girdiğiniz kategori adı kayıtlı.";
                }
                
               
            }
            return View(kategoriler);
        }


        [Authorize(Roles = "Admin")]


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public string Sil(int id)
        {
            Kategoriler kategoriler = db.Kategorilers.FirstOrDefault(x => x.KategoriId == id);
            db.Kategorilers.Remove(kategoriler);
            try
            {
                db.SaveChanges();
                return "başarılı";
            }
            catch (Exception e)
            {
                return "hata";
            }


        }
    }
}
