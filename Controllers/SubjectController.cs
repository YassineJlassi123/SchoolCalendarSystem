using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;
using SchoolCalendarSystem.Dto;

[Route("api/[controller]")]
[ApiController]
public class SubjectController : ControllerBase
{
    private readonly SchoolContext _context;

    public SubjectController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/Subject
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetSubjects()
    {
        var subjects = await _context.Subjects
            .Select(s => new SubjectDto
            {
                SubjectName = s.SubjectName,
                WeeklyHours = s.WeeklyHours,
                DailyHoursLimit = s.DailyHours
            })
            .ToListAsync();

        return Ok(subjects);
    }

    // GET: api/Subject/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectDto>> GetSubject(int id)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.SubjectId == id);

        if (subject == null)
        {
            return NotFound();
        }

        var subjectDto = new SubjectDto
        {
            SubjectName = subject.SubjectName,
            WeeklyHours = subject.WeeklyHours,
            DailyHoursLimit = subject.DailyHours
        };

        return Ok(subjectDto);
    }

    // POST: api/Subject
    [HttpPost]
    public async Task<ActionResult<SubjectDto>> PostSubject(SubjectDto subjectDto)
    {
        var subject = new Subject
        {
            SubjectName = subjectDto.SubjectName,
            WeeklyHours = subjectDto.WeeklyHours,
            DailyHours = subjectDto.DailyHoursLimit
        };

        _context.Subjects.Add(subject);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetSubject), new { id = subject.SubjectId }, subjectDto);
    }

    // PUT: api/Subject/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutSubject(int id, SubjectDto subjectDto)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            return NotFound();
        }

        subject.SubjectName = subjectDto.SubjectName;
        subject.WeeklyHours = subjectDto.WeeklyHours;
        subject.DailyHours = subjectDto.DailyHoursLimit;

        _context.Entry(subject).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!SubjectExists(id))
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

    // DELETE: api/Subject/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var subject = await _context.Subjects.FindAsync(id);
        if (subject == null)
        {
            return NotFound();
        }

        _context.Subjects.Remove(subject);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool SubjectExists(int id)
    {
        return _context.Subjects.Any(e => e.SubjectId == id);
    }
}
