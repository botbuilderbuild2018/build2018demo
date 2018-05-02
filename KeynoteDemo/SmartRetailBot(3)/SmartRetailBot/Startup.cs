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
                var luisOptions = new LuisRequest { Verbose = true };
                options.Middleware.Add(new LuisRecognizerMiddleware(
                    new LuisModel(
                        "586c6eba-c656-4a86-adc5-b963769bbae", 
                        "be30825b782843dcbbe520ac5338f567", 
                        new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/")), luisOptions: luisOptions));
                

                /*var qnamakerEndpoint = new QnAMakerEndpoint("POST /knowledgebases/5d820e39-3b6e-405d-aede-433c0c20e835/generateAnswer Host: https://westus.api.cognitive.microsoft.com/qnamaker/v2.0 Ocp-Apim-Subscription-Key: 443816d6948b4669882f95957c5a4096 Content-Type: application/json");
                options.Middleware.Add(new QnAMakerMiddleware(qnamakerEndpoint, new QnAMakerMiddlewareOptions()));*/

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
