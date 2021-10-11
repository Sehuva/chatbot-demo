using System;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Models;
using EchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace EchoBot.Dialogs
{
    public class GreetingDialog: ComponentDialog
    {
        private readonly StateService _botStateService;
        public GreetingDialog(StateService botStateService)
        {
            _botStateService = botStateService;
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));

            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
        }
        
        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile =
                await _botStateService.UserProfileAccesor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name",
                    new PromptOptions()
                    {
                        Prompt = MessageFactory.Text("What is your name?")
                    }, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }
        
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationtoken)
        {
            UserProfile userProfile =
                await _botStateService.UserProfileAccesor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                userProfile.Name = (string)stepContext.Result;
                await _botStateService.UserProfileAccesor.SetAsync(stepContext.Context, userProfile);
            }

            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Hi {userProfile.Name}. How can I help you today?"));
            return await stepContext.EndDialogAsync(null, cancellationtoken);
        }
    }
}