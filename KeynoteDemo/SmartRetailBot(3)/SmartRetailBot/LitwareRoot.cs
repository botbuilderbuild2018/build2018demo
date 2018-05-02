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
        private LuisModel retailModel = new LuisModel("586c6eba-c656-4a86-adc5-b963769bbae", "be30825b782843dcbbe520ac5338f567", new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/"), Microsoft.Cognitive.LUIS.LuisApiVersion.V2);
        public async Task OnTurn(ITurnContext context)
        {
            switch (context.Activity.Type)
            {
                case ActivityTypes.Message:
                    var luisRecognizer = new LuisRecognizer(retailModel);
                    var luisResult = await luisRecognizer.Recognize(context.Activity.Text, System.Threading.CancellationToken.None);
                    
                    if (luisResult != null)
                    {
                        (string key, double score) topItem = luisResult.GetTopScoringIntent();
                        switch (topItem.key)
                        {
                            case "Greeting":
                                await context.SendActivity($"Hello, I'm the contoso cafe bot. How can I help you?");
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
                            await context.SendActivity("Hello and welcome to the Cafe bot.");
                        }
                    }
                    break;
            }
        }
    }
}
