using System.Threading.Tasks;
using DailyStudent.Api.DTOs.Assignment;
using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.Services.Assignment;
using DailyStudent.Api.Services.Course;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyStudent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AssignmentsController : ControllerBase
    {

        private readonly IAssignmentService _assignmentService;

        public AssignmentsController(
            IAssignmentService assignmentService
        )
        {
            _assignmentService = assignmentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]AssignmentInsertOrUpdateInput assignment)
        {
            var output = await _assignmentService.Insert(assignment);
            return Ok(true);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] AssignmentInsertOrUpdateInput assignment)
        {
            var output = await _assignmentService.Update(assignment);
            return Ok(output);
        }

        [HttpPut("{assignmentId}/Complete")]
        public async Task<IActionResult> Complete(int assignmentId)
        {
            await _assignmentService.Complete(assignmentId);
            return Ok(true);
        }

        [HttpDelete("{assignmentId}")]
        public async Task<IActionResult> Delete(int assignmentId)
        {
            await _assignmentService.Delete(assignmentId);
            return Ok(true);
        }


        [HttpGet("{courseId}/ByCourse")]
        public async Task<IActionResult> GetByCourse(int courseId)
        {
            var output = await _assignmentService.GetByCourse(courseId);
            return Ok(output);
        }

        [HttpGet("{userCareerId}/ByCareer")]
        public async Task<IActionResult> GetByCareer(int userCareerId)
        {
            var output = await _assignmentService.GetByCareer(userCareerId);
            return Ok(output);
        }

        [HttpGet("Calendar")]
        public async Task<IActionResult> GetToCalendar()
        {
            var output = await _assignmentService.GetToCalendar();

            return Ok(output);
        }
    }
}
