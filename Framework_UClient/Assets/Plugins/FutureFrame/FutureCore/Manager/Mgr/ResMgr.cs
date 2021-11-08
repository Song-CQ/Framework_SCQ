using System.Text;
using UnityEngine;

namespace FutureCore
{
    public class ResMgr:BaseMgr<ResMgr>
    {

        public TextAsset GetExcelData(string tableName)
        {
            string path = @"Data\ExcelConfig"+@"\"+tableName;
            TextAsset data = Resources.Load<TextAsset>(path);
            return data;
        }

       

        #region SyncLoad
        public T SyncLoad<T>(string assetPath) where T : Object
        {
            T asset = Resources.Load<T>(assetPath);
            return asset;
        }
        #endregion
    }
}