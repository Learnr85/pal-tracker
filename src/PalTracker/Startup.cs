﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Steeltoe.CloudFoundry.Connector.MySql.EFCore;
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Common.HealthChecks;
using Steeltoe.Management.Endpoint.Info;

namespace PalTracker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton(sp => new WelcomeMessage(
              Configuration.GetValue<string>("WELCOME_MESSAGE", "WELCOME_MESSAGE not configured.")));

              services.AddDbContext<TimeEntryContext>(options => options.UseMySql(Configuration));
              services.AddScoped<ITimeEntryRepository, MySqlTimeEntryRepository>();

              services.AddSingleton(sp => new CloudFoundryInfo(
              
              Configuration.GetValue<string>("PORT","Port not configured"),
              Configuration.GetValue<string>("MEMORY_LIMIT","MEMORY_LIMIT not configured"),
              Configuration.GetValue<string>("CF_INSTANCE_INDEX","CF_INSTANCE_INDEX not configured"),
              Configuration.GetValue<string>("CF_INSTANCE_ADDR","CF_INSTANCE_ADDR not configured")

              

              ));

              services.AddCloudFoundryActuators(Configuration);
              services.AddScoped<IHealthContributor, TimeEntryHealthContributor>();
              services.AddSingleton<IOperationCounter<TimeEntry>, OperationCounter<TimeEntry>>();
              services.AddSingleton<IInfoContributor, TimeEntryInfoContributor>();

              
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseCloudFoundryActuators();
        }
    }
}
