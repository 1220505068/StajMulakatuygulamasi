using Microsoft.EntityFrameworkCore;
using StajMulakatuygulamasi.Models;
using StajMulakatuygulaması.Models;

namespace StajMulakatuygulamasi.Services
{
    public class HocaMusaitlikService
    {
        // Factory tanımı SADECE BİR KEZ yapılmalı. Hatanızın sebebi buydu.
        private readonly IDbContextFactory<StajMulakatDbContext> _factory;

        public HocaMusaitlikService(IDbContextFactory<StajMulakatDbContext> factory)
        {
            _factory = factory;
        }

        // Hocanın mevcut müsaitliklerini getir
        public async Task<bool[,]> GetMusaitlikDurumuAsync(int hocaId)
        {
            using var context = _factory.CreateDbContext();
            bool[,] durum = new bool[5, 8];

            // DOĞRU İSİM: KullaniciID
            var kayitlar = await context.HocaMusaitliklar
                                        .Where(x => x.KullaniciID == hocaId)
                                        .ToListAsync();

            foreach (var kayit in kayitlar)
            {
                // DOĞRU İSİMLER: Gun ve Saat
                int gunIndex = kayit.Gun - 1;
                int saatIndex = kayit.Saat - 9;

                if (gunIndex >= 0 && gunIndex < 5 && saatIndex >= 0 && saatIndex < 8)
                {
                    durum[gunIndex, saatIndex] = true;
                }
            }
            return durum;
        }

        // Müsaitlikleri kaydet
        public async Task KaydetAsync(int hocaId, bool[,] yeniDurum)
        {
            using var context = _factory.CreateDbContext();

            // DOĞRU İSİM: KullaniciID
            var eskiKayitlar = context.HocaMusaitliklar.Where(x => x.KullaniciID == hocaId);
            context.HocaMusaitliklar.RemoveRange(eskiKayitlar);

            for (int gunIndex = 0; gunIndex < 5; gunIndex++)
            {
                for (int saatIndex = 0; saatIndex < 8; saatIndex++)
                {
                    if (yeniDurum[gunIndex, saatIndex])
                    {
                        context.HocaMusaitliklar.Add(new HocaMusaitlik
                        {
                            // DOĞRU İSİMLER: KullaniciID, Gun, Saat
                            KullaniciID = hocaId,
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