
Entity Framework, Linq, SQL, Bootstrap, Css, Html5  ile Vitrin - Admin - Kullanıcı Paneli olan bir  .net projesidir.

Action link kullanımı : 
<td>@Html.ActionLink("Sil","YazarSil",new {id=y.ID },new {@class = "btn btn-danger", onclick = "return confirm('Silmek istediğinizden emin misiniz ?')"  })</td>

Return confirm: Bize bir alert sağlıyor, gerçekten silmek isteyip istemediğimizi oradan sordurabiliyoruz!

Yazar Ekleme : 
kaydı eklerken :   @Html.TextBoxFor(y => y.AD, new { @class = "form-control" })

Yazarı Controller da ekleme : 
 [HttpGet]
        public ActionResult YazarEkle()
        {
            return View();
        }
        [HttpPost]
        public ActionResult YazarEkle(TBLYAZAR p)
        {
            db.TBLYAZAR.Add(p);
            db.SaveChanges();
            return View();
        }

** Dropdown kullanarak yeni kitap ekle denildiğinde sayfaya kategorilerin getirilmesi : 

  @Html.DropDownListFor(k => k.TBLKATEGORI.ID, (List < SelectListItem >) ViewBag.dgr1, new { @class = "form-control" })
Controller tarafı : 
  public ActionResult KitapEkle()
        {
            List<SelectListItem> deger1 = (from i in db.TBLKATEGORI.ToList()
                                           select new SelectListItem
                                           {
                                               Text = i.AD,
                                               Value = i.ID.ToString()
                                           }).ToList();
            ViewBag.dgr1 = deger1; //viewbag ile taşıma işlemini yapıyoruz!
            return View();
        }


