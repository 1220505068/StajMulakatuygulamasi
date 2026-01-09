using Microsoft.EntityFrameworkCore;
using StajMulakatuygulamasi.Models;
using StajMulakatuygulaması.Models;

namespace StajMulakatuygulamasi.Services
{
    public class BildirimService
    {
        // HATA DÜZELTİLDİ: 'AppDbContext' yerine projenizdeki gerçek isim olan 'StajMulakatDbContext' kullanıldı.
        private readonly IDbContextFactory<StajMulakatDbContext> _factory;

        // HATA DÜZELTİLDİ: Yapıcı metotta da isim düzeltildi.
        public BildirimService(IDbContextFactory<StajMulakatDbContext> factory)
        {
            _factory = factory;
        }

        // YENİ BİLDİRİM GÖNDER (Öğrenci İşleri kullanacak)
        public async Task<bool> BildirimGonderAsync(int gonderenId, string konu, string mesaj)
        {
            using var context = _factory.CreateDbContext();
            var yeniBildirim = new Bildirim
            {
                GonderenID = gonderenId,
                HedefRol = "Admin",
                Konu = konu,
                Mesaj = mesaj,
                Tarih = DateTime.Now,
                OkunduMu = false
            };

            context.Bildirimler.Add(yeniBildirim);
            return await context.SaveChangesAsync() > 0;
        }

        // OKUNMAMIŞ ADMİN BİLDİRİMLERİNİ GETİR (Admin başlıkta görecek)
        public async Task<List<Bildirim>> GetOkunmamisAdminBildirimleriAsync()
        {
            using var context = _factory.CreateDbContext();
            // Hedefi 'Admin' olan ve henüz okunmamış bildirimleri, gönderen bilgisiyle beraber getir.
            return await context.Bildirimler
                .Include(b => b.Gonderen)
                .Where(b => b.HedefRol == "Admin" && b.OkunduMu == false)
                .OrderByDescending(b => b.Tarih)
                .ToListAsync();
        }
    }
}