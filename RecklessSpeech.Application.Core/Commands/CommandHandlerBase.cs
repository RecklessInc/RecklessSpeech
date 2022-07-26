﻿using MediatR;
using RecklessSpeech.Domain.Shared;

namespace RecklessSpeech.Application.Core.Commands;

public abstract class CommandHandlerBase<TCommand> : IRequestHandler<TCommand, IReadOnlyCollection<IDomainEvent>>
    where TCommand : IEventDrivenCommand
{
    public async Task<IReadOnlyCollection<IDomainEvent>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return await Handle(request);
    }

    protected abstract Task<IReadOnlyCollection<IDomainEvent>> Handle(TCommand command);
}