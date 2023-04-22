/****************************************************
    文件：slotrewardVOModel.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：L_老虎机奖励表_A.xlsx 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class slotrewardVOModel:VOModel<slotrewardVOModel,slotrewardVO>
    {
        public override string SheetName { get {return "slotreward";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","Seq","wheel_item","quantity","weight","isMulti","cont_index"};}}

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