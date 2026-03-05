using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureCore
{
    // Creator 意在优化创建对象时的反射消耗
    public interface IItemCreator
    {
        IObjectPoolItem CreatItem(Type type);
    }
}
