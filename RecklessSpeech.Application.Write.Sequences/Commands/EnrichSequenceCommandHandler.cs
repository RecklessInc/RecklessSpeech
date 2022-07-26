﻿using RecklessSpeech.Application.Core.Commands;
using RecklessSpeech.Application.Write.Sequences.Ports;
using RecklessSpeech.Application.Write.Sequences.Ports.TranslatorGateways.Mijnwoordenboek;
using RecklessSpeech.Domain.Sequences.Explanations;
using RecklessSpeech.Domain.Sequences.Sequences;
using RecklessSpeech.Domain.Shared;

namespace RecklessSpeech.Application.Write.Sequences.Commands;

public record EnrichSequenceCommand(Guid SequenceId) : IEventDrivenCommand;

public class EnrichSequenceCommandHandler : CommandHandlerBase<EnrichSequenceCommand>
{
    private readonly ISequenceRepository sequenceRepository;
    private readonly IExplanationRepository explanationRepository;
    private readonly ITranslatorGateway translatorGateway;

    public EnrichSequenceCommandHandler(
        ISequenceRepository sequenceRepository,
        IExplanationRepository explanationRepository,
        ITranslatorGateway translatorGateway)
    {
        this.sequenceRepository = sequenceRepository;
        this.explanationRepository = explanationRepository;
        this.translatorGateway = translatorGateway;
    }

    protected override async Task<IReadOnlyCollection<IDomainEvent>> Handle(EnrichSequenceCommand command)
    {
        Sequence sequence = await this.sequenceRepository.GetOne(command.SequenceId);

        Explanation? existingExplanation = this.explanationRepository.TryGetByTarget(sequence.Word.Value);

        Explanation explanation = existingExplanation ?? this.translatorGateway.GetExplanation(sequence.Word.Value);

        List<IDomainEvent> events = new()
        {
            new ExplanationAssignedToSequenceEvent(sequence.SequenceId, explanation.ExplanationId)
        };
        if (existingExplanation is null)
        {
            events.Add(new ExplanationAddedEvent(explanation));
        }

        return events;
    }
}