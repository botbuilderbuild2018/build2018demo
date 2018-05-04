using Microsoft.Bot.Builder.Dialogs;
//using Microsoft.Bot.Builder.Prompts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Recognizers.Text;
using Newtonsoft.Json.Linq;

namespace ContosoCafeBot.Dialogs
{
    public class BookTable : DialogContainer
    {
        public BookTable()
            : base("BookTable")
        {
            Dialogs.Add("BookTable",
                new WaterfallStep[]
                {
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State = new Dictionary<string, object>();
                        // TODO: make this a choice prompt
                        await dc.Prompt("textPrompt", "Sure. I can help with that. What City?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["city"] = args["Value"];
                        await dc.Prompt("DateTimePrompt", "Ok. Did you have a date in mind?");
                    },
                    async (dc, args, next) =>
                    {
                        List<Microsoft.Bot.Builder.Prompts.DateTimeResult.DateTimeResolution> p1 = (List<Microsoft.Bot.Builder.Prompts.DateTimeResult.DateTimeResolution>)args["Resolution"];
                        dc.ActiveDialog.State["date"] = p1[0].Value;
                        await dc.Prompt("DateTimePrompt", "What time?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["time"] = ((List<Microsoft.Bot.Builder.Prompts.DateTimeResult.DateTimeResolution>)args["Resolution"])[0].Value;
                        await dc.Prompt("PartySizePrompt", "How many guests?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["partySize"] = args["Value"];
                        var dialogState = dc.ActiveDialog.State;
                        await dc.Prompt("confirmPrompt", $"Should I go ahead and book a table for {dialogState["partySize"]} in {dialogState["city"]} for {dialogState["date"]} at {dialogState["time"]}?");
                    },
                    async (dc, args, next) =>
                    {
                        var dialogState = dc.ActiveDialog.State;
                        // TODO: evaluate outcome of confirm prompt, route to change dialog if user said no
                        await dc.Context.SendActivity($"I've booked your table for for {dialogState["partySize"]} in {dialogState["city"]} for {dialogState["date"]} at {dialogState["time"]}.");
                        await dc.End(dc.ActiveDialog.State);
                    }
                }
            );
            Dialogs.Add("DateTimePrompt", new DateTimePrompt(Culture.English));
            Dialogs.Add("PartySizePrompt", new NumberPrompt<int>(Culture.English));
            Dialogs.Add("confirmPrompt", new ConfirmPrompt(Culture.English));
            Dialogs.Add("textPrompt", new TextPrompt());
        }
    }
}
