using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataTransferObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CourseProjectClient.Services
{
    internal class AuthenticationProvider
    {
        private static AuthenticationProvider _instance;
        public static AuthenticationProvider GetInstance()
        {
            if (_instance == null)
            {
                return _instance = new AuthenticationProvider();
            }
            return _instance;
        }

        [JsonProperty]
        private string _accessToken;
        [JsonIgnore]
        public string AccessToken { get => _accessToken; }
        
        [JsonProperty]
        private int _userId;
        [JsonIgnore]
        public int UserId { get => _userId; }
        
        [JsonProperty]
        private string _name;
        [JsonIgnore]
        public string Name { get => _name; }

        [JsonProperty]
        private DateTime _created;
        [JsonIgnore]
        public DateTime Created { get => _created; }

        [JsonProperty]
        private DateTime _loggedIn;
        [JsonIgnore]
        public DateTime LoggedIn { get => _loggedIn; }

        [JsonIgnore]
        public bool Available { get => _accessToken != null; }

        public AuthenticationProvider()
        {
            _fileReadFailed = false;
        }

        [JsonIgnore]
        private bool _fileReadFailed = false;
        [JsonIgnore]
        public bool FileReadFailed => _fileReadFailed;

        public void RetrieveFromServer()
        {
            throw new NotImplementedException();
        }

        public void Apply(AuthResult authResult)
        {
            _accessToken = authResult.AccessToken;
            _name = authResult.Name;
            _created = DateTimeOffset.FromUnixTimeSeconds(authResult.CreatedDate).DateTime;
            _loggedIn = DateTime.Now;
            SaveToFile();
        }

        public bool ReadFromFile()
        {
            try
            {
                FileInfo auth = new FileInfo("auth.json");
                using (StreamReader reader = new StreamReader(auth.Open(FileMode.Open)))
                {
                    string json = reader.ReadToEnd();
                    AuthenticationProvider provider = JsonConvert.DeserializeObject<AuthenticationProvider>(json);

                    _accessToken = provider._accessToken;
                    _userId = provider._userId;
                    _created = provider._created;
                    _loggedIn = provider._loggedIn;
                    _name = provider._name;
                }
                return true;
            } catch (Exception)
            {
                _fileReadFailed = true;
                return false;
            }
        }

        private void SaveToFile()
        {
            FileInfo auth = new FileInfo("auth.json");
            byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(this, Formatting.Indented));
            using (FileStream writer = auth.Open(FileMode.Create))
            {
                writer.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
