using Microsoft.AspNetCore.Mvc;
using SchoolCalendarSystem.Models;
using System.Collections.Generic;
using SchoolCalendarSystem.Dto;

namespace SchoolCalendarSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleEntryService _scheduleEntryService;

        public ScheduleController(ScheduleEntryService scheduleEntryService)
        {
            _scheduleEntryService = scheduleEntryService;
        }

        // POST: api/Schedule/generate
        [HttpPost("generate")]
        public ActionResult<List<ScheduleEntry>> GenerateWeeklySchedule([FromBody] ClassDto className)
        {
            if (string.IsNullOrEmpty(className.ClassName))
            {
                return BadRequest("Class name is required.");
            }

            try
            {
                var scheduleEntries = _scheduleEntryService.GenerateWeeklyScheduleForClass(className.ClassName);
                return Ok(scheduleEntries);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}