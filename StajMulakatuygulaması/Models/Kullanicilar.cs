using System;
using System.Collections.Generic;
using StajMulakatuygulamasi.Models;

namespace StajMulakatuygulaması.Models;

public partial class Kullanicilar
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Ad { get; set; } = null!;

    public string Soyad { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual ICollection<HocaMusaitlik> HocaMusaitliklari { get; set; }
    public virtual ICollection<OgrenciMesguliyet> OgrenciMesguliyetleri { get; set; }


    public virtual ICollection<OgrenciUygunluk> OgrenciUygunluks { get; set; } = new List<OgrenciUygunluk>();

    public virtual ICollection<PlanlananMulakatlar> PlanlananMulakatlarHocas { get; set; } = new List<PlanlananMulakatlar>();

    public virtual ICollection<PlanlananMulakatlar> PlanlananMulakatlarStudents { get; set; } = new List<PlanlananMulakatlar>();

    public virtual Roller Role { get; set; } = null!;
}
