﻿using System.ComponentModel.DataAnnotations;
using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;

namespace Trasen.PaperFree.Application.MedicalRecord.Commands.Recall;

/// <summary>
/// 审批-召回申请
/// </summary>
public record ApprovalRecallCmd : IRequest<bool>
{
    /// <summary>
    /// 召回申请ID
    /// </summary>
    public string RecallApplyId { get; private set; } = string.Empty;
    /// <summary>
    /// 审批结果
    /// </summary>
    [Required]
    public EventDirectionType ApprovalResult { get; set; }

    /// <summary>
    /// 是否驳回指定节点
    /// </summary>
    public bool? IsRejectToNode { get; set; }
    /// <summary>
    /// 驳回节点
    /// </summary>
    public string? RejectNodeId { get; set; }
    /// <summary>
    /// 审批批注
    /// </summary>
    public string? ApprovalRemark { get; set; }

    /// <summary>
    /// 设置归档申请ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ApprovalRecallCmd SetArchiveApplyId(string id)
    {
        this.RecallApplyId = id;
        return this;
    }
}

/// <summary>
/// 验证规则
/// </summary>
public class ApprovalRecallValidate : AbstractValidator<ApprovalRecallCmd>
{
    /// <summary>
    ///
    /// </summary>
    public ApprovalRecallValidate()
    {
        RuleFor(x => x.RecallApplyId).NotEmpty()
            .WithMessage("档案号不能为空！")
            .MaximumLength(50)
            .WithMessage("档案号最大不能超过50字符！");
        RuleFor(x => x.ApprovalResult).IsInEnum().WithMessage("无效的审批结果值");
        RuleFor(x => x.ApprovalRemark).MaximumLength(300).WithMessage("审批批注最大不能超过300字符！");
        RuleFor(x => x.RejectNodeId).MaximumLength(50).WithMessage("驳回指定节点ID不能超过50字符！");
    }
}