using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using MvcKutuphane.Models.Entity;

namespace MvcKutuphane.Controllers
{
    public class IstatistikController : Controller
    {
        DbKutuphaneEntities db = new DbKutuphaneEntities();
        // GET: Istatistik
        public ActionResult Index()
        {
            var deger1 = db.TBLUYELER.Count();
            var deger2 = db.TBLKITAP.Count();
            var deger3 = db.TBLKITAP.Where(x =>x.DURUM == false).Count();
            var deger4 = db.TBLCEZALAR.Sum(x => x.PARA);
            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            ViewBag.dgr4 = deger4;
            return View();
        }
        public ActionResult Hava()
        {
            return View();
        }
        public ActionResult HavaKart()
        {
            return View();
        }
        public ActionResult Galeri()
        {
            return View();
        }
        
        public ActionResult resimyukle(HttpPostedFileBase dosya)
        {
            if(dosya.ContentLength > 0)
            {
                string dosyayolu = Path.Combine(Server.MapPath("~/web2/resimler/"), Path.GetFileName(dosya.FileName));
                dosya.SaveAs(dosyayolu);
            }
            return RedirectToAction("Galeri");
        }
        public ActionResult LinqKart()
        {
            var deger1 = db.TBLKITAP.Count();
            var deger2 = db.TBLUYELER.Count();
            var deger3 = db.TBLCEZALAR.Sum(x=>x.PARA);
            var deger4 = db.TBLKITAP.Where(x=>x.DURUM == false).Count();
            var deger5 = db.TBLKATEGORI.Count();
            var deger6 = db.TBLHAREKET.GroupBy(x => x.TBLUYELER.AD + x.TBLUYELER.SOYAD).OrderByDescending(z => z.Count()).Select(y => new { y.Key }).FirstOrDefault();
            var deger7 = db.TBLHAREKET.GroupBy(x => x.TBLKITAP.AD).OrderByDescending(z => z.Count()).Select(y => new { y.Key }).FirstOrDefault();
            var deger8 = db.EnFazlaKitapYazar().FirstOrDefault();
            var deger9 = db.TBLKITAP.GroupBy(x => x.YAYINEVI).OrderByDescending(z => z.Count()).Select(y => new { y.Key }).FirstOrDefault();
            var deger10 = db.TBLHAREKET.GroupBy(x => x.TBLPERSONEL.PERSONEL).OrderByDescending(z => z.Count()).Select(y => new { y.Key }).FirstOrDefault();
            var deger11 = db.TBLILETISIM.Count();
           


            ViewBag.dgr1 = deger1;
            ViewBag.dgr2 = deger2;
            ViewBag.dgr3 = deger3;
            ViewBag.dgr4 = deger4;
            ViewBag.dgr5 = deger5;
            ViewBag.dgr6 = deger6;
            ViewBag.dgr7 = deger7;


            ViewBag.dgr8 = deger8;
            ViewBag.dgr9 = deger9;
            ViewBag.dgr10 = deger10;
            ViewBag.dgr11 = deger11;



            return View();
        }

    }
}