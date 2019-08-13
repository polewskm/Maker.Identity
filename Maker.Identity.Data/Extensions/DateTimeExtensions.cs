using System;

namespace Maker.Identity.Data.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
        {
            return new DateTimeOffset(dateTime, offset);
        }

    }
}