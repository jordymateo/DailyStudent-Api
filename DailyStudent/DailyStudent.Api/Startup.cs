using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.Extensions;
using DailyStudent.Api.Services.Account;
using DailyStudent.Api.Services.Assignment;
using DailyStudent.Api.Services.Careers;
using DailyStudent.Api.Services.Countries;
using DailyStudent.Api.Services.Course;
using DailyStudent.Api.Services.Extensions;
using DailyStudent.Api.Services.Institution;
using DailyStudent.Api.Services.Messages;
using DailyStudent.Api.Services.Note;
using DailyStudent.Api.Services.Pensums;
using DailyStudent.Api.Services.Security;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DailyStudent.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<DailyStudentDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

            
            services.UseDailyStudentAuthentication();
            services.UseGoogleCloud(Configuration);


            services.Configure<MessagesOptions>(x =>
                Configuration.GetSection("Messages").Bind(x)
            );

            //My services
            services.AddTransient<INoteService, NoteService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<ICareersService, CareersService>();
            services.AddTransient<IPensumsService, PensumsService>();
            services.AddTransient<ICountriesService, CountriesService>();
            services.AddTransient<IAssignmentService, AssignmentService>();
            services.AddTransient<IInstitutionService, InstitutionService>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseExceptionHandler(err => 
                err.UseExceptionsMiddleware()
            );

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(x => x
              .AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
          );


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
