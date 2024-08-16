using Microsoft.EntityFrameworkCore;
using Trasen.PaperFree.Application.SystemBasicInfo.Commands.DeptMeunTree;
using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;
using Trasen.PaperFree.Domain.SystemBasicData.Entity;
using Trasen.PaperFree.Domain.SystemBasicData.Repository;

namespace Trasen.PaperFree.Application.SystemBasicInfo.Handlers.DeptMeunTreeHandler
{
    internal class ModifyDeptMenuTreeHandler : IRequestHandler<ModifyDeptMeunTreeCmdsListCmd, bool>
    {
        private readonly IDeptMenuTreeRepo deptMeumTreeRepo;
        private readonly Validate<ModifyDeptMeunTreeCmd> validate;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGuidGenerator guidGenerator;

        public ModifyDeptMenuTreeHandler(IDeptMenuTreeRepo deptMeumTreeRepo,
            IUnitOfWork unitOfWork,
            IGuidGenerator guidGenerator,
            Validate<ModifyDeptMeunTreeCmd> validate)
        {
            this.deptMeumTreeRepo = deptMeumTreeRepo;
            this.unitOfWork = unitOfWork;
            this.guidGenerator = guidGenerator;
            this.validate = validate;
        }

        public async Task<bool> Handle(ModifyDeptMeunTreeCmdsListCmd request, CancellationToken cancellationToken)
        {
            var ids = request.cmd.ListTreeData.Select(x => x.ArchiverMeumId);
            var entitys = await deptMeumTreeRepo.QueryAll().Where(x => x.DeptId == request.cmd.DeptId && ids.Contains(x.ArchiverMeumId)).ToListAsync();

            var insert = new List<DeptMeMenuTreeEntity>();
            if (entitys is null) throw new BusinessException(MessageType.Error, "更新失败!", "必传文件配置实体不存在!");
            foreach (var item in entitys)
            {
                var requs = request.cmd.ListTreeData.FirstOrDefault(x => x.ArchiverMeumId == item.ArchiverMeumId && request.cmd.DeptId == entitys.FirstOrDefault().DeptId);
                if (requs == null)
                    continue;
                    item.UpadteMeumTree(requs.ArchiverMeumId, requs.IsRequired, requs.ParentId);
            }
            //新增
            var insertList = request.cmd.ListTreeData.Where(t => !entitys.Any(b => b.ArchiverMeumId == t.ArchiverMeumId && b.DeptId == request.cmd.DeptId))
                .Select(x => new DeptMeMenuTreeEntity(guidGenerator.Create().ToString(), request.cmd.DeptId, x.ArchiverMeumId, x.ParentId, x.IsRequired, request.cmd.OrgCode, request.cmd.HospCode, request.cmd.InputCode));

            deptMeumTreeRepo.Update(entitys);
            await deptMeumTreeRepo.AddAsyncList(insertList.ToList(), cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}