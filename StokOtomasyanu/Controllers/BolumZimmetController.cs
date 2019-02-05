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
using StokOtomasyanu;

namespace StokOtomasyanu.Controllers
{
    public class BolumZimmetController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

       
        public ActionResult Index()
        {

            var bolumZimmets = db.BolumZimmets.Include(b => b.Bolum).Include(b => b.Urunler);
           
            return View(bolumZimmets.ToList());
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Yeni()
        {
            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi");
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi");
            return View();
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni([Bind(Include = "BolumZimmetId,BolumId,UrunId,Durum")] BolumZimmet bolumZimmet)
        {
            if (ModelState.IsValid)
            {
                var bolumzimmet1 = db.BolumZimmets.FirstOrDefault(x =>
                    x.BolumId == bolumZimmet.BolumId && x.UrunId == bolumZimmet.UrunId);
                if (bolumzimmet1==null)
                {
                    db.BolumZimmets.Add(bolumZimmet);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Aynı ürün Bir daha zimmetlenemez.";

                }
            }

            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi", bolumZimmet.BolumId);
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", bolumZimmet.UrunId);
            return View(bolumZimmet);
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Duzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BolumZimmet bolumZimmet = db.BolumZimmets.Find(id);
            if (bolumZimmet == null)
            {
                return HttpNotFound();
            }
            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi", bolumZimmet.BolumId);
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", bolumZimmet.UrunId);
            return View(bolumZimmet);
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle([Bind(Include = "BolumZimmetId,BolumId,UrunId,Durum")] BolumZimmet bolumZimmet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bolumZimmet).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BolumId = new SelectList(db.Bolums, "BolumId", "BolumAdi", bolumZimmet.BolumId);
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", bolumZimmet.UrunId);
            return View(bolumZimmet);
        }

        public string Sil(int id)
        {
            BolumZimmet bolumZimmet = db.BolumZimmets.FirstOrDefault(x => x.BolumZimmetId == id);
            db.BolumZimmets.Remove(bolumZimmet);
            try
            {
                db.SaveChanges();
                return "Silindi";
            }
            catch (Exception e)
            {
                return "Hata";
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
                return File(stream.ToArray(), "application/pdf", "BolumZimmetRapor.pdf");
            }
        }

       
    }
}
