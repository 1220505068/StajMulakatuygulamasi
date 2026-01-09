using StajMulakatuygulaması.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajMulakatuygulamasi.Models
{
    [Table("Bildirimler")]
    public class Bildirim
    {
        [Key]
        public int BildirimID { get; set; }

        public int GonderenID { get; set; }

        [ForeignKey("GonderenID")]
        public virtual Kullanicilar Gonderen { get; set; } // Gönderen kişinin detaylarına erişmek için

        public string HedefRol { get; set; } = "Admin";

        [Required]
        public string Konu { get; set; }

        [Required]
        public string Mesaj { get; set; }

        public DateTime Tarih { get; set; } = DateTime.Now;

        public bool OkunduMu { get; set; } = false;
    }
}