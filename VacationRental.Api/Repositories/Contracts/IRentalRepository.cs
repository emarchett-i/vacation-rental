using VacationRental.Api.Models.Rental;

namespace VacationRental.Api.Repositories.Contracts
{
    public interface IRentalRepository
    {
        Rental Get(int id);
        int Add(Rental rental);
        void Update(Rental rental);
    }
}
