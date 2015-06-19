using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ImportApi
{
    public static class ApiManagementAdmin
    {
        #region Fields and Constructors

        private const string ApimAdminRestBaseUrl = "https://sagedatacloud1.management.azure-api.net/";
        private const string ApimAdminRestIdentifier = "54d1252698b1c4046a030003";
        private const string ApimAdminRestPrimaryKey = "rf4pwosUFwVs2s2KLc6X/7Uk1nig5jDdiogSZ3vcBOoUqwIR03ma8FU16ARWQRt39v8TuB3RVIHNseNPYKXUEA==";
        private const string ApimAdminRestApiVersion = "2014-02-14-preview";
        private static readonly DateTime ApimAdminRestAuthorizationExpiry;

        #endregion

        static ApiManagementAdmin()
        {
            // API Management Admin REST authorization expires 1-Jan-2017
            ApimAdminRestAuthorizationExpiry = new DateTime(2017, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        #region Public Static Methods

        public static async Task<string> ImportApiUsingSwaggerDoc(string apiId, string apiSwaggerMetadata, string path)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                SetupHttpClientForApimAdmin(httpClient);

                // create PUT request with body containing API Swagger metadata document
                string requestUri = string.Format("/apis/{0}?api-version={1}&import=true&path={2}", apiId, ApimAdminRestApiVersion, path);
                StringContent requestBody = new StringContent(apiSwaggerMetadata, Encoding.UTF8, "application/vnd.swagger.doc+json");
                HttpResponseMessage response = await httpClient.PutAsync(requestUri, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    // 201 - Created
                    // 400 - Bad Request
                    // 409 - Conflict (API already exists)
                    return string.Format("ImportApiUsingSwaggerDoc() failed: StatusCode = {0}, ReasonPhrase = {1}", response.StatusCode, response.ReasonPhrase);
                }

                return "ImportApiUsingSwaggerDoc() succeeded";
            }
        }

        public static async Task<string> ImportApiUsingSwaggerLink(string apiId, string apiSwaggerLink, string path)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                SetupHttpClientForApimAdmin(httpClient);

                // create PUT request with body containing API Swagger link
                string requestUri = string.Format("/apis/{0}?api-version={1}&import=true&path={2}", apiId, ApimAdminRestApiVersion, path);
                string swaggerLinkJson = string.Format("{{ \"link\" : \"{0}\" }}", apiSwaggerLink);
                StringContent requestBody = new StringContent(swaggerLinkJson, Encoding.UTF8, "application/vnd.swagger.link+json");
                HttpResponseMessage response = await httpClient.PutAsync(requestUri, requestBody);

                if (!response.IsSuccessStatusCode)
                {
                    // 201 - Created
                    // 400 - Bad Request
                    // 409 - Conflict (API already exists)
                    return string.Format("ImportApiUsingSwaggerLink() failed: StatusCode = {0}, ReasonPhrase = {1}", response.StatusCode, response.ReasonPhrase);
                }

                return "ImportApiUsingSwaggerLink() succeeded";
            }
        }

        //public static async Task<string> ImportApiFromSwagger(string apiId, string apiSwaggerMetadata)
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        SetupHttpClientForApimAdmin(httpClient);

        //        // create PUT request with body containing API Swagger metadata
        //        string requestUrl = string.Format("/apis/{0}?api-version={1}&import=true", apiId, ApimAdminRestApiVersion);
        //        //StringContent bodyContent = new StringContent(apiSwaggerMetadata, Encoding.UTF8, "application/json");
        //        StringContent bodyContent = new StringContent(apiSwaggerMetadata, Encoding.UTF8, "application/vnd.swagger.doc+json");
        //        HttpResponseMessage response = await httpClient.PutAsync(requestUrl, bodyContent);

        //        if (!response.IsSuccessStatusCode)
        //        {
        //            return string.Format("ImportApiFromSwagger() failed: isSuccessStatusCode = {0}, statusCode = {1}, ReasonPhrase = {2}", response.IsSuccessStatusCode, response.StatusCode, response.ReasonPhrase);
        //        }

        //        return "ImportApiFromSwagger() succeeded";
        //    }
        //}

        // https://localhost:44301/admin/listusers
        public static async Task<string> ListUsers()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                SetupHttpClientForApimAdmin(httpClient);

                string url = string.Format("/users?api-version={0}", ApimAdminRestApiVersion);
                HttpResponseMessage response = await httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return string.Format("ListUsers() failed: StatusCode ={0}, ReasonPhrase = {1}", response.StatusCode, response.ReasonPhrase);
                }

                string users = await response.Content.ReadAsStringAsync();
                return string.Format("ListUser() succeeded: Users = {0}", users);
            }
        }

        // https://localhost:44301/admin/createuser?username=pdirac&firstName=Paul&lastname=Dirac&email=pdirac@gmail.com&password=secret&isactive=true&Note=This is a note
        public static async Task<string> CreateUser(string username, string firstName, string lastName, string email, string password, bool isActive = true, string note = "")
        {
            using (HttpClient httpClient = new HttpClient())
            {
                SetupHttpClientForApimAdmin(httpClient);

                // serialize user to json
                User user = new User(firstName, lastName, email, password, isActive, note);
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };
                string jsonSerializedUser = JsonConvert.SerializeObject(user, jsonSerializerSettings);

                // create PUT request with json User body
                string url = string.Format("/users/{0}?api-version={1}", username, ApimAdminRestApiVersion);
                StringContent stringContent = new StringContent(jsonSerializedUser, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PutAsync(url, stringContent);

                // handle error
                if (!response.IsSuccessStatusCode || response.StatusCode != HttpStatusCode.Created)
                {
                    // 400 - Bad Request
                    // 409 - Conflict (user already exists)
                    return string.Format("CreateUser() failed: StatusCode = {0}, ReasonPhrase = {1}", response.StatusCode, response.ReasonPhrase);
                }

                return "CreateUser() succeeded";
            }
        }

        #endregion

        #region Private Static Methods

        private static void SetupHttpClientForApimAdmin(HttpClient httpClient)
        {
            httpClient.BaseAddress = new Uri(ApimAdminRestBaseUrl);
            string apimRestAuthHeader = GetApimAdminRestAuthorizationHeader(ApimAdminRestPrimaryKey, ApimAdminRestIdentifier, ApimAdminRestAuthorizationExpiry);
            httpClient.DefaultRequestHeaders.Add("Authorization", apimRestAuthHeader);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.swagger.doc+json"));
        }

        private static string GetApimAdminRestAuthorizationHeader(string apimRestPrimaryKey, string apimRestId, DateTime apimRestExpiry)
        {
            using (HMACSHA512 encoder = new HMACSHA512(Encoding.UTF8.GetBytes(apimRestPrimaryKey)))
            {
                string dataToSign = apimRestId + "\n" + apimRestExpiry.ToString("O", CultureInfo.InvariantCulture);
                byte[] bytesToSign = Encoding.UTF8.GetBytes(dataToSign);
                byte[] hash = encoder.ComputeHash(bytesToSign);
                string signature = Convert.ToBase64String(hash);
                string encodedToken = string.Format("SharedAccessSignature uid={0}&ex={1:o}&sn={2}", apimRestId, apimRestExpiry, signature);
                return encodedToken;
            }
        }

        #endregion
    }
}
