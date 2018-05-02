using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using ContosoCafeBot.Dialogs;

namespace ContosoCafeBot
{
    public class CafeBot : IBot
    {
        private DialogSet _dialogs;
        public CafeBot()
        {
            _dialogs = new DialogSet();
            _dialogs.Add("WhoAreYou", new WhoAreYou());
            /*
            _dialogs.Add("firstRun",
                new WaterfallStep[]
                {
                    async (dc, args, next) =>
                    {
                         await dc.Context.SendActivity("Welcome! We need to ask a few questions to get started.");
                         await dc.Begin("getProfile");
                    },
                    async (dc, args, next) =>
                    {
                        await dc.Context.SendActivity($"Thanks {args["name"]} I have your phone number as {args["phone"]}!");
                        await dc.End();
                    }
                }
            );*/
            

        }
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
                    var dc = _dialogs.CreateContext(context, conversationState.);
                    // top level dispatch
                    switch (context.Activity.Text)
                    {
                        case "hi":
                            await context.SendActivity("Hello, I'm the contoso cafe bot. How can I help you?");
                            //await context.SendActivity(CreateResponse(context.Activity, createWelcomeCardAttachment()));
                            break;
                        case "book table":
                            break;
                        case "find locations":
                            break;
                        case "Who are you?":
                            await context.SendActivity("Hello, I'm the contoso cafe bot. What is your name?");
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
