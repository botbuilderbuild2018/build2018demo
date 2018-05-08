using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Ai.QnA;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ContosoCafeBot
{
    public class CafeBot : IBot
    {
        public async Task OnTurn(ITurnContext context)
        {
            switch (context.Activity.Type)
            {
                case ActivityTypes.ConversationUpdate:
                    var newUserName = context.Activity.MembersAdded[0].Name;
                    if (!string.IsNullOrWhiteSpace(newUserName) && newUserName != "Bot")
                    {
                        await context.SendActivity($"Hello {newUserName}! I'm the Cafe bot!");
                        await context.SendActivity("I can help you find contoso cafe locations, book a table and answer questions!");
                    }
                    break;
                case ActivityTypes.Message:
                    await context.SendActivity("Hello, I'm the contoso cafe bot. How can I help you?");
                    await getQnAResult(context);
                    break;
            }
        } 

        // Methods to get QnA result
        private async Task getQnAResult(ITurnContext context) {
            var qEndpoint = new QnAMakerEndpoint()
            {
                Host = "https://contosocafeqnamaker.azurewebsites.net/qnamaker",
                EndpointKey = "09e2d55b-a44c-41b6-a08a-76a7df9ddffe",
                KnowledgeBaseId = "b5534d70-bded-45e1-998a-5945174d4ff3"
            };
            var qOptions = new QnAMakerOptions()
            {
                ScoreThreshold = 0.4F,
                Top = 1
            };
            var qnamaker = new QnAMaker(qEndpoint, qOptions);
            QueryResult[] qResult = await qnamaker.GetAnswers(context.Activity.Text);
            if (qResult.Length == 0)
            {
                await context.SendActivity("Sorry, I do not understand.");
                await context.SendActivity("You can say hi or book table or find locations");
            }
            else
            {
                await context.SendActivity(qResult[0].Answer);
            }
        }       
    }    
}
