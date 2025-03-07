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
    public class #Class : BaseStaticVO
    {
        private static #Class m_instance;
        public static #Class Instance
        {
            get
            {

                return m_instance;
            }
        }

        public const ConfigVO VOType = ConfigVO.#ConfigVO;

        public override string Key { get { return VOType.ToString(); } }



        public static void SetData(#Class val)
        {
            m_instance = val;
        }
        public static void ResetData()
        {
            m_instance = null;
        }

        #Val
        
    }
}