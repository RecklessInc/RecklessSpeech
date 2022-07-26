﻿using FluentAssertions;
using RecklessSpeech.AcceptanceTests.Configuration;
using RecklessSpeech.Application.Write.Sequences.Tests.Notes;
using RecklessSpeech.Infrastructure.Sequences;
using RecklessSpeech.Shared.Tests.Explanations;
using RecklessSpeech.Shared.Tests.Notes;
using RecklessSpeech.Shared.Tests.Sequences;
using TechTalk.SpecFlow;

namespace RecklessSpeech.AcceptanceTests.Features.Notes;

[Binding, Scope(Feature = "Send Notes to Anki")]
public class SendNotesToAnkiSteps : StepsBase
{
    private readonly Guid sequenceId;
    private readonly InMemorySequencesDbContext dbContext;
    private readonly SpyNoteGateway spyNoteGateway;
    private const string ContentForQuestion = "<style>some html here to be found in sequence and note";
    private readonly Guid explanationId = Guid.Parse("E138CB07-4EEF-4E60-8F66-3DE8108EFDE7");

    public SendNotesToAnkiSteps(ScenarioContext context) : base(context)
    {
        this.sequenceId = Guid.Parse("977343D4-0432-4BDF-BE78-5731C45CE00A");
        this.dbContext = GetService<InMemorySequencesDbContext>();
        this.spyNoteGateway = GetService<SpyNoteGateway>();
    }

    [Given(@"an existing sequence")]
    public void GivenAnExistingSequence()
    {
        SequenceBuilder? builder = SequenceBuilder.Create(this.sequenceId) with
        {
            HtmlContent = new(ContentForQuestion),
            TranslatedSentence = new("er is geen brood.")
        };
        this.dbContext.Sequences.Add(builder.BuildEntity());
    }
    
    [Given(@"some note for a dutch sequence")]
    public void GivenSomeNoteForADutchSequence()
    {
        SequenceBuilder? builder = SequenceBuilder.Create(this.sequenceId) with
        {
            Word = new ("brood"),
            Explanation = ExplanationBuilder.Create(this.explanationId)
        };
        this.dbContext.Sequences.Add(builder.BuildEntity());
    }
    
    [Given(@"some existing explanation for this dutch word")]
    public void GivenSomeExistingExplanationForThisDutchWord()
    {
        ExplanationBuilder explanationBuilder = ExplanationBuilder.Create(this.explanationId) with
        {
            Target = new("brood"),
            Content = new("pain"),
            SourceUrl = new("https://www.mijnwoordenboek.nl/vertaal/NL/FR/brood")
        };
        this.dbContext.Explanations.Add(explanationBuilder.BuildEntity());
    }

    [When(@"the user sends the sequence to Anki")]
    public async Task WhenTheUserSendsTheSequenceToAnki()
    {
        await this.Client.Latest().SequenceRequests().SendToAnki(new List<Guid>() {
            this.sequenceId});
    }

    [Then(@"a corresponding note is sent to Anki")]
    public void ThenACorrespondingNoteIsSentToAnki()
    {
        NoteBuilder? builder = NoteBuilder.Create(this.sequenceId) with
        {
            Question = new(ContentForQuestion),
            After = new AfterBuilder("translated sentence from Netflix: \"er is geen brood.\""),
            Source = new SourceBuilder("")
        };
        this.spyNoteGateway.Notes.Should().ContainEquivalentOf(builder.BuildDto());
    }
    
    [Then(@"the anki note contains the translation for the word in the after field")]
    public void ThenTheAnkiNoteContainsTheTranslationForTheWordInTheAfterField()
    {
        this.spyNoteGateway.Notes.Should().HaveCount(1);
        this.spyNoteGateway.Notes.First().After.Value.Should().Contain("pain");
    }
    
    [Then(@"the anki note contains the source")]
    public void ThenTheAnkiNoteContainsTheSource()
    {
        this.spyNoteGateway.Notes.Should().HaveCount(1);
        this.spyNoteGateway.Notes.Single().Source.Value.Should().Contain("https://www.mijnwoordenboek.nl/vertaal/NL/FR/brood");
    }
}