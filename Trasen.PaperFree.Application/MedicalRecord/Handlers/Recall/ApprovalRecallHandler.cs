using Microsoft.EntityFrameworkCore;
using Trasen.PaperFree.Application.MedicalRecord.Commands.Recall;
using Trasen.PaperFree.Domain.ProcessRecord.Repository;
using Trasen.PaperFree.Domain.RecallRecord.DomainService;
using Trasen.PaperFree.Domain.RecallRecord.Entity;
using Trasen.PaperFree.Domain.RecallRecord.Repository;

namespace Trasen.PaperFree.Application.MedicalRecord.Handlers.Recall;

/// <summary>
/// 审批-召回申请处理器
/// </summary>
internal class ApprovalRecallHandler : IRequestHandler<ApprovalRecallCmd, bool>
{
    private readonly Validate<ApprovalRecallCmd> validate;
    private readonly ICurrentUser currentUser;
    private readonly IRecallApplyRepo recallApplyRepo;
    private readonly IUnitOfWork unitOfWork;
    private readonly IProcessDesignRepo processDesignRepo;
    private readonly IOutpatientInfoRepo outpatientInfoRepo;
    private readonly IGuidGenerator guidGenerator;
    private readonly IRecallRecordService recallRecordService;

    public ApprovalRecallHandler(
        Validate<ApprovalRecallCmd> validate,
        ICurrentUser currentUser,
        IRecallApplyRepo recallApplyRepo,
        IUnitOfWork unitOfWork,
        IProcessDesignRepo processDesignRepo,
        IOutpatientInfoRepo outpatientInfoRepo,
        IGuidGenerator guidGenerator,
        IRecallRecordService recallRecordService)
    {
        this.validate = validate;
        this.currentUser = currentUser;
        this.recallApplyRepo = recallApplyRepo;
        this.unitOfWork = unitOfWork;
        this.processDesignRepo = processDesignRepo;
        this.outpatientInfoRepo = outpatientInfoRepo;
        this.guidGenerator = guidGenerator;
        this.recallRecordService = recallRecordService;
    }

    public async Task<bool> Handle(ApprovalRecallCmd request, CancellationToken cancellationToken)
    {
        //校验请求参数
        await validate.ValidateAsync(request);

        //申请归档信息
        var recallApply = await recallApplyRepo.FindById(request.RecallApplyId);
        if (recallApply is null)
            throw new BusinessException(MessageType.Warn, "非法召回申请信息,审批失败！");
        if (recallApply.CurrentStatus == ProcessStatusType.END)
            throw new BusinessException(MessageType.Warn, "流程已结束,请勿重复审批！");

        //流程模板信息
        var processDesign = await processDesignRepo.QueryAll().AsTracking()
            .Include(x => x.ProcessNodes)
            .ThenInclude(node => node.NodeApprovers)
            .SingleOrDefaultAsync(x => x.Id == recallApply.ProcessDesignId);

        if (processDesign is null)
            throw new BusinessException(MessageType.Warn, "流程模板缺失无法进行审批操作!");
        //流程事件状态流
        var medicalRecordStatus = recallRecordService.ProcessEnventStatuFlow(
            request.IsRejectToNode ?? false,
            request.RejectNodeId ?? string.Empty,
            request.ApprovalResult,
            processDesign,
            recallApply);
        //更新归档申请
        recallApplyRepo.Update(recallApply);

        //出院信息
        var outpatientInfo = await outpatientInfoRepo.FindById(recallApply.ArchiveId);
        if (outpatientInfo is null)
            throw new BusinessException(MessageType.Warn, "病历信息缺失无法进行审批操作!");
        //出院信息状态更改
        outpatientInfo.ChnageStatus(medicalRecordStatus);
        outpatientInfoRepo.Update(outpatientInfo);

        //召回申请-审批信息
        await recallApplyRepo.AddAsync(new RecallApprover(
            guidGenerator.Create().ToString(),
            request.RecallApplyId,
            WorkFlowState.AWAITCOMMIT,
            request.ApprovalResult,
            request.ApprovalRemark,
            currentUser.Id,
            DateTime.Now),cancellationToken);
        //保存
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}