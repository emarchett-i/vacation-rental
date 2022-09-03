using AutoMapper;
using System;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public class RentalService
    {
        private readonly RentalRepository _rentalRepository;
        private readonly IMapper _mapper;

        public RentalService(RentalRepository rentalRepository,
                             IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        public RentalViewModel Get(int id)
        {
            Rental rental = _rentalRepository.Get(id);

            if (rental == null)
                throw new ApplicationException("Rental not found");

            return _mapper.Map<RentalViewModel>(rental);
        }

        public bool Exists(int id)
        {
            Rental rental = _rentalRepository.Get(id);

            return rental != null;
        }

        public ResourceIdViewModel Create(RentalBindingModel rentalCreateRequest)
        {
            Rental rental = _mapper.Map<Rental>(rentalCreateRequest);

            int rentalId = _rentalRepository.Add(rental);

            return new ResourceIdViewModel { Id = rentalId };
        }
    }
}
