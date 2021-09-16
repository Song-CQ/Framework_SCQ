using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// Sheet_g_Model 数据Model
    /// </summary>
    public class Sheet_gVOModel:VOModel<Sheet_gVOModel,Sheet_gVO>
    {
        public override string SheetName { get => "Sheet_g";}
        public override bool HasStringKey { get => true; }
        public override bool HasStringId { get => true; }
        public override bool HasStaticField { get => false; }
        public override List<string> HeadFields { get => new List<string>{"id","key","ske"};}

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