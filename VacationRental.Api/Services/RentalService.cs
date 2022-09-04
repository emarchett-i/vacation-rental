using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories.Contracts;

namespace VacationRental.Api.Services
{
    public class RentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly BookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly ILogger<RentalService> _logger;

        public RentalService(IRentalRepository rentalRepository,
                             BookingService bookingService,
                             IMapper mapper,
                             ILogger<RentalService> logger)
        {
            _rentalRepository = rentalRepository;
            _bookingService = bookingService;
            _mapper = mapper;
            _logger = logger;
        }

        public RentalViewModel Get(int id)
        {
            try
            {
                Rental rental = _rentalRepository.Get(id);

                if (rental == null)
                    throw new ApplicationException("Rental not found");

                return _mapper.Map<RentalViewModel>(rental);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to get the Rental");

                throw ex;
            }
        }

        public bool Exists(int id)
        {
            try
            {
                Rental rental = _rentalRepository.Get(id);

                return rental != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to check if Rental exists");

                throw ex;
            }
        }

        public ResourceIdViewModel Create(RentalBindingModel rentalCreateRequest)
        {
            try
            {
                Rental rental = _mapper.Map<Rental>(rentalCreateRequest);

                int rentalId = _rentalRepository.Add(rental);

                return new ResourceIdViewModel { Id = rentalId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error tryung to create Rental");

                throw ex;
            }
        }

        public void Update(int rentalId, RentalBindingModel updateRequest)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError("There was an error trying to update Rental");

                throw ex;
            }
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
