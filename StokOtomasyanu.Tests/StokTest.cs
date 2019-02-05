using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;
using System.Web.Mvc;
using System.Web.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StokOtomasyanu.Controllers;


namespace StokOtomasyanu.Tests
{
    [TestClass]
    public class StokTest
    {
        
        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public void KullaniciKayitVar()
        {
            KullaniciController controller=new KullaniciController();
            Kullanicilar k=new Kullanicilar();
            k.Ad = "Mehmet";
            k.Soyad = "Ceylan";
            k.Sifre = "123456";
            k.KullaniciAdi = "admin";


            var sonuc = controller.Yeni(k) as ViewResult;
            var urun = (Kullanicilar) sonuc.ViewData.Model;
            Assert.AreEqual("admin",k.KullaniciAdi);


        }
        [TestMethod]
      
        public void UrunDuzenle()
        {
            UrunController urun=new UrunController();

            var sonuc = urun.Duzenle(-1) as RedirectToRouteResult;
            Assert.AreEqual(sonuc.RouteValues["action"],"Index");




        }

        [TestMethod]
        public void AtikListe(){
            AtikController atik=new AtikController();
            ViewResult sonuc=atik.Index() as ViewResult;
            
            Assert.IsInstanceOfType(sonuc.Model,typeof(List<Atiklar>));
        }

        [TestMethod]
        public void UrunSil()
        {
            UrunController urun=new UrunController();
           
            var sonuc = urun.Sil(19);
            Assert.AreSame("başarılı", sonuc);
        }

        [TestMethod]
        public void AramaYap()
        {
            UrunController urun=new UrunController();
            ViewResult sonuc = urun.Ara("Kingston") as ViewResult;
            Assert.IsInstanceOfType(sonuc.Model,typeof(List<Urunler>));
        }
    }
}
