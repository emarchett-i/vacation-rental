using System;

namespace VacationRental.Api.Utils
{
    public static class DateTimeExtensions
    {
        public static bool IsBetween(this DateTime dateBetween, DateTime date1, DateTime date2)
        {
            return date1 <= dateBetween && dateBetween <= date2;
        }
    }
}
