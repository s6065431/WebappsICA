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
        public VenuesClient()
        {

        }

        public IEnumerable<EventTypesDto> GetEventTypes()
        {
            throw new Exception();
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

        public async Task<IEnumerable<ReservationPostDto>> PostAvailabilities(Event @event)
        {
            

            HttpClient client = new HttpClient
            {
                BaseAddress = new System.Uri("http://localhost:23652/")
            };

            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");


            var reservations = new List<ReservationPostDto>().AsEnumerable();
            {

            };

            HttpResponseMessage response = await client.PostAsJsonAsync("api/reservations", reservations);
            return reservations;
        }



    }
}