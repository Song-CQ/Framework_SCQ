/****************************************************
    文件: ExportAB.cs
    作者: Clear
    日期: 2022/5/31 17:49:51
    类型: 逻辑脚本
    功能: 导出
*****************************************************/
using FutureCore;
using FutureCore.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace FutureEditor
{
    public static class ExportABTool
    {

        private static ABConfig ABConfig;

        private static string abRoodPath;

        private static string outputPath;

        #region 全局变量

        /// <summary>
        /// 文件校验对象
        /// </summary>
        private static AssetBundleVerify versiondata;
        /// <summary>
        /// 全被依赖关系对象
        /// </summary>
        private static BeDependData allbedependData;
        /// <summary>
        /// 全依赖关系对象
        /// </summary>
        private static DependData alldependData;
        /// <summary>
        /// 文件MD5表
        /// </summary>
        private static Dictionary<string, string> filedict;
        /// <summary>
        /// AB包单包所含文件个数表
        /// </summary>
        private static Dictionary<string, int> bagdict;
        /// <summary>
        /// 全被依赖关系表
        /// </summary>
        private static Dictionary<string, List<string>> allbedependdict;
        /// <summary>
        /// 全依赖关系表
        /// </summary>
        private static Dictionary<string, List<string>> alldependdict;

      

        /// <summary>
        /// 待打包表
        /// </summary>
        private static List<AssetBundleBuild> addAssetBundleBuildList;

        #endregion

        private static List<string> tempPath = new List<string>();
        public static void Export(ABConfig aBConfig)
        {
            ABConfig = aBConfig;

            DateTime starttime = DateTime.Now;

            #region 打包前数据初始化

            filedict = new Dictionary<string, string>();
            bagdict = new Dictionary<string, int>();
            allbedependdict = new Dictionary<string, List<string>>();
            alldependdict = new Dictionary<string, List<string>>();

            addAssetBundleBuildList = new List<AssetBundleBuild>();

            //读取校验文件和被依赖信息,将其中的数据读进字典中
            versiondata = new AssetBundleVerify();
            allbedependData = new BeDependData();
            alldependData = new DependData();
            if (File.Exists(ABConfig.verifyPath))
            {
                string json = File.ReadAllText(ABConfig.verifyPath);
                versiondata = JsonUtility.FromJson<AssetBundleVerify>(json);
            }
            if (ABConfig.isIncrementalBulie)
            {
                for (int i = 0; i < versiondata.filemap.Count; i++)
                {
                    filedict.Add(versiondata.filemap[i].Path, versiondata.filemap[i].MD5);
                }
                for (int i = 0; i < versiondata.bagmap.Count; i++)
                {
                    bagdict.Add(versiondata.bagmap[i].bagName, versiondata.bagmap[i].num);
                }

                if (File.Exists(ABConfig.allBeDependPath))
                {
                    string json = File.ReadAllText(ABConfig.allBeDependPath);
                    allbedependData = JsonUtility.FromJson<BeDependData>(json);
                }
                for (int i = 0; i < allbedependData.bedepsmap.Count; i++)
                {
                    allbedependdict.Add(allbedependData.bedepsmap[i].selfbag, allbedependData.bedepsmap[i].bedepends);
                }
                if (File.Exists(ABConfig.allDependPath))
                {
                    string json = File.ReadAllText(ABConfig.allDependPath);
                    alldependData = JsonUtility.FromJson<DependData>(json);
                }
                for (int i = 0; i < alldependData.depsmap.Count; i++)
                {
                    alldependdict.Add(alldependData.depsmap[i].selfbag, alldependData.depsmap[i].depends);
                }
            }



            #endregion

            abRoodPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + ABConfig.abRoot;

            //遍历项目中AB包文件夹目录,设置里面各个文件的包名
            DirectoryInfo rootfolder = new DirectoryInfo(abRoodPath);
            //对该目录设置打包
            SetAllAssetBundles(rootfolder);

           
            Debug.Log(string.Concat("【设置AB包的打包数据】消耗时间 :", (DateTime.Now - starttime).TotalSeconds));
            //最终打包
            if (BuildAll())
            {
                Debug.Log("导出AB包成功");
                Debug.Log(string.Concat("【打包】消耗时间 ", (DateTime.Now - starttime).TotalSeconds));
                UpdateVersion();
                Debug.Log("校验文件 依赖文件 被依赖文件 保存成功");
                Debug.Log("<size=15>" + string.Concat("【总计用时】 ", (DateTime.Now - starttime).TotalSeconds) + "</size>");
            }
            BuildAssetBundleWnd.ShowNotificationTips("打包完成");


        }


        /// <summary>
        /// 打包最后一步 注意生成后的实际文件路径全是小写
        /// </summary>
        private static bool BuildAll()
        {
            try
            {
                outputPath = Path.GetFullPath(ABConfig.outputPath.Replace(@"Assets\..\..\", Application.dataPath + @"\..\..\")) + "/" + ABConfig.abPlatform.ToString();
                //检测路径是否存在，没有则创建
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                Dictionary<string, List<string>> tempAssetBuild = new Dictionary<string, List<string>>();

                foreach (var item in addAssetBundleBuildList)
                {
                    List<string> assetList;
                    if (!tempAssetBuild.TryGetValue(item.assetBundleName, out assetList))
                    {
                        assetList = new List<string>();
                    }

                    foreach (var name in item.assetNames)
                    {
                        if (!assetList.Contains(name))
                        {
                            assetList.Add(name);
                        }
                    }

                    tempAssetBuild[item.assetBundleName] = assetList;

                }

                List<AssetBundleBuild> _allBuildAssetBundleBuilds = new List<AssetBundleBuild>();

                foreach (var item in tempAssetBuild)
                {
                    for (int i = 0; i < item.Value.Count; i++)
                    {
                        item.Value[i] = item.Value[i].Replace("\\", "/").Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                    }
                    AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
                    assetBundleBuild.assetBundleName = item.Key;
                    assetBundleBuild.assetNames = item.Value.ToArray();
                    _allBuildAssetBundleBuilds.Add(assetBundleBuild);
  
                }
               

                //导出ab包 ab包存储位置，打包设置，平台
                BuildPipeline.BuildAssetBundles(outputPath, _allBuildAssetBundleBuilds.ToArray(), ABConfig.abOptions, ABConfig.abPlatform);
            }
            catch
            {
                Debug.LogError("打包出现异常");
                return false;
            }
            return true;
        }

        private static void SetAllAssetBundles(DirectoryInfo folder)
        {
            string newbagname = folder.FullName.Replace("\\", "/").Replace(@"\", "/").
                Replace(abRoodPath + "/", string.Empty).ToLower(); // abres/...
            int checknum = 0;
            bool Nofile = false;  //是否有文件

            //遍历文件夹
            foreach (DirectoryInfo child in folder.GetDirectories())
            {
                if (!Nofile) Nofile = true;
                SetAllAssetBundles(child);
            }
            //文件夹和文件不会并存
            if (Nofile) return;

            tempPath.Clear();
            //遍历文件
            foreach (FileInfo file in folder.GetFiles("*",SearchOption.TopDirectoryOnly))
            {
                //todo 排除某些文件
                if (file.Extension.Equals(".meta"))
                {
                    continue;
                }
                tempPath.Add(file.FullName.Replace(Application.dataPath,"Assets"));

                checknum++;
            }
            //该目录下没有文件
            if (checknum == 0) return;

            AddAssetBundleBuild(tempPath.ToArray(),newbagname);
        }


       

        public static bool AddAssetBundleBuild_ToDirectory(DirectoryInfo folder, string newbagname, SearchOption searchOption)
        {
            int checknum = 0;
            List<string> vs = new List<string>();
            foreach (FileInfo tar in folder.GetFiles("*", searchOption))
            {
                //todo 排除某些文件
                if (tar.Extension.Equals(".meta"))
                {
                    continue;
                }
                vs.Add(tar.FullName.Replace(Application.dataPath, "Assets/"));
                checknum++;

            }
            //可能没文件
            if (checknum == 0) return false;

            AddAssetBundleBuild(vs.ToArray(), newbagname);


            return true;
        }
     
        /// <summary>
        /// 添加入打包项目
        /// </summary>
        /// <param name="filePath">要打包的文件路径 Tips: 路径必须是 Asset\*****开头</param>
        /// <param name="buildBagName"></param>
        public static void AddAssetBundleBuild(string filePath, string buildBagName)
        {
            

            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = buildBagName;
            assetBundleBuild.assetNames = new string[] { filePath };
            addAssetBundleBuildList.Add(assetBundleBuild);
        }

        /// <summary>
        /// 添加入打包项目
        /// </summary>
        /// <param name="filePath">要打包的文件路径 Tips: 路径必须是 Asset\*****开头</param>
        /// <param name="buildBagName"></param>
        public static void AddAssetBundleBuild(string[] filePath, string buildBagName)
        {
           
            AssetBundleBuild assetBundleBuild = new AssetBundleBuild();
            assetBundleBuild.assetBundleName = buildBagName;
            assetBundleBuild.assetNames = filePath;
            addAssetBundleBuildList.Add(assetBundleBuild);
        }

        /// <summary>
        /// 更新校验文件信息,依赖信息，被依赖信息
        /// </summary>
        private static void UpdateVersion()
        {

            #region 更新校验文件数据并写入

            versiondata.filemap.Clear();
            versiondata.bagmap.Clear();
            for (int i = 0; i < filedict.Count; i++)
            {
                FileMsg filemsg = new FileMsg();
                filemsg.Path = filedict.ElementAt(i).Key;
                filemsg.MD5 = filedict.ElementAt(i).Value;
                versiondata.filemap.Add(filemsg);
            }
            for (int i = 0; i < bagdict.Count; i++)
            {
                BundleMsg bagmsg = new BundleMsg();
                bagmsg.bagName = bagdict.ElementAt(i).Key;
                bagmsg.num = bagdict.ElementAt(i).Value;
                bagmsg.MD5 = VerifyUtil.GetFileMD5(outputPath + "/" + bagmsg.bagName);
                bagmsg.size = VerifyUtil.GetFileSize(outputPath + "/" + bagmsg.bagName);

                versiondata.bagmap.Add(bagmsg);
            }
            string mainBagPath = Path.Combine(outputPath, ABConfig.abPlatform.ToString());
            //加入主包
            BundleMsg mainBagmsg = new BundleMsg()
            {
                bagName = ABConfig.abPlatform.ToString(),
                num = 1,
                MD5 = VerifyUtil.GetFileMD5(mainBagPath),
                size = VerifyUtil.GetFileSize(mainBagPath),
            };
            versiondata.bagmap.Add(mainBagmsg);

            if (ABConfig.isAutoAddVersion)
            {
                versiondata.version++;
            }
            versiondata.buildDate = DateTime.Now.ToString();
            
            string json = JsonUtility.ToJson(versiondata, true);
            FutureCore.FileUtil.WriteAllText(ABConfig.verifyPath, json);
            if (ABConfig.isImputVersion)
            {
                File.WriteAllText(outputPath + "/version.json", json);
            }

            if (ABConfig.isCopyStreamingAssets)
            {
                string tar = Application.streamingAssetsPath + "/" + ABConfig.abPlatform.ToString();
                if (Directory.Exists(tar))
                {
                    Directory.Delete(tar, true);
                }
                UnityEditor.FileUtil.CopyFileOrDirectoryFollowSymlinks(outputPath, tar);
            }

            #endregion

            #region 读取主包

            //读取打包后的主包
            AssetBundle main = AssetBundle.LoadFromFile(mainBagPath);

            AssetBundleManifest fest = main.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            #endregion

            #region 更新依赖数据 生成被依赖数据
            string[] _AllAssetBundles = fest.GetAllAssetBundles();
            alldependdict = new Dictionary<string, List<string>>();
            //更新全依赖信息数据
            for (int i = 0; i < _AllAssetBundles.Length; i++)
            {
                string bagname = _AllAssetBundles[i];
                string[] alldeps = fest.GetAllDependencies(bagname);

                //如果全依赖信息中有打出的这个包,将新的依赖信息覆盖原信息
                if (alldependdict.ContainsKey(bagname))
                {
                    alldependdict[bagname].Clear();
                    if (alldeps.Length != 0) alldependdict[bagname].AddRange(alldeps);
                }
                //如果全依赖信息中没有打出的这个包
                else
                {
                    //不记录没有依赖的包
                    if (alldeps.Length != 0)
                    {
                        List<string> temp = new List<string>();
                        temp.AddRange(alldeps);
                        alldependdict.Add(bagname, temp);
                    }
                }
            }

            //用最新的全依赖信息,生成全被依赖信息
            allbedependdict = new Dictionary<string, List<string>>();
            for (int i = 0; i < alldependdict.Count; i++)
            {
                List<string> values = alldependdict.ElementAt(i).Value;
                string key = alldependdict.ElementAt(i).Key;
                for (int p = 0; p < values.Count; p++)
                {
                    //如果这个被依赖包已经登记了
                    if (allbedependdict.ContainsKey(values[p]))
                    {
                        allbedependdict[values[p]].Add(key);
                    }
                    //如果这个被依赖包还没登记，是个新的被依赖包
                    else
                    {
                        List<string> temp = new List<string>();
                        temp.Add(key);
                        allbedependdict.Add(values[p], temp);
                    }
                }
            }

            #endregion

            #region 将依赖信息和被依赖信息写入文件

            BeDependData allbedependlist = new BeDependData();
            DependData alldependlist = new DependData();
            foreach (KeyValuePair<string, List<string>> kv in alldependdict)
            {
                DependMsg alldependmsg = new DependMsg();
                alldependmsg.selfbag = kv.Key;
                alldependmsg.depends = kv.Value;
                alldependlist.depsmap.Add(alldependmsg);
            }
            foreach (KeyValuePair<string, List<string>> kv in allbedependdict)
            {
                BeDependMsg allbedependmsg = new BeDependMsg();
                allbedependmsg.selfbag = kv.Key;
                allbedependmsg.bedepends = kv.Value;
                allbedependlist.bedepsmap.Add(allbedependmsg);
            }

            if (!File.Exists(ABConfig.allDependPath))
            {
                File.Create(ABConfig.allDependPath).Dispose();
            }
            json = JsonUtility.ToJson(alldependlist, true);
            File.WriteAllText(ABConfig.allDependPath, json);

            if (!File.Exists(ABConfig.allBeDependPath))
            {
                File.Create(ABConfig.allBeDependPath).Dispose();
            }
            json = JsonUtility.ToJson(allbedependlist, true);
            File.WriteAllText(ABConfig.allBeDependPath, json);

            #endregion



            main.Unload(true);

            AssetDatabase.Refresh();

        }

        #region Old
        /// <summary>
        /// 最终遍历，设置包名
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="bagname"></param>
        /// <param name="assetsfilepath"></param>
        /// <param name="check">是否要检测待打包表</param>
        //private static bool FinalSetName(DirectoryInfo folder, string newbagname, StringBuilder md5, bool check = true)
        //{
        //    if (check)
        //    {
        //        //将自己被依赖的和依赖的全添加到待打包表中
        //        List<string> bedeps;
        //        if (allbedependdict.TryGetValue(newbagname, out bedeps))
        //        {
        //            for (int i = 0; i < bedeps.Count; i++)
        //            {
        //                if (!addbagdict.ContainsKey(bedeps[i]))
        //                {
        //                    addbagdict.Add(bedeps[i], 0);
        //                }
        //            }
        //        }
        //        List<string> deps;
        //        if (alldependdict.TryGetValue(newbagname, out deps))
        //        {
        //            for (int i = 0; i < deps.Count; i++)
        //            {
        //                if (!addbagdict.ContainsKey(deps[i]))
        //                {
        //                    addbagdict.Add(deps[i], 0);
        //                }
        //            }
        //        }

        //    }
        //    int checknum = 0;
        //    //最终遍历该AB包的所有文件,并更新AB校验文件中的信息
        //    foreach (FileInfo tar in folder.GetFiles())
        //    {
        //        //todo 排除某些文件
        //        if (tar.Extension.Equals(".meta"))
        //        {
        //            continue;
        //        }
        //        md5.Clear();
        //        //如果dict中有当前文件的记录，进行比较
        //        if (filedict.ContainsKey(tar.FullName))
        //        {
        //            md5.Append(filedict[tar.FullName]);
        //            //如果MD5一样，说明该文件没有被修改
        //            if (VerifyUtil.CompareMD5(tar.FullName, md5))
        //            {
        //                //设置包名
        //                DealSingleABName(tar, newbagname, md5);
        //            }
        //            //如果该文件被修改了
        //            else
        //            {
        //                //设置包名
        //                DealSingleABName(tar, newbagname, md5);
        //                //更新校验文件中该文件的md5
        //                filedict[tar.FullName] = md5.ToString();
        //            }
        //        }
        //        //如果dict中没有当前文件的记录，添加信息
        //        else
        //        {
        //            //设置包名
        //            DealSingleABName(tar, newbagname, md5);
        //            //添加当前文件的校验信息
        //            filedict.Add(tar.FullName, VerifyUtil.GetFileMD5(tar.FullName));
        //        }
        //        checknum++;
        //    }
        //    //可能没文件
        //    if (checknum == 0) return false;
        //    //更新当前包的文件个数
        //    if (bagdict.ContainsKey(newbagname))
        //    {
        //        if (bagdict[newbagname] != checknum) bagdict[newbagname] = checknum;
        //    }
        //    else
        //    {
        //        bagdict.Add(newbagname, checknum);
        //    }
        //    //登记打包
        //    builtbagdict.Add(newbagname, 0);
        //    return true;
        //}



        ///// <summary>
        ///// 具体设置单个资源的包名
        ///// </summary>
        ///// <param name="file">文件对象</param>
        ///// <param name="bagname">包名</param>
        ///// <param name="assetsfilepath">用于检索文件的路径变量</param>
        //private static void DealSingleABName(FileInfo file, string newbagname, StringBuilder assetsfilepath)
        //{
        //    assetsfilepath.Clear();
        //    assetsfilepath.Append(file.FullName.Substring(file.FullName.IndexOf("Assets")));
        //    //找到文件
        //    AssetImporter res = AssetImporter.GetAtPath(assetsfilepath.ToString());
        //    //设置包名
        //    if (res != null)
        //    {
        //        res.assetBundleName = newbagname;
        //    }
        //    else
        //    {
        //        Debug.LogError("设置包名异常：" + assetsfilepath.ToString());
        //    }
        //}


        ///// <summary>
        ///// 增加,修改,修改都会会导致该文件所在包被记录
        ///// </summary>
        ///// <param name="folder"></param>
        //private static void SetAssetBundlesName(DirectoryInfo folder)
        //{
        //    string newbagname = folder.FullName.Replace("\\", "/").Replace(@"\", "/").
        //        Replace(abRoodPath + "/", string.Empty).ToLower(); // abres/...
        //    StringBuilder md5 = new StringBuilder();
        //    int checknum = 0;  //记录当前目录检测完毕的文件个数(处理文件删减情况)
        //    bool Nofile = false;  //是否有文件

        //    //遍历文件夹
        //    foreach (DirectoryInfo child in folder.GetDirectories())
        //    {
        //        if (!Nofile) Nofile = true;
        //        SetAssetBundlesName(child);
        //    }
        //    //文件夹和文件不会并存
        //    if (Nofile) return;
        //    //如果自己在待打包表中，将其去除,并直接打包
        //    if (addbagdict.ContainsKey(newbagname))
        //    {
        //        addbagdict.Remove(newbagname);
        //        if (FinalSetName(folder, newbagname, md5))
        //        {
        //            Debug.Log("设置包名：" + newbagname);
        //            bagnum++;
        //        }
        //        return;
        //    }

        //    //遍历文件
        //    foreach (FileInfo file in folder.GetFiles())
        //    {

        //        //todo 排除某些文件
        //        if (file.Extension.Equals(".meta"))
        //        {
        //            continue;
        //        }
        //        md5.Clear();
        //        //校验当前文件的更新情况
        //        //如果dict中有当前文件的记录，进行比较
        //        if (filedict.ContainsKey(file.FullName))
        //        {
        //            md5.Append(filedict[file.FullName]);
        //            //如果MD5一样，说明该文件没有被修改
        //            if (VerifyUtil.CompareMD5(file.FullName, md5.ToString()))
        //            {
        //                //取消其包名 
        //                //DealSingleABName(file, string.Empty, md5);不再使用包名打包
        //                checknum++;
        //                continue;
        //            }
        //            //如果该文件被修改了，重新打包
        //            else
        //            {
        //                md5.Clear();
        //                if (FinalSetName(folder, newbagname, md5))
        //                {
        //                    Debug.Log("设置包名：" + newbagname);
        //                    bagnum++;
        //                }
        //                //结束对该目录的操作
        //                return;
        //            }
        //        }
        //        //该文件是新文件,直接进行对该AB包设置名字
        //        else
        //        {
        //            md5.Clear();
        //            if (FinalSetName(folder, newbagname, md5))
        //            {
        //                Debug.Log("设置包名：" + newbagname);
        //                bagnum++;
        //            }
        //            //结束对该目录的操作
        //            return;
        //        }
        //    }
        //    //该目录下没有文件
        //    if (checknum == 0) return;
        //    //走到这里说明当前目录所有文件都没被修改,因此检测是否有文件删减
        //    if (bagdict.ContainsKey(newbagname))
        //    {
        //        if (bagdict[newbagname] != checknum)
        //        {
        //            md5.Clear();
        //            if (FinalSetName(folder, newbagname, md5))
        //            {
        //                Debug.Log("设置包名：" + newbagname);
        //                bagnum++;
        //            }
        //        }
        //    }
        //    //理论上走不到这个分支
        //    else
        //    {
        //        bagdict.Add(newbagname, checknum);
        //        md5.Clear();
        //        if (FinalSetName(folder, newbagname, md5))
        //        {
        //            Debug.Log("设置包名：" + newbagname);
        //            bagnum++;
        //        }
        //    }
        //}
        #endregion

    }
}