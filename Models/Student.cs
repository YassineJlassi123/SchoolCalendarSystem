using System.Security.Claims;

public class Student
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public DateTime DateOfBirth { get; set; }

    // Relationships
    public int ClassId { get; set; }
    public Class Class { get; set; }  // Each student belongs to one class
}