using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// 表#Name 数据Model
    /// </summary>
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