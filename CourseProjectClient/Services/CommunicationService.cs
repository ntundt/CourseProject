using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.Model;
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
        private static readonly string _baseAddress = "https://localhost:7252/api/";
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

        public static async Task<GetTestsResult> GetTests()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"tests");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    GetTestsResult tests = JsonConvert.DeserializeObject<GetTestsResult>(response);
                    return tests;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<List<AttemptInfo>> GetAttemptInfos()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"attempts");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    List<AttemptInfo> attempts = JsonConvert.DeserializeObject<List<AttemptInfo>>(response);
                    return attempts;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<GetStartTestResult> StartTest(int testId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"tests/{testId}/start");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    GetStartTestResult result = JsonConvert.DeserializeObject<GetStartTestResult>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<AttemptInfo> EndTest(int attemptId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"attempts/{attemptId}/end");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    AttemptInfo result = JsonConvert.DeserializeObject<AttemptInfo>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<List<QuestionInfo>> GetAttemptQuestions(int attemptId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"attempts/{attemptId}/questions");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    List<QuestionInfo> result = JsonConvert.DeserializeObject<List<QuestionInfo>>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task SaveAnswer(int attemptId, Question question)
        {
            using (HttpClient client = new HttpClient())
            {
                PutAnswer answer;

                if (question.QuestionType == QuestionType.SingleChoise)
                {
                    answer = new PutAnswer
                    {
                        SelectedOption = question.AnswerOptions.First(x => x.IsChecked).Id
                    };
                } else if (question.QuestionType == QuestionType.MultipleChoise)
                {
                    answer = new PutAnswer
                    {
                        SelectedOptions = question.AnswerOptions.Where(x => x.IsChecked).Select(x => x.Id).ToList().ToArray()
                    };
                } else
                {
                    answer = new PutAnswer
                    {
                        Answer = question.AnswerOptions.Select(x => x.Text).First() ?? ""
                    };
                }

                string json = JsonConvert.SerializeObject(answer);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.PutAsync(_baseAddress + $"attempts/{attemptId}/questions/{question.Index}/answer", content);
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    return;
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
