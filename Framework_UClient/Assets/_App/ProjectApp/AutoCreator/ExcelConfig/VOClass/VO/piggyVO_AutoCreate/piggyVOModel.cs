/****************************************************
    文件：piggyVOModel.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：X_小猪存钱表_A.xlsx 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class piggyVOModel:VOModel<piggyVOModel,piggyVO>
    {
        public override string SheetName { get {return "piggy";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","itemID","cashGoal","cashReceive","range","gameNum","adNum"};}}

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