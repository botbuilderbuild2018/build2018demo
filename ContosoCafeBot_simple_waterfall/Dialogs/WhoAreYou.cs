using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace ContosoCafeBot.Dialogs
{
    public class WhoAreYou : DialogContainer
    {
        public WhoAreYou()
            : base("WhoAreYou")
        {
            Dialogs.Add("textPrompt", new TextPrompt());

            Dialogs.Add("WhoAreYou",
                new WaterfallStep[]
                {
                    async (dc, args, next) =>
                    {
                        dc.ActiveDialog.State = new Dictionary<string, object>();
                        await dc.Prompt("textPrompt", "Hi, I'm the contoso cafe bot! What's your name?");
                    },
                    async (dc, args, next) =>
                    {
                        await dc.Context.SendActivity($"Hello {args["Value"]}! Nice to meet you.");
                        // TODO: Remember this in user state
                        await dc.End(dc.ActiveDialog.State);
                    }
                }
            );

        }
    }
}
