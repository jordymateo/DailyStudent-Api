using System.Threading.Tasks;
using DailyStudent.Api.DTOs.Course;
using DailyStudent.Api.Services.Course;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyStudent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CoursesController : ControllerBase
    {

        private readonly ICourseService _courseService;

        public CoursesController(
            ICourseService courseService
        )
        {
            _courseService = courseService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CourseInsertOrUpdateInput institution)
        {
            var output = await _courseService.Insert(institution);
            return Ok(output);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CourseInsertOrUpdateInput institution)
        {
            var output = await _courseService.Update(institution);
            return Ok(output);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var output = await _courseService.Get(id);
            return Ok(output);
        }
    }
}
