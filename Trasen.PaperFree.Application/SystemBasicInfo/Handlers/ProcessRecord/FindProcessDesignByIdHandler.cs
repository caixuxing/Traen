using Microsoft.EntityFrameworkCore;
using Trasen.PaperFree.Application.SystemBasicInfo.Dto.ProcessRecord;
using Trasen.PaperFree.Application.SystemBasicInfo.Query.ProcessRecord;
using Trasen.PaperFree.Domain.ProcessRecord.Repository;

namespace Trasen.PaperFree.Application.SystemBasicInfo.Handlers.ProcessRecord
{
    internal sealed class FindProcessDesignByIdHandler : IRequestHandler<FindProcessDesignByIdQry, ProcessDesignDetaiDto?>
    {
        private readonly IProcessDesignRepo processDesignRepo;

        public FindProcessDesignByIdHandler(IProcessDesignRepo processDesignRepo)
        {
            this.processDesignRepo = processDesignRepo;
        }

        public async Task<ProcessDesignDetaiDto?> Handle(FindProcessDesignByIdQry request, CancellationToken cancellationToken)
        {
            return await processDesignRepo.QueryAll().AsNoTracking()
                .Select(_ => new ProcessDesignDetaiDto
                {
                    Id = _.Id,
                    IsEnable = _.IsEnable,
                    ProcessCode = _.ProcessCode,
                    ProcessName = _.ProcessName,
                    processTempType = _.ProcessTempType,
                    DeptCode = _.DeptCode,
                    OrgCode = _.OrgCode,
                    HospCode = _.HospCode
                }).FirstOrDefaultAsync(_ => _.Id == request.Id);
        }
    }
}