using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories;

namespace VacationRental.Api.Services
{
    public class RentalService
    {
        private readonly RentalRepository _rentalRepository;
        private readonly BookingService _bookingService;
        private readonly IMapper _mapper;

        public RentalService(RentalRepository rentalRepository,
                             BookingService bookingService,
                             IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _bookingService = bookingService;
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

        public void Update(int rentalId, RentalBindingModel updateRequest)
        {
            Rental rental = _rentalRepository.Get(rentalId);
            if (rental == null)
                throw new ApplicationException("Rental not found");

            IEnumerable<Booking> rentalBookings = _bookingService.GetByRentalId(rentalId);

            ValidateUnitsCanChange(updateRequest, rentalBookings);

            ValidatePreparationTimeInDaysCanChange(updateRequest, rental, rentalBookings);

            rental.Units = updateRequest.Units;
            rental.PreparationTimeInDays = updateRequest.PreparationTimeInDays;

            _rentalRepository.Update(rental);
        }

        private void ValidateUnitsCanChange(RentalBindingModel updateRequest, IEnumerable<Booking> rentalBookings)
        {
            if (updateRequest.Units < rentalBookings.Select(x => x.Unit).Distinct().Count())
                throw new ApplicationException("Can't decrease the number of units because they are already reserved");
        }

        private void ValidatePreparationTimeInDaysCanChange(RentalBindingModel updateRequest, Rental rental, IEnumerable<Booking> rentalBookings)
        {
            if (updateRequest.PreparationTimeInDays > rental.PreparationTimeInDays)
            {
                foreach (Booking booking in rentalBookings)
                {
                    DateTime startDate = booking.EndDate.AddDays(rental.PreparationTimeInDays + 1);
                    bool isRentalAvailable = _bookingService.IsRentalAvailable(rental.Id, startDate, startDate.AddDays(updateRequest.PreparationTimeInDays - rental.PreparationTimeInDays - 1), out int? nextAvailableUnit);

                    if (!isRentalAvailable)
                        throw new ApplicationException("Can't increase the preparationTimeInDays because there is no availability");
                }
            }
        }
    }
}
