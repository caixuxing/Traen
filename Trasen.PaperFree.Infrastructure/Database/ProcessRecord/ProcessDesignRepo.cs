using Trasen.PaperFree.Domain.ProcessRecord.Entity;
using Trasen.PaperFree.Domain.ProcessRecord.Repository;
using Trasen.PaperFree.Infrastructure.Database.DbContext;

namespace Trasen.PaperFree.Infrastructure.Database.ProcessRecord
{
    public class ProcessDesignRepo : IProcessDesignRepo
    {
        private readonly AppDbContext dbContext;

        /// <summary>
        /// 实体集合
        /// </summary>
        private DbSet<ProcessDesign> DbSet { get; }

        public ProcessDesignRepo(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
            DbSet = dbContext.Set<ProcessDesign>();
        }

        public async Task<bool> AddAsync(ProcessDesign entity, CancellationToken cancellationToken)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            return true;
        }

        public async ValueTask<ProcessDesign?> FindById(string Id)
        {
            return await DbSet.FindAsync(Id);
        }

        public IQueryable<ProcessDesign> QueryAll()
        {
            return DbSet;
        }

        public bool Update(ProcessDesign entity)
        {
            DbSet.Update(entity);
            return true;
        }
    }
}