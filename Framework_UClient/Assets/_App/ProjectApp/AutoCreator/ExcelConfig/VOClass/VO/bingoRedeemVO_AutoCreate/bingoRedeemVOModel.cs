/****************************************************
    文件：bingoRedeemVOModel.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：bingoRedeem 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class bingoRedeemVOModel:VOModel<bingoRedeemVOModel,bingoRedeemVO>
    {
        public override string SheetName { get {return "bingoRedeem";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","item","redeem_num","item_need","ad_redeem","ad_redeem_time","card_id","logo_id","redeem_ID"};}}

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