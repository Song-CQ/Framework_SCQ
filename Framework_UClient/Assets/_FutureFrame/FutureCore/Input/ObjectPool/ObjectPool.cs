using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FutureCore
{

    public enum ObjectPoolMode
    {
        NEAT,
        NOT_NEAT
    }


    /// <summary>
    /// 对象池
    /// </summary>
    public class Combo_ObjectPool<T> : IDisposable where T : IObjectPoolItem
    {
        protected ObjectPoolMode _mode;
        // 类型
        protected Type _type;
        // 容量
        protected int _capacity;
        // 预创建对象数量
        protected int _preCreateCount;
        // 空闲表
        protected Dictionary<IObjectPoolItem, bool> _freeMap_Neat;
        // 使用表
        protected Dictionary<IObjectPoolItem, bool> _usedMap_Neat;
        // 空闲表
        public Dictionary<Type, List<T>> _freeMap_NotNeat;
        // 使用表
        public Dictionary<Type, List<T>> _usedMap_NotNeat;

        protected List<IObjectPoolItem> _freeList;

        protected bool _locked = false;

        protected IItemCreator _itemCreator;

        public Combo_ObjectPool(ObjectPoolMode mode, int capacity = 50, int preCreateCount = 50)
        {
            _mode = mode;
            ResetPool<T>(capacity, preCreateCount);
        }

        public void Dispose()
        {
            _locked = true;
            if (_mode == ObjectPoolMode.NEAT)
            {
                foreach (KeyValuePair<IObjectPoolItem, bool> kvp in _freeMap_Neat)
                {
                    kvp.Key.Dispose();
                }
                foreach (KeyValuePair<IObjectPoolItem, bool> kvp in _usedMap_Neat)
                {
                    kvp.Key.Dispose();
                }
                _freeMap_Neat = null;
                _usedMap_Neat = null;
                _freeList = null;
            }
            else if (_mode == ObjectPoolMode.NOT_NEAT)
            {
                foreach (KeyValuePair<Type, List<T>> kvp in _freeMap_NotNeat)
                {
                    List<T> list = kvp.Value;
                    for (int i = 0; i < list.Count; i++)
                    {
                        T item = list[i];
                        if (item != null) item.Dispose();
                    }

                }
                foreach (KeyValuePair<Type, List<T>> kvp in _usedMap_NotNeat)
                {
                    List<T> list = kvp.Value;
                    for (int i = 0; i < list.Count; i++)
                    {
                        T item = list[i];
                        if (item != null) item.Dispose();
                    }
                }
                _freeMap_NotNeat = null;
                _usedMap_NotNeat = null;
                _freeList = null;
            }
            _itemCreator = null;
        }

        /// <summary>
        /// 重置对象池
        /// 重置对象池会导致原先初始化过的对象和对象池脱离联系
        /// </summary>
        public void ResetPool<TN>() where TN : T
        {
            ResetPool<TN>(_capacity, _preCreateCount);
        }

        /// <summary>
        /// 重置对象池
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="capacity">容量</param>
        public void ResetPool<TN>(int capacity, int preCreateCount) where TN : T
        {
            _type = typeof(TN);
            _capacity = capacity;
            _preCreateCount = preCreateCount <= capacity ? preCreateCount : capacity;
            // 如果是工整类型,预创建对象
            if (_mode == ObjectPoolMode.NEAT)
            {
                _freeMap_Neat = new Dictionary<IObjectPoolItem, bool>(_capacity);
                _usedMap_Neat = new Dictionary<IObjectPoolItem, bool>(_capacity);
                _freeList = new List<IObjectPoolItem>(_capacity);
                for (int i = 0; i < _preCreateCount; i++)
                {
                    IObjectPoolItem item = CreatItem(_type);
                    _freeMap_Neat[item] = true;
                    _freeList.Add(item);
                }
            }
            else if (_mode == ObjectPoolMode.NOT_NEAT)
            {
                _freeMap_NotNeat = new Dictionary<Type, List<T>>();
                _usedMap_NotNeat = new Dictionary<Type, List<T>>();
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public IObjectPoolItem GetItem()
        {
            if (_locked) return null;
            IObjectPoolItem item = null;
            // 空闲对象数量不为0
            if (_freeList.Count > 0)
            {
                // 取一个
                int idx = _freeList.Count - 1;
                item = _freeList[idx];
                _freeList.RemoveAt(idx);
                _freeMap_Neat.Remove(item);
            }
            else // 没有空闲对象
            {
                // 创建新对象
                item = CreatItem(_type);
                // 容量增加
                _capacity++;
            }
            // 放入使用表中(到此item总是不为空的)
            _usedMap_Neat[item] = true;
            return item;
        }

        public TN GetItem<TN>() where TN : T, new()
        {
            TN item = default(TN);
            if (_locked) return item;
            // 类型
            Type type = typeof(TN);
            #region 取出对像
            // 空闲列表
            List<T> list = null;
            // 判断空闲表里是否有类型
            if (_freeMap_NotNeat.ContainsKey(type))
            {
                list = _freeMap_NotNeat[type];
            }
            else
            {
                list = new List<T>();
                _freeMap_NotNeat[type] = list;
            }
            // 如果空闲列表里有空闲对象
            if (list.Count > 0)
            {
                int idx = list.Count - 1;
                item = (TN)list[idx];
                list.RemoveAt(idx);
            }
            else
            {
                item = (TN)CreatItem(type);
            }
            #endregion

            #region 放入使用表
            // 使用列表
            list = null;
            // 放入使用表
            if (_usedMap_NotNeat.ContainsKey(type))
            {
                list = _usedMap_NotNeat[type];
            }
            else
            {
                list = new List<T>();
                _usedMap_NotNeat[type] = list;
            }
            list.Add(item);
            #endregion
            return item;
        }

        private IObjectPoolItem CreatItem(Type type)
        {
            IObjectPoolItem item;
            if (_itemCreator != null)
            {
                item = _itemCreator.CreatItem(type);
            }
            else
            {
                item = Activator.CreateInstance(type) as IObjectPoolItem;
            }
            //UnityEngine.Log.infoError("CreatItem");
            return item;
        }

        public IItemCreator itemCreator { set { _itemCreator = value; } }

        /// <summary>
        /// 归还对象
        /// </summary>
        /// <param name="item">从此对象池中获取过的对象</param>
        public bool ReturnItem(T item)
        {
            bool suc = false;
            if (_locked || item == null) return false;
            if (_mode == ObjectPoolMode.NEAT)
            {
                // 如果使用表包含此元素
                if (_usedMap_Neat.ContainsKey(item))
                {
                    // 移出使用表
                    _usedMap_Neat.Remove(item);
                    // 放入空闲表
                    _freeMap_Neat[item] = true;
                    // 重置
                    item.Reset();
                    // 重置完才加入列表
                    _freeList.Add(item);
                    suc = true;
                }
                else
                {
                    item.Reset();
                    //throw new Exception("归还的对象原本不包含在对象池中 " + item);
                }
            }
            else if (_mode == ObjectPoolMode.NOT_NEAT)
            {
                // 类型
                Type type = item.GetType();
                List<T> list = null;

                // 移出使用表
                if (_usedMap_NotNeat.TryGetValue(type, out list))
                {
                    // 如果包含在使用表
                    if (list.Contains(item))
                    {
                        item.Reset();
                        // 移除
                        list.Remove(item);
                        // 加入空闲表
                        _freeMap_NotNeat[type].Add(item);
                        suc = true;
                    }

                }
                else
                {
                    item.Reset();
                }
            }
            return suc;
        }

        /// <summary>
        /// 检查对象在对象池中是否是空闲状态
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckItemIsFree(IObjectPoolItem item)
        {

            bool result = false;
            if (_mode == ObjectPoolMode.NEAT)
            {
                if (_freeMap_Neat.ContainsKey(item))
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 检测对象是否属于此对象池
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool CheckItemBelongTo(IObjectPoolItem item)
        {
            bool result = false;
            if (_mode == ObjectPoolMode.NEAT)
            {
                result = _usedMap_Neat.ContainsKey(item) || _freeMap_Neat.ContainsKey(item);
            }
            return result;
        }

        /// <summary>
        /// 容量
        /// </summary>
        public int Capacity
        {
            get
            {
                return _capacity;
            }
            set // 重设容量
            {
                if (_mode == ObjectPoolMode.NEAT)
                {
                    // 如果设置的值小于容量，尝试最大范围内的缩容
                    if (value < _capacity)
                    {
                        // 使用表计数比设置值大的话，容量设置为使用表计数
                        if (_usedMap_Neat.Count >= value)
                        {
                            _freeMap_Neat.Clear();
                            _capacity = _usedMap_Neat.Count;
                        }
                        else // 使用表计数比设置值小的话
                        {
                            int removeCount = value - _usedMap_Neat.Count;
                            // 销毁多余的对象
                            foreach (KeyValuePair<IObjectPoolItem, bool> kvp in _freeMap_Neat)
                            {
                                _freeMap_Neat.Remove(kvp.Key);
                                kvp.Key.Dispose();
                                removeCount--;
                                if (removeCount == 0)
                                {
                                    break;
                                }
                            }
                            _capacity = value;
                        }
                    }
                    else // 如果大于容量，新增对象
                    {
                        // 新增数量
                        int addCount = value - _capacity;
                        for (int i = 0; i < addCount; i++)
                        {
                            IObjectPoolItem item = CreatItem(_type);
                            _freeMap_Neat[item] = true;
                        }
                        _capacity = value;
                    }
                }
            }
        }
        public int freeCount
        {
            get { return _freeList.Count; }
        }
    }
}