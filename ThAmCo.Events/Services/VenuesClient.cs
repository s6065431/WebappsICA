using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ThAmCo.Events.Data;
using ThAmCo.Events.Models;

namespace ThAmCo.Events.Services
{
    public class VenuesClient
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
       
        public VenuesClient(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

        }

        public IEnumerable<EventTypesDto> GetEventTypes()
        {
            throw new Exception();
        }


        public async Task<ReservationGetDto> GetReservation(Event @event)
        {
            string reservationUri = CreateReservationGetString(@event);

            return await HttpGetAsync<ReservationGetDto>(reservationUri);
        }

        public async Task<ReservationGetDto> DeleteReservation(Event @event)
        {
            return await HttpDeleteAsync<ReservationGetDto>("/api/reservations/" + @event.ReservationRef);
        }


        private async Task<T> HttpDeleteAsync<T>(string v)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:23652");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            HttpResponseMessage responseMessage = await client.DeleteAsync(v);
            T response = await responseMessage.Content.ReadAsAsync<T>();

            return response;
        }

        public async Task<ReservationGetDto> AddReservation(ReservationPostDto post)
        {


            return await HttpPostAsync<ReservationPostDto, ReservationGetDto>("/api/reservations/", post);
        }

        public IEnumerable<AvailabilityDto> GetAvailablities(Event @event)
        {
            var venues = new List<AvailabilityDto>().AsEnumerable();

            HttpClient client = new HttpClient
            {
                BaseAddress = new System.Uri("http://localhost:23652/")
            };
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            var url = $"api/Availability?eventType=" + @event.TypeId + "&beginDate=" + @event.Date.ToString("yyyy-MM-dd") + "&endDate=" + @event.Date.Add(@event.Duration.Value).ToString("yyyy-MM-dd");
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                venues = response.Content.ReadAsAsync<IEnumerable<AvailabilityDto>>().Result;
            }

            else
            {
                Debug.WriteLine("Index received a bad response from the web service. ");
                throw new Exception();
            }

            return venues;
        }

        public async Task<ReservationGetDto> GetReservations(Event @event)
        {
            string reservation = CreateReservationGetString(@event);
            return await HttpGetAsync<ReservationGetDto>(reservation);
        }

        private string CreateReservationGetString(Event @event)
        {
            return "/api/reservations/" + @event.ReservationRef;
        }

        public async Task<T> HttpGetAsync<T>(string uri)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:23652");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            HttpResponseMessage responseMessage = await client.GetAsync(uri);
            T response = await responseMessage.Content.ReadAsAsync<T>();

            return response;
        }

        public async Task<B> HttpPostAsync<T, B>(string uri, T content)
        {
            HttpClient client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:23652");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");

            HttpResponseMessage responseMessage = await client.PostAsJsonAsync(uri, content);
            B response = await responseMessage.Content.ReadAsAsync<B>();

            return response;
        }


    }
}