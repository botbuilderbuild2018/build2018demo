using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace ContosoCafeBot
{
    public class CafeBot : IBot
    {
        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            var userState = context.GetUserState<CafeBotUserState>();
            var conversationState = context.GetConversationState<CafeBotConvState>();

            switch (context.Activity.Type)

            {
                case ActivityTypes.ConversationUpdate:
                    var newUserName = context.Activity.MembersAdded[0].Name;
                    if (!string.IsNullOrWhiteSpace(newUserName) && newUserName != "Bot" && string.IsNullOrEmpty(userState.name))
                    {
                        await context.SendActivity($"Hello {newUserName}! I'm the Cafe bot!");
                        // remember the user's name
                        userState.name = newUserName;

                        await context.SendActivity("I can help you find contoso cafe locations, book a table and answer questions about Contoso cafe!");

                        // send a welcome card
                        //await context.SendActivity(CreateResponse(context.Activity, createWelcomeCardAttachment()));
                    }
                    break;
                case ActivityTypes.Message:
                    //await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");

                    // top level dispatch
                    switch(context.Activity.Text)
                    {
                        case "hi":
                            await context.SendActivity("Hello, I'm the contoso cafe bot. How can I help you?");
                            //await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");
                            break;
                        case "book table":
                            break;
                        case "find locations":
                            break;
                        default:
                            await context.SendActivity("Sorry, I do not understand.");
                            await context.SendActivity("You can say hi or book table or find locations");
                            break;
                    }
                    break;
            }
        }
        private Activity CreateResponse(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }
        // Methods to generate welcome card
        private Attachment createWelcomeCardAttachment()
        {
            var adaptiveCard = File.ReadAllText(@".\Assets\cards\welcomeCard.json");
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard)
            };
        }
    }    
}
