using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;
using SchoolCalendarSystem.Dto;

[Route("api/[controller]")]
[ApiController]
public class TeacherController : ControllerBase
{
    private readonly SchoolContext _context;

    public TeacherController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/Teacher
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherDto>>> GetTeachers()
    {
        var teachers = await _context.Teachers
            .Select(t => new TeacherDto
            {
                TeacherName = t.TeacherName,
                MaxWeeklyHours = t.MaxWeeklyHours,
                MaxDailyHours = t.MaxDailyHours
            })
            .ToListAsync();

        return Ok(teachers);
    }

    // GET: api/Teacher/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TeacherDto>> GetTeacher(int id)
    {
        var teacher = await _context.Teachers
            .FirstOrDefaultAsync(t => t.TeacherId == id);

        if (teacher == null)
        {
            return NotFound();
        }

        var teacherDto = new TeacherDto
        {
            TeacherName = teacher.TeacherName,
            MaxWeeklyHours = teacher.MaxWeeklyHours,
            MaxDailyHours = teacher.MaxDailyHours
        };

        return Ok(teacherDto);
    }

    // POST: api/Teacher
    [HttpPost]
    public async Task<ActionResult<TeacherDto>> PostTeacher(TeacherDto teacherDto)
    {
        var teacher = new Teacher
        {
            TeacherName = teacherDto.TeacherName,
            MaxWeeklyHours = teacherDto.MaxWeeklyHours,
            MaxDailyHours = teacherDto.MaxDailyHours
        };

        _context.Teachers.Add(teacher);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTeacher), new { id = teacher.TeacherId }, teacherDto);
    }

    // PUT: api/Teacher/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutTeacher(int id, TeacherDto teacherDto)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        teacher.TeacherName = teacherDto.TeacherName;
        teacher.MaxWeeklyHours = teacherDto.MaxWeeklyHours;
        teacher.MaxDailyHours = teacherDto.MaxDailyHours;

        _context.Entry(teacher).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TeacherExists(id))
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

    // DELETE: api/Teacher/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(int id)
    {
        var teacher = await _context.Teachers.FindAsync(id);
        if (teacher == null)
        {
            return NotFound();
        }

        _context.Teachers.Remove(teacher);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TeacherExists(int id)
    {
        return _context.Teachers.Any(e => e.TeacherId == id);
    }
}
