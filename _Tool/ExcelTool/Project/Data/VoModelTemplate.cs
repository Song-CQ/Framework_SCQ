/****************************************************
    文件：#Class.cs
	作者：Clear
    日期：#CreateTime#
    类型: 工具自动创建(请勿修改)
	功能：#Name 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public class #Class:VOModel<#Class,#DataVo>
    {
        public override string SheetName { get {return #SheetName;}}
        public override bool HasStringKey { get {return #HasStringKey;}}
        public override bool HasStringId { get {return #HasStringId;}}
        public override bool HasStaticField { get {return #HasStaticField;}}
        public override List<string> HeadFields { get {return new List<string>{#HeadFields};}}

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