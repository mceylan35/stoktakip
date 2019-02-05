using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StokOtomasyanu;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.text.html.simpleparser;

namespace StokOtomasyanu.Controllers
{
    public class KisiZimmetController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

       
        public ActionResult Index()
        {
            
            var kisiZimmets = db.KisiZimmets.Include(k => k.Kullanicilar).Include(k => k.Urunler);
            
            return View(kisiZimmets.ToList());
        }



        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Yeni()
        {
            ViewBag.KisiId = new SelectList(db.Kullanicilars, "KullaniciId", "KullaniciAdi");
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi");
            return View();
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni([Bind(Include = "KisiZimmetId,KisiId,UrunId,Durum")] KisiZimmet kisiZimmet)
        {
            if (ModelState.IsValid)
            {
               
                var kisizimmet1=db.KisiZimmets.FirstOrDefault(x=>x.UrunId==kisiZimmet.UrunId && x.UrunId==kisiZimmet.UrunId);
                if (kisizimmet1==null)
                {
                    db.KisiZimmets.Add(kisiZimmet);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Bu ürün daha önce zimmetlendi.";
                }

            }

            ViewBag.KisiId = new SelectList(db.Kullanicilars, "KullaniciId", "KullaniciAdi", kisiZimmet.KisiId);
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", kisiZimmet.UrunId);
            return View(kisiZimmet);
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Duzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KisiZimmet kisiZimmet = db.KisiZimmets.Find(id);
            if (kisiZimmet == null)
            {
                return HttpNotFound();
            }
            ViewBag.KisiId = new SelectList(db.Kullanicilars, "KullaniciId", "KullaniciAdi", kisiZimmet.KisiId);
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", kisiZimmet.UrunId);
            return View(kisiZimmet);
        }
        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle([Bind(Include = "KisiZimmetId,KisiId,UrunId,Durum")] KisiZimmet kisiZimmet)
        {
            if (ModelState.IsValid)
            {
             


                db.Entry(kisiZimmet).State = EntityState.Modified;
                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
             

                
                
            }
            ViewBag.KisiId = new SelectList(db.Kullanicilars, "KullaniciId", "KullaniciAdi", kisiZimmet.KisiId);
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", kisiZimmet.UrunId);
            return View(kisiZimmet);
        }
        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        public string Sil(int id)
        {
            KisiZimmet kisi = db.KisiZimmets.FirstOrDefault(x => x.KisiZimmetId == id);
            db.KisiZimmets.Remove(kisi);
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


        [HttpPost]
        [ValidateInput(false)]
        public FileResult Pdf(string GridHtml)
        {
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                StringReader sr = new StringReader(GridHtml);
                Document pdfDoc = new Document(PageSize.A4, 50f, 10f, 50f, 20f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "KisiZimmetRapor.pdf");
            }
        }


       
    }
}
