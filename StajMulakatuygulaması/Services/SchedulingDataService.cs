using Microsoft.EntityFrameworkCore;
using StajMulakatuygulamasi.Models;
using StajMulakatuygulaması.Models;
using StajMulakatuygulamasi.Models.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StajMulakatuygulamasi.Services
{
    public class SchedulingDataService
    {
        // --- KİLİT KONTROLÜ ---
        // Eğer planlanmış mülakat tablosunda veri varsa, sistem kilitlenmiş demektir.
        public async Task<bool> IsSystemLockedAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.PlanlananMulakatlar.AnyAsync();
        }

        // --- ÖĞRENCİ MEŞGULİYET (DERS PROGRAMI) İŞLEMLERİ ---
        public async Task<List<OgrenciMesguliyet>> GetOgrenciMesguliyetAsync(int ogrenciId)
        {
            using var context = _factory.CreateDbContext();
            return await context.OgrenciMesguliyet
                .Where(x => x.KullaniciID == ogrenciId)
                .ToListAsync();
        }

        public async Task SaveOgrenciMesguliyetAsync(int ogrenciId, List<OgrenciMesguliyet> mesguliyetler)
        {
            using var context = _factory.CreateDbContext();

            // Önce eskileri temizle (Güncelleme mantığı)
            var eskiler = context.OgrenciMesguliyet.Where(x => x.KullaniciID == ogrenciId);
            context.OgrenciMesguliyet.RemoveRange(eskiler);

            // Yenileri ekle
            if (mesguliyetler != null && mesguliyetler.Any())
            {
                await context.OgrenciMesguliyet.AddRangeAsync(mesguliyetler);
            }

            await context.SaveChangesAsync();
        }

        // --- HOCA MÜSAİTLİK İŞLEMLERİ ---
        public async Task<List<HocaMusaitlik>> GetHocaMusaitlikAsync(int hocaId)
        {
            using var context = _factory.CreateDbContext();
            return await context.HocaMusaitlik
                .Where(x => x.KullaniciID == hocaId)
                .ToListAsync();
        }

        public async Task SaveHocaMusaitlikAsync(int hocaId, List<HocaMusaitlik> musaitlikler)
        {
            using var context = _factory.CreateDbContext();

            var eskiler = context.HocaMusaitlik.Where(x => x.KullaniciID == hocaId);
            context.HocaMusaitlik.RemoveRange(eskiler);

            if (musaitlikler != null && musaitlikler.Any())
            {
                await context.HocaMusaitlik.AddRangeAsync(musaitlikler);
            }

            await context.SaveChangesAsync();
        }
        private readonly IDbContextFactory<StajMulakatDbContext> _factory;

        public SchedulingDataService(IDbContextFactory<StajMulakatDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<PlanlananMulakatlar>> GetAllMulakatlarAsync()
        {
            using var context = _factory.CreateDbContext();
            return await context.PlanlananMulakatlar
                .Include(m => m.Student) // Öğrenci adını görmek için
                .Include(m => m.Hoca)    // Hoca adını görmek için
                .OrderBy(m => m.BaslangicSaati) // Tarihe göre sırala
                .ToListAsync();
        }
        // --- 1. PANEL İŞLEMLERİ ---
        public async Task<List<PlanlananMulakatlar>> GetMulakatlarByHocaIdAsync(int hocaId)
        {
            using var context = _factory.CreateDbContext();
            return await context.PlanlananMulakatlar
                .Include(m => m.Student)
                .Where(m => m.HocaId == hocaId)
                .OrderBy(m => m.BaslangicSaati)
                .ToListAsync();
        }

        public async Task<List<PlanlananMulakatlar>> GetMulakatlarByOgrenciIdAsync(int ogrenciId)
        {
            using var context = _factory.CreateDbContext();
            return await context.PlanlananMulakatlar
                .Include(m => m.Hoca)
                .Where(m => m.StudentId == ogrenciId)
                .OrderBy(m => m.BaslangicSaati)
                .ToListAsync();
        }

        // --- 2. ALGORİTMA HAZIRLIK İŞLEMLERİ ---

        public async Task<List<SchedulingStudent>> GetStudentsForAlgorithmAsync()
        {
            using var context = _factory.CreateDbContext();

            // Öğrencileri çek (Role mantığına göre filtrele)
            var dbStudents = await context.Kullanicilar
                                   .Include(k => k.Role)
                                   .Where(k => k.Role.RoleName == "Ogrenci")
                                   .ToListAsync();

            return dbStudents.Select(s => new SchedulingStudent
            {
                StudentId = s.UserId,
                Name = s.Ad + " " + s.Soyad,
                PriorityScore = 0
            }).ToList();
        }

        public async Task<List<SchedulingInstructor>> GetInstructorsForAlgorithmAsync()
        {
            using var context = _factory.CreateDbContext();

            var dbInstructors = await context.Kullanicilar
                                     .Include(k => k.Role)
                                     .Include(k => k.HocaMusaitliklari)
                                     .Where(k => k.Role.RoleName == "OgretimGorevlisi")
                                     .ToListAsync();

            var result = new List<SchedulingInstructor>();

            foreach (var hoca in dbInstructors)
            {
                var musaitlikler = new List<TimeSlot>();

                foreach (var m in hoca.HocaMusaitliklari)
                {
                    // DÜZELTME: m.Gun artık INT olduğu için direkt alıyoruz.
                    int gunIndex = m.Gun;

                    // Tarih Hesaplama
                    DateTime buGun = DateTime.Today;
                    int bugunIndex = (int)buGun.DayOfWeek;
                    if (bugunIndex == 0) bugunIndex = 7; // Pazar düzeltmesi

                    int fark = gunIndex - bugunIndex;
                    DateTime hedefTarih = buGun.AddDays(fark);

                    // Saat Hesaplama (m.Saat INT varsayıldı)
                    DateTime baslangicZamani = hedefTarih.Date.AddHours(m.Saat);

                    musaitlikler.Add(new TimeSlot
                    {
                        SlotId = m.MusaitlikID,
                        // DÜZELTME: Ekranda güzel gözüksün diye int'i yazıya çeviriyoruz
                        Day = GunIndexiMetneCevir(gunIndex),
                        DayOfWeek = gunIndex,
                        StartTime = baslangicZamani,
                        EndTime = baslangicZamani.AddMinutes(20)
                    });
                }

                result.Add(new SchedulingInstructor
                {
                    InstructorId = hoca.UserId,
                    Name = hoca.Ad + " " + hoca.Soyad,
                    AvailableSlots = musaitlikler
                });
            }

            return result;
        }

        // --- 3. KAYDETME İŞLEMİ ---
        public async Task SaveScheduleToDbAsync(List<AssignedInterview> schedule)
        {
            using var context = _factory.CreateDbContext();

            // 1. Önce eski kayıtları temizle
            var eskiKayitlar = context.PlanlananMulakatlar.ToList();
            if (eskiKayitlar.Any())
            {
                context.PlanlananMulakatlar.RemoveRange(eskiKayitlar);
                await context.SaveChangesAsync();
            }

            // 2. Yeni listeyi güvenli bir şekilde oluştur
            var dbList = new List<PlanlananMulakatlar>();

            foreach (var item in schedule)
            {
                // GÜVENLİK KONTROLÜ:
                // Eğer mülakat nesnesi, hoca, öğrenci veya slot BOŞ ise bu kaydı atla (Sistemi çökertme)
                if (item == null) continue;
                if (item.Instructor == null || item.Student == null || item.Slot == null) continue;

                dbList.Add(new PlanlananMulakatlar
                {
                    HocaId = item.Instructor.InstructorId,
                    StudentId = item.Student.StudentId,
                    BaslangicSaati = item.Slot.StartTime,
                    BitisSaati = item.Slot.EndTime,
                    Durum = "Planlandı"
                });
            }

            // 3. Sadece geçerli kayıtları veritabanına ekle
            if (dbList.Any())
            {
                await context.PlanlananMulakatlar.AddRangeAsync(dbList);
                await context.SaveChangesAsync();
            }
        }

        // --- YARDIMCI METOT (YENİ) ---
        // Sayı (1) gelirse "Pazartesi" string'i döndürür
        private string GunIndexiMetneCevir(int gunIndex)
        {
            return gunIndex switch
            {
                1 => "Pazartesi",
                2 => "Salı",
                3 => "Çarşamba",
                4 => "Perşembe",
                5 => "Cuma",
                6 => "Cumartesi",
                7 => "Pazar",
                _ => "Bilinmiyor"
            };
        }
    }
}