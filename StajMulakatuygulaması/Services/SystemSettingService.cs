using Microsoft.EntityFrameworkCore;
using StajMulakatuygulaması.Models;
using StajMulakatuygulamasi.Models;

namespace StajMulakatuygulamasi.Services
{
    public class SystemSettingService
    {
        private readonly IDbContextFactory<StajMulakatDbContext> _factory;

        public SystemSettingService(IDbContextFactory<StajMulakatDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<DonemAyari> GetAktifDonemAsync()
        {
            using var context = _factory.CreateDbContext();
            // Varsayılan olarak ilk kaydı veya aktif olanı getir
            var ayar = await context.DonemAyarlari.FirstOrDefaultAsync(x => x.AktifMi);
            return ayar ?? new DonemAyari(); // Yoksa boş yeni bir tane döndür
        }

        public async Task KaydetAsync(DonemAyari ayar)
        {
            using var context = _factory.CreateDbContext();
            if (ayar.AyarlarID == 0)
            {
                context.DonemAyarlari.Add(ayar);
            }
            else
            {
                context.DonemAyarlari.Update(ayar);
            }
            await context.SaveChangesAsync();
        }
    }
}