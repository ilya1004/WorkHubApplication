namespace ChatService.Tests.UnitTests.Extensions;

public static class DateTimeExtensions
{
    public static DateTime TruncateToMilliseconds(this DateTime dateTime)
    {
        return new DateTime(
            dateTime.Year, dateTime.Month, dateTime.Day,
            dateTime.Hour, dateTime.Minute, dateTime.Second,
            dateTime.Millisecond, dateTime.Kind);
    }
}