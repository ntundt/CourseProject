using CourseProjectClient.Exceptions;
using DataTransferObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CourseProjectClient.Services
{
    internal class CommunicationService
    {
        private static readonly string _baseAddress = "http://localhost:5000/api/";
        public static async Task<AuthResult> GetAuth(string login, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                var serverResponse = await client.GetAsync(_baseAddress + $"auth?login={login}&password={password}");
                string response = await serverResponse.Content.ReadAsStringAsync();

                AuthResult result = JsonConvert.DeserializeObject<AuthResult>(response);
                if (result == null)
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
                return result;
            }
        }

        public static async Task<AuthResult> GetAuth(string name)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(new PostUserAuth
                {
                    Name = name
                }), Encoding.UTF8, "application/json");

                var serverResponse = await client.PostAsync(_baseAddress + $"auth", content);
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    AuthResult result = JsonConvert.DeserializeObject<AuthResult>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }
    }
}
