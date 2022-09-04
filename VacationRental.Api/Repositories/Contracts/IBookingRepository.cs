using System.Collections.Generic;
using VacationRental.Api.Models.Booking;

namespace VacationRental.Api.Repositories.Contracts
{
    public interface IBookingRepository
    {
        Booking Get(int id);
        IEnumerable<Booking> GetAll();
        int Add(Booking booking);
    }
}
