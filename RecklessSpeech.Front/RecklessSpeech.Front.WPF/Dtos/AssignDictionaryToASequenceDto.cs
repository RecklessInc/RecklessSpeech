using System;
using RecklessSpeech.Front.WPF.ViewModels;

namespace RecklessSpeech.Front.WPF.Dtos;

public record AssignDictionaryToASequenceDto(SequenceDto SequenceDto,
    Guid DictionaryId);