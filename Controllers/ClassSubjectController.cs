using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;
using SchoolCalendarSystem.Dto;

[Route("api/[controller]")]
[ApiController]
public class ClassSubjectController : ControllerBase
{
    private readonly SchoolContext _context;

    public ClassSubjectController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/ClassSubject/{classId}
    [HttpGet("{classId}")]
    public async Task<ActionResult<IEnumerable<Subject>>> GetSubjectsByClass(int classId)
    {
        var subjects = await _context.ClassSubjects
            .Where(cs => cs.ClassId == classId)
            .Include(cs => cs.Subject) // Eager load Subject
            .Select(cs => cs.Subject)  // Select only the Subject
            .ToListAsync();

        if (!subjects.Any())
        {
            return NotFound($"No subjects found for the class with ID {classId}.");
        }

        return Ok(subjects);
    }

    // POST: api/ClassSubject
    [HttpPost]
    public async Task<ActionResult<ClassSubject>> AddSubjectToClass(ClassSubjectDto classSubjectDto)
    {
        var classExists = await _context.Classes.AnyAsync(c => c.ClassId == classSubjectDto.ClassId);
        var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == classSubjectDto.SubjectId);

        if (!classExists)
        {
            return BadRequest("The specified Class ID does not exist.");
        }

        if (!subjectExists)
        {
            return BadRequest("The specified Subject ID does not exist.");
        }

        var classSubject = new ClassSubject
        {
            ClassId = classSubjectDto.ClassId,
            SubjectId = classSubjectDto.SubjectId
        };

        _context.ClassSubjects.Add(classSubject);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSubjectsByClass), new { classId = classSubject.ClassId }, classSubject);
    }

    // DELETE: api/ClassSubject/{classId}/{subjectId}
    [HttpDelete("{classId}/{subjectId}")]
    public async Task<IActionResult> RemoveSubjectFromClass(int classId, int subjectId)
    {
        var classSubject = await _context.ClassSubjects
            .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId);

        if (classSubject == null)
        {
            return NotFound($"No association found between class ID {classId} and subject ID {subjectId}.");
        }

        _context.ClassSubjects.Remove(classSubject);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/ClassSubject/{classId}/{subjectId}
    [HttpPut("{classId}/{subjectId}")]
    public async Task<IActionResult> UpdateSubjectInClass(int classId, int subjectId, ClassSubjectDto updatedClassSubjectDto)
    {
        if (classId != updatedClassSubjectDto.ClassId || subjectId != updatedClassSubjectDto.SubjectId)
        {
            return BadRequest("The Class ID or Subject ID in the URL does not match the provided data.");
        }

        var classSubject = await _context.ClassSubjects
            .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.SubjectId == subjectId);

        if (classSubject == null)
        {
            return NotFound($"No association found between class ID {classId} and subject ID {subjectId}.");
        }

        classSubject.ClassId = updatedClassSubjectDto.ClassId;
        classSubject.SubjectId = updatedClassSubjectDto.SubjectId;

        _context.Entry(classSubject).State = EntityState.Modified;

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
