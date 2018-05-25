using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.Bot.Builder;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Recognizers.Text;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Prompts.Choices;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace ContosoCafeBot.Dialogs
{
    public class BookTable : DialogContainer
    {
        public BookTable()
            : base("BookTable")
        {

            var promptOptions = new ChoicePromptOptions
            {
                Choices = new List<Choice>
                {
                    new Choice { Value = "Seattle" },
                    new Choice { Value = "Bellevue" },
                    new Choice { Value = "Renton" },
                }
            };

            //Dialogs.Add("textPrompt", new TextPrompt());

            Dialogs.Add("choicePrompt", new ChoicePrompt(Culture.English) { Style = Microsoft.Bot.Builder.Prompts.ListStyle.Auto });
            Dialogs.Add("numberPrompt", new NumberPrompt<int>(Culture.English));
            Dialogs.Add("timexPrompt", new TimexPrompt(Culture.English, TimexValidator));
            Dialogs.Add("confirmationPrompt", new ConfirmPrompt(Culture.English));

            Dialogs.Add("BookTable",
                new WaterfallStep[]
                {
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State = new Dictionary<string, object>();
                        // await dc.Prompt("textPrompt", "Sure. I can help with that. What City?");
                        await dc.Prompt("choicePrompt", "Which of our locations would you like?", promptOptions);
                    },
                    async (dc, args, next) =>
                    {
                        var choiceResult = (FoundChoice)args["Value"];
                        dc.ActiveDialog.State["bookingLocation"] = choiceResult.Value;
                        await dc.Prompt("timexPrompt", "When would you like to arrive? (We open at 4PM.)",
                            new PromptOptions { RetryPromptString = "We only accept reservations for the next 2 weeks and in the evenings between 4PM - 8PM" });
                    },
                    async (dc, args, next) =>
                    {
                        var timexResult = (TimexResult)args;
                        var timexResolution = timexResult.Resolutions.First();
                        var timexProperty = new TimexProperty(timexResolution.ToString());
                        var bookingDateTime = $"{timexProperty.ToNaturalLanguage(DateTime.Now)}";
                        dc.ActiveDialog.State["bookingDateTime"] = bookingDateTime;

                        await dc.Prompt("numberPrompt", "How many in your party?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["bookingGuestCount"] = args["Value"];
                        var dialogState = dc.ActiveDialog.State;

                        await dc.Prompt("confirmationPrompt", $"Thanks, Should I go ahead and book a table for {dialogState["bookingGuestCount"].ToString()} guests at our {dialogState["bookingLocation"].ToString()} location for {dialogState["bookingDateTime"].ToString()} ?");
                    },
                    async (dc, args, next) =>
                    {
                        var dialogState = dc.ActiveDialog.State;

                        // TODO: Verify user said yes to confirmation prompt

                        // TODO: book the table! 

                        await dc.Context.SendActivity($"Thanks, I have {dialogState["bookingGuestCount"].ToString()} guests booked for our {dialogState["bookingLocation"].ToString()} location for {dialogState["bookingDateTime"].ToString()}.");
                    }
                }
            );
        }
        // The notion of a Validator is a standard pattern across all the Prompts
        private static Task TimexValidator(ITurnContext context, TimexResult value)
        {
            var cadidates = value.Resolutions;

            var constraints = new[] {
                TimexCreator.ThisWeek(),                /* Take any entries for this week, no entries from past please */
                TimexCreator.NextWeek(),                /* Take any entries for next week, no dates from the past please */
                TimexCreator.Evening,                   /* Evenings only */
                
            };

            var resolutions = TimexRangeResolver.Evaluate(cadidates, constraints);

            if (resolutions.Count == 0)
            {
                value.Resolutions = new string[] { };
                value.Status = Microsoft.Bot.Builder.Prompts.PromptStatus.OutOfRange;
            }
            else
            {
                value.Resolutions = new[] { resolutions.First().TimexValue };
                value.Status = Microsoft.Bot.Builder.Prompts.PromptStatus.Recognized;
            }

            return Task.CompletedTask;
        }
    }
}
