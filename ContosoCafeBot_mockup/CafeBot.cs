using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
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
                    await context.SendActivity("Hello, I'm the contoso cafe bot. How can I help you?");
                    break;
            }
        }        
    }    
}
