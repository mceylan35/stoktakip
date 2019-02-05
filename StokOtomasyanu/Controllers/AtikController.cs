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

    public class AtikController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

        
        public ActionResult Index()
        {
            var atiklars = db.Atiklars.Include(a => a.Urunler);
            return View(atiklars.ToList());
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public ActionResult Yeni()
        {
            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi");
            return View();
        }

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni([Bind(Include = "AtikId,UrunId")] Atiklar atiklar)
        {
            if (ModelState.IsValid)
            {
                var atik1 = db.Atiklars.FirstOrDefault(x => x.UrunId == atiklar.UrunId);
                if (atik1==null)
                {
                    db.Atiklars.Add(atiklar);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Aynı atıklara atılamaz. .";
                }
              
            }

            ViewBag.UrunId = new SelectList(db.Urunlers, "UrunId", "UrunaAdi", atiklar.UrunId);
            return View(atiklar);
        }
       
       

      

        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        [HttpPost]
        public string Sil(int id)
        {
            Atiklar atiklar = db.Atiklars.FirstOrDefault(x => x.AtikId == id);
            db.Atiklars.Remove(atiklar);
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
                Document pdfDoc = new Document(PageSize.A4, 50f, 10f, 50f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);
                pdfDoc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                pdfDoc.Close();
                return File(stream.ToArray(), "application/pdf", "AtikRapor.pdf");
            }
        }

    }
}
