﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        public IEnumerable<AvailabilityDto> GetAvailablities(string eventType)
        {
            var venues = new List<AvailabilityDto>().AsEnumerable();

            HttpClient client = new HttpClient
            {
                BaseAddress = new System.Uri("http://localhost:23652/")
            };
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            var url = $"api/Availability?eventType={eventType}&beginDate=2018-11-20&endDate=2018-11-30";
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

    }
}
