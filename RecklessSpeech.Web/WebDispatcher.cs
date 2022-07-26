﻿using System.Collections.Generic;
using System.Threading.Tasks;
using RecklessSpeech.Application.Core.Commands;
using RecklessSpeech.Application.Core.Queries;
using RecklessSpeech.Infrastructure.Orchestration.Dispatch;
using RecklessSpeech.Infrastructure.Orchestration.Dispatch.Transactions;

namespace RecklessSpeech.Web;

public class WebDispatcher
{
    private readonly IRecklessSpeechDispatcher dispatcher;

    public WebDispatcher(IRecklessSpeechDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    public Task<TResponse> Dispatch<TResponse>(IQuery<TResponse> query) => this.dispatcher.Dispatch(query);

    public async Task Dispatch(IEventDrivenCommand command)
    {
        IReadOnlyCollection<DomainEventIdentifier>? domainEvents = await this.dispatcher.Dispatch(new RootTransactionalStrategy(), command);
        await Publish(domainEvents);
    }

    public Task Publish(IEnumerable<DomainEventIdentifier> domainEvents) => this.dispatcher.Publish(domainEvents);
}