using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EchoBot.Models;
using EchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoBot.Bots
{
    public class GreetingBot: ActivityHandler
    {
        private StateService _stateService;
        public GreetingBot(StateService stateService)
        {
            _stateService = stateService?? throw new ArgumentException(nameof(stateService));
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile =
                await _stateService.UserProfileAccesor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData =
                await _stateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData(), cancellationToken);
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(
                    MessageFactory.Text($"Hi {userProfile.Name} . How can I help you today?"), cancellationToken);
            }
            else
            {
                if (conversationData.PromptedUserForName)
                {
                    userProfile.Name = turnContext.Activity.Text?.Trim();
                    await turnContext.SendActivityAsync(
                        MessageFactory.Text($"Thanks {userProfile.Name}. How can I help you?"));
                }
                else
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is your name?"), cancellationToken);
                    conversationData.PromptedUserForName = true;
                }
                
            }

            await _stateService.UserProfileAccesor.SetAsync(turnContext, userProfile);
            await _stateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);
            await _stateService.UserState.SaveChangesAsync(turnContext);
            await _stateService.ConversationState.SaveChangesAsync(turnContext);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            await GetName(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            const string welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }
    }
}