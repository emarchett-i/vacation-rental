using AutoMapper;
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

        public BookingService(BookingRepository bookingRepository,
                              RentalRepository rentalRepository,
                              IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _rentalRepository = rentalRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a Booking by its specified Id.
        /// </summary>
        /// <param name="id">The booking Id</param>
        /// <returns>The requested Booking</returns>
        /// <exception cref="ApplicationException"></exception>
        public BookingViewModel Get(int id)
        {
            Booking booking = _bookingRepository.Get(id);

            if (booking == null)
                throw new ApplicationException("Booking not found");

            return _mapper.Map<BookingViewModel>(booking);
        }

        /// <summary>
        /// Creates a booking if rental is available for the specified time.
        /// </summary>
        /// <param name="createBookingRequest">Required booking data</param>
        /// <returns>The created resource Id</returns>
        /// <exception cref="ApplicationException"></exception>
        public ResourceIdViewModel Create(BookingBindingModel createBookingRequest)
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

        public IEnumerable<Booking> GetByRentalId(int rentalId)
        {
            return _bookingRepository.GetAll().Where(x => x.RentalId == rentalId);
        }

        public bool IsRentalAvailable(int rentalId, DateTime startDate, DateTime endDate, out int? nextAvailableUnit)
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
    }
}
