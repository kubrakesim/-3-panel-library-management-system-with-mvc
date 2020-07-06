using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;
using PagedList;
using PagedList.Mvc;

namespace MvcKutuphane.Controllers
{
    public class UyeController : Controller
    {

        // GET: Uye
        DbKutuphaneEntities db = new DbKutuphaneEntities();
        // sayfa=1 demek : sayfalamanın kaçıncı sayfadan başlayacağını söylüyor
        public ActionResult Index(int sayfa=1)
        {
            //var degerler = db.TBLUYELER.ToList();
            var degerler = db.TBLUYELER.ToList().ToPagedList(sayfa, 3);
            return View(degerler);
        }

        [HttpGet]
        public ActionResult UyeEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UyeEkle(TBLUYELER u)
        {
            if (!ModelState.IsValid)
            {
                return View("UyeEkle");
            }
            db.TBLUYELER.Add(u);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UyeSil(int id)
        {
            var uye = db.TBLUYELER.Find(id);
            db.TBLUYELER.Remove(uye);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public ActionResult UyeGetir(int id)
        {
            var uye = db.TBLUYELER.Find(id);
            return View("UyeGetir", uye);
        }
        public ActionResult UyeGuncelle(TBLUYELER u)
        {
            var uye = db.TBLUYELER.Find(u.ID);
            uye.AD = u.AD;
            uye.SOYAD = u.SOYAD;
            uye.MAIL = u.MAIL;
            uye.KULLANICIADI = u.KULLANICIADI;
            uye.SIFRE = u.SIFRE;
            uye.OKUL = u.OKUL;
            uye.TELEFON = u.TELEFON;
            uye.FOTOGRAF = u.FOTOGRAF;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult UyeKitapGecmis(int id)
        {
            var ktpgcms = db.TBLHAREKET.Where(x => x.UYE == id).ToList();
            var uyekit = db.TBLUYELER.Where(y => y.ID == id).Select(z => z.AD + " " + z.SOYAD).FirstOrDefault();
            ViewBag.u1 = uyekit;
            return View(ktpgcms);
        }
    }
}