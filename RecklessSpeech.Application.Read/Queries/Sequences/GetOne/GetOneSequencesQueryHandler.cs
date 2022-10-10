using RecklessSpeech.Application.Core.Queries;
using RecklessSpeech.Application.Read.Ports;
using RecklessSpeech.Application.Read.Queries.Sequences.GetAll;

namespace RecklessSpeech.Application.Read.Queries.Sequences.GetOne;

public record GetOneSequenceQuery(Guid SequenceId) : IQuery<SequenceSummaryQueryModel>;

public class GetOneSequencesQueryHandler : QueryHandler<GetOneSequenceQuery, SequenceSummaryQueryModel>
{
    private readonly ISequenceQueryRepository sequenceQueryRepository;

    public GetOneSequencesQueryHandler(ISequenceQueryRepository sequenceQueryRepository)
    {
        this.sequenceQueryRepository = sequenceQueryRepository;
    }

    protected override async Task<SequenceSummaryQueryModel> Handle(GetOneSequenceQuery query)
    {
        return await this.sequenceQueryRepository.GetOne(query.SequenceId);
    }
}