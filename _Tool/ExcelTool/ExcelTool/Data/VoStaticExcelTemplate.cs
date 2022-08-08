/****************************************************
    文件：#Class.cs
	作者：Clear
    日期：#CreateTime#
    类型: 工具自动创建(请勿修改)
	功能：#Name 静态表
*****************************************************/
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public class #Class
    {
        private static #Class m_instance;
        public static #Class Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new #Class();
                }
                return m_instance;
            }
        }

        public static void SetData(#Class val)
        {
            m_instance = val;
        }
        #Val
        
    }
}