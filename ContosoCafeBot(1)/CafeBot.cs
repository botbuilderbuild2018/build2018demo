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
using Newtonsoft.Json.Linq;

namespace ContosoCafeBot
{
    public class CafeBot : IBot
    {
        private DialogSet _dialogs;
        public CafeBot()
        {
            _dialogs = new DialogSet();

            _dialogs.Add("WhoAreYou", new WhoAreYou());
            _dialogs.Add("BookTable", new BookTable());

        }
        public async Task OnTurn(ITurnContext context)
        {
            //TODO: is this the right way to handle cards?
            string utterance = context.Activity.Text;
            JObject cardData = (JObject)context.Activity.Value;
            if (cardData != null && cardData.Property("intent") != null) utterance = cardData["utterance"].ToString();

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

                        userState.sendCards = true;

                        await context.SendActivity("I can help you find contoso cafe locations, book a table and answer questions about Contoso cafe!");

                        // send a welcome card
                        if(userState.sendCards) await context.SendActivity(CreateCardResponse(context.Activity, createWelcomeCardAttachment()));
                    }
                    break;
                case ActivityTypes.Message:
                    
                    // create dialogContext
                    var dc = _dialogs.CreateContext(context, conversationState);
                    // continue with any active dialogs
                    await dc.Continue();

                    if(!context.Responded)
                    {
                        // top level dispatch
                        switch (utterance)
                        {
                            case "hi":
                                await context.SendActivity("Hello, I'm the contoso cafe bot. How can I help you?");
                                if (userState.sendCards) await context.SendActivity(CreateCardResponse(context.Activity, createWelcomeCardAttachment()));
                                break;
                            case "book table":
                                await dc.Begin("BookTable");
                                break;
                            case "who are you?":
                                await dc.Begin("WhoAreYou");
                                break;
                            default:
                                await context.SendActivity("Sorry, I do not understand.");
                                await context.SendActivity("You can say hi or book table or find locations");
                                break;
                        }
                    } 
                    break;
            }
        }
        
        private Activity CreateCardResponse(Activity activity, Attachment attachment)
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
