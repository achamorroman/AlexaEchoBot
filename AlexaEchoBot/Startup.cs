﻿using Bot.Builder.Community.Adapters.Alexa.Integration.AspNet.Core;
using AlexaEchoBot.Bots;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AlexaEchoBot
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
            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, Bots.Bot>();

            services.AddAlexa()
                    .AddIntents();

            services.Configure<IISServerOptions>(options =>
            {
                options.AllowSynchronousIO = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("api/messages", async context =>
                {
                    var bot = context.RequestServices.GetRequiredService<IBot>();
                    var adapter = context.RequestServices.GetRequiredService<IBotFrameworkHttpAdapter>();

                    await adapter.ProcessAsync(context.Request, context.Response, bot);
                });
                endpoints.Map("api/AlexaSkill", async context =>
                {
                    var bot = context.RequestServices.GetRequiredService<AlexaBot> ();
                    var adapter = context.RequestServices.GetRequiredService<IAlexaHttpAdapter>();

                    await adapter.ProcessAsync(context.Request, context.Response, bot);
                });
            });
        }
    }
}
