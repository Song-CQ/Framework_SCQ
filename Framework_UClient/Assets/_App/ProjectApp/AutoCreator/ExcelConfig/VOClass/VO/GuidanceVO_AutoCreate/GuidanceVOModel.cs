/****************************************************
    文件：GuidanceVOModel.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：X_新手引导_A.xlsx 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class GuidanceVOModel:VOModel<GuidanceVOModel,GuidanceVO>
    {
        public override string SheetName { get {return "Guidance";}}
        public override bool HasStringKey { get {return false;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","keyGuide","targetLv","guideIndex","triggerMsg","desc","hasTalk","talkIndex","cardPosObj","maskBtnType","maskPos","maskSize","completeType","topComName","completeMsg","maskPositionObj","arrowPostioinOffset","arrowPositionObj","handPostionObj","autoFinishTime"};}}

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