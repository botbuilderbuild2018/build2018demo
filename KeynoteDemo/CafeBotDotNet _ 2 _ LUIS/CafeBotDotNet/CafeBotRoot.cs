using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Ai.LUIS;
using System;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder.Core.Extensions;

namespace Microsoft.Bot.Samples.CafeBotDotNet
{
    public class HelloBot : IBot
    {
        public async Task OnTurn(ITurnContext context)
        {
            switch (context.Activity.Type)
            {
                case ActivityTypes.Message:

                    var luisResult = context.Services.Get<RecognizerResult>(LuisRecognizerMiddleware.LuisRecognizerResultKey);

                    if (luisResult != null)
                    {
                        (string key, double score) topItem = luisResult.GetTopScoringIntent();
                        Console.WriteLine($"top scoring intent: {topItem.key}");
                        switch(topItem.key)
                        {
                            case "Greeting":
                                await context.SendActivity($"Hello, I'm the Litware Lifestyle bot. How can I help you?");
                                break;
                            default:
                                await context.SendActivity($"Sorry, I do not understand that.");
                                break;
                        }
                    }
                    break;
                case ActivityTypes.ConversationUpdate:
                    foreach (var newMember in context.Activity.MembersAdded)
                    {
                        if (newMember.Id != context.Activity.Recipient.Id)
                        {
                            await context.SendActivity("Hello and welcome to the Litware lifestyle bot.");
                        }
                    }
                    break;
            }
        }
    }
}