using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Trasen.PaperFree.Application.Home.Dto;
using Trasen.PaperFree.Application.Home.Query;
using Trasen.PaperFree.Domain.Shared.Extend;

namespace Trasen.PaperFree.Application.Home.Handlers
{
    internal class FindTotalQueryHandlers : IRequestHandler<FindTotalQueryQry, TotalQueryDto>
    {
        private IOutpatientInfoRepo _outpatientInfoRepo;

        public FindTotalQueryHandlers(IOutpatientInfoRepo outpatientInfoRepo)
        {
            _outpatientInfoRepo = outpatientInfoRepo;
        }

        public async Task<TotalQueryDto> Handle(FindTotalQueryQry request, CancellationToken cancellationToken)
        {
            //var query = _outpatientInfoRepo.QueryAll().AsNoTracking()
            //            .WhereIf(x => x.OutDate >= request.BeginDate && x.OutDate <= request.EndDate, request.BeginDate != null && request.EndDate != null)
            //            .GroupBy(p => p.Status).Select(g => new { Status = g.Key, Count = g.Count() }).ToList().ForEach(item => { });

            throw new NotImplementedException();
        }
    }
}
