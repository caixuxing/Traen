using Trasen.PaperFree.Domain.Base;
using Trasen.PaperFree.Domain.Shared.Enums.SystemBasicData;

namespace Trasen.PaperFree.Domain.ProcessRecord.Entity
{
    /// <summary>
    /// 流程设计表
    /// </summary>
    public record ProcessDesign : FullRoot
    {
        private ProcessDesign()
        {
        }

        public ProcessDesign(string processName, string processCode, bool isEnable, string orgCode, string hospCode, ProcessTempType processTempType, string deptCode)
        {
            ProcessName = processName;
            ProcessCode = processCode;
            IsEnable = isEnable;
            OrgCode = orgCode;
            HospCode = hospCode;
            ProcessTempType = processTempType;
            DeptCode = deptCode;
        }
        /// <summary>
        /// 流程名称
        /// </summary>
        public string ProcessName { get; private set; }
        /// <summary>
        /// 流程代码值
        /// </summary>
        public string ProcessCode { get; private set; }

        /// <summary>
        /// 流程模板类型【归档、召回】
        /// </summary>
        public ProcessTempType ProcessTempType { get; private set; }

        /// <summary>
        /// 科室编码
        /// </summary>
        public string DeptCode { get; private set; }

        /// <summary>
        /// 是否启用（用于流程模板切换）
        /// </summary>
        public bool IsEnable { get; private set; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string OrgCode { get; set; }
        /// <summary>
        /// 院区编码
        /// </summary>
        public string HospCode { get; set; }

        /// <summary>
        /// 流程节点
        /// </summary>
        public virtual ICollection<ProcessNode> ProcessNodes { get; set; } = new HashSet<ProcessNode>();

        /// <summary>
        /// 修改流程设计信息
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="processCode"></param>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public ProcessDesign ChangeProcessDesign(string processName, string processCode, bool isEnable,
            string deptCode, ProcessTempType processTempType,
            string orgCode, string hospCode
            )
        {
            this.ProcessName = processName;
            this.ProcessCode = processCode;
            this.IsEnable = isEnable;
            this.DeptCode = deptCode;
            this.ProcessTempType = processTempType;
            this.OrgCode = orgCode;
            this.HospCode = hospCode;
            return this;
        }
        /// <summary>
        /// 修改是否启用
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public ProcessDesign ChangeIsEnable(bool isEnable)
        {
            IsEnable = isEnable;
            return this;
        }
    }
}