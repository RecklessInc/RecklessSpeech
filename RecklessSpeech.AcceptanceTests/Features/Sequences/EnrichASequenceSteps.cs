﻿using FluentAssertions;
using RecklessSpeech.AcceptanceTests.Configuration;
using RecklessSpeech.Infrastructure.Sequences;
using RecklessSpeech.Shared.Tests.Sequences;
using RecklessSpeech.Web.ViewModels.Sequences;
using TechTalk.SpecFlow;

namespace RecklessSpeech.AcceptanceTests.Features.Sequences;

[Binding, Scope(Feature = "Enrich A Sequence")]
public class EnrichASequenceSteps : StepsBase
{
    private readonly SequenceBuilder sequenceBuilder;
    private readonly ISequencesDbContext inMemorySequencesDbContext;
    private IReadOnlyCollection<SequenceSummaryPresentation> SequenceListResponse { get; set; } = default!;

    public EnrichASequenceSteps(ScenarioContext context) : base(context)
    {
        this.inMemorySequencesDbContext = GetDbContext();
        this.sequenceBuilder = SequenceBuilder.Create(Guid.Parse("825B8D27-301A-4974-8024-7DE798C17765")) with
        {
            Word = new WordBuilder("brood")
        };
    }

    [Given(@"a sequence to be enriched")]
    public void GivenASequenceToBeEnriched()
    {
        this.inMemorySequencesDbContext.Sequences.Add(this.sequenceBuilder.BuildEntity());
    }

    [When(@"the user enriches this sequence")]
    public async Task WhenTheUserEnrichesThisSequence()
    {
        await this.Client.Latest().SequenceRequests().Enrich(this.sequenceBuilder.SequenceId.Value);
    }

    [Then(@"the sequence enriched data contains the raw explanation")]
    public async Task ThenTheSequenceEnrichedDataContainsTheRawExplanation()
    {
        this.SequenceListResponse = (await this.Client.Latest().SequenceRequests().GetAll());
        SequenceSummaryPresentation? sequencePresentationForBrood = this.SequenceListResponse.Single(x => x.Id == this.sequenceBuilder.SequenceId.Value);
        sequencePresentationForBrood.Explanation.Should().NotBeNullOrEmpty();
    }
}