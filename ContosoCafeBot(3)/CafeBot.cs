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
using Microsoft.Bot.Builder.Ai.LUIS;
using Microsoft.Bot.Builder.Ai.QnA;


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

                        //userState.sendCards = true;

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
                        // call LUIS and get results
                        LuisRecognizerOptions lOptions = new LuisRecognizerOptions() { Verbose = true };
                        LuisModel lModel = new LuisModel(
                            "edaadd9b-b632-4733-a25c-5b67271035dd", 
                            "be30825b782843dcbbe520ac5338f567", 
                            new System.Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/"), Microsoft.Cognitive.LUIS.LuisApiVersion.V2);
                        LuisRecognizer lRecognizer = new LuisRecognizer(lModel, lOptions);
                        System.Threading.CancellationToken ct;
                        cafeLUISModel lResult = await lRecognizer.Recognize<cafeLUISModel>(context.Activity.Text, ct);
                        
                        // top level dispatch
                        
                        switch (lResult.TopIntent().intent)
                        {
                            case cafeLUISModel.Intent.Greeting:
                            //case "hi":
                                await context.SendActivity("Hello, I'm the contoso cafe bot. How can I help you?");
                                if (userState.sendCards) await context.SendActivity(CreateCardResponse(context.Activity, createWelcomeCardAttachment()));
                                break;
                            case cafeLUISModel.Intent.Book_Table:
                            // case "book table":
                                await dc.Begin("BookTable");
                                break;
                            case cafeLUISModel.Intent.Who_are_you_intent:
                            // case "who are you?":
                                await dc.Begin("WhoAreYou");
                                break;
                            case cafeLUISModel.Intent.None:
                            default:
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

                                //await context.SendActivity("Sorry, I do not understand.");
                                //await context.SendActivity("You can say hi or book table or find locations");
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
