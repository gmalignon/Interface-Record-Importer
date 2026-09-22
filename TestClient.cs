using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceRecordImporter
{
    public class TestClient
    {

        private static readonly HttpClient client = new HttpClient();
        public  async Task<dynamic> GetPNRList()
        {
            dynamic response;

            client.DefaultRequestHeaders.Accept.Clear();

            //client.BaseAddress = new Uri("https://taas.smartagent.nyc");
            //client.BaseAddress = new Uri("https://localhost:5000");
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("ApiKey", "8afe4c68-096b-4b63-8b53-ba27c8dd079d");
            //client.DefaultRequestHeaders.Add("Client_ID", "2");
            //client.DefaultRequestHeaders.Add("API_Type", "8");
            // Exito
            client.BaseAddress = new Uri("https://taas.exitotravel.com");
            client.DefaultRequestHeaders.Add("ApiKey", "54f48b37-60ef-4e4f-a6f7-07edff21a4eb");
            client.DefaultRequestHeaders.Add("Client_ID", "19");
            client.DefaultRequestHeaders.Add("API_Type", "8");
            

            string Query = "api/ThirdParty/getPNRList?date=&time=&id=0&gdsid=0&recordlocator=CI7OC5";
            //string MyResponse = null;
            response = null;
            try
            {
                //HttpResponseMessage response2 = await client.GetAsync(Query).ConfigureAwait(false);
                 HttpResponseMessage tresponse =  await client.GetAsync(Query).ConfigureAwait(false);

                if (!tresponse.IsSuccessStatusCode)
                {
                    return false;
                }
                //response = await tresponse.Content.ReadAsAsync<Object>();
                response = await tresponse.Content.ReadAsStringAsync();

                //response =await JsonConvert.DeserializeObject<Object>( await streamtask);
            }
            catch (Exception ex)
            {

            }
           
            return response;
        }

        public async Task<dynamic> GetCommission()
        {
            dynamic response;

            client.DefaultRequestHeaders.Accept.Clear();
            client.BaseAddress = new Uri("https://taas.exitotravel.com");
            //client.BaseAddress = new Uri("https://taas.smartagent.nyc");
            //client.BaseAddress = new Uri("https://localhost:5000");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("ApiKey", "bed56061-5d8a-4186-b77c-ede1719fbe34");
            client.DefaultRequestHeaders.Add("Client_ID", "19");

            string Query = "api/ThirdParty/getCommission?request={\"airlinecode\":\"AA\",\"farecalculation\":\"15AUG22NYC AA SDQ146.00AA NYC146.00USD292.00END\",\"segments\":[{\"legindex\":0,\"index\":0,\"type\":\"O\",\"departureairport\":\"JFK\",\"departuredate\":\"AUG15\",\"destinationairport\":\"SDQ\",\"marketingcarrier\":\"AA\",\"operatingcarrier\":\"AA\",\"bookingclass\":\"S\",\"farebasis\":\"SNN5MQM1\",\"ticketdesignator\":\"\",\"fare\":0},{\"legindex\":0,\"index\":0,\"type\":\"O\",\"departureairport\":\"SDQ\",\"departuredate\":\"AUG25\",\"destinationairport\":\"JFK\",\"marketingcarrier\":\"AA\",\"operatingcarrier\":\"AA\",\"bookingclass\":\"S\",\"farebasis\":\"SNN5MQM1\",\"ticketdesignator\":\"\",\"fare\":0}]}";
            //string MyResponse = null;
            response = null;
            try
            {
                //HttpResponseMessage response2 = await client.GetAsync(Query).ConfigureAwait(false);
                HttpResponseMessage tresponse = await client.GetAsync(Query).ConfigureAwait(false);

                if (!tresponse.IsSuccessStatusCode)
                {
                    return false;
                }
                //response = await tresponse.Content.ReadAsAsync<Object>();
                response = await tresponse.Content.ReadAsStringAsync();

                //response =await JsonConvert.DeserializeObject<Object>( await streamtask);
            }
            catch (Exception ex)
            {

            }

            return response;
        }
    }
}







