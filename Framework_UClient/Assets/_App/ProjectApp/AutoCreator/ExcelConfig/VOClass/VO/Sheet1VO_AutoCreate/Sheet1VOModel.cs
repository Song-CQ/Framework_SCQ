/****************************************************
    文件：Sheet1VOModel.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：Sheet1 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class Sheet1VOModel:VOModel<Sheet1VOModel,Sheet1VO>
    {
        public override string SheetName { get {return "Sheet1";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return false;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"1","999","[1]","10","True","3"};}}

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