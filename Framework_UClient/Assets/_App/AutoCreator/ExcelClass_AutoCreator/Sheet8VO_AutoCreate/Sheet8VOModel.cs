using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// 表test - 副本 (2).xlsx 数据Model
    /// </summary>
    public class Sheet8VOModel:VOModel<Sheet8VOModel,Sheet8VO>
    {
        public override string SheetName { get {return "Sheet8";}}
        public override bool HasStringKey { get {return true;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","key","name","syss","dadsa"};}}

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