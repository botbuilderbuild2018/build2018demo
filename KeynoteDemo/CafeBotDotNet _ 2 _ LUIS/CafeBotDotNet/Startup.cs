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

namespace Microsoft.Bot.Samples.CafeBotDotNet
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
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            var p = Configuration["services:1:appId"];
            Console.WriteLine($"option1 = {Configuration["name"]}");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddBot<HelloBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                // If you want to get all intents scorings, add verbose in luisOptions
                /*var luisOptions = new LuisRequest { Verbose = true };
                options.Middleware.Add(new LuisRecognizerMiddleware(
                    new LuisModel(
                        "586c6eba-c656-4a86-adc5-b963769bbaed", 
                        "be30825b782843dcbbe520ac5338f567", 
                        new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/")), luisOptions: luisOptions));

                options.Middleware.Add(new QnAMakerMiddleware(
                                new QnAMakerMiddlewareOptions()
                                {
                                    SubscriptionKey = "d534abd71a0d438d95d5a001025ee074",
                                    KnowledgeBaseId = "40080f40-0200-482e-8e55-fae74d973490",
                                    EndActivityRoutingOnAnswer = true
                                }));*/

                var luisOptions = new LuisRequest { Verbose = true };
                options.Middleware.Add(new LuisRecognizerMiddleware(
                    new LuisModel(
                        "653f443e-da2d-43ee-ae47-8da7a836fc25",
                        "be30825b782843dcbbe520ac5338f567",
                        new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/")), luisOptions: luisOptions));

                options.Middleware.Add(new QnAMakerMiddleware(
                                new QnAMakerMiddlewareOptions()
                                {
                                    SubscriptionKey = "d534abd71a0d438d95d5a001025ee074",
                                    KnowledgeBaseId = "40080f40-0200-482e-8e55-fae74d973490",
                                    EndActivityRoutingOnAnswer = true
                                }));

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