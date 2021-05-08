using DailyStudent.Api.Constants;
using DailyStudent.Api.DTOs.Career;
using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.Services.Careers;
using DailyStudent.Api.Services.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CareersController : ControllerBase
    {
        private readonly ICareersService _careersService;

        public CareersController(
            ICareersService careersService
        )
        {
            _careersService = careersService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CareerInsertOrUpdateInput career)
        {
            var output = await _careersService.Insert(career);
            return Ok(output);
        }


        [HttpPost("User")]
        public async Task<IActionResult> CreateUserCareer([FromBody] UserCareerInput input)
        {
            var data = await _careersService.InsertUserCareer(input);
            return Ok(data);
        }

        [HttpPost("Period")]
        public async Task<IActionResult> CreateUserPeriod([FromBody] UserPeriodInsertOrUpdateInput input)
        {
            await _careersService.InsertUserPeriod(input);
            return Ok();
        }

        [HttpPost("PeriodSubject")]
        public async Task<IActionResult> CreatePeriodSubject([FromBody] CourseInsertOrUpdateInput input)
        {
            await _careersService.InsertPeriodSubject(input);
            return Ok();
        }

        [HttpPut]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Update([FromForm] CareerInsertOrUpdateInput career)
        {
            var output = await _careersService.Update(career);
            return Ok(output);
        }

        [HttpPut("Period")]
        public async Task<IActionResult> UpdatePeriod([FromBody] UserPeriodInsertOrUpdateInput input)
        {
            await _careersService.UpdateUserPeriod(input);
            return Ok();
        }

        [HttpGet]
        //[AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Get()
        {
            var output = await _careersService.GetAll();
            return Ok(output);
        }

        [HttpGet("GetByInstitution/{institutionId}")]
        public async Task<IActionResult> GetByInstitution(int institutionId)
        {
            var output = await _careersService.GetByInstitution(institutionId);
            return Ok(output);
        }

        [HttpGet("UserPeriods/{userCareerId}")]
        public async Task<IActionResult> GetUserPeriods(int userCareerId)
        {
            var output = await _careersService.GetUserPeriods(userCareerId);
            return Ok(output);
        }

        [HttpPut("Approve/{careerId}")]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Approve(int careerId)
        {
            await _careersService.Approve(careerId);
            return Ok();
        }


        //[HttpGet("UserInstitutions")]
        //public async Task<IActionResult> GetUserInstitutions()
        //{
        //    var output = await _institutionService.GetUserInstitutions();
        //    return Ok(output);
        //}
    }
}
