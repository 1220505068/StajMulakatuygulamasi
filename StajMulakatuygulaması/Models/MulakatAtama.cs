using StajMulakatuygulaması.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajMulakatuygulamasi.Models
{
    [Table("MulakatAtamalari")]
    public class MulakatAtama
    {
        [Key]
        public int AtamaID { get; set; }
        public int OgrenciID { get; set; }
        public int HocaID { get; set; }
        public int Gun { get; set; }
        public int Saat { get; set; }
        public DateTime AtamaTarihi { get; set; } = DateTime.Now;

        [ForeignKey("OgrenciID")]
        public virtual Kullanicilar Ogrenci { get; set; }
        [ForeignKey("HocaID")]
        public virtual Kullanicilar Hoca { get; set; }
    }
}