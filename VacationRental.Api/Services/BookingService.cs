using AutoMapper;
using System;
using System.Collections.Generic;
using VacationRental.Api.Models;
using VacationRental.Api.Models.Booking;
using VacationRental.Api.Models.Rental;
using VacationRental.Api.Repositories;

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
            IEnumerable<Booking> bookings = _bookingRepository.GetAll();

            Rental requestedRental = _rentalRepository.Get(createBookingRequest.RentalId);

            for (var i = 0; i < createBookingRequest.Nights; i++)
            {
                var count = 0;
                foreach (var booking in bookings)
                {
                    if (booking.RentalId == createBookingRequest.RentalId
                        && (booking.StartDateTime <= createBookingRequest.Start.Date && booking.StartDateTime.AddDays(booking.NumberOfNights) > createBookingRequest.Start.Date)
                        || (booking.StartDateTime < createBookingRequest.Start.AddDays(createBookingRequest.Nights) && booking.StartDateTime.AddDays(booking.NumberOfNights) >= createBookingRequest.Start.AddDays(createBookingRequest.Nights))
                        || (booking.StartDateTime > createBookingRequest.Start && booking.StartDateTime.AddDays(booking.NumberOfNights) < createBookingRequest.Start.AddDays(createBookingRequest.Nights)))
                    {
                        count++;
                    }
                }
                if (count >= requestedRental.Units)
                    throw new ApplicationException("Not available");
            }

            int bookingId = _bookingRepository.Add(_mapper.Map<Booking>(createBookingRequest));

            return new ResourceIdViewModel { Id = bookingId };
        }
    }
}
