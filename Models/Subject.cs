using SchoolCalendarSystem.Models;
public class Subject
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } // e.g., "Mathematics", "Algorithms"
    public int WeeklyHours { get; set; } // Total hours per week
    public int DailyHours { get; set; } // Hours per day

    // Relationships
    public List<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>(); // Many-to-Many with Class
    public List<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>(); // Many-to-Many with Teacher
    public List<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>(); // One-to-Many with ScheduleEntry
}
