/****************************************************
    文件：out_cashVOModel.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：out_cash 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class out_cashVOModel:VOModel<out_cashVOModel,out_cashVO>
    {
        public override string SheetName { get {return "out_cash";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","totalCash","cashRange","cashRange2","isVideo","range"};}}

        public override void Init()
        {
            base.Init();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}