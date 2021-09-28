using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// 表test - 副本.xlsx 数据Model
    /// </summary>
    public class Sheet_gVOModel:VOModel<Sheet_gVOModel,Sheet_gVO>
    {
        public override string SheetName { get {return "Sheet_g";}}
        public override bool HasStringKey { get {return true;}}
        public override bool HasStringId { get {return true;}}
        public override bool HasStaticField { get {return false;}}
        public override List<string> HeadFields { get {return new List<string>{"id","key","ske"};}}

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