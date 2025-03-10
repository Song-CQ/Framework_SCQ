/****************************************************
    文件：pathStaticVO.cs
	作者：Clear
    日期：2025/3/10 20:59:29
    类型: 工具自动创建(请勿修改)
	功能：path 静态表
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public class pathStaticVO : BaseStaticVO
    {
        private static pathStaticVO m_instance;
        public static pathStaticVO Instance
        {
            get
            {

                return m_instance;
            }
        }

        public const ConfigVO VOType = ConfigVO.path;

        public override string Key { get { return VOType.ToString(); } }



        public static void SetData(pathStaticVO val)
        {
            m_instance = val;
        }
        public static void ResetData()
        {
            m_instance = null;
        }

        

        /// <summary>
        /// id = 1
        /// 更新路径
        /// E:/UnityPro/Framework_SCQ/_Resources/UpData
        /// </summary>
        public string UpDataPath = "E:/UnityPro/Framework_SCQ/_Resources/UpData";
        
    }
}