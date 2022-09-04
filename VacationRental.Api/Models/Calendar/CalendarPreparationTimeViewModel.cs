namespace VacationRental.Api.Models.Calendar
{
    public class CalendarPreparationTimeViewModel
    {
        public CalendarPreparationTimeViewModel(int unit)
        {
            Unit = unit;
        }

        public int Unit { get; private set; }
    }
}
