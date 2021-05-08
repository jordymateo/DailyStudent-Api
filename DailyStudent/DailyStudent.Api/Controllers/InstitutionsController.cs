using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.DTOs.Institution;
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
    public class InstitutionsController : ControllerBase
    {

        private readonly IInstitutionService _institutionService;

        public InstitutionsController(
            IInstitutionService institutionService
        )
        {
            _institutionService = institutionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]InstitutionInsertOrUpdateInput institution)
        {
            var output = await _institutionService.Insert(institution);
            return Ok(output);
        }

        [HttpPut]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Update([FromForm] InstitutionInsertOrUpdateInput institution)
        {
            var output = await _institutionService.Update(institution);
            return Ok(output);
        }

        [HttpGet]
        //[AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Get()
        {
            var output = await _institutionService.GetAll();
            return Ok(output);
        }

        [HttpGet("Available")]
        public async Task<IActionResult> GetAvailable()
        {
            var output = await _institutionService.GetAvailable();
            return Ok(output);
        }

        [HttpPut("ToggleState/{institutionId}")]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> ToggleState(int institutionId)
        {
            await _institutionService.ToggleState(institutionId);
            return Ok();
        }


        [HttpPut("ToggleUserCareerState/{userCareerId}")]
        [AccessRol(UserRoles.Student)]
        public async Task<IActionResult> ToggleUserCareerState(int userCareerId)
        {
            await _institutionService.ToggleUserCareerState(userCareerId);
            return Ok();
        }

        [HttpPut("ToggleUserCourseState/{userCourseId}")]
        [AccessRol(UserRoles.Student)]
        public async Task<IActionResult> ToggleUserCourseState(int userCourseId)
        {
            await _institutionService.ToggleUserCourseState(userCourseId);
            return Ok();
        }

        [HttpPut("Approve/{institutionId}")]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Approve(int institutionId)
        {
            await _institutionService.Approve(institutionId);
            return Ok();
        }


        [HttpGet("UserInstitutions")]
        public async Task<IActionResult> GetUserInstitutions()
        {
            var output = await _institutionService.GetUserInstitutions();
            return Ok(output);
        }

        [HttpGet("UserInstitutionsInLine")]
        public async Task<IActionResult> GetUserInstitutionsInLine()
        {
            var output = await _institutionService.GetUserInstitutionsInLine();
            return Ok(output);
        }

        [HttpGet("UserCareers")]
        public async Task<IActionResult> GetUserCareers()
        {
            var output = await _institutionService.GetUserCareers();
            return Ok(output);
        }
    }
}
