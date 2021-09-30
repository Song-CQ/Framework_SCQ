using UnityEngine;

namespace FutureCore
{
    public class ResMgr:BaseMgr<ResMgr>
    {

        public TextAsset GetExcelData(string tableName)
        {
            string path = @"Data\ExcelConfig"+@"\"+tableName;
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            return textAsset;
        }

    }
}