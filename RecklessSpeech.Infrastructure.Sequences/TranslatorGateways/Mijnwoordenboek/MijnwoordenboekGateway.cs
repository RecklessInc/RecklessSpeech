﻿using RecklessSpeech.Application.Write.Sequences.Ports.TranslatorGateways.Mijnwoordenboek;
using RecklessSpeech.Domain.Sequences.Explanations;

namespace RecklessSpeech.Infrastructure.Sequences.TranslatorGateways.Mijnwoordenboek;

public class MijnwoordenboekGateway : ITranslatorGateway
{
    private readonly IMijnwoordenboekGatewayAccess access;

    public MijnwoordenboekGateway(IMijnwoordenboekGatewayAccess access)
    {
        this.access = access;
    }

    public Explanation GetExplanation(string word)
    {
        (string translations, string sourceUrl) = this.access.GetTranslationsAndSourceForAWord(word);

        Explanation explanation = Explanation.Create(Guid.NewGuid(), translations, word, sourceUrl);

        return explanation;
    }
}