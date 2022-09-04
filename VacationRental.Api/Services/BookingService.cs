using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories;
using VacationRental.Api.Utils;

namespace VacationRental.Api.Services
{
    public class BookingService
    {
        private readonly BookingRepository _bookingRepository;
        private readonly RentalRepository _rentalRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;

        public BookingService(BookingRepository bookingRepository,
                              RentalRepository rentalRepository,
                              IMapper mapper,
                              ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets a Booking by its specified Id.
        /// </summary>
        /// <param name="id">The booking Id</param>
        /// <returns>The requested Booking</returns>
        /// <exception cref="ApplicationException"></exception>
        public BookingViewModel Get(int id)
        {
            try
            {
                Booking booking = _bookingRepository.Get(id);

                if (booking == null)
                    throw new ApplicationException("Booking not found");

                return _mapper.Map<BookingViewModel>(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to get Booking");

                throw ex;
            }
        }

        /// <summary>
        /// Creates a booking if rental is available for the specified time.
        /// </summary>
        /// <param name="createBookingRequest">Required booking data</param>
        /// <returns>The created resource Id</returns>
        /// <exception cref="ApplicationException"></exception>
        public ResourceIdViewModel Create(BookingBindingModel createBookingRequest)
        {
            try
            {
                bool isRentalAvailable = IsRentalAvailable(createBookingRequest.RentalId,
                                                               createBookingRequest.Start.Date,
                                                               createBookingRequest.EndDate,
                                                               out int? nextAvailableUnit);

                if (!isRentalAvailable)
                    throw new ApplicationException("Rental not available");

                Booking booking = _mapper.Map<Booking>(createBookingRequest);

                booking.Unit = nextAvailableUnit.Value;

                int bookingId = _bookingRepository.Add(booking);

                return new ResourceIdViewModel { Id = bookingId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to create Booking");

                throw ex;
            }
        }

        public IEnumerable<Booking> GetByRentalId(int rentalId)
        {
            return _bookingRepository.GetAll().Where(x => x.RentalId == rentalId);
        }

        /// <summary>
        /// Verifies if the given rental is available for the specified time.
        /// </summary>
        /// <param name="rentalId">The rental Id.</param>
        /// <param name="startDate">The time range start date.</param>
        /// <param name="endDate">The time range end date.</param>
        /// <param name="nextAvailableUnit">output variable indicating which is the next available unit</param>
        /// <returns>True if the rental is available, otherwise false.</returns>
        public bool IsRentalAvailable(int rentalId, DateTime startDate, DateTime endDate, out int? nextAvailableUnit)
        {
            try
            {
                nextAvailableUnit = null;

                Rental requestedRental = _rentalRepository.Get(rentalId);

                int occupiedRentals = _bookingRepository.GetAll()
                    .Where(r => r.RentalId == rentalId)
                    .Count(booking => startDate.IsBetween(booking.StartDate, booking.EndDate.AddDays(requestedRental.PreparationTimeInDays))
                                   || endDate.IsBetween(booking.StartDate, booking.EndDate.AddDays(requestedRental.PreparationTimeInDays))
                                   || (booking.StartDate > startDate && booking.EndDate.AddDays(requestedRental.PreparationTimeInDays) < endDate));

                bool isRentalAvailable = requestedRental.Units > occupiedRentals;

                if (isRentalAvailable)
                    nextAvailableUnit = occupiedRentals + 1;

                return isRentalAvailable;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was an error trying to calculate Rental Availability");

                throw ex;
            }
        }
    }
}
