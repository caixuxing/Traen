using Trasen.PaperFree.Application.MedicalRecord.Commands.PatientDetails;
using Trasen.PaperFree.Domain.PatientDetails.Repository;

namespace Trasen.PaperFree.Application.MedicalRecord.Handlers.PatientDetails
{
    internal class DeleteUploadHandlers : IRequestHandler<DeleteUploadCmd, bool>
    {
        private readonly IFilesOtherRepo _filesOtherRepo;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUploadHandlers(IFilesOtherRepo filesOtherRepo, IUnitOfWork unitOfWork)
        {
            _filesOtherRepo = filesOtherRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteUploadCmd request, CancellationToken cancellationToken)
        {
            var entity = await _filesOtherRepo.FindById(request.id);
            if (entity == null) throw new BusinessException(MessageType.Error, "删除失败！", "当前删除文件不存在无法执行删除操作！");
            entity.ChangeDelete();
            _filesOtherRepo.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}