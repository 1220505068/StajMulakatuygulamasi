using Microsoft.EntityFrameworkCore;
using StajMulakatuygulaması.Models;

namespace StajMulakatuygulamasi.Services
{
    public class AuthService
    {
        // ARTIK DİREKT CONTEXT DEĞİL, FABRİKASINI TUTUYORUZ
        private readonly IDbContextFactory<StajMulakatDbContext> _factory;

        // Oturum açan kullanıcıyı tutacak değişken
        public Kullanicilar? CurrentUser { get; private set; }

        // Yapıcı metotta Fabrikayı (Factory) alıyoruz
        public AuthService(IDbContextFactory<StajMulakatDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<Kullanicilar?> LoginAsync(string username, string password, string requiredRole)
        {
            // DİKKAT: Her işlem için fabrikadan taze bir context (bağlantı) üretiyoruz.
            // "using" bloğu, işlem bitince bağlantının güvenle kapatılmasını sağlar.
            using var context = _factory.CreateDbContext();

            var user = await context.Kullanicilar
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            // Kullanıcı bulundu mu ve rolü var mı?
            if (user != null && user.Role != null)
            {
                // SENARYO 1: Kullanıcının rolü, girmek istediği kapıdaki rolle birebir aynı (Normal durum)
                // Örn: Öğrenci -> Öğrenci sekmesinden giriyor.
                bool isRoleMatch = user.Role.RoleName == requiredRole;

                // SENARYO 2 (GİZLİ GİRİŞ): Kullanıcı gerçekte 'Admin' ise ve 'Öğrenci İşleri' kapısından girmek istiyorsa izin ver.
                bool isAdminEnteringAsOis = (user.Role.RoleName == "Admin" && requiredRole == "OgrenciIsleri");

                // Eğer iki senaryodan biri geçerliyse girişi onayla
                if (isRoleMatch || isAdminEnteringAsOis)
                {
                    CurrentUser = user;
                    return user;
                }
            }

            CurrentUser = null;
            return null;
        }

        public void Logout()
        {
            CurrentUser = null;
        }

        public async Task<List<Kullanicilar>> GetAllStudentsAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Kullanicilar
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Ogrenci")
                .OrderBy(u => u.Ad) // İsim sırasına göre getir
                .ToListAsync();
        }

        // YENİ METOT 2: Rolü 'OgretimGorevlisi' olan tüm kullanıcıları getirir.
        public async Task<List<Kullanicilar>> GetAllInstructorsAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.Kullanicilar
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "OgretimGorevlisi")
                .OrderBy(u => u.Ad)
                .ToListAsync();
        }
    }
}