using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;
using SchoolCalendarSystem.Dto;

[Route("api/[controller]")]
[ApiController]
public class StudentController : ControllerBase
{
    private readonly SchoolContext _context;

    public StudentController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/Student
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
    {
        var students = await _context.Students
            .Include(s => s.Class)
            .Select(s => new StudentDto
            {
                StudentName = s.StudentName,
                DateOfBirth = s.DateOfBirth,
                ClassId = s.ClassId
            })
            .ToListAsync();

        return Ok(students);
    }

    // GET: api/Student/5
    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetStudent(int id)
    {
        var student = await _context.Students
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            return NotFound();
        }

        var studentDto = new StudentDto
        {
            StudentName = student.StudentName,
            DateOfBirth = student.DateOfBirth,
            ClassId = student.ClassId
        };

        return Ok(studentDto);
    }

    // POST: api/Student
    [HttpPost]
    public async Task<ActionResult<StudentDto>> PostStudent(StudentDto studentDto)
    {
        // Ensure that the class exists
        var classExists = await _context.Classes.AnyAsync(c => c.ClassId == studentDto.ClassId);
        if (!classExists)
        {
            return BadRequest("The Class with the specified ID does not exist.");
        }

        var student = new Student
        {
            StudentName = studentDto.StudentName,
            DateOfBirth = studentDto.DateOfBirth,
            ClassId = studentDto.ClassId
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        studentDto.ClassId = student.ClassId; // Set ID in the DTO
        return CreatedAtAction(nameof(GetStudent), new { id = student.StudentId }, studentDto);
    }

    // PUT: api/Student/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int id, StudentDto studentDto)
    {
        if (id != studentDto.ClassId)
        {
            return BadRequest();
        }

        // Ensure that the class exists
        var classExists = await _context.Classes.AnyAsync(c => c.ClassId == studentDto.ClassId);
        if (!classExists)
        {
            return BadRequest("The Class with the specified ID does not exist.");
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        student.StudentName = studentDto.StudentName;
        student.DateOfBirth = studentDto.DateOfBirth;
        student.ClassId = studentDto.ClassId;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!StudentExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/Student/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.StudentId == id);
    }
}
