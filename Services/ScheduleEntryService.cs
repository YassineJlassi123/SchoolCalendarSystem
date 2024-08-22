using System;
using System.Collections.Generic;
using System.Linq;
using SchoolCalendarSystem.Models;
using Microsoft.EntityFrameworkCore;

public class ScheduleEntryService
{
    private readonly SchoolContext _context;

    public ScheduleEntryService(SchoolContext context)
    {
        _context = context;
    }

    public List<ScheduleEntry> GenerateWeeklyScheduleForClass(string className)
    {
        // Fetch the class based on the provided class name
        var schoolClass = _context.Classes
            .Include(c => c.ClassSubjects)
            .ThenInclude(cs => cs.Subject)
            .Include(c => c.ScheduleEntries)
            .FirstOrDefault(c => c.ClassName == className);

        if (schoolClass == null)
            throw new Exception($"Class with name {className} not found.");

        // Define the days for the schedule (excluding Sunday)
        var daysOfWeek = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>()
            .Where(d => d != DayOfWeek.Sunday).ToList();

        // Dictionary to track the current time slot for each day
        var dailyStartTime = new Dictionary<DayOfWeek, TimeSpan>();
        foreach (var day in daysOfWeek)
        {
            dailyStartTime[day] = new TimeSpan(8, 0, 0); // Start at 8:00 AM
        }

        // Track remaining hours for each subject
        var remainingHours = schoolClass.ClassSubjects.ToDictionary(
            cs => cs.Subject.SubjectId,
            cs => cs.Subject.WeeklyHours
        );

        // Track assigned teachers for each subject within this class
        var assignedTeachers = new Dictionary<int, Teacher>();

        bool allSubjectsScheduled;
        do
        {
            allSubjectsScheduled = true;

            foreach (var classSubject in schoolClass.ClassSubjects)
            {
                var subject = classSubject.Subject;

                foreach (var day in daysOfWeek)
                {
                    if (remainingHours[subject.SubjectId] <= 0)
                    {
                        continue;
                    }

                    // Calculate end time based on current start time and daily hours
                    var currentTime = dailyStartTime[day];
                    var endTime = currentTime.Add(new TimeSpan(subject.DailyHours, 0, 0));

                    // Ensure we don't schedule beyond a reasonable time (e.g., 4:00 PM)
                    if (endTime > new TimeSpan(16, 0, 0))
                    {
                        continue;
                    }

                    // Check if a teacher has already been assigned to this subject for this class
                    Teacher selectedTeacher;
                    if (assignedTeachers.ContainsKey(subject.SubjectId))
                    {
                        selectedTeacher = assignedTeachers[subject.SubjectId];
                    }
                    else
                    {
                        // Find all teachers who can teach the subject
                        var availableTeachers = _context.TeacherSubjects
                            .Where(ts => ts.SubjectId == subject.SubjectId)
                            .Select(ts => ts.Teacher)
                            .ToList();

                        // Find a suitable teacher with enough remaining weekly hours
                        selectedTeacher = availableTeachers.FirstOrDefault(teacher =>
                        {
                            var scheduledHours = _context.ScheduleEntries
                                .Where(se => se.TeacherId == teacher.TeacherId)
                                .AsEnumerable()
                                .Sum(se => (se.EndTime - se.StartTime).TotalHours);

                            var remainingWeeklyHours = teacher.MaxWeeklyHours - scheduledHours;

                            return remainingWeeklyHours >= remainingHours[subject.SubjectId];
                        });

                        // If no teacher can fully meet the requirement, select the one with the most remaining hours
                        if (selectedTeacher == null)
                        {
                            selectedTeacher = availableTeachers.OrderByDescending(teacher =>
                            {
                                var scheduledHours = _context.ScheduleEntries
                                    .Where(se => se.TeacherId == teacher.TeacherId)
                                    .AsEnumerable()
                                    .Sum(se => (se.EndTime - se.StartTime).TotalHours);

                                return teacher.MaxWeeklyHours - scheduledHours;
                            }).FirstOrDefault();

                            if (selectedTeacher != null)
                            {
                                // Increase the subject's weekly hours to match the selected teacher's remaining hours
                                var teacherScheduleEntries = _context.ScheduleEntries
     .Where(se => se.TeacherId == selectedTeacher.TeacherId)
     .AsEnumerable(); // Load the data into memory for further processing

                                var teacherRemainingHours = selectedTeacher.MaxWeeklyHours -
                                    teacherScheduleEntries.Sum(se => (int)(se.EndTime - se.StartTime).TotalHours);

                                // Reduce the subject's remaining hours by the teacher's remaining hours
                                var assignedHours = Math.Min(remainingHours[subject.SubjectId], teacherRemainingHours);

                                // Decrease the remaining hours for the subject
                                remainingHours[subject.SubjectId] -= assignedHours;

                            }
                        }

                        if (selectedTeacher == null)
                        {
                            continue; // No suitable teacher found, skip this subject for now
                        }

                        // Assign this teacher to the subject for this class
                        assignedTeachers[subject.SubjectId] = selectedTeacher;
                    }

                    // Check if the teacher is already teaching during this time slot and adjust if necessary
                    var existingEntries = _context.ScheduleEntries
                        .Where(se => se.TeacherId == selectedTeacher.TeacherId && se.Day == day)
                        .AsEnumerable();

                    bool slotFound = false;
                    while (!slotFound)
                    {
                        var conflictingEntry = existingEntries
                            .FirstOrDefault(se => (currentTime < se.EndTime && endTime > se.StartTime));  // Overlapping entry

                        if (conflictingEntry != null)
                        {
                            // Move the start time to after the conflicting entry's end time
                            currentTime = conflictingEntry.EndTime;
                            endTime = currentTime.Add(new TimeSpan(subject.DailyHours, 0, 0));

                            // Ensure we don't schedule beyond a reasonable time (e.g., 4:00 PM)
                            if (endTime > new TimeSpan(16, 0, 0))
                            {
                                break; // Cannot schedule this subject on this day
                            }

                            // Update the existing entries
                            existingEntries = _context.ScheduleEntries
                                .Where(se => se.TeacherId == selectedTeacher.TeacherId && se.Day == day)
                                .AsEnumerable();
                        }
                        else
                        {
                            slotFound = true;
                        }
                    }

                    if (!slotFound)
                    {
                        continue; // Skip this slot if no valid time found
                    }

                    // Create the schedule entry
                    var scheduleEntry = new ScheduleEntry
                    {
                        Day = day,
                        StartTime = currentTime,
                        EndTime = endTime,
                        ClassId = schoolClass.ClassId,
                        Class = schoolClass,
                        SubjectId = subject.SubjectId,
                        Subject = subject,
                        TeacherId = selectedTeacher.TeacherId
                    };

                    // Add the schedule entry to the class's schedule
                    schoolClass.ScheduleEntries.Add(scheduleEntry);
                    _context.SaveChanges();

                    // Update the current time slot for that day
                    dailyStartTime[day] = endTime;

                    // Decrease the remaining hours for the subject
                    remainingHours[subject.SubjectId] -= subject.DailyHours;
                }

                // If any subject still has remaining hours, we need to schedule more
                if (remainingHours[subject.SubjectId] > 0)
                {
                    allSubjectsScheduled = false;
                }
            }

        } while (!allSubjectsScheduled);

        // Save the schedule entries to the database
        return schoolClass.ScheduleEntries.ToList();
    }
}
