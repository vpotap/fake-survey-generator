﻿using AutoWrapper;
using FakeSurveyGenerator.API.Configuration;
using FakeSurveyGenerator.API.Configuration.HealthChecks;
using FakeSurveyGenerator.API.Configuration.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace FakeSurveyGenerator.API
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization();

            services.AddControllers()
                .AddJsonConfiguration()
                .AddValidationConfiguration();

            services.AddHealthChecksConfiguration(_configuration);
            services.AddSwaggerConfiguration(_configuration);
            services.AddAuthenticationConfiguration(_configuration);
            services.AddForwardedHeadersConfiguration();
            services.AddApplicationInsightsConfiguration(_configuration);
            services.AddApplicationServicesConfiguration(_configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSecurityHeaders();

            app.UseForwardedHeaders();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            app.UseApiResponseAndExceptionWrapper();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.UseHealthChecksConfiguration();
            });

            app.UseSwaggerConfiguration();
        }
    }
}