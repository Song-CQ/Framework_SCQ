/****************************************************
    文件：adRewardVOModel.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：G_广告奖励表_A.xlsx 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class adRewardVOModel:VOModel<adRewardVOModel,adRewardVO>
    {
        public override string SheetName { get {return "adReward";}}
        public override bool HasStringKey { get {return true;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","key","desc","reward","rewardAmount","ratio","probability","cdTime"};}}

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