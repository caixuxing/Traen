﻿namespace Trasen.PaperFree.Application.SystemBasicInfo.Commands.BaseWatermark
{
    public class ModifyBaseWatermarkCmd : IRequest<bool>
    {
        /// <summary>
        /// 水印id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// 水印名称
        /// </summary>
        public string WatermarkName { get; set; }

        /// <summary>
        /// 使用场景
        /// </summary>
        public string UseScene { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// x坐标
        /// </summary>
        public string Xstation { get; set; }

        /// <summary>
        /// y坐标
        /// </summary>
        public string Ystation { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public string Angle { get; set; }

        /// <summary>
        /// 方向
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// 字体
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// 透明度
        /// </summary>
        public int Pellucidity { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public string Hight { get; set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public string Width { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Picture { get; set; }

        /// <summary>
        /// PicX
        /// </summary>
        public string PicX { get; set; }

        /// <summary>
        /// PicY
        /// </summary>
        public string PicY { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 机构编码
        /// </summary>
        public string OrgCode { get; set; }
        /// <summary>
        /// 院区编码
        /// </summary>
        public string HospCode { get; set; }
        /// <summary>
        /// 所属辖区
        /// </summary>
        public string InputCode { get; set; }

        public ModifyBaseWatermarkCmd SetId(string id)
        {
            Id = id;
            return this;
        }

        /// <summary>
        /// 验证规则
        /// </summary>
        public class ModifyBaseWatermarkValidate : AbstractValidator<ModifyBaseWatermarkCmd>
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            public ModifyBaseWatermarkValidate()
            {
                RuleFor(x => x.Id).NotEmpty().WithMessage("水印id不能为空！").MaximumLength(50).WithMessage("水印ID长度不能超过50个字符");
                RuleFor(x => x.Status).NotEmpty().WithMessage("使用状态不能为空！").MaximumLength(10).WithMessage("使用状态长度不能超过10个字符");
                RuleFor(x => x.UseScene).NotEmpty().WithMessage("使用场景不能为空！").MaximumLength(20).WithMessage("使用场景长度不能超过20个字符");
                RuleFor(x => x.Color).NotEmpty().WithMessage("颜色不能为空！").MaximumLength(20).WithMessage("颜色长度不能超过20个字符");
                RuleFor(x => x.Xstation).NotEmpty().WithMessage("文字X坐标不能为空！");
                RuleFor(x => x.Ystation).NotEmpty().WithMessage("文字Y坐标不能为空！");
                RuleFor(x => x.Angle).NotEmpty().WithMessage("文字角度不能为空！");
                RuleFor(x => x.Direction).NotEmpty().WithMessage("文字方向不能为空！").MaximumLength(10).WithMessage("文字方向长度不能超过10个字符"); ;
                RuleFor(x => x.Font).NotEmpty().WithMessage("字体不能为空！").MaximumLength(10).WithMessage("字体长度不能大于10个字符！");
                RuleFor(x => x.Hight).NotEmpty().WithMessage("高度不能为空！");
                RuleFor(x => x.Width).NotEmpty().WithMessage("宽度不能为空！");
                RuleFor(x => x.PicX).MaximumLength(10).WithMessage("PicX长度不能超过20个字符");
                RuleFor(x => x.PicY).MaximumLength(10).WithMessage("PicY长度不能超过20个字符");
                RuleFor(x => x.Color).NotEmpty().WithMessage("颜色不能为空！").MaximumLength(20).WithMessage("颜色长度不能超过20个字符");

                RuleFor(x => x.WatermarkName).NotEmpty().WithMessage("水印名称不能为空！").MaximumLength(20).WithMessage("水印名称长度不能超过20个字符");
                RuleFor(x => x.OrgCode).MaximumLength(50).WithMessage("机构编码长度不能超过").NotEmpty().WithMessage("机构编码不能为空");
                RuleFor(x => x.HospCode).MaximumLength(50).WithMessage("院区编码长度不能超过").NotEmpty().WithMessage("院区编码不能为空");
                RuleFor(x => x.InputCode).MaximumLength(50).WithMessage("辖区编码长度不能超过").NotEmpty().WithMessage("辖区编码不能为空");
            }
        }
    }
}