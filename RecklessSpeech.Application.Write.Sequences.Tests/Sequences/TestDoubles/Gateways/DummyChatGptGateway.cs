﻿using RecklessSpeech.Application.Write.Sequences.Ports.TranslatorGateways.Dutch;

namespace RecklessSpeech.Application.Write.Sequences.Tests.Sequences.TestDoubles.Gateways
{
    public class DummyChatGptGateway : IChatGptGateway
    {
        public Task<Explanation> GetExplanation(string word, string sentence) => throw new NotImplementedException();
    }
}