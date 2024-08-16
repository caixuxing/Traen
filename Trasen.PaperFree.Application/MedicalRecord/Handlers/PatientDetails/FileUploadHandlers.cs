﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Trasen.PaperFree.Application.MedicalRecord.Commands.PatientDetails;
using Trasen.PaperFree.Application.SystemBasicInfo.Common;
using Trasen.PaperFree.Application.SystemBasicInfo.Dto;
using Trasen.PaperFree.Domain.FileTable.Entity;
using Trasen.PaperFree.Domain.PatientDetails.Repository;
using Trasen.PaperFree.Domain.SeedWork;
using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;
using Trasen.PaperFree.Domain.Shared.Extend;
using Trasen.PaperFree.Domain.Shared.FileConversion;

namespace Trasen.PaperFree.Application.MedicalRecord.Handlers.PatientDetails
{
    internal class FileUploadHandlers : IRequestHandler<CreateFilesOtherCmd, string>
    {
        private readonly IOutpatientInfoRepo _outpatientInfoRepo;
        private readonly IFilesOtherRepo _filesOtherRepo;
        private readonly Validate<CreateFilesOtherCmd> validate;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ICurrentUser currentUser;
        private readonly IGuidGenerator guidGenerator;

        public FileUploadHandlers(Validate<CreateFilesOtherCmd> validate, IUnitOfWork unitOfWork, IFilesOtherRepo filesOtherRepo, IHttpClientFactory httpClientFactory, ICurrentUser currentUser, IGuidGenerator guidGenerator, IOutpatientInfoRepo outpatientInfoRepo)
        {
            this.validate = validate;
            this.unitOfWork = unitOfWork;
            _filesOtherRepo = filesOtherRepo;
            this.httpClientFactory = httpClientFactory;
            this.currentUser = currentUser;
            this.guidGenerator = guidGenerator;
            _outpatientInfoRepo = outpatientInfoRepo;
        }

        public async Task<string> Handle(CreateFilesOtherCmd request, CancellationToken cancellationToken)
        {
            await validate.ValidateAsync(request);
            var rootPath = Path.Combine(Directory.GetCurrentDirectory()
                    );
            var path = Path.Combine("WZH_FILES", request.OrgCode, DateTime.Now.ToLocalTime().Year.ToString(), DateTime.Now.ToLocalTime().Month.ToString(),
                                     DateTime.Now.ToLocalTime().Day.ToString(), request.ArchiveId, request.MenuName);
            if (!Directory.Exists(Path.Combine(rootPath, path)))
            {
                Directory.CreateDirectory(path);
            }
          

     var entity = new FilesOther(request.ArchiveId, request.MenuId, guidGenerator.Create().ToString(), request.FileSavename, request.FileSavename, request.FileType, path, WhetherType.YES
                                        , request.SourceCode);
          
            var qry = await _outpatientInfoRepo.QueryAll().AnyAsync(x => x.ArchiveId == request.ArchiveId && 
            new List<WorkFlowState>() { WorkFlowState.AWAITCOMMIT, WorkFlowState.NONE, WorkFlowState.ALREADYCOMMIT }.Contains(x.Status));
            if (qry) throw new BusinessException(MessageType.Error, "上传失败!", "当前病历状态不能上传病历文件,只能在：待提交、已提交状态下进行文件上传");

            await _filesOtherRepo.AddAsync(entity, cancellationToken);
            //流转文件保存
            var fileName = Path.GetFileName(request.FileStreams.FileName);
            var fileExtension = Path.GetExtension(fileName);
            await FileConversionClass.SaveFile(request.FileStreams, Path.Combine(rootPath, path,request.FileSavename), Path.GetExtension(fileName));
            await unitOfWork.SaveChangesAsync();
            return entity.Id;
        }
    }
}