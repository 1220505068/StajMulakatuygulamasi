using StajMulakatuygulaması.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StajMulakatuygulamasi.Models
{
    [Table("PlanlananMulakatlar")]
    public partial class PlanlananMulakatlar
    {
        [Key]
        public int MulakatId { get; set; }

        public int HocaId { get; set; }

        public int StudentId { get; set; }

        public DateTime BaslangicSaati { get; set; }

        public DateTime BitisSaati { get; set; }

        public string Durum { get; set; } = "Planlandı";

        // İlişkiler (Navigation Properties)
        public virtual Kullanicilar? Hoca { get; set; }
        public virtual Kullanicilar? Student { get; set; }
    }
}