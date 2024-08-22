using SchoolCalendarSystem.Models;

public class Teacher
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; }
    public int MaxWeeklyHours { get; set; } // Maximum teaching hours per week
    public int MaxDailyHours { get; set; } // Maximum teaching hours per day

    // Relationships
    public List<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>(); // Many-to-Many with Subject
    public List<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>(); // One-to-Many with ScheduleEntry

}
