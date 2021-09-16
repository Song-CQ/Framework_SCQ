using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// Sheet1_Model 数据Model
    /// </summary>
    public class Sheet1VOModel:VOModel<Sheet1VOModel,Sheet1VO>
    {
        public override string SheetName { get => "Sheet1";}
        public override bool HasStringKey { get => true; }
        public override bool HasStringId { get => true; }
        public override bool HasStaticField { get => false; }
        public override List<string> HeadFields { get => new List<string>{"id","key","name","syss","dadsa"};}

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