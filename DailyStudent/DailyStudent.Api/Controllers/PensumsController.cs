using DailyStudent.Api.Constants;
using DailyStudent.Api.DTOs.Pensum;
using DailyStudent.Api.Services.Extensions;
using DailyStudent.Api.Services.Pensums;
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
    public class PensumsController : ControllerBase
    {
        private readonly IPensumsService _pensumsService;
        public PensumsController(
            IPensumsService pensumsService
        )
        {
            _pensumsService = pensumsService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var output = await _pensumsService.Get(id);
            return Ok(output);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PensumInsertOrUpdateInput pensum)
        {
            var output = await _pensumsService.Insert(pensum);
            return Ok(output);
        }


        [HttpPut]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Update([FromForm] PensumInsertOrUpdateInput pensum)
        {
            var output = await _pensumsService.Update(pensum);
            return Ok(output);
        }

        [HttpGet]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Get()
        {
            var output = await _pensumsService.GetAll();
            return Ok(output);
        }

        [HttpGet("GetByCareer/{careerId}")]
        public async Task<IActionResult> GetByCareer(int careerId)
        {
            var output = await _pensumsService.GetByCareer(careerId);
            return Ok(output);
        }

        [HttpPut("ToggleState/{pensumId}")]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> ToggleState(int pensumId)
        {
            await _pensumsService.ToggleState(pensumId);
            return Ok();
        }

        [HttpPut("Approve/{pensumId}")]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> Approve(int pensumId)
        {
            await _pensumsService.Approve(pensumId);
            return Ok();
        }

        [HttpGet("Subjects/{pensumId}")]
        public async Task<IActionResult> GetSubjects(int pensumId)
        {
            var output = await _pensumsService.GetSubjects(pensumId);
            return Ok(output);
        }

        [HttpPost("Subjects")]
        public async Task<IActionResult> InsertSubject([FromBody] SubjectInsertOrUpdateInput input)
        {
            await _pensumsService.InsertSubject(input);
            return Ok();
        }

        [HttpPut("Subjects")]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectInsertOrUpdateInput input)
        {
            await _pensumsService.UpdateSubject(input);
            return Ok();
        }

        [HttpDelete("Subjects/{subjectId}")]
        public async Task<IActionResult> DeleteSubject(int subjectId)
        {
            await _pensumsService.DeleteSubject(subjectId);
            return Ok();
        }

        [HttpGet("UserSubjects/{userCareerId}")]
        public async Task<IActionResult> GetUserSubjects(int userCareerId)
        {
            var output = await _pensumsService.GetUserSubjects(userCareerId);
            return Ok(output);
        }

        [HttpPost("GenerateAcademicPlan/{pensumId}")]
        public async Task<IActionResult> GenerateAcademicPlan(int pensumId, [FromBody] List<string> completedSubjects)
        {
            var output = await _pensumsService.GenerateAcademicPlan(pensumId, completedSubjects);
            return Ok(output);
        }

        //[HttpGet("UserInstitutions")]
        //public async Task<IActionResult> GetUserInstitutions()
        //{
        //    var output = await _institutionService.GetUserInstitutions();
        //    return Ok(output);
        //}
    }
}
