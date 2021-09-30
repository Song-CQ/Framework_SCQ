using System.Collections.Generic;

namespace ExcelTool.Data
{
    /// <summary>
    /// #Name 静态表
    /// </summary>
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