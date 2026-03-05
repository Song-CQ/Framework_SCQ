using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using FutureCore;

namespace ActionGameLibrary
{
    public class ComboInput : IDisposable, IDoCombo
    {
        public const int idleOutTimeFrame = 600;
        protected static Combo_ObjectPool<JudgeData> pool;
        protected static Combo_ObjectPool<ComboResultData> resultPool;
        protected static List<ComboInput> comboInputList;
        protected static JudgeData _judgeData = new JudgeData();
        protected ComboResultDataCompare comboResultDataCompare = new ComboResultDataCompare();
        //protected ArrayList _reserveList;
        protected List<ICombo> _reserveList;

        protected List<string> _keyCode;
        protected List<string> _keyLabel;
        protected Dictionary<string, string> _codeMap;
        protected Dictionary<string, bool> _keyState;//加日志，记录上次的键盘状态。如果上帧是down，这次是up，才算up；反之亦然。
        protected List<ComboData> _comboList;
        protected List<JudgeData> _judgeList;
        protected List<ComboResultData> _resultList;
        protected Dictionary<string, int> _resultKeyMap;
        private bool _exclusionMode = false;
        private bool _disposed = false;

        private List<ComboResultData> _tempResultList;
        private List<JudgeData> _tempJudgeList;

        protected float _joyAngle = 0;
        protected float _joyPower = 0;

        protected int _idleOutTimeFrame;
        protected bool _isIdle = false;
        protected string _lastCombo;
        // 可用性
        public bool enable = true;
        // 断网锁
        public bool netLock = false;
        // 饭庄方向
        public bool reverseDirection = false;

        public float actionSpeed = 1f;


