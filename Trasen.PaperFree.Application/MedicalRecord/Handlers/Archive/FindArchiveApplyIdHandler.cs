﻿using Microsoft.EntityFrameworkCore;
using System.Data;
using Trasen.PaperFree.Application.Dto;
using Trasen.PaperFree.Application.MedicalRecord.Dto.Archive;
using Trasen.PaperFree.Application.MedicalRecord.Query.Archive;
using Trasen.PaperFree.Domain.ArchiveRecord.Repository;
using Trasen.PaperFree.Domain.ProcessRecord.Repository;
using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;
using Trasen.PaperFree.Domain.Shared.Extend;

namespace Trasen.PaperFree.Application.MedicalRecord.Handlers.Archive
{
    internal class FindArchiveApplyIdHandler : IRequestHandler<FindArchiveApplyIdQry, ArchiveApplyDetailDto>
    {
        private readonly IArchiveApplyRepo archiveApplyRepo;
        private readonly IProcessNodeRepo processNodeRepo;

        public FindArchiveApplyIdHandler(IArchiveApplyRepo archiveApplyRepo, IProcessNodeRepo processNodeRepo)
        {
            this.archiveApplyRepo = archiveApplyRepo;
            this.processNodeRepo = processNodeRepo;
        }

        public async Task<ArchiveApplyDetailDto> Handle(FindArchiveApplyIdQry request, CancellationToken cancellationToken)
        {
            var model = await archiveApplyRepo.QueryAll().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id);
            if (model is null) throw new BusinessException(MessageType.Warn, "非法流程信息");

            if (model.IsEnd) throw new BusinessException(MessageType.Warn, "流程已结束,无法加载审批信息");

            var nodeList = await processNodeRepo.QueryAll().AsNoTracking().Where(x => x.ProcessDesignId == model.ProcessDesignId).ToListAsync();
            var nodelModel = nodeList.FirstOrDefault(x => x.Id == model.CurrentApprovalNodeId);
            if (nodelModel is null) throw new BusinessException(MessageType.Warn, "未找到当前流程节点审批配置信息");
            List<DropSelectDto<int>> eventDirectionTypes = new();
            nodelModel.EventDirectionBranch.Split(",").ToList().ForEach(item =>
            {
                var EventDirectionType = (EventDirectionType)Enum.Parse(typeof(EventDirectionType), item.ToString(), true);
                eventDirectionTypes.Add(new DropSelectDto<int>() { Id = EventDirectionType.GetHashCode(), Name = EventDirectionType.ToDescription() });
            });
            return new ArchiveApplyDetailDto
            {
                NodeName = nodelModel.NodeName,
                IsRejectToNode = nodelModel.IsRejectToNode,
                NodeList = (nodelModel.IsRejectToNode ?? false) ? 
                 nodeList.Where(x => x.Id != model.CurrentApprovalNodeId)
                .Select(x => new DropSelectDto<string>() { Id = x.Id.ToString(), Name = x.NodeName }).ToList()
                : new(),
                UpperNodeId = nodelModel.UpperNodeId,
                eventDirectionTypes = eventDirectionTypes
            };
        }
    }
}