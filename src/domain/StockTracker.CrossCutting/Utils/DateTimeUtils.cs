using System.Globalization;

namespace StockTracker.CrossCutting.Utils;

public static class DateTimeUtils
{
    public static DateTime[] GetDatesInRange(string dateFrom, string dateTo)
    {
        var result = new List<DateTime>();
        var targetDays = new[]
            {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday};

        DateTime dateTimeFrom = DateTime.Parse(dateFrom);
        DateTime dateTimeTo = DateTime.Parse(dateTo);

        for (var i = dateTimeFrom; i <= dateTimeTo; i = i.AddDays(1))
        {
            if (targetDays.Contains(i.DayOfWeek))
                result.Add(i);
        }

        return result.ToArray();
    }

    public static string ToRowKeyFormat(this DateTime source)
    {
        return source.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    public static bool ReadyToKpiCalculation(this DateTime source)
    {
        var targetDays = new[]
            {DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday};

        return targetDays.Contains(source.DayOfWeek);
    }

    public static DateTime[] GetPreviousWeek(this DateTime source)
    {
        return GetDatesInRange(source.AddDays(-8).ToRowKeyFormat(), source.AddDays(-1).ToRowKeyFormat());
    }
}