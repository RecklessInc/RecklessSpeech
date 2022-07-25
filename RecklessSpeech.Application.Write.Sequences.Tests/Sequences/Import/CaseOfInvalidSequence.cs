﻿using FluentAssertions;
using RecklessSpeech.Application.Write.Sequences.Commands;
using RecklessSpeech.Domain.Sequences.Sequences;
using RecklessSpeech.Domain.Shared;
using RecklessSpeech.Shared.Tests;
using RecklessSpeech.Shared.Tests.Sequences;
using Xunit;

namespace RecklessSpeech.Application.Write.Sequences.Tests.Sequences.Import;

public class CaseOfInvalidSequence
{
    private readonly ImportSequencesCommandHandler sut;

    public CaseOfInvalidSequence()
    {
        this.sut = new ImportSequencesCommandHandler();
    }
    
    [Fact]
    public async Task Should_throw_exception_if_audio_does_not_end_with_right_extension()
    {
        //Arrange
        var sequenceBuilder = SequenceBuilder.Create() with
        {
            AudioFileNameWithExtension = AudioFileNameWithExtension.Hydrate("344348.xml")
        };

        ImportSequencesCommand command = new(sequenceBuilder.BuildUnformatedSequence());

        //Act & Assert
        await sut.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidAudioFileFormatException>();
    }
    
    [Fact]
    public async Task Should_throw_exception_if_html_does_not_start_with_style()
    {
        //Arrange
        var sequenceBuilder = SequenceBuilder.Create() with
        {
            HtmlContent = HtmlContent.Hydrate("some content that does not start with \"style")
        };

        ImportSequencesCommand command = new(sequenceBuilder.BuildUnformatedSequence());

        //Act & Assert
        await sut.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should()
            .ThrowAsync<InvalidHtmlContentException>();
    }
}