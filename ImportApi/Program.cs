using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImportApi.Properties;

namespace ImportApi
{
    class Program
    {
        static void Main()
        {
            Task<string> result1 = GetUsers();
            Console.WriteLine(result1.Result);

            Console.WriteLine();

            Task<string> result2 = ImportApiUsingSwaggerDoc();
            Console.WriteLine(result2.Result);

            Task<string> result3 = ImportApiUsingSwaggerLink();
            Console.WriteLine(result3.Result);

            Console.ReadKey();
        }

        public static async Task<string> ImportApiUsingSwaggerDoc()
        {
            const string apiId = "Api1";
            string apiSwaggerMetadata = Resources.ApiSwaggerMetadata;
            const string path = "ExampleAPI";
            string result = await ApiManagementAdmin.ImportApiUsingSwaggerDoc(apiId, apiSwaggerMetadata, path);
            return result;
        }

        public static async Task<string> ImportApiUsingSwaggerLink()
        {
            const string apiId = "Api2";
            const string apiSwaggerLink = "http://exampleapi.azurewebsites.net/swagger/docs/v1";
            const string path = "ExampleAPI";
            string result = await ApiManagementAdmin.ImportApiUsingSwaggerLink(apiId, apiSwaggerLink, path);
            return result;
        }

        public static async Task<string> GetUsers()
        {
            string result = await ApiManagementAdmin.ListUsers();
            return result;
        }
    }
}
