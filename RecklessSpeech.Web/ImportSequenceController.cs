﻿using Microsoft.AspNetCore.Mvc;
using RecklessSpeech.Application.Write.Sequences.Commands;

namespace RecklessSpeech.Web;

[Route("api/v0/sequences")]
public class ImportSequenceController : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> ImportSequences(IFormFile file)
    {
        using var reader = new StreamReader(file.OpenReadStream());
        var data = await reader.ReadToEndAsync();
        var command = new ImportSequencesCommand(data);
        var handler = new ImportSequencesCommandHandler();
        await handler.Handle(command);
        return this.Ok();
    }
}