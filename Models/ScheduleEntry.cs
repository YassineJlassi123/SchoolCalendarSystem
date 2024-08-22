namespace SchoolCalendarSystem.Models
{
    public class ScheduleEntry
    {
        public int ScheduleEntryId { get; set; }

        public DayOfWeek Day { get; set; } // Day of the week
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // Relationships
        public int ClassId { get; set; }
        public Class Class { get; set; }

        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public string TimeInterval
        {
            get
            {
                return $"{FormatTime(StartTime)} - {FormatTime(EndTime)}";
            }
        }

        // Helper method to format TimeSpan as "10 AM"
        private string FormatTime(TimeSpan time)
        {
            int hours = time.Hours;
            int minutes = time.Minutes;
            string period = hours >= 12 ? "PM" : "AM";
            hours = hours % 12;
            hours = hours == 0 ? 12 : hours; // Convert 0 hours to 12 for AM
            return $"{hours}:{minutes:D2} {period}";
        }
    }

}
