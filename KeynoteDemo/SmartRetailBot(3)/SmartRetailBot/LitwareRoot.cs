using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Ai.LUIS;

namespace SmartRetailBot
{
    public class LitwareRoot : IBot
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
                        switch (topItem.key)
                        {
                            case "Greeting":
                                await context.SendActivity($"Hello, I'm the Litware lifestyle bot. How can I help you?");
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
