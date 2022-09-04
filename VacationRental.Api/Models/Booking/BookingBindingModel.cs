using System;
using System.Text.Json.Serialization;

namespace VacationRental.Api.Models
{
    public class BookingBindingModel
    {
        public int RentalId { get; set; }

        public DateTime Start
        {
            get => _startIgnoreTime;
            set => _startIgnoreTime = value.Date;
        }

        private DateTime _startIgnoreTime;
        public int Nights { get; set; }

        [JsonIgnore]
        public DateTime EndDate
        {
            get
            {
                return Start.AddDays(Nights - 1);
            }
        }
    }
}
