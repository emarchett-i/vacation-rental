using System;
using System.Collections.Generic;
using VacationRental.Api.Models.Rental;

namespace VacationRental.Api.Repositories
{
    public class RentalRepository
    {
        private readonly IDictionary<int, Rental> _rentals;

        public RentalRepository(IDictionary<int, Rental> rentals)
        {
            _rentals = rentals;
        }

        public Rental Get(int id)
        {
            if (_rentals.ContainsKey(id))
                return _rentals[id];

            return default(Rental);
        }

        public int Add(Rental rental)
        {
            if (rental == null)
                throw new ArgumentNullException("rental");

            rental.Id = GenerateNewId();

            _rentals.Add(rental.Id, rental);

            return rental.Id;
        }

        public void Update(Rental rental)
        {
            if (rental == null)
                throw new ArgumentNullException("rental");

            if (!_rentals.ContainsKey(rental.Id))
                throw new ApplicationException("Rental not found");

            _rentals[rental.Id] = rental;
        }

        private int GenerateNewId()
        {
            return _rentals.Count + 1;
        }
    }
}
