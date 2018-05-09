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
            Dialogs.Add("textPrompt", new TextPrompt());
            
            Dialogs.Add("BookTable",
                new WaterfallStep[]
                {
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State = new Dictionary<string, object>();
                        await dc.Prompt("textPrompt", "Sure. I can help with that. What City?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["city"] = args["Value"];
                        await dc.Prompt("textPrompt", "Ok. Did you have a date in mind?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["date"] = args["Value"];
                        await dc.Prompt("textPrompt", "What time?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["time"] = args["Value"];
                        await dc.Prompt("textPrompt", "How many guests?");
                    },
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State["partySize"] = args["Value"];
                        var dialogState = dc.ActiveDialog.State;
                        await dc.Prompt("textPrompt", $"Should I go ahead and book a table for {dialogState["partySize"]} in {dialogState["city"]} for {dialogState["date"]} at {dialogState["time"]}?");
                    },
                    async (dc, args, next) =>
                    {
                        var dialogState = dc.ActiveDialog.State;
                        // TODO: Book table
                        await dc.Context.SendActivity($"I've booked your table for for {dialogState["partySize"]} in {dialogState["city"]} for {dialogState["date"]} at {dialogState["time"]}.");
                        await dc.End(dc.ActiveDialog.State);
                    }
                }
            );
            
        }
    }
}
