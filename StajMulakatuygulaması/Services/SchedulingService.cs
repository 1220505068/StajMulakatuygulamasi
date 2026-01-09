using System;
using System.Collections.Generic;
using System.Linq;
using StajMulakatuygulamasi.Models.Scheduling;


namespace StajMulakatuygulamasi.Services
{
    public class SchedulingService
    {
        // Sonuç listesi
        private List<AssignedInterview> _finalSchedule;

        // --- YENİ: Doluluk durumunu nesnelerin üzerinde değil, burada tutacağız ---
        // Key: HocaID, Value: Dolu olduğu SlotID'ler kümesi
        private Dictionary<int, HashSet<int>> _instructorBusySlots;

        // Öğrenci ve Hoca listelerini global tutalım ki özyinelemede (recursion) erişebilelim
        private List<SchedulingStudent> _students;
        private List<SchedulingInstructor> _instructors;

        public List<AssignedInterview> GenerateSchedule(List<SchedulingStudent> students, List<SchedulingInstructor> instructors)
        {
            _finalSchedule = new List<AssignedInterview>();
            _students = students;
            _instructors = instructors;

            // 1. Yerel takip sözlüğünü hazırla (Her hoca için boş bir sepet oluştur)
            _instructorBusySlots = new Dictionary<int, HashSet<int>>();
            foreach (var inst in _instructors)
            {
                _instructorBusySlots[inst.InstructorId] = new HashSet<int>();
            }

            // 2. Backtracking algoritmasını başlat
            if (BacktrackSolve(0))
            {
                return _finalSchedule;
            }
            else
            {
                // Tam çözüm bulunamadıysa bile (nadir durum), şu ana kadar yapılanları döndür
                return _finalSchedule;
            }
        }

        private bool BacktrackSolve(int studentIndex)
        {
            // --- DURMA KOŞULU: Tüm öğrenciler yerleştiyse BİTİR ---
            if (studentIndex >= _students.Count)
            {
                return true;
            }

            var currentStudent = _students[studentIndex];

            // Bu öğrenci için her hocayı ve hocanın müsait slotlarını dene
            foreach (var instructor in _instructors)
            {
                // Hocanın müsaitliklerini gez
                // (Burada AvailableSlots listesini değiştirmemeye dikkat ediyoruz, sadece okuyoruz)
                foreach (var slot in instructor.AvailableSlots)
                {
                    // --- KONTROLLER ---

                    // 1. Hoca bu saatte dolu mu? (Yerel sözlükten kontrol et)
                    if (_instructorBusySlots[instructor.InstructorId].Contains(slot.SlotId))
                    {
                        continue; // Hoca dolu, pas geç
                    }

                    // 2. Öğrencinin dersi var mı? (Çakışma kontrolü)
                    bool studentHasClass = currentStudent.BusySlots.Any(busy =>
                        busy.DayOfWeek == slot.DayOfWeek &&
                        busy.StartTime.Hour == slot.StartTime.Hour);

                    if (studentHasClass)
                    {
                        continue; // Öğrencinin dersi var, pas geç
                    }

                    // --- ATAMA YAP (DENEME) ---

                    // A) Listeye ekle
                    var assignment = new AssignedInterview
                    {
                        Student = currentStudent,
                        Instructor = instructor,
                        Slot = slot
                    };
                    _finalSchedule.Add(assignment);

                    // B) Hocayı bu saatte "DOLU" olarak işaretle (Yerel sözlükte)
                    _instructorBusySlots[instructor.InstructorId].Add(slot.SlotId);

                    // --- ÖZYİNELEME (Bir sonraki öğrenciye geç) ---
                    if (BacktrackSolve(studentIndex + 1))
                    {
                        return true; // Zincirleme başarı! Çık.
                    }

                    // --- GERİ AL (Backtrack) - Eğer yukarıdaki yol başarısız olduysa ---
                    // Yaptığımız atamayı geri alıp diğer slotu deneyeceğiz
                    _finalSchedule.Remove(assignment);
                    _instructorBusySlots[instructor.InstructorId].Remove(slot.SlotId);
                }
            }

            // Eğer hiçbir hoca/slot bu öğrenciye uymadıysa,
            // (Opsiyonel) Bu öğrenciyi boş geçip diğerlerini denemeye devam etsin mi?
            // "return false" dersek algoritma komple tıkanır. 
            // "return BacktrackSolve(studentIndex + 1)" dersek bu öğrenciyi atlayıp devam eder.
            // Şimdilik basitlik adına atlayıp devam etmesini sağlıyoruz:
            return BacktrackSolve(studentIndex + 1);
        }
    }
}