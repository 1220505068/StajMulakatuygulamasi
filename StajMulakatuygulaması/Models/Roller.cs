using System;
using System.Collections.Generic;

namespace StajMulakatuygulaması.Models;

public partial class Roller
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public virtual ICollection<Kullanicilar> Kullanicilars { get; set; } = new List<Kullanicilar>();
}
