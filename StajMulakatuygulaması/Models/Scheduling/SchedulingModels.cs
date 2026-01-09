using System;
using System.Collections.Generic;

namespace StajMulakatuygulamasi.Models.Scheduling
{
    // --- 1. ZAMAN DİLİMİ (TimeSlot) ---
    public class TimeSlot
    {
        public int SlotId { get; set; }     
        public int DayOfWeek { get; set; }
        public string Day { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    // --- 2. ALGORİTMA İÇİN ÖĞRENCİ MODELİ ---
    public class SchedulingStudent
    {
        public int StudentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double PriorityScore { get; set; } = 0;
        public List<TimeSlot> BusySlots { get; set; } = new List<TimeSlot>();
        public AssignedInterview? AssignedInterview { get; set; }


    }

    // --- 3. ALGORİTMA İÇİN HOCA MODELİ ---
    public class SchedulingInstructor
    {
        public int InstructorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TimeSlot> AvailableSlots { get; set; } = new List<TimeSlot>();
        public HashSet<int> TakenSlots { get; set; } = new HashSet<int>();
    }

    // --- 4. ATAMA SONUCU MODELİ ---
    public class AssignedInterview
    {
        public SchedulingStudent Student { get; set; } = new();
        public SchedulingInstructor Instructor { get; set; } = new();
        public TimeSlot Slot { get; set; } = new();
    }
}