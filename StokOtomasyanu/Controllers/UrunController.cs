using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;
using StokOtomasyanu;

namespace StokOtomasyanu.Controllers
{

    public  class UrunController : Controller
    {
        private StokTakipContext db = new StokTakipContext();



        public ActionResult Index()
        {
           
            var urunlers = db.Urunlers.Include(u => u.Bolum).Include(u => u.Kategoriler).Include(u => u.Tedarikci);
           
              
                return View(urunlers.ToList());
            
            
        }

        public ActionResult Ara(string q)
        {
            var urunlers = db.Urunlers.Include(u => u.Bolum).Include(u => u.Kategoriler).Include(u => u.Tedarikci);
          
                urunlers = urunlers.Where(x => x.UrunaAdi.Contains(q));
                
            
            return View(urunlers.ToList());
        }



        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Yeni()
        {
            ViewBag.BolumID = new SelectList(db.Bolums, "BolumId", "BolumAdi");
            ViewBag.KategoriId = new SelectList(db.Kategorilers, "KategoriId", "KategoriAdi");
            ViewBag.TedarikciId = new SelectList(db.Tedarikcis, "TedarikciId", "TedarikciAdi");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Yeni([Bind(Include = "UrunId,KategoriId,TedarikciId,UrunaAdi,UrunBirimFiyati,SatinAlinmaTarihi,Stok,StokDurum,BolumID")] Urunler urunler)
        {
            if (ModelState.IsValid)
            {
                var urun = db.Urunlers.FirstOrDefault(x => x.UrunaAdi == urunler.UrunaAdi);
                if (urun == null)
                {
                    db.Urunlers.Add(urunler);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Ürün Bulunmaktadır.";
                }

            }

            

            ViewBag.BolumID = new SelectList(db.Bolums, "BolumId", "BolumAdi", urunler.BolumID);
            ViewBag.KategoriId = new SelectList(db.Kategorilers, "KategoriId", "KategoriAdi", urunler.KategoriId);
            ViewBag.TedarikciId = new SelectList(db.Tedarikcis, "TedarikciId", "TedarikciAdi", urunler.TedarikciId);
            return View(urunler);
        }

        // GET: Urun/Edit/5
        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Duzenle(int? id)
        {
           
            Urunler urunler = db.Urunlers.Find(id);
            if (urunler.UrunId <= 0)
            {
                return RedirectToAction("Index");
            }


            if (urunler == null)
            {
                return HttpNotFound();
            }
            ViewBag.BolumID = new SelectList(db.Bolums, "BolumId", "BolumAdi", urunler.BolumID);
            ViewBag.KategoriId = new SelectList(db.Kategorilers, "KategoriId", "KategoriAdi", urunler.KategoriId);
            ViewBag.TedarikciId = new SelectList(db.Tedarikcis, "TedarikciId", "TedarikciAdi", urunler.TedarikciId);
            return View(urunler);

            

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Duzenle([Bind(Include = "UrunId,KategoriId,TedarikciId,UrunaAdi,UrunBirimFiyati,SatinAlinmaTarihi,Stok,StokDurum,BolumID")] Urunler urunler)
        {
          

            if (ModelState.IsValid)
            {

                var urun2 = db.Urunlers.FirstOrDefault(x => x.UrunaAdi == urunler.UrunaAdi);
            if (urun2 == null)
            {
                db.Entry(urunler).State = EntityState.Modified;
                db.SaveChanges();
                    return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Yanlis = "Ürün bulunmaktadır.";
            }



            
                
                
            }
            ViewBag.BolumID = new SelectList(db.Bolums, "BolumId", "BolumAdi", urunler.BolumID);
            ViewBag.KategoriId = new SelectList(db.Kategorilers, "KategoriId", "KategoriAdi", urunler.KategoriId);
            ViewBag.TedarikciId = new SelectList(db.Tedarikcis, "TedarikciId", "TedarikciAdi", urunler.TedarikciId);
            return View(urunler);
        }




        [Authorize(Roles = "Admin ,Satın Alma Şefi")]


        [HttpPost]
        public string Sil(int id)
        {
        StokTakipContext dba = new StokTakipContext();
        Urunler urunler = dba.Urunlers.FirstOrDefault(x => x.UrunId == id);
            dba.Urunlers.Remove(urunler);
            try
            {
                dba.SaveChanges();
                return "başarılı";
            }
            catch (Exception e)
            {
                return "hata";
            }


        }

       

    }
}
