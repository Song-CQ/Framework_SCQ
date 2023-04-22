/****************************************************
    文件：out_coinVOModel.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：D_动态金币发放表_A.xlsx 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class out_coinVOModel:VOModel<out_coinVOModel,out_coinVO>
    {
        public override string SheetName { get {return "out_coin";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","totalCoin","coinRange","isVideo"};}}

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