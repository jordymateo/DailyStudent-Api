using DailyStudent.Api.Services.Cloud;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Extensions
{
    public static class CloudExtensions
    {
        public static void UseGoogleCloud(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GoogleCloudOptions>(x =>
                configuration.GetSection("GoogleCloud").Bind(x)
            );

            services.AddSingleton<IGoogleCloudService, GoogleCloudService>();
        }
    }
}
