using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace EchoBot.Dialogs
{
    public class BugReportDialog: ComponentDialog
    {
        private readonly StateService _botStateService;

        public BugReportDialog(
            StateService botStateService)
        {
            _botStateService = botStateService;
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {

            };

            AddDialog(new WaterfallDialog($"{nameof(BugReportDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.description"));
            AddDialog(new DateTimePrompt($"{nameof(BugReportDialog)}.callbackTimem", CallbackTimeValidatorAsync));
            AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.phoneNumber", PhoneValidatorAsync));
            AddDialog(new ChoicePrompt($"{nameof(BugReportDialog)}.bug"));

            InitialDialogId = $"{nameof(BugReportDialog)}.mainFlow";

        }

        private async Task<DialogTurnResult> DescriptionStepASync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.description", 
                new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Enter a description for your report")
            }, cancellationToken);
        }
        
        private async Task<DialogTurnResult> CallbackTimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            stepContext.Values["description"] = (string) stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.callbackTime",
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text($"Please enter in a callback time"),
                    RetryPrompt = MessageFactory.Text("The value entered must be between the hours of 9 am and 5 pm"),
                }, cancellationtoken);

        }
        
        private async Task<DialogTurnResult> PhoneNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            stepContext.Values["callbackTime"] = Convert.ToDateTime(((List<DateTimeResolution>) stepContext.Result).FirstOrDefault()?.Value);
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.phoneNumber",
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text($"Please enter in a phone number that we can call you back at"),
                    RetryPrompt = MessageFactory.Text("Please enter a valid phone number"),
                }, cancellationtoken);
        }

        private async Task<DialogTurnResult> BugStepAsync(WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            stepContext.Values["phoneNumber"] = (string) stepContext.Result;
            return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.bug",
                new PromptOptions()
                {
                    Prompt = MessageFactory.Text($"Please enter the type of bug."),
                    Choices = ChoiceFactory.ToChoices(new List<string>
                    {
                        "Security", "Crash", "Performance", "Usability", "Serious Bug", "Other" 
                    })
                }, cancellationToken);
        }



    }
}