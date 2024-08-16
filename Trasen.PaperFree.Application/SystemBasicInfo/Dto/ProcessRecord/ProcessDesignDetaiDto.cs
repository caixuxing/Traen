using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;

namespace Trasen.PaperFree.Application.SystemBasicInfo.Dto.ProcessRecord
{
    /// <summary>
    /// 流程设计详细
    /// </summary>
    public class ProcessDesignDetaiDto
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// 科室编码
        /// </summary>
        public string DeptCode { get; set; }

        /// <summary>
        /// 流程代码值
        /// </summary>
        public string ProcessCode { get; set; }

        /// <summary>
        /// 流程模板类型
        /// </summary>
        public ProcessTempType processTempType { get; set; }

        /// <summary>
        /// 是否启用（用于流程模板切换）
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 机构编码
        /// </summary>
        public string OrgCode { get; set; }

        /// <summary>
        /// 院区编码
        /// </summary>
        public string HospCode { get; set; }
    }
}