using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs.Institution;
using DailyStudent.Api.Services.Countries;
using DailyStudent.Api.Services.Extensions;
using DailyStudent.Api.Services.Institution;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DailyStudent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CountriesController : ControllerBase
    {

        private readonly ICountriesService _countriesService;

        public CountriesController(
            ICountriesService countriesService

        )
        {
            _countriesService = countriesService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var output = await _countriesService.Get();
            return Ok(output);
        }

    }
}
