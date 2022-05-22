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

        public static async Task<GetUserInfo> GetUserInfo()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"auth/info");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    GetUserInfo result = JsonConvert.DeserializeObject<GetUserInfo>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<ActionResult> SaveUserInfo(PutUserInfo info) 
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var content = new StringContent(JsonConvert.SerializeObject(info), Encoding.UTF8, "application/json");

                var serverResponse = await client.PutAsync(_baseAddress + $"auth", content);
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    ActionResult result = JsonConvert.DeserializeObject<ActionResult>(response);
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
                        SelectedOption = question.AnswerOptions.First(x => x.SingleChoiseSelected).Id
                    };
                } else if (question.QuestionType == QuestionType.MultipleChoise)
                {
                    answer = new PutAnswer
                    {
                        SelectedOptions = question.AnswerOptions.Where(x => x.MultipleChoiseSelected).Select(x => x.Id).ToList().ToArray()
                    };
                } else
                {
                    answer = new PutAnswer
                    {
                        Answer = question.StringInputAnswerOption.Text
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

        public static async Task<PostTestResult> CreateTest(TestInfoSetter parameters)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(parameters);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.PostAsync(_baseAddress + $"tests", content);
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    PostTestResult result = JsonConvert.DeserializeObject<PostTestResult>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }
    
        public static async Task<TestInfo> GetTest(int testId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"tests/{testId}");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    TestInfo result = JsonConvert.DeserializeObject<TestInfo>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }
    
        public static async Task DeleteQuestion(int testId, int questionId) {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.DeleteAsync(_baseAddress + $"tests/{testId}/questions/{questionId}");
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
    
        public static async Task<PostQuestionResult> CreateQuestion(int testId) {
            using (HttpClient client = new HttpClient())
            {
                HttpContent content = new StringContent("", Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.PostAsync(_baseAddress + $"tests/{testId}/questions", content);
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    PostQuestionResult result = JsonConvert.DeserializeObject<PostQuestionResult>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }
    
        public static async Task<List<QuestionInfo>> GetTestQuestions(int testId) 
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"tests/{testId}/questions");
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
    
        public static async Task SaveQuestion(int testId, int questionId, PutQuestion question) {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(question);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.PutAsync(_baseAddress + $"tests/{testId}/questions/{questionId}", content);
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

        public static async Task<ResultInfo> GetResultInfo(int attemptId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"attempts/{attemptId}/results");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    ResultInfo result = JsonConvert.DeserializeObject<ResultInfo>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<List<ResultInfo>> GetTestResultsInfo(int testId)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.GetAsync(_baseAddress + $"tests/{testId}/results");
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    List<ResultInfo> result = JsonConvert.DeserializeObject<List<ResultInfo>>(response);
                    return result;
                }
                else
                {
                    ErrorInfo error = JsonConvert.DeserializeObject<ErrorInfo>(response);
                    throw new DefaultException(error.Code, error.Message);
                }
            }
        }

        public static async Task<TestInfo> PutTest(int testId, TestInfoSetter setter)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(setter);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthenticationProvider.GetInstance().AccessToken);
                var serverResponse = await client.PutAsync(_baseAddress + $"tests/{testId}", content);
                string response = await serverResponse.Content.ReadAsStringAsync();

                if (serverResponse.IsSuccessStatusCode)
                {
                    TestInfo result = JsonConvert.DeserializeObject<TestInfo>(response);
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
