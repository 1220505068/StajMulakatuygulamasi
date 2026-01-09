using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajMulakatuygulamasi.Models
{
    [Table("DonemAyarlari")]
    public class DonemAyari
    {
        [Key]
        public int AyarlarID { get; set; }
        [Required]
        public string DonemAdi { get; set; } = "Yeni Dönem";
        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;
        public DateTime BitisTarihi { get; set; } = DateTime.Now.AddDays(14); // Varsayılan 2 hafta
        public bool AktifMi { get; set; } = true;
    }
}