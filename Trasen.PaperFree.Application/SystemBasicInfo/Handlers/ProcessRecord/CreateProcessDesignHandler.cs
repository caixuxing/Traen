using Trasen.PaperFree.Application.SystemBasicInfo.Commands.ProcessRecord;
using Trasen.PaperFree.Domain.ProcessRecord.Entity;
using Trasen.PaperFree.Domain.ProcessRecord.Repository;

namespace Trasen.PaperFree.Application.SystemBasicInfo.Handlers.ProcessRecord
{
    internal sealed class CreateProcessDesignHandler : IRequestHandler<CreateProcessDesignCmd, string>
    {
        private readonly IProcessDesignRepo processDesignRepo;
        private readonly Validate<CreateProcessDesignCmd> validate;
        private readonly IUnitOfWork unitOfWork;
        private readonly ICurrentUser currentUser;

        public CreateProcessDesignHandler(IProcessDesignRepo processDesignRepo,
            Validate<CreateProcessDesignCmd> validate, IUnitOfWork unitOfWork, ICurrentUser currentUser)
        {
            this.processDesignRepo = processDesignRepo;
            this.validate = validate;
            this.unitOfWork = unitOfWork;
            this.currentUser = currentUser;
        }

        public async Task<string> Handle(CreateProcessDesignCmd request, CancellationToken cancellationToken)
        {
            await validate.ValidateAsync(request);
            var entity = new ProcessDesign(
                request.ProcessName,
                request.ProcessCode,
                request.IsEnable,
                request.OrgCode,
                request.HospCode,
                request.processTempType,
                request.DeptCode);
            await processDesignRepo.AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}