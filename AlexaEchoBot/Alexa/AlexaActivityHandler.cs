﻿using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AlexaEchoBot.Alexa
{
    public abstract class AlexaActivityHandler : ActivityHandler
    {
        public override Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    return OnMessageActivityAsync(new DelegatingTurnContext<IMessageActivity>(turnContext), cancellationToken);
                case AlexaRequestType.LaunchRequest:
                    return OnMembersAddedAsync(turnContext.Activity.MembersAdded, new DelegatingTurnContext<IConversationUpdateActivity>(turnContext), cancellationToken);
                case AlexaRequestType.IntentRequest:
                    return OnUnrecognizedActivityTypeAsync(turnContext, cancellationToken);
                case AlexaRequestType.SessionEndedRequest:
                    return OnMembersRemovedAsync(
                        turnContext.Activity.MembersRemoved,
                        new DelegatingTurnContext<IConversationUpdateActivity>(turnContext), cancellationToken);
                case AlexaRequestType.AccountLinkSkillEventRequest:
                    return OnMembersLinked(turnContext, cancellationToken);
                default:
                    break;
            }
            return base.OnTurnAsync(turnContext, cancellationToken);
        }

        protected abstract Task OnMembersLinked(ITurnContext turnContext, CancellationToken cancellationToken);
    }
}
