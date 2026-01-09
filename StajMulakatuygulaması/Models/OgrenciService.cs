using Microsoft.EntityFrameworkCore;
using StajMulakatuygulamasi.Models;
using StajMulakatuygulaması.Models;

namespace StajMulakatuygulamasi.Services
{
    public class OgrenciService
    {
        private readonly IDbContextFactory<StajMulakatDbContext> _factory;

        public OgrenciService(IDbContextFactory<StajMulakatDbContext> factory)
        {
            _factory = factory;
        }

        // Öğrencinin ders programını getir (true = derste/meşgul)
        public async Task<bool[,]> GetMesguliyetDurumuAsync(int ogrenciId)
        {
            using var context = _factory.CreateDbContext();
            bool[,] durum = new bool[5, 8];

            var kayitlar = await context.OgrenciMesguliyetleri
                                        .Where(x => x.KullaniciID == ogrenciId)
                                        .ToListAsync();

            foreach (var kayit in kayitlar)
            {
                int gunIndex = kayit.Gun - 1;
                int saatIndex = kayit.Saat - 9;
                if (gunIndex >= 0 && gunIndex < 5 && saatIndex >= 0 && saatIndex < 8)
                {
                    durum[gunIndex, saatIndex] = true;
                }
            }
            return durum;
        }

        // Ders programını kaydet
        public async Task KaydetAsync(int ogrenciId, bool[,] yeniDurum)
        {
            using var context = _factory.CreateDbContext();
            var eskiKayitlar = context.OgrenciMesguliyetleri.Where(x => x.KullaniciID == ogrenciId);
            context.OgrenciMesguliyetleri.RemoveRange(eskiKayitlar);

            for (int gunIndex = 0; gunIndex < 5; gunIndex++)
            {
                for (int saatIndex = 0; saatIndex < 8; saatIndex++)
                {
                    if (yeniDurum[gunIndex, saatIndex]) // Eğer seçiliyse dersi vardır
                    {
                        context.OgrenciMesguliyetleri.Add(new OgrenciMesguliyet
                        {
                            KullaniciID = ogrenciId,
                            Gun = gunIndex + 1,
                            Saat = saatIndex + 9
                        });
                    }
                }
            }
            await context.SaveChangesAsync();
        }
    }
}