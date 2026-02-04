namespace TamilHoroscope.Core.Utilities;

/// <summary>
/// Utilities for Julian Day calculations
/// </summary>
public static class JulianDay
{
    /// <summary>
    /// Convert DateTime to Julian Day Number
    /// </summary>
    /// <param name="dateTime">DateTime in local time</param>
    /// <param name="timeZoneOffset">Time zone offset from UTC in hours</param>
    /// <returns>Julian Day Number</returns>
    public static double ToJulianDay(DateTime dateTime, double timeZoneOffset)
    {
        // Convert to UTC
        var utcDateTime = dateTime.AddHours(-timeZoneOffset);
        
        int year = utcDateTime.Year;
        int month = utcDateTime.Month;
        int day = utcDateTime.Day;
        double hour = utcDateTime.Hour + utcDateTime.Minute / 60.0 + utcDateTime.Second / 3600.0;

        // Adjust for January and February
        if (month <= 2)
        {
            year--;
            month += 12;
        }

        // Calculate Julian Day
        int a = year / 100;
        int b = 2 - a + (a / 4);
        
        double jd = Math.Floor(365.25 * (year + 4716)) +
                    Math.Floor(30.6001 * (month + 1)) +
                    day + hour / 24.0 + b - 1524.5;

        return jd;
    }

    /// <summary>
    /// Convert Julian Day Number to DateTime
    /// </summary>
    /// <param name="julianDay">Julian Day Number</param>
    /// <returns>DateTime in UTC</returns>
    public static DateTime FromJulianDay(double julianDay)
    {
        double jd = julianDay + 0.5;
        int z = (int)Math.Floor(jd);
        double f = jd - z;

        int a;
        if (z >= 2299161)
        {
            int alpha = (int)Math.Floor((z - 1867216.25) / 36524.25);
            a = z + 1 + alpha - (alpha / 4);
        }
        else
        {
            a = z;
        }

        int b = a + 1524;
        int c = (int)Math.Floor((b - 122.1) / 365.25);
        int d = (int)Math.Floor(365.25 * c);
        int e = (int)Math.Floor((b - d) / 30.6001);

        int day = b - d - (int)Math.Floor(30.6001 * e);
        int month = e < 14 ? e - 1 : e - 13;
        int year = month > 2 ? c - 4716 : c - 4715;

        double hours = f * 24.0;
        int hour = (int)Math.Floor(hours);
        double minutes = (hours - hour) * 60.0;
        int minute = (int)Math.Floor(minutes);
        double seconds = (minutes - minute) * 60.0;
        int second = (int)Math.Floor(seconds);

        return new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
    }
}
