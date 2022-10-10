using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecklessSpeech.Application.Read.Queries.Sequences.GetAll;
using RecklessSpeech.Application.Read.Queries.Sequences.GetOne;
using RecklessSpeech.Application.Write.Sequences.Commands;
using RecklessSpeech.Web.Configuration;
using RecklessSpeech.Web.Configuration.Swagger;
using RecklessSpeech.Web.ViewModels.Sequences;

namespace RecklessSpeech.Web.Sequences;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sequences")]
[ApiController]
public class SequenceController : ControllerBase
{
    private readonly WebDispatcher dispatcher;

    public SequenceController(WebDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    [HttpPost]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<string>> ImportSequences(IFormFile file)
    {
        using StreamReader? reader = new(file.OpenReadStream());
        string? data = await reader.ReadToEndAsync();
        ImportSequencesCommand? command = new(data);

        await this.dispatcher.Dispatch(command);

        return Ok();
    }

    [HttpGet]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SequenceSummaryPresentation>), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<SequenceSummaryPresentation>>> Get()
    {
        IReadOnlyCollection<SequenceSummaryQueryModel>? result = await this.dispatcher.Dispatch(new GetAllSequencesQuery());
        return Ok(result.ToPresentation());
    }

    [HttpGet("{sequenceId:guid}")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(SequenceSummaryPresentation), (int) HttpStatusCode.OK)]
    [SwaggerResponseErrors((int) HttpStatusCode.NotFound, ApiErrors.ReadSequenceNotFound)]
    public async Task<ActionResult<SequenceSummaryPresentation>> GetOne(Guid sequenceId)
    {
        SequenceSummaryQueryModel result = await this.dispatcher.Dispatch(new GetOneSequenceQuery(sequenceId));
        return Ok(result.ToPresentation());
    }

    [HttpPost]
    [Route("Anki/")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<SequenceSummaryPresentation>>> SendToAnki(
        IReadOnlyCollection<Guid> ids)
    {
        await this.dispatcher.Dispatch(new SendNotesCommand(ids));
        return Ok();
    }

    [HttpPost]
    [Route("Dictionary/")]
    [MapToApiVersion("1.0")]
    [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
    public async Task<ActionResult<IReadOnlyCollection<SequenceSummaryPresentation>>> Enrich(
        IReadOnlyCollection<Guid> ids)
    {
        await this.dispatcher.Dispatch(new EnrichSequenceCommand(ids.First()));
        return Ok();
    }
}