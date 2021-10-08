using System.Collections.Generic;

namespace ProjectApp.Data
{
    /// <summary>
    /// 通用表.xlsx 静态表
    /// </summary>
    public class CommonsStaticVO
    {
        private static CommonsStaticVO m_instance;
        public static CommonsStaticVO Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new CommonsStaticVO();
                }
                return m_instance;
            }
        }

        public static void SetData(CommonsStaticVO val)
        {
            m_instance = val;
        }
        

        /// <summary>
        /// id = 0
        /// 数量时间
        /// 1000
        /// </summary>
        public int SumTime = 1000;

        /// <summary>
        /// id = 1
        /// 名字
        /// 网架
        /// </summary>
        public string name = "网架";

        /// <summary>
        /// id = 2
        /// 测试表
        /// [啥,点击,点击]
        /// </summary>
        public string[] sys = new string[]{"啥","点击","点击"};

        /// <summary>
        /// id = 3
        /// 测试属猪
        /// 1.5,50.6
        /// </summary>
        public float[] sysa = new float[]{1.5f,50.6f};

        /// <summary>
        /// id = 4
        /// 测试哇
        /// 1,3
        /// </summary>
        public List<int> hdaslk;
        
    }
}