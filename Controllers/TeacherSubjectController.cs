using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;
using SchoolCalendarSystem.Dto;

[Route("api/[controller]")]
[ApiController]
public class TeacherSubjectController : ControllerBase
{
    private readonly SchoolContext _context;

    public TeacherSubjectController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/TeacherSubject/{teacherId}
    [HttpGet("{teacherId}")]
    public async Task<ActionResult<IEnumerable<Subject>>> GetSubjectsByTeacher(int teacherId)
    {
        var subjects = await _context.TeacherSubjects
            .Where(ts => ts.TeacherId == teacherId)
            .Include(ts => ts.Subject) // Eager load Subject
            .Select(ts => ts.Subject)  // Select only the Subject
            .ToListAsync();

        if (!subjects.Any())
        {
            return NotFound($"No subjects found for the teacher with ID {teacherId}.");
        }

        return Ok(subjects);
    }

    // POST: api/TeacherSubject
    [HttpPost]
    public async Task<ActionResult<TeacherSubject>> AddSubjectToTeacher(TeacherSubjectDto teacherSubjectDto)
    {
        var teacherExists = await _context.Teachers.AnyAsync(t => t.TeacherId == teacherSubjectDto.TeacherId);
        var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == teacherSubjectDto.SubjectId);

        if (!teacherExists)
        {
            return BadRequest("The specified Teacher ID does not exist.");
        }

        if (!subjectExists)
        {
            return BadRequest("The specified Subject ID does not exist.");
        }

        var teacherSubject = new TeacherSubject
        {
            TeacherId = teacherSubjectDto.TeacherId,
            SubjectId = teacherSubjectDto.SubjectId
        };

        _context.TeacherSubjects.Add(teacherSubject);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSubjectsByTeacher), new { teacherId = teacherSubject.TeacherId }, teacherSubject);
    }

    // DELETE: api/TeacherSubject/{teacherId}/{subjectId}
    [HttpDelete("{teacherId}/{subjectId}")]
    public async Task<IActionResult> RemoveSubjectFromTeacher(int teacherId, int subjectId)
    {
        var teacherSubject = await _context.TeacherSubjects
            .FirstOrDefaultAsync(ts => ts.TeacherId == teacherId && ts.SubjectId == subjectId);

        if (teacherSubject == null)
        {
            return NotFound($"No association found between teacher ID {teacherId} and subject ID {subjectId}.");
        }

        _context.TeacherSubjects.Remove(teacherSubject);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/TeacherSubject/{teacherId}/{subjectId}
    [HttpPut("{teacherId}/{subjectId}")]
    public async Task<IActionResult> UpdateSubjectForTeacher(int teacherId, int subjectId, TeacherSubjectDto updatedTeacherSubjectDto)
    {
        if (teacherId != updatedTeacherSubjectDto.TeacherId || subjectId != updatedTeacherSubjectDto.SubjectId)
        {
            return BadRequest("The Teacher ID or Subject ID in the URL does not match the provided data.");
        }

        var teacherSubject = await _context.TeacherSubjects
            .FirstOrDefaultAsync(ts => ts.TeacherId == teacherId && ts.SubjectId == subjectId);

        if (teacherSubject == null)
        {
            return NotFound($"No association found between teacher ID {teacherId} and subject ID {subjectId}.");
        }

        teacherSubject.TeacherId = updatedTeacherSubjectDto.TeacherId;
        teacherSubject.SubjectId = updatedTeacherSubjectDto.SubjectId;

        _context.Entry(teacherSubject).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw; // Handle concurrency exception as needed
        }

        return NoContent();
    }
}
