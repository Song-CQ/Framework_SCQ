/****************************************************
    文件：paymentTypeVOModel.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：B_本地化支付显示_A.xlsx 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class paymentTypeVOModel:VOModel<paymentTypeVOModel,paymentTypeVO>
    {
        public override string SheetName { get {return "paymentType";}}
        public override bool HasStringKey { get {return true;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","key","payicon","logo_id","","","","","",""};}}

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