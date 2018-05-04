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
                            case "Product lookup":
                                await context.SendActivity("I can help you with that! Let me see what I can find.");
                                var results = lookUpResults(context.Activity.Text, luisResult);
                                await context.SendActivity("Here's what I found..");
                                await context.SendActivity(createCarouselCards());
                                await context.SendActivity(CreateResponse(context.Activity, CreateHeroCardAttachment()));
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
        
        private Attachment CreateHeroCardAttachment()
        {
            return new HeroCard()
            {
                Buttons = new List<CardAction>()
                {
                    new CardAction()
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "Not what I'm looking for",
                        Value = "https://docs.microsoft.com/en-us/azure/bot-service/"
                    },
                    new CardAction()
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "These look great",
                        Value = "https://docs.microsoft.com/en-us/azure/bot-service/"
                    }
                }
            }.ToAttachment();
        }

        private Activity createCarouselCards()
        {
            return (Activity) MessageFactory.Carousel(
                new Attachment[]
                {
                    new HeroCard(
                            images: new CardImage[] { new CardImage(url: "https://build2018demostorage.blob.core.windows.net/vktest/shutterstock_267798710.jpg") }
                        ).ToAttachment(),
                    new HeroCard(
                            images: new CardImage[] { new CardImage(url: "https://build2018demostorage.blob.core.windows.net/vktest/shutterstock_352316924.jpg") }
                        ).ToAttachment(),
                    new HeroCard(
                            images: new CardImage[] { new CardImage(url: "https://build2018demostorage.blob.core.windows.net/vktest/shutterstock_370222298.jpg") }
                        ).ToAttachment(),
                    new HeroCard(
                            images: new CardImage[] { new CardImage(url: "https://build2018demostorage.blob.core.windows.net/vktest/shutterstock_462355030.jpg") }
                        ).ToAttachment()
                }
            );
        }

        private Activity CreateResponse(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        private Attachment CreateThumbnailCardAttachment()
        {
            return new ThumbnailCard()
            {
                Title = "BotFramework Thumbnail Card",
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = "https://sec.ch9.ms/ch9/7ff5/e07cfef0-aa3b-40bb-9baa-7c9ef8ff7ff5/buildreactionbotframework_960.jpg"
                    }
                },
                Buttons = new List<CardAction>()
                {
                    new CardAction()
                    {
                        Type = ActionTypes.OpenUrl,
                        Title = "Get Started",
                        Value = "https://docs.microsoft.com/en-us/azure/bot-service/"
                    }
                },
                Subtitle = "Your bots — wherever your users are talking",
                Text = "Build and connect intelligent bots to interact with your users naturally wherever they are, from text/sms to Skype, Slack, Office 365 mail and other popular services."
            }.ToAttachment();
        }

        private bool lookUpResults(string x, RecognizerResult luisResult) {
            return true;
        }
    }
}
