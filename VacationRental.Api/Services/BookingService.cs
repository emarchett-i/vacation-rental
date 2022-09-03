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

        public BookingViewModel Get(int id)
        {
            Booking booking = _bookingRepository.Get(id);

            if (booking == null)
                throw new ApplicationException("Booking not found");

            return _mapper.Map<BookingViewModel>(booking);
        }

        public ResourceIdViewModel Create(BookingBindingModel createBookingRequest)
        {

            bool isRentalAvailable = IsRentalAvailable(createBookingRequest.RentalId, 
                                                       createBookingRequest.Start.Date, 
                                                       createBookingRequest.EndDate);
            
            if (!isRentalAvailable)
                throw new ApplicationException("Rental not available");
            
            int bookingId = _bookingRepository.Add(_mapper.Map<Booking>(createBookingRequest));

            return new ResourceIdViewModel { Id = bookingId };
        }

        private bool IsRentalAvailable(int rentalId, DateTime startDate, DateTime endDate)
        {
            int occupiedRentals = _bookingRepository.GetAll()
                .Where(r => r.RentalId == rentalId)
                .Count(booking => startDate.IsBetween(booking.StartDate, booking.EndDate)
                               || endDate.IsBetween(booking.StartDate, booking.EndDate)
                               || (booking.StartDate > startDate && booking.EndDate < endDate));

            Rental requestedRental = _rentalRepository.Get(rentalId);

            return requestedRental.Units > occupiedRentals;
        }
    }
}
