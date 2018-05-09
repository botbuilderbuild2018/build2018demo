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
                    if(context.Activity.Text == "start over") {
                            //restart the conversation
                            await context.SendActivity("Sure.. Let's start over");        
                    }
                    
                    // await getQnAResult(context);
                    break;
            }
        } 

        // Methods to get QnA result
        private async Task getQnAResult(ITurnContext context) {
            var qEndpoint = new QnAMakerEndpoint()
            {
                Host = "https://contosocafeqnab8.azurewebsites.net/qnamaker",
                EndpointKey = "0fa7f711-6a82-4155-9cf9-5c8168967df6",
                KnowledgeBaseId = "dfa449da-1fb7-449e-b753-53af1b1f7b5b"
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
