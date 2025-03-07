/****************************************************
    文件：ConfigVO.cs
	作者：Clear
    日期：#CreateTime#
    类型: 工具自动创建(请勿修改)
	功能：表类型 枚举类
*****************************************************/
using System;
using System.Collections.Generic;

namespace ProjectApp.Data
{
    public enum ConfigVO
    {
        #ConfigVO
    }

    public class ConfigData
    {
        public Dictionary<uint, BaseStaticVO> configStaticVODic = new Dictionary<uint, BaseStaticVO>();
        public Dictionary<uint, BaseVO[]> configVODic = new Dictionary<uint, BaseVO[]>();


        public void Dispose()
        {
            configStaticVODic.Clear();
            configVODic.Clear();


            configVODic = null;
            configStaticVODic = null;

        }
    }
}
        