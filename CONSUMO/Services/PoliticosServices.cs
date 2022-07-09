using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CONSUMO.Services
{
    public class PoliticosServices
    {
        protected readonly string _baseAddress;
        public PoliticosServices(string address)
        {
            _baseAddress = "https://localhost:5001/api/" + address;
        }        

        public async Task<string> GetTodos()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_baseAddress);
            var response = await client.GetAsync("");
                   
            string content = await response.Content.ReadAsStringAsync();
            
            return (content);              
        }

        public async Task<string> GetPorId(int id)
        {
            var client = new HttpClient();
            var response = await client.GetAsync($"{_baseAddress}/{id}");
                   
            var content = response.Content.ReadAsStringAsync().Result;

            return (content);         
        }        
    }
}