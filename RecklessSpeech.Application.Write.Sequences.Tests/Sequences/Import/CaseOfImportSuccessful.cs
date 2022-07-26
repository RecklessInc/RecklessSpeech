﻿using ExCSS;
using FluentAssertions;
using HtmlAgilityPack;
using RecklessSpeech.Application.Write.Sequences.Commands;
using RecklessSpeech.Domain.Sequences.Sequences;
using RecklessSpeech.Domain.Shared;
using RecklessSpeech.Shared.Tests;
using RecklessSpeech.Shared.Tests.Sequences;
using Xunit;

namespace RecklessSpeech.Application.Write.Sequences.Tests.Sequences.Import;

public class CaseOfImportSuccessful
{
    private readonly ImportSequencesCommandHandler sut;
    private readonly SequenceBuilder builder;

    public CaseOfImportSuccessful()
    {
        this.sut = new ImportSequencesCommandHandler();
        this.builder = SequenceBuilder.Create(Guid.Parse("259FD4F4-082E-46CB-BF1A-94F99780D2E2"));

    }

    [Fact]
    public async Task Should_add_a_new_sequence()
    {
        //Arrange
        ImportSequencesCommand command = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        events.Should().HaveCount(1);
        ((events.First() as AddedSequenceEvent)!).HtmlContent.Value.Should().NotBeNullOrEmpty();
        ((events.First() as AddedSequenceEvent)!).AudioFileNameWithExtension.Value.Should()
            .NotBeNullOrEmpty();
        ((events.First() as AddedSequenceEvent)!).Tags.Value.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Should_add_a_known_sequence()
    {
        //Arrange
        ImportSequencesCommand command = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        events.Should().ContainEquivalentOf(this.builder.BuildEvent(), AssertExtensions.IgnoreId);
    }

    [Fact]
    public async Task Should_add_two_sequences()
    {
        //Arrange
        ImportSequencesCommand command = new(Fixture.SomeContentWithTwoSequences);

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        events.Should().HaveCount(2);
    }

    [Fact]
    public async Task Should_html_not_specify_background_color_for_dc_card()
    {
        //Arrange
        ImportSequencesCommand? importSequencesCommand = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(importSequencesCommand, CancellationToken.None);

        //Assert
        AddedSequenceEvent importEvent = (AddedSequenceEvent) events.First();
        IStyleRule? dcCard = await Fixture.GetStyleRule(importEvent.HtmlContent.Value);
        dcCard.Style.Declarations.Where(property => property.Name == "background-color").Should().BeEmpty();
    }

    [Fact]
    public async Task Should_get_word_in_sequence()
    {
        //Arrange
        ImportSequencesCommand command = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        AddedSequenceEvent importEvent = (AddedSequenceEvent) events.First();
        importEvent.Word.Value.Should().Be("gimmicks");
    }
    
    [Fact]
    public async Task Should_get_translated_sentence_in_sequence()
    {
        //Arrange
        ImportSequencesCommand command = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        AddedSequenceEvent importEvent = (AddedSequenceEvent) events.First();
        importEvent.TranslatedSentence.Value.Should().Be("Et ça n'arrive pas par quelques astuces statistiques.");
    }
    
    [Fact]
    public async Task Should_html_not_contain_any_c1_for_flag()
    {
        //Arrange
        ImportSequencesCommand command = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        AddedSequenceEvent importEvent = (AddedSequenceEvent) events.First();
        importEvent.HtmlContent.Value.Should().NotContain("c1::");
    }
    
    [Fact]
    public async Task Should_show_word_in_red_background_in_html()
    {
        //Arrange
        ImportSequencesCommand? importSequencesCommand = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(importSequencesCommand, CancellationToken.None);

        //Assert
        AddedSequenceEvent importEvent = (AddedSequenceEvent) events.First();
        Fixture.VerifyWordHasAttributeBackgroundInRed(importEvent.HtmlContent.Value);
    }
    
    [Fact]
    public async Task Should_html_not_contain_the_translated_sentence()
    {
        //Arrange
        ImportSequencesCommand command = this.builder.BuildImportCommand();

        //Act
        IReadOnlyCollection<IDomainEvent> events = await this.sut.Handle(command, CancellationToken.None);

        //Assert
        AddedSequenceEvent importEvent = (AddedSequenceEvent) events.First();
        importEvent.HtmlContent.Value.Should().NotContain(this.builder.TranslatedSentence.Value);
    }


    private static class Fixture
    {
        public const string SomeContentWithTwoSequences =
            "\"<style>a lot of things in html\"	[sound:1658501397855.mp3]	\"word-naked lang-nl netflix Green pron \"" +
            "\"<style>a lot of other things in html\"	[sound:123456.mp3]	\"some other tags \"";

        public static async Task<IStyleRule> GetStyleRule(string htmlContent)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(htmlContent);
            HtmlNode? styleNode = htmlDoc.DocumentNode.SelectSingleNode("style");
            StylesheetParser? parser = new();
            Stylesheet? stylesheet = await parser.ParseAsync(styleNode.InnerText);
            IStyleRule? dcCard = stylesheet.StyleRules.First(rule =>
                rule.SelectorText == ".dc-card");
            return dcCard;
        }
        
        public static void VerifyWordHasAttributeBackgroundInRed(string htmlContent)
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(htmlContent);
            HtmlNodeCollection? nodes = htmlDoc.DocumentNode.SelectNodes("//span[@class='" + "dc-gap" + "']");
            HtmlNode? dcgapNode =  nodes.Single();
            HtmlNode? wordNode = dcgapNode.ChildNodes.Single();
             wordNode.Attributes.Single(x => x.Name == "style").Value.Should().Be("background-color: rgb(157, 0, 0);");
        }
    }
}