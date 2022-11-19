using System;

namespace ToDo_App.Helpers
{
    public static class DateTimeHelpers
    {
        public static TimeSpan ConvertUtcTo24HrTime(int utcTime)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(utcTime).ToLocalTime();
            return dateTime.TimeOfDay;
        }
    }
}