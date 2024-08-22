using SchoolCalendarSystem.Models;

public class Class
{
    public int ClassId { get; set; }
    public string ClassName { get; set; } // e.g., "First Grade", "Second Grade"

    // Relationships
    public List<ClassSubject> ClassSubjects { get; set; } = new List<ClassSubject>();// Many-to-Many with Subject
    public List<ScheduleEntry> ScheduleEntries { get; set; } = new List<ScheduleEntry>(); // One-to-Many with ScheduleEntry
    public List<Student> Students { get; set; } = new List<Student>(); // One-to-Many with Student
}