** Kaydı Silmek için :

   public ActionResult KitapSil(int id)
        {
            var kitap = db.TBLKITAP.Find(id);
            db.TBLKITAP.Remove(kitap);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



** Arama işlemi kodu :
 
 public ActionResult Index(string p)
        {
            var kitaplar = from k in db.TBLKITAP select k;
            if (!string.IsNullOrEmpty(p))
            {
                kitaplar = kitaplar.Where(m=>m.AD.Contains(p));
            }
           // var kitaplar = db.TBLKITAP.ToList();
            return View(kitaplar.ToList());
        }

** Index sayfası : 

@using (Html.BeginForm("Index", "Kitap", FormMethod.Get))
{
<div style="margin-bottom:15px; margin-top:15px;">
    <b>Aranacak kitap adını giriniz &nbsp;</b>@Html.TextBox("p")
    <input type="submit" value="Ara" />
</div>
}




** model1.tt nin içinde tblpersonel tablosunda data annotations kullanımı : 

  //data annotations kullanımı: personel adının boş geçilmemesi için

        [Required(ErrorMessage ="Personel Adı Boş Geçilemez")]

-> PersonelController a
  [HttpPost]
        public ActionResult PersonelEkle(TBLPERSONEL p)
        {
            if (!ModelState.IsValid)
            {
                return View("PersonelEkle");
            }
            db.TBLPERSONEL.Add(p);
            db.SaveChanges();
            return View();
        }

@Html.TextBoxFor(x => x.PERSONEL, new { @class = "form-control" })
        @Html.ValidationMessageFor(x => x.PERSONEL, "", new { @style = "color:orange" })

kodunu yazarak çalışır hale getirdik


! Üyeler kısmında sayfalama işlemiş için pagedlist kullandık

** Index tarafı : 

@using PagedList
@using PagedList.Mvc
@model PagedList.IPagedList<TBLUYELER>

** tüm kayıtları listelemek için yazmış olduğumuz sayfalama kodu: 

@Html.PagedListPager((IPagedList)Model, sayfa=>Url.Action("Index", new { sayfa }))

-> controller tarafı : 

using PagedList;
using PagedList.Mvc;


  public ActionResult Index(int sayfa=1)
        {
            //var degerler = db.TBLUYELER.ToList();
            var degerler = db.TBLUYELER.ToList().ToPagedList(sayfa, 3);
            return View(degerler);
        }
** Kitap Ödünç alırken kitaplar sayfasında durumu true olanlar ödünç verilebilir ve bir kişi bir kitabı aynı tarihlerde tekrar ödünç alamaz

** Tarih Formatı kullanımı : 
 <div style="margin-top:15px">
            <label>Kitap Alış Tarihi </label>
            <input type="text" class="form-control" name="ALISTARIH" value="@DateTime.Now.ToShortDateString()" />
        </div>
        <div style="margin-top:15px">
            <label>Kitap İade Tarihi</label>
            <input type="text" class="form-control" name="IADETARIH" value="@DateTime.Now.AddDays(7).ToShortDateString()" />
        </div>

--> İndex sayfasında tarih formatı şeklinde gösterme : 

 <td>@Convert.ToDateTime(k.ALISTARIH).ToString("dd/MM/yyyy")</td>
            <td>@Convert.ToDateTime(k.IADETARIH).ToString("dd/MM/yyyy")</td>

**Kitabın durumunu direk true getirmek için yapılan çalışma (Kitap ödünç alma işlemi için gerekli durum) :

 <input type="hidden" name="Durum" value="True" />
yada controller tarafına aşağıdaki kodu yazabiliriz
  kitap.DURUM = true;

****
-- TRİGER kullanrak kitap durumunun değiştirilmesi : 

CREATE TRIGGER KITAPDURUM
ON TBLHAREKET
AFTER INSERT
AS
DECLARE @KITAP INT
SELECT @KITAP=KITAP FROM inserted
UPDATE TBLKITAP SET DURUM=0 WHERE ID=@KITAP

---
** ÖDÜNÇ ALINAN kitabın iade edilmesi ve iade edildikten sonra sql de durumunun true olması:
 Controller a:
  public ActionResult OduncGuncelle(TBLHAREKET p)
        {
            var hrk = db.TBLHAREKET.Find(p.ID);
            hrk.UYEGETIRTARIH = p.UYEGETIRTARIH;
            hrk.ISLEMDURUM = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

** İade alınan kıtabın kitaplar sayfasındada durumunun değiştirilmesi ve iadesi alınan kitabın iade alma sayfaısndan düşmesi : 


TRİGGER İLE DURUMU KİTAP TABLOSUNDADA GÜNCELLEDİK:

CREATE TRIGGER KITAPDURUM2
ON TBLHAREKET
AFTER UPDATE
AS
DECLARE @KITAP INT
SELECT @KITAP=KITAP FROM inserted
update TBLKITAP SET DURUM=1 WHERE ID=@KITAP


** İslem durumu false olanları getirmesi için yazılan linq sorgusu :
 var degerler = db.TBLHAREKET.Where(x => x.ISLEMDURUM == false).ToList();
 var degerler = db.TBLHAREKET.Where(x => x.ISLEMDURUM == false).ToList();

*****
 AYNI SAYFADA BİRDEN FAZLA TABLO KULLANIMI İÇİN => IEnumerable kullanımı : 
Models klasöründe siniflarim adında bir klasör oluşturduk ve bir class ekledik
içerisine kullancagımız tabloların propertylerini ekledik : 
Önce kütüphanemiizi ekledik :

using MvcKutuphane.Models.Entity;

  public IEnumerable<TBLKITAP> Deger1 { get; set; }
        public IEnumerable<TBLHAKKIMIZDA> Deger2{ get; set; }
** controller tarafına geçtik ve kütühaneden sınıfımızı çağırdık :
using MvcKutuphane.Models.Siniflarim;
** tablolarımızı ekledık : 
  public ActionResult Index()
        {
            Class1 cs = new Class1();
            cs.Deger1 = db.TBLKITAP.ToList();
            cs.Deger2 = db.TBLHAKKIMIZDA.ToList();
            //var degerler = db.TBLKITAP.ToList();
            return View(cs);
        }

***
Yazar eklerken Ad ve soyad alanlarının boş geçilemez yapılması için Data annotations  kullandık : 
Model kısmında yazar tablosuna yaptıklarımız: 

 public int ID { get; set; }
        [Required(ErrorMessage ="Yazar adını boş geçemezsiniz!")]
        public string AD { get; set; }
        [Required(ErrorMessage ="Yazar Soyadını boş geçemezsiniz!")]

YazarEkle.cshtml sayfasında yaptıklarımız : 
 <label>Yazar Adı</label>
            @Html.TextBoxFor(y => y.AD, new { @class = "form-control" })
            @Html.ValidationMessageFor(y => y.AD);
        </div>
        <div style="margin-top:15px">
            <label>Yazar Soyadı</label>
            @Html.TextBoxFor(y => y.SOYAD, new { @class = "form-control" })
            @Html.ValidationMessageFor(y => y.SOYAD);
        </div>

***
Yazar controller tarafında yaptıklarımız: 
 public ActionResult YazarEkle(TBLYAZAR p)
        {
            if (!ModelState.IsValid)
            {
                return View("YazarEkle");
            }
---------,----,--
																
}
Hava kartı olusturmak için kullandıgımız site : 
https://weatherwidget.io/

***
GALERİ kısmına resım yüklemek için : 

<div style="margin-top:100px">
    <form action="/Istatistik/resimyukle" method="post" enctype="multipart/form-data">
        <input type="file" name="dosya" id="dosya" class="btn btn-primary" />
        <br />
        <input type="submit" value="Kaydet" class="btn btn-warning" />
    </form>
</div>

enctype ="multipart/form-data" dosya yüklemek için kullandık 
****
linq kartlar kısmında En çok kıtabı olan  yazar için yapılan çalışma : 
Her bir yazarın kaç kitabı olduğunu sorguladığımız sql sorgumuz : 

SELECT TBLYAZAR.AD + TBLYAZAR.SOYAD,COUNT(*) AS 'TOPLAM' FROM TBLKITAP
INNER JOIN TBLYAZAR ON TBLYAZAR.ID=TBLKITAP.YAZAR
 GROUP BY TBLYAZAR.AD,TBLYAZAR.SOYAD

Sorgumuzu prosedüre attık : 

CREATE PROCEDURE EnFazlaKitapYazar
AS
SELECT TOP 1 TBLYAZAR.AD + TBLYAZAR.SOYAD  FROM TBLKITAP
INNER JOIN TBLYAZAR ON TBLYAZAR.ID=TBLKITAP.YAZAR
 GROUP BY TBLYAZAR.AD,TBLYAZAR.SOYAD ORDER BY COUNT(*) DESC
****
En iyi yayınevini bulduğumuz sql sorgumuz :
SELECT TOP 1 YAYINEVI, COUNT(*) FROM TBLKITAP GROUP BY YAYINEVI ORDER BY COUNT(*) DESC

aynı işlemi yapan linq sorgumuz : 
        var deger9 = db.TBLKITAP.GroupBy(x => x.YAYINEVI).OrderByDescending(z => z.Count()).Select(y => new { y.Key }).FirstOrDefault();

Linq Kartlarımızı çağırdığımız linq sorgularımız : 
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

*****
Login İşlemi için önce web.config dosyamıza bir authentication tanımladık : 
 <system.web>
    <compilation debug="true" targetFramework="4.7.2" />
    <httpRuntime targetFramework="4.7.2" />
    <authentication>
      <forms loginUrl="/Login/GirisYap/"></forms>
    </authentication>
  </system.web>

Sonra Panel Controllerı mıza index ine authorize ettik 

 [Authorize]
        public ActionResult Index()
        {
            return View();
        }
***
LoginController da 
using System.Web.Security; 

kütüphanemizi ekledik 
Post işlemi ile bilgilerimizin veritabanında girilen bilgilerle doğruluğunu kıyasladık :

 [HttpPost]
        public ActionResult GirisYap(TBLUYELER p)
        {
            var bilgiler = db.TBLUYELER.FirstOrDefault(x => x.MAIL == p.MAIL && x.SIFRE == p.SIFRE);
            if(bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.MAIL, false);
                return RedirectToAction("Index","Panelim");
            }
            else
            {
                return View();
            }
            
        }

**
Session ile kullanıcı bilgilerini sayfaya çektik : 
LoginController : 
 var bilgiler = db.TBLUYELER.FirstOrDefault(x => x.MAIL == p.MAIL && x.SIFRE == p.SIFRE);
            if(bilgiler != null)
            {
                FormsAuthentication.SetAuthCookie(bilgiler.MAIL, false);
                Session["Ad"] = bilgiler.AD.ToString();
}
Index tarafında  tablo oluşturarak içerisine çağırdık : 
<tr>
        <td>
            <label style="font-weight:bolder">Mail Adresi:</label>  @HttpContext.Current.User.Identity.Name
        </td>
    </tr>
    <tr>
        <td>
            <label style="font-weight:bolder">Ad:</label>:@Session["Ad"].ToString()
        </td>
    </tr>
    <tr>
******
! SADECE ÜYEYE AİT KİTAPLARI GETİREN LİNQ SORGUSU : 
 public ActionResult Kitaplarim()
        {
            var kullanici = (string)Session["Mail"];
            var id = db.TBLUYELER.Where(x => x.MAIL == kullanici.ToString()).Select(z => z.ID).FirstOrDefault();
            var degerler = db.TBLHAREKET.Where(x => x.UYE == id).ToList();
            return View(degerler);
        }
******

** İLİŞKİLİ TABLOLARDA SİLME KULLANILMAZ DURUM DEĞİŞTİRİLİR  : 
 public ActionResult KategoriSil(int id)
        {
            var kategori = db.TBLKATEGORI.Find(id);
            //db.TBLKATEGORI.Remove(kategori);
            kategori.DURUM = false;
            db.SaveChanges();
            return RedirectToAction("Index");

        }

***
! YAZARA AİT KİTAPLARI GETİREN LİNQ SORGUSU : 
   public ActionResult YazarKitaplar(int id)
        {
            var yazar = db.TBLKITAP.Where(x => x.YAZAR == id).ToList();
            return View(yazar);
        }


***


