using System;
using EchoBot.Models;
using Microsoft.Bot.Builder;
using DialogState = Microsoft.Bot.Builder.Dialogs.DialogState;

namespace EchoBot.Services
{
    public class StateService
    {
        public UserState UserState { get; set; }
        public ConversationState ConversationState { get; set; }
        public static string UserProfileId { get; } = $"{nameof(StateService)}.UserProfile";
        public static string ConversationDataId { get; } = $"{nameof(StateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(StateService)}.DialogState";
        public IStatePropertyAccessor<UserProfile> UserProfileAccesor { get; set; }
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        
        public StateService(
            UserState userState,
            ConversationState conversationState)
        {
            UserState = userState ?? throw new ArgumentException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentException(nameof(conversationState));
            InitializeAccessors();
        }

        private void InitializeAccessors()
        {
            UserProfileAccesor = UserState.CreateProperty<UserProfile>(UserProfileId);
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}