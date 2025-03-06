/****************************************************
    文件: ConfigDataMgr.cs
    作者: Clear
    日期: 2023/8/10 16:43:36
    类型: 框架核心脚本(请勿修改)
    功能: 表数据管理器
*****************************************************/
using ProjectApp.Data;
namespace ProjectApp
{
    public sealed partial class ConfigDataMgr 
    {

        public enum ConfigDataType
        {
            
        }

        public class game
        {
            public static uint Index;
        }


        public VO GetConfigVO<VO>(ConfigDataType type, int key) where VO : BaseVO
        {
            //typeof(VO);
            //return bingoRedeemVOModel.Instance.GetVO(key);
            return null;
  
            
        }




    }
}