        public class ComboResultDataCompare : IComparer<ComboResultData>
        {
            int IComparer<ComboResultData>.Compare(ComboResultData x, ComboResultData y)
            {
                return x.priority - y.priority;
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="exclusionMode">互斥模式</param>
        public ComboInput(bool exclusionMode)
        {
            _exclusionMode = exclusionMode;
            _reserveList = new List<ICombo>(2);
            _codeMap = new Dictionary<string, string>();
            _keyState = new Dictionary<string, bool>();
            _comboList = new List<ComboData>();
            _judgeList = new List<JudgeData>();
            _resultList = new List<ComboResultData>();
            _resultKeyMap = new Dictionary<string, int>(10);

            _tempResultList = new List<ComboResultData>(10);
            _tempJudgeList = new List<JudgeData>(5);
            //
            if (pool == null)
            {
                pool = new Combo_ObjectPool<JudgeData>(ObjectPoolMode.NEAT, 1000, 500);
                resultPool = new Combo_ObjectPool<ComboResultData>(ObjectPoolMode.NEAT, 500, 20);
            }
            if (comboInputList == null)
            {
                comboInputList = new List<ComboInput>();
            }
            comboInputList.Add(this);
            _idleOutTimeFrame = idleOutTimeFrame;
        }

        public ComboInput()
            : this(false)
        {
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (comboInputList.Contains(this))
            {
                comboInputList.Remove(this);
            }
            if (comboInputList.Count == 0)
            {
                comboInputList = null;
                pool.Dispose();
                pool = null;

                resultPool.Dispose();
                resultPool = null;
            }
        }


        public void ResetIdle()
        {
            _idleOutTimeFrame = idleOutTimeFrame;
            _isIdle = false;
        }

        /// <summary>
        /// 设置键控代码列表
        /// 注意此列表和键控标签列表的元素是相对应的
        /// </summary>
        /// <param name="list"></param>
        public void SetKeyCode(List<string> list)
        {
            _keyCode = list;
        }

        public List<string> GetKeyCode()
        {
            return _keyCode;
        }

        /// <summary>
        /// 设置键控标签列表
        /// 注意此列表和键控代码列表的元素是相对应的
        /// </summary>
        /// <param name="list"></param>
        public void SetKeyLabel(List<string> list)
        {
            _keyLabel = list;
            _codeMap.Clear();
            for (int i = 0; i < _keyCode.Count; i++)
            {
                _codeMap[_keyLabel[i]] = _keyCode[i];
            }
            // 添加通配符
            _codeMap["*"] = "-1";
        }

        /// <summary>
        /// 添加一个combo
        /// 格式参照 : left(KD)=0-0,left(KU)=0-10,left(KD)=1-12
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="priority"></param>
        /// <param name="extendFrame"></param>
        public void AddCombo(string combo, int priority, int extendFrame)
        {
            ComboData cd = GetComboData(combo, priority, extendFrame);
            if (cd != null)
            {
                _comboList.Add(cd);
            }
        }

        public void AddCombo(string combo, int priority)
        {
            AddCombo(combo, priority, 0);
        }

        /// <summary>
        /// 获取comboData对象
        /// </summary>
        /// <param name="combo"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public ComboData GetComboData(string combo, int priority)
        {
            return GetComboData(combo, priority, 0);
        }

        public ComboData GetComboData(string combo, int priority, int extendFrame)
        {
            ComboData cd = new ComboData();
            cd.combo = combo;
            cd.priority = priority;
            cd.extendFrame = extendFrame;
            string[] arr = combo.Split(new char[] { ',' });
            for (int i = 0; i < arr.Length; i++)
            {
                string partStr = arr[i];
                int idx1 = partStr.IndexOf("(");
                // by a5
                if (idx1 == -1)
                {
                   LogUtil.LogError("combo string error >> " + combo);
                    return null;
                }
                string label = partStr.Substring(0, idx1 - 0);
                string code = "";
                if (_codeMap.ContainsKey(label))
                {
                    code = _codeMap[label];
                }
                else
                {
                    LogUtil.LogError("label is not in map !!  " + combo);
                }

                int type = ComboInput.GetInputType(partStr.Substring(idx1 + 1, 2));
                //
                int idx2 = partStr.IndexOf("=") + 1;
                string[] lh = partStr.Substring(idx2).Split(new char[] { '-' });

                int low = int.Parse(lh[0]);
                int high = int.Parse(lh[1]);
                int duration = 0;
                if (lh.Length > 2)
                    duration = int.Parse(lh[2]);
                InputOrderData iod = new InputOrderData(code, label, type, low, high, duration);
                cd.ordweList.Add(iod);
                // trace(iod);
            }
            return cd;
        }

        /// <summary>
        /// 获取输入类型
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetInputType(string str)
        {
            int inputType = 0;
            switch (str)
            {
                case "KD":
                    inputType = 1;
                    break;
                case "KU":
                    inputType = 2;
                    break;
                case "KH":
                    inputType = 3;
                    break;
                case "KW":
                    inputType = 4;
                    break;
            }
            return inputType;
        }

        /// <summary>
        /// 获取键值
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public string GetKeyByLabel(string label)
        {
            return _codeMap[label];
        }

        /// <summary>
        /// 判断某个键是否按下
        /// </summary>
        /// <param name="label">键标签</param>
        /// <returns></returns>
        public bool IsDown(string label)
        {
            return KeyIsDown(_codeMap[label]);
        }

        public virtual bool KeyIsDown(string key)
        {
            //强制关闭了输入
            if (!enable || netLock) return false;
            return KeyState.IsDown(key, reverseDirection);
        }

        /// <summary>
        /// 记录按键状态
        /// </summary>
        public void RecordKeyState()
        {
            for (int i = 0; i < _keyCode.Count; i++)
            {
                string code = _keyCode[i];
                _keyState[code] = KeyIsDown(code);
            }
        }

        /// <summary>
        /// 更新摇杆参数
        /// </summary>
        public virtual void UpdateJoy()
        {
            Vector2 agl = Vector2.zero;
            bool up = _keyState[_keyCode[0]];
            bool down = _keyState[_keyCode[1]];
            bool left = _keyState[_keyCode[2]];
            bool right = _keyState[_keyCode[3]];
            if (up) agl.y += 1;
            if (down) agl.y -= 1;
            if (right) agl.x += 1;
            if (left) agl.x -= 1;
            _joyAngle = Vector2.Angle(Vector2.right, agl);
            if (agl.y < 0) _joyAngle = 360 - _joyAngle;
            _joyAngle *= -1;
            _joyPower = agl.magnitude != 0 ? 1 : 0;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            // 检测新启动的combo条目添加到验证条目到列表
            List<JudgeData> tempJudgeList = AddJudgeData();
            // combo规则判断，已有的，判断到了一半的ordweList
            DoJudge();
            // 合并新旧JudgeData
            if (tempJudgeList != null)
            {
                _judgeList.AddRange(tempJudgeList);
            }
            // 记录按键状态
            RecordKeyState();
            UpdateJoy();
            // 键盘和虚拟joy会共同生效，有按键被按下的时候，会广播joy数据
            if (tempJudgeList.Count > 0 || _resultList.Count > 0)
            {
                NoticeJoyData(_joyAngle, _joyPower);
            }
            if (_resultList.Count > 0)
            {
                _idleOutTimeFrame = idleOutTimeFrame;
                _isIdle = false;
            }
            // 更新结果
            if (!_exclusionMode)
            {
                Notice();
            }
            else
            {
                Notice_exclusion();
            }
            // 闲置超时
            if (_idleOutTimeFrame == 0)
            {
                _isIdle = true;
            }
            _idleOutTimeFrame--;
        }

        /// <summary>
        /// 添加判断数据
        /// </summary>
        /// <returns></returns>
        protected List<JudgeData> AddJudgeData()
        {
            _tempJudgeList.Clear();
            for (int i = 0; i < _comboList.Count; i++)
            {
                ComboData cd = _comboList[i];
                _judgeData.Reset();
                //JudgeData jd = new JudgeData();
                _judgeData.comboData = cd;
                JudgeData judgeData = _judgeData;
                bool needAdd = false;

                /*因为可能同时按下多个键，故此处用了一个while，允许一次验证通过多个key
                 * 从第一个键开始验证，
                 * 如果满足了整个List，则立刻加入并停止 (1)
                 * 如果满足了一部分，下一个key仍未过期属于未到时间限制的情况(2)，或者蓄力（按住n帧）的状态（3)，则加入一个特别的队列
                 */
                while (true)
                {
                    // 返回1為符合定義的combo
                    int ret = JudgeInput(_judgeData);
                    if (ret == 1)
                    {
                        _judgeData.step++;
                        needAdd = true;
                        if (_judgeData.step >= _judgeData.comboData.ordweList.Count)
                        {
                            ComboResultData crd = resultPool.GetItem() as ComboResultData;
                            crd.Fill(_judgeData.comboData);
                            _resultList.Add(crd);//(1)
                            break;
                        }
                    }
                    // 返回0為未達成combo，但是還未超出combo限制的時間和條件範圍
                    else if (ret == 0)
                    {
                        if (needAdd)
                        {
                            judgeData = pool.GetItem() as JudgeData;
                            judgeData.FillData(_judgeData);
                            _tempJudgeList.Add(judgeData);//(2)
                        }
                        break;
                    }
                    // 返回2為達成蓄力起始條件
                    else if (ret == 2)
                    {
                        judgeData = pool.GetItem() as JudgeData;
                        judgeData.FillData(_judgeData);
                        _tempJudgeList.Add(judgeData);//(3)
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                judgeData.runFrame++;
            }
            return _tempJudgeList;
        }

        protected void DoJudge()
        {
            for (int i = 0; i < _judgeList.Count; i++)
            {
                JudgeData jd = _judgeList[i];
                jd.countReady = true;
                //此处循环为了找到可能在同一时间完成的组合键
                while (true)
                {
                    int ret = JudgeInput(jd);
                    if (ret == 1)
                    {
                        jd.step++;
                        if (jd.step >= jd.comboData.ordweList.Count)
                        {
                            ComboResultData crd = resultPool.GetItem() as ComboResultData;
                            crd.Fill(jd.comboData);
                            _resultList.Add(crd);
                            _judgeList.RemoveRange(i, 1);
                            pool.ReturnItem(jd);
                            i--;
                            break;
                        }
                    }
                    else if (ret == -1)
                    {
                        _judgeList.RemoveRange(i, 1);
                        pool.ReturnItem(jd);
                        i--;
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                jd.runFrame++;
            }
        }

        /// <summary>
        /// 判断当前输入是否对应规则
        /// </summary>
        /// <param name="judgeData"></param>
        /// <returns></returns>
        protected virtual int JudgeInput(JudgeData judgeData)
        {
            int result = 0;

            //if(!enable) return result;

            InputOrderData iod = judgeData.comboData.ordweList[judgeData.step] as InputOrderData;

            if (judgeData.runFrame >= iod.lowLim && judgeData.runFrame <= iod.highLim)
            {
                bool ret = true;

                if (iod.type == InputType.KeyDown)
                {
                    //if ((_keyState.ContainsKey(iod.code)&&(!_keyState[iod.code])) && KeyIsDown(iod.code))
                    //{
                    //    result = 1;
                    //}

                    if (_keyState.TryGetValue(iod.code, out ret))
                    {
                        if (!ret && KeyIsDown(iod.code))
                        {
                            result = 1;
                        }

                    }
                }
                else if (iod.type == InputType.KeyUp)
                {
                    //if (_keyState.ContainsKey(iod.code) && _keyState[iod.code] && !KeyIsDown(iod.code))
                    //{
                    //    result = 1;
                    //}

                    if (_keyState.TryGetValue(iod.code, out ret))
                    {
                        if (ret && KeyIsDown(iod.code) == false)
                        {
                            result = 1;
                        }
                    }


                }
                else if (iod.type == InputType.KeyHold)
                {
                    //if (_keyState.ContainsKey(iod.code) && _keyState[iod.code] && KeyIsDown(iod.code))
                    //{
                    //    if (judgeData.countReady)
                    //    {
                    //        judgeData.count++;
                    //        judgeData.countReady = false;
                    //        //trace(jd.comboData.combo,jd.runFrame,jd.count);
                    //    }
                    //    result = 2;
                    //    //按下帧数大于需要持续时间时
                    //    if (judgeData.count >= iod.duration)
                    //    {
                    //        result = 1;
                    //    }
                    //}

                    if (_keyState.TryGetValue(iod.code, out ret))
                    {
                        if (ret && KeyIsDown(iod.code))
                        {
                            if (judgeData.countReady)
                            {
                                judgeData.count += actionSpeed;
                                judgeData.countReady = false;
                                //trace(jd.comboData.combo,jd.runFrame,jd.count);
                            }
                            result = 2;
                            //按下帧数大于需要持续时间时
                            if (judgeData.count >= iod.duration)
                            {
                                result = 1;
                            }
                        }
                    }
                }
                else if (iod.type == InputType.KeyWait)
                {
                    result = 1;
                }
                else
                {
                    result = -1;
                }
            }
            else
            {
                if (judgeData.runFrame < iod.lowLim)
                {
                    result = 0;
                }
                else
                {
                    result = -1;
                }
            }

            return result;
        }

        protected void Notice()
        {
            _tempResultList.Clear();
            for (int i = 0; i < _resultList.Count; i++)
            {
                ComboResultData ord = _resultList[i];
                ComboResultData rd = resultPool.GetItem() as ComboResultData;
                rd.Fill(ord);
                _tempResultList.Add(rd);

                ord.extendFrame--;
                if (ord.extendFrame <= 0)
                {
                    _resultList.RemoveAt(i);
                    resultPool.ReturnItem(ord);
                    i--;
                }
            }

            _tempResultList.Sort(comboResultDataCompare);
            for (int k = 0; k < _tempResultList.Count; k++)
            {
                ComboResultData rd = _tempResultList[k] as ComboResultData;
                _lastCombo = rd.combo;
                NoticeCombo(rd.combo);
                resultPool.ReturnItem(rd);
            }
        }

        //所有满足条件的comb组合，要按最后一个key进行分类。每个分类按照优先级排序，只执行优先级最高的那一个。
        protected void Notice_exclusion()
        {
            _resultKeyMap.Clear();
            _tempResultList.Clear();
            //
            for (int i = 0; i < _resultList.Count; i++)
            {
                ComboResultData ord = _resultList[i];
                ComboResultData rd = resultPool.GetItem() as ComboResultData;
                rd.Fill(ord);

                if (!_resultKeyMap.ContainsKey(rd.lastKeyCode))
                {
                    _resultKeyMap[rd.lastKeyCode] = _tempResultList.Count;
                    _tempResultList.Add(rd);
                }
                else
                {
                    // 在索引表裡取出lastKeyCode關聯的數據索引
                    int index = _resultKeyMap[rd.lastKeyCode];
                    ComboResultData rdIn = _tempResultList[index] as ComboResultData;
                    if (rd.priority < rdIn.priority)
                    {
                        _tempResultList[index] = rd;
                    }
                }

                ord.extendFrame--;
                if (ord.extendFrame <= 0)
                {
                    _resultList.RemoveAt(i);
                    resultPool.ReturnItem(ord);
                    i--;
                }
            }

            _tempResultList.Sort(comboResultDataCompare);

            for (int k = 0; k < _tempResultList.Count; k++)
            {
                ComboResultData rd = _tempResultList[k] as ComboResultData;
                _lastCombo = rd.combo;
                NoticeCombo(rd.combo);
                resultPool.ReturnItem(rd);
            }
        }

        /// <summary>
        /// 订阅Combo
        /// </summary>
        /// <param name="iCombo"></param>
        public void ReserveCombo(ICombo iCombo)
        {
            if (!_reserveList.Contains(iCombo))
            {
                _reserveList.Add(iCombo);
            }
        }

        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="iCombo"></param>
        public void UnreserveCombo(ICombo iCombo)
        {
            for (int i = 0; i < _reserveList.Count; i++)
            {
                if (iCombo == _reserveList[i])
                {
                    _reserveList.RemoveRange(i, 1);
                    break;
                }
            }
        }

        public void NoticeCombo(string combo)
        {
            // trace("combo:" + combo);
            for (int i = 0; i < _reserveList.Count; i++)
            {
                ICombo ic = _reserveList[i];// as ICombo;
                ic.ReceivedCombo(combo);
            }
        }

        public void NoticeJoyData(float angle, float power)
        {
            for (int i = 0; i < _reserveList.Count; i++)
            {
                ICombo ic = _reserveList[i];// as ICombo;
                ic.ReceivedJoyData(angle, power);
            }
        }

        /// <summary>
        /// 清除catch
        /// </summary>
        public void ClearCache()
        {
            for (int i = 0; i < _judgeList.Count; i++)
            {
                pool.ReturnItem(_judgeList[i]);
            }
            _judgeList.Clear();
            _resultList.Clear();
            // _keyState.Clear();
        }

        public void SetKeyState(string keyLabel, bool state)
        {
            _keyState[_codeMap[keyLabel]] = state;
        }

        public bool isIdle
        {
            get { return _isIdle; }
        }

        public string lastCombo
        {
            get { return _lastCombo; }
        }
    }
}

