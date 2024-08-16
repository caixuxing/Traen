using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;

namespace Trasen.PaperFree.Domain.MedicalRecord.Entity
{
    public record OutpatientEmergency
    {
        private OutpatientEmergency() { }
              
               
        public OutpatientEmergency(string archiveid, string hosprecordid, string orgcode, string hospcode, string name, SexEnum sextype, DateTime? dateofbirth, int? age, string idcard, DateTime seepatientsdate,
                                   string seedeptcode, string receivedoctorcode, string icdcode, string icdname)
        {
            ArchiveId = archiveid;
            HospRecordId = hosprecordid;
            OrgCode = orgcode;
            HospCode = hospcode;
            Name = name;
            SexType = sextype;
            DateOfBirth = dateofbirth;
            Age = age;
            IdCard = idcard;
            SeePatientsDate = seepatientsdate;
            SeeDeptCode = seedeptcode;
            ReceiveDoctorCode = receivedoctorcode;
            IcdCode = icdcode;
            IcdName = icdname;
        }
        /// <summary>
        /// 档案号
        /// </summary>
        public string ArchiveId {  get; set; }
        /// <summary>
        /// 就诊号
        /// </summary>
        public string HospRecordId { get; set; }

        /// <summary>
        /// 机构编码
        /// </summary>
        public string OrgCode { get; init; }
        /// <summary>
        /// 院区编码
        /// </summary>
        public string HospCode { get; init; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public SexEnum SexType { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public int? Age {  get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string? IdCard {  get; set; }
        /// <summary>
        /// 接诊时间
        /// </summary>
        public DateTime SeePatientsDate { get; set; }
        /// <summary>
        /// 接诊科室
        /// </summary>
        public string SeeDeptCode {  get; set; }
        /// <summary>
        /// 接诊医生编码
        /// </summary>
        public string ReceiveDoctorCode {  get; set; }
        /// <summary>
        /// 门诊诊断编码
        /// </summary>
        public string IcdCode {  get; set; }
        /// <summary>
        /// 门诊诊断名称
        /// </summary>
        public string IcdName { get; set;}
    }
}
