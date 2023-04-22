/****************************************************
    文件：pathStaticVO.cs
	作者：Clear
    日期：2022/8/8 11:7:27
    类型: 工具自动创建(请勿修改)
	功能：L_路劲表_A.xlsx 静态表
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public class pathStaticVO
    {
        private static pathStaticVO m_instance;
        public static pathStaticVO Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new pathStaticVO();
                }
                return m_instance;
            }
        }

        public static void SetData(pathStaticVO val)
        {
            m_instance = val;
        }
        

        /// <summary>
        /// id = 1
        /// 更新路径
        /// E:/UnityPro/Framework_SCQ/_Resources/UpData
        /// </summary>
        public string UpDataPath = "E:/UnityPro/Framework_SCQ/_Resources/UpData";
        
    }
}