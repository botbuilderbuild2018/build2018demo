using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Cognitive.LUIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SmartRetailBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("cafebot.bot")
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            var p = Configuration["services:1:appId"];
            Console.WriteLine($"option1 = {Configuration["name"]}");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddBot<LitwareRoot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                

                /*options.Middleware.Add(new QnAMakerMiddleware(
                                new QnAMakerMiddlewareOptions()
                                {
                                    SubscriptionKey = "d534abd71a0d438d95d5a001025ee074",
                                    KnowledgeBaseId = "40080f40-0200-482e-8e55-fae74d973490",
                                    EndActivityRoutingOnAnswer = true
                                }));*/

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}
