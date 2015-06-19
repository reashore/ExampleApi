using System;
using System.Collections.Generic;
using ExampleApi.Models;

namespace ExampleApi.DataAccess
{
    public class Companies
    {
        public static List<Company> GetSampleCompanies(string version)
        {
            List<Company> companies = new List<Company>();
            var numberCompanies = version == "v1" ? 5 : 10;

            for (int n = 1; n <= numberCompanies; n++)
            {
                Company company = new Company
                {
                    CompanyId = n,
                    CompanyName = string.Format("Company_version{0}_{1}", version, n),
                    CompanyCode = Guid.NewGuid().ToString(),
                    Ein = Guid.NewGuid().ToString(),
                    IsGovernmentAgency = n%2 == 0
                };

                companies.Add(company);
            }

            return companies;
        }
    }
}
