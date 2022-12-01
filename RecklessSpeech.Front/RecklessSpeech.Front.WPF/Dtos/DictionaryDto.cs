using System;

namespace RecklessSpeech.Front.WPF.Dtos
{
    public record DictionaryDto(
    Guid Id,
    string Name,
    string FromLanguage,
    string ToLanguage);
}
