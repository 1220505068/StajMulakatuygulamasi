using System;
using System.Collections.Generic;

namespace StajMulakatuygulaması.Models;

public partial class OgrenciUygunluk
{
    public int AvailabilityId { get; set; }

    public int StudentId { get; set; }

    public DateTime BaslangicZamani { get; set; }

    public DateTime BitisZamani { get; set; }

    public string Tip { get; set; } = null!;

    public virtual Kullanicilar Student { get; set; } = null!;
}
