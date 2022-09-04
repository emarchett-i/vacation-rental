﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using VacationRental.Api.Models;
using VacationRental.Api.Services;

namespace VacationRental.Api.Controllers
{
    [Route("api/v1/rentals")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly RentalService _rentalService;

        public RentalsController(RentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [HttpGet]
        [Route("{rentalId:int}")]
        public RentalViewModel Get(int rentalId)
        {
            if (rentalId == 0)
                throw new ApplicationException("Rental Id can't be 0");

            return _rentalService.Get(rentalId);
        }

        [HttpPost]
        public ResourceIdViewModel Post(RentalBindingModel model)
        {
            if (model.Units <= 0)
                throw new ApplicationException("Units must be a positive number");
            if (model.PreparationTimeInDays < 0)
                throw new ApplicationException("PreparationTimeInDays can't be a negative number");

            return _rentalService.Create(model);
        }

        [HttpPut]
        [Route("{rentalId:int}")]
        public void Put(int rentalId, RentalBindingModel model)
        {
            if (model.Units <= 0)
                throw new ApplicationException("Units must be a positive number");
            if (model.PreparationTimeInDays < 0)
                throw new ApplicationException("PreparationTimeInDays can't be a negative number");

            _rentalService.Update(rentalId, model);
        }
    }
}
