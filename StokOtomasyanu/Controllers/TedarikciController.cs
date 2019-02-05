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

    public class TedarikciController : Controller
    {
        private StokTakipContext db = new StokTakipContext();

        // GET: Tedarikci
        public ActionResult Index()
        {
            return View(db.Tedarikcis.ToList());
        }

        [Authorize(Roles = "Admin ,Satın Alma Şefi")]
        public ActionResult Yeni()
        {
            return View();
        }

        [Authorize(Roles = "Admin ,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yeni([Bind(Include = "TedarikciId,TedarikciAdi")] Tedarikci tedarikci)
        {
            
            if (ModelState.IsValid)
            {
                var tedarik1 = db.Tedarikcis.FirstOrDefault(x => x.TedarikciAdi == tedarikci.TedarikciAdi);
                if (tedarik1== null)
                {
                    db.Tedarikcis.Add(tedarikci);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Tedarikci Bulunmaktadır.";
                }
                
            }

            return View(tedarikci);
        }

        [Authorize(Roles = "Admin ,Satın Alma Şefi")]
        public ActionResult Duzenle(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tedarikci tedarikci = db.Tedarikcis.Find(id);
            if (tedarikci == null)
            {
                return HttpNotFound();
            }
            return View(tedarikci);
        }

        [Authorize(Roles = "Admin ,Satın Alma Şefi")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle([Bind(Include = "TedarikciId,TedarikciAdi")] Tedarikci tedarikci)
        {
            if (ModelState.IsValid)
            {
                var tedarik2 = db.Tedarikcis.FirstOrDefault(x => x.TedarikciAdi == tedarikci.TedarikciAdi);
                if (tedarik2 == null)
                {
                    db.Entry(tedarikci).State = EntityState.Modified;
                   
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Yanlis = "Tedarikci Bulunmaktadır.";
                }

                
            }
            return View(tedarikci);
        }


        [Authorize(Roles = "Admin")]


        [HttpPost]
        [Authorize(Roles = "Admin,Satın Alma Şefi")]
        public string Sil(int id)
        {
            Tedarikci tedarikci = db.Tedarikcis.FirstOrDefault(x => x.TedarikciId == id);
            db.Tedarikcis.Remove(tedarikci);
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
                return File(stream.ToArray(), "application/pdf", "TedarikcilerRapor.pdf");
            }
        }

    }
}
