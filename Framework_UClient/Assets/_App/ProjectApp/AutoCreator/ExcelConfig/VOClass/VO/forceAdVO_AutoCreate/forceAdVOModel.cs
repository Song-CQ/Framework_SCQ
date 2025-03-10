﻿/****************************************************
    文件：forceAdVOModel.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：forceAd 数据Model
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public partial class forceAdVOModel:VOModel<forceAdVOModel,forceAdVO>
    {
        public override string SheetName { get {return "forceAd";}}
        public override bool HasStringKey { get {return true;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","key","desc","cdtime"};}}

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