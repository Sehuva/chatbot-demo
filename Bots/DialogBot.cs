using System.Threading;
using System.Threading.Tasks;
using EchoBot.Helpers;
using EchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace EchoBot.Bots
{
    public class DialogBot<T>: ActivityHandler where T : Dialog
    {
        #region Variables

        protected readonly Dialog _dialog;
        protected readonly StateService _stateService;
        protected readonly ILogger _logger;

        #endregion

        public DialogBot(StateService botStateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _stateService = botStateService;
            _dialog = dialog;
            _logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);
            
            //Save any state during the turn everytime
            await _stateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _stateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Running dialog with Message Activity.");
            await _dialog.RunAsync(turnContext, _stateService.DialogStateAccessor,cancellationToken);
        }
    }
}