using Trasen.PaperFree.Application.MedicalRecord.Commands.Archive;
using Trasen.PaperFree.Domain.ArchiveRecord.Repository;
using Trasen.PaperFree.Domain.Shared.Extend;

namespace Trasen.PaperFree.Application.MedicalRecord.Handlers.Archive
{
    internal class DeleteArchiveApplyHandler : IRequestHandler<DeleteArchiveApplyCmd, bool>
    {
        private readonly IArchiveApplyRepo archiveApplyRepo;
        private readonly IUnitOfWork unitOfWork;

        public DeleteArchiveApplyHandler(IArchiveApplyRepo archiveApplyRepo, IUnitOfWork unitOfWork)
        {
            this.archiveApplyRepo = archiveApplyRepo;
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteArchiveApplyCmd request, CancellationToken cancellationToken)
        {
            var entity = await archiveApplyRepo.FindById(request.id);
            if (entity is null) throw new BusinessException(MessageType.Error, "删除失败", "实体信息不存在,无法执行删除！");
            if (entity.CurrentStatus != ProcessStatusType.AWAITAPPROVAL)
                throw new BusinessException(MessageType.Warn, $"流程审批状态非【{ProcessStatusType.AWAITAPPROVAL.ToDescription()}】不允许删除！");
            entity.ChangeDelete();
            archiveApplyRepo.Update(entity);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}