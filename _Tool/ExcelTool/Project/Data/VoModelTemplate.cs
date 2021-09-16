using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// #Name 数据Model
    /// </summary>
    public class #Class:VOModel<#Class,#DataVo>
    {
        public override string SheetName { get => #SheetName;}
        public override bool HasStringKey { get => #HasStringKey; }
        public override bool HasStringId { get => #HasStringId; }
        public override bool HasStaticField { get => #HasStaticField; }
        public override List<string> HeadFields { get => new List<string>{#HeadFields};}

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