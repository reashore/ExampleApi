using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using ExampleApi.DataAccess;
using ExampleApi.Models;

namespace ExampleApi.Controllers
{
    [RoutePrefix("api")]
    public class CompaniesController : ApiController
    {
        private const string BadVersionMessage = "URL version segment must be v1 or v2";

        #region Version 1

        [HttpGet]
        [Route("{version}/companies")]
        public IHttpActionResult GetCompanies([FromUri] string version)
        {
            try
            {
                version = version.Trim();

                if (version != "v1" && version != "v2")
                {
                    return BadRequest(BadVersionMessage);
                }

                List<Company> companies = Companies.GetSampleCompanies(version);
                return Ok(companies);
            }
            catch (Exception)
            {
                // todo log exception
                return InternalServerError();
            }
        }

        //[ResponseType(typeof(Company))]
        [HttpGet]
        [Route("{version}/companies/{companyId}")]
        public IHttpActionResult GetCompany([FromUri] string version, int companyId)
        {
            try
            {
                if (version != "v1" && version != "v2")
                {
                    return BadRequest(BadVersionMessage);
                }

                List<Company> companies = Companies.GetSampleCompanies(version);
                var company = companies.FirstOrDefault(c => c.CompanyId == companyId);

                if (company == null)
                {
                    return NotFound();
                }

                return Ok(company);
            }
            catch (Exception)
            {
                // todo log exception
                return InternalServerError();
            }
        }

        [HttpPost]
        [Route("{version}/companies")]
        public IHttpActionResult PostCompany([FromUri] string version, [FromBody] Company company)
        {
            try
            {
                if (version != "v1" && version != "v2")
                {
                    return BadRequest(BadVersionMessage);
                }

                if (company == null)
                {
                    return BadRequest();
                }

                // todo check that there are no conflicting company Ids
                List<Company> companies = Companies.GetSampleCompanies(version);
                companies.Add(company);

                string url = string.Format("{0}/{1}", Request.RequestUri, company.CompanyId);
                return Created(url, company);
            }
            catch (Exception)
            {
                // todo log exception
                return InternalServerError();
            }
        }

        [HttpDelete]
        [Route("{version}/companies/{companyId}")]
        public IHttpActionResult DeleteCompany([FromUri] string version, int companyId)
        {
            try
            {
                if (version != "v1" && version != "v2")
                {
                    return BadRequest(BadVersionMessage);
                }

                List<Company> companies = Companies.GetSampleCompanies(version);
                var company = companies.FirstOrDefault(c => c.CompanyId == companyId);

                if (company == null)
                {
                    return NotFound();
                }

                companies.Remove(company);

                return StatusCode(HttpStatusCode.NoContent);
            }
            catch (Exception)
            {
                // todo log exception
                return InternalServerError();
            }
        }

        //[Route("{version}/companies/{companyId}/employees")]
        //public IHttpActionResult GetEmployeesForCompany([FromUri] string version, int companyId)
        //{
        //    try
        //    {
        //        // todo implement later
        //        return NotFound();
        //    }
        //    catch (Exception)
        //    {
        //        // todo log exception
        //        return InternalServerError();
        //    }
        //}

        #endregion
    }
}
