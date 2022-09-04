using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Api.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class RentalTests
    {
        private readonly HttpClient _client;

        public RentalTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
            }
        }

        [Fact]
        public async Task GivenAnExistingRental_WhenPutRental_ThenAGetReturnsTheUpdatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRequest = new RentalBindingModel
            {
                Units = 50
            };

            ResourceIdViewModel putResult;
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postResult.Id}", putRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
                putResult = await putResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"/api/v1/rentals/{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(putRequest.Units, getResult.Units);
            }
        }

        [Fact]
        public async Task GivenAnExistingRentalWithBookings_WhenPutRental_DecreasingPreparationTimeInDays_ShouldUpdateRentalCorrectly()
        {
            var request = new RentalBindingModel { Units = 1, PreparationTimeInDays = 3 };

            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var bookingRequest = new BookingBindingModel { Start = DateTime.Now.Date, Nights = 1, RentalId = postRentalResult.Id };
            ResourceIdViewModel postBookingResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", bookingRequest))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postBookingResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var putRentalRequest = new RentalBindingModel { Units = 1, PreparationTimeInDays = 2 };
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRentalRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
            }
        }

        [Fact]
        public async Task GivenAnExistingRentalWithBookings_WhenPutRental_DecreasingUnits_AndAlreadyReserved_ShouldThrowAnError()
        {
            var request = new RentalBindingModel { Units = 2, PreparationTimeInDays = 1 };

            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var booking1Request = new BookingBindingModel { Start = DateTime.Now.Date, Nights = 1, RentalId = postRentalResult.Id };
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking1Request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
            }

            var booking2Request = new BookingBindingModel { Start = DateTime.Now.Date, Nights = 1, RentalId = postRentalResult.Id };
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking2Request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
            }

            var putRentalRequest = new RentalBindingModel { Units = 1, PreparationTimeInDays = 1 };

            await Assert.ThrowsAsync<ApplicationException>(async () => await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRentalRequest));
        }

        [Fact]
        public async Task GivenAnExistingRentalWithBookings_WhenPutRental_IncreasingPreparationTimeInDays_ShouldUpdateRentalCorrectly()
        {
            var request = new RentalBindingModel { Units = 1, PreparationTimeInDays = 1 };

            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var booking1Request = new BookingBindingModel { Start = DateTime.Now.Date, Nights = 1, RentalId = postRentalResult.Id };
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking1Request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
            }

            var booking2Request = new BookingBindingModel { Start = DateTime.Now.AddDays(3).Date, Nights = 1, RentalId = postRentalResult.Id };
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking2Request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
            }

            var putRentalRequest = new RentalBindingModel { Units = 1, PreparationTimeInDays = 2 };
            using (var putResponse = await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRentalRequest))
            {
                Assert.True(putResponse.IsSuccessStatusCode);
            }
        }

        [Fact]
        public async Task GivenAnExistingRentalWithBookings_WhenPutRental_IncreasingPreparationTimeInDays_WithNoAvailability_ShouldThrowAnError()
        {
            var request = new RentalBindingModel { Units = 1, PreparationTimeInDays = 1 };

            ResourceIdViewModel postRentalResult;
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/rentals", request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postRentalResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var booking1Request = new BookingBindingModel { Start = DateTime.Now.Date, Nights = 1, RentalId = postRentalResult.Id };
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking1Request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
            }

            var booking2Request = new BookingBindingModel { Start = DateTime.Now.AddDays(2).Date, Nights = 1, RentalId = postRentalResult.Id };
            using (var postResponse = await _client.PostAsJsonAsync($"/api/v1/bookings", booking2Request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
            }

            var putRentalRequest = new RentalBindingModel { Units = 1, PreparationTimeInDays = 2 };

            await Assert.ThrowsAsync<ApplicationException>(async () => await _client.PutAsJsonAsync($"/api/v1/rentals/{postRentalResult.Id}", putRentalRequest));
        }
    }
}
