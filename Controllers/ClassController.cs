using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCalendarSystem.Models;
using SchoolCalendarSystem.Dto;

[Route("api/[controller]")]
[ApiController]
public class ClassController : ControllerBase
{
    private readonly SchoolContext _context;

    public ClassController(SchoolContext context)
    {
        _context = context;
    }

    // GET: api/Class
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClassDto>>> GetClasses()
    {
        var classes = await _context.Classes
            .Include(c => c.Students)
            .Select(c => new ClassDto
            {
                ClassName = c.ClassName
            })
            .ToListAsync();

        return Ok(classes);
    }

    // GET: api/Class/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ClassDto>> GetClass(int id)
    {
        var @class = await _context.Classes
            .Include(c => c.Students)
            .FirstOrDefaultAsync(c => c.ClassId == id);

        if (@class == null)
        {
            return NotFound();
        }

        var classDto = new ClassDto
        {
            ClassName = @class.ClassName
        };

        return Ok(classDto);
    }

    // POST: api/Class
    [HttpPost]
    public async Task<ActionResult<ClassDto>> CreateClass(ClassDto classDto)
    {
        var @class = new Class
        {
            ClassName = classDto.ClassName
        };

        _context.Classes.Add(@class);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetClass), new { id = @class.ClassId }, classDto);
    }

    // PUT: api/Class/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateClass(int id, ClassDto classDto)
    {
        var @class = await _context.Classes.FindAsync(id);

        if (@class == null)
        {
            return NotFound();
        }

        @class.ClassName = classDto.ClassName;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ClassExists(id))
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

    // DELETE: api/Class/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteClass(int id)
    {
        var @class = await _context.Classes.FindAsync(id);
        if (@class == null)
        {
            return NotFound();
        }

        _context.Classes.Remove(@class);
        await _context.SaveChangesAsync();

        return NoContent();
    }

 

    private bool ClassExists(int id)
    {
        return _context.Classes.Any(e => e.ClassId == id);
    }
}
