using StajMulakatuygulaması.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace StajMulakatuygulamasi.Models
{
    // Veritabanı tablosu ile tam eşleşen model
    [Table("HocaMusaitlik")]
    public class HocaMusaitlik
    {
        [Key]
        public int MusaitlikID { get; set; }

        // Hocanın ID'si (Kullanicilar tablosundaki UserId ile eşleşir)
        public int KullaniciID { get; set; }

        // Gün (1=Pzt ... 5=Cuma)
        public int Gun { get; set; }

        // Saat (9 ... 16)
        public int Saat { get; set; }

        [ForeignKey("KullaniciID")]
        public virtual Kullanicilar Hoca { get; set; }
    }
}