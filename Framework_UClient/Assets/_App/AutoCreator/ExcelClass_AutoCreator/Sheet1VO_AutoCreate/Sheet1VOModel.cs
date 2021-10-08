using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// 表test.xlsx 数据Model
    /// </summary>
    public class Sheet1VOModel:VOModel<Sheet1VOModel,Sheet1VO>
    {
        public override string SheetName { get {return "Sheet1";}}
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