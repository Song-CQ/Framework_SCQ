using System.Collections.Generic;

namespace FutureCore
{
    public static class ListExtend
    {

        /// <summary>
        /// 随机打乱list元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> original)
        {
            System.Random randomNum = new System.Random();
            int index = 0;
            T temp;
            for (int i = 0; i < original.Count; i++)
            {
                index = randomNum.Next(0, original.Count - 1);
                if (index != i)
                {
                    temp = original[i];
                    original[i] = original[index];
                    original[index] = temp;
                }
            }
            return original;
        }
        
      
        
        /// <summary>
        /// 转换列表为新类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T1"></typeparam>
        /// <param name="original"></param>
        /// <returns></returns>
        public static List<T1> OfType<T,T1>(this List<T> original) where T1 : class 
        {
            List<T1> t1List = new List<T1>();
            foreach (T t in original)
            {
                T1 t1 = t as T1;
                if (t1 != null)
                {
                    t1List.Add(t1);
                }
            }
            return t1List;
        }


    }
}
