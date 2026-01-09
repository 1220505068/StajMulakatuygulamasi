using StajMulakatuygulaması.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajMulakatuygulamasi.Models
{
    [Table("OgrenciMesguliyet")]
    public class OgrenciMesguliyet
    {
        [Key]
        public int MesguliyetID { get; set; }
        public int KullaniciID { get; set; }
        public int Gun { get; set; }
        public int Saat { get; set; }

        [ForeignKey("KullaniciID")]
        public virtual Kullanicilar Ogrenci { get; set; }
    }
}