/****************************************************
    文件：FileUtil.cs
	作者：Clear
    日期：2022/2/3 18:34:13
    类型: 框架核心脚本(请勿修改)
	功能：文件Util
*****************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace FutureCore
{
    public static class FileUtil 
    {
        

        public static bool DeleteFileOrDirectory(string path)
        {  
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            if (Directory.Exists(path))
            {
                Directory.Delete(path,true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 拷贝文件夹
        /// </summary>
        /// <param name="strFromPath">源文件夹</param>
        /// <param name="strToPath">复制的目标文件夹</param>
        /// <param name="NewName">复制的 新文件夹是否要取新名字（不填为原文件夹名字）</param>
        /// <param name="searchPattern"> 设置要复制文件类型 (*为所有 （.cs|.txt）为.cs和.txt) </param>
        /// <param name="copyFileCallback">复制一个文件成功回调 (sourceFileName,destFileName) </param>
        public static void CopyFolder(string strFromPath, string strToPath,string NewName,string searchPattern = "*",Action<string,string> copyFileCallback = null)
        {
            //如果源文件夹不存在，则返回
            if (!Directory.Exists(strFromPath))
            {
                return;
            }
            //取得要拷贝的文件夹名
            Path.GetDirectoryName(strFromPath);

            string strFolderName;
            if (NewName == "")
            {
                strFolderName = Path.GetFileName(strFromPath);
            }
            else
            {
                strFolderName = NewName;
            }
           


            //如果目标文件夹中没有源文件夹则在目标文件夹中创建源文件夹
            if (!Directory.Exists(strToPath + "/" + strFolderName))
            {
                Directory.CreateDirectory(strToPath + "/" + strFolderName);
            }
            //创建数组保存源文件夹下的文件名
            string[] strFiles = Directory.GetFiles(strFromPath,searchPattern);
          
           
            
            //循环拷贝文件
            for (int i = 0; i < strFiles.Length; i++)
            {
                //取得拷贝的文件名，只取文件名，地址截掉。
                //string strFileName = strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1, strFiles[i].Length - strFiles[i].LastIndexOf("\\") - 1);

                string strFileName = strToPath + "/" + strFolderName + "/" + Path.GetFileName(strFiles[i]);
                if (copyFileCallback == null)
                {
                    //开始拷贝文件,true表示覆盖同名文件
                    File.Copy(strFiles[i], strFileName, true);
                }
                else
                {
                    copyFileCallback.Invoke(strFiles[i], strFileName);
                }
                
               
            }
            //创建DirectoryInfo实例
            DirectoryInfo dirInfo = new DirectoryInfo(strFromPath);
            //取得源文件夹下的所有子文件夹名称
            DirectoryInfo[] ZiPath = dirInfo.GetDirectories();
            for (int j = 0; j < ZiPath.Length; j++)
            {
                //获取所有子文件夹名
                string strZiPath = ZiPath[j].FullName;
                
                //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
                CopyFolder(strZiPath, strToPath + "/" + strFolderName, "",searchPattern,copyFileCallback);
            }
        }

        public static void CopyFile(string source, string dest,bool overwrite =true)
        {
            if (!File.Exists(source))
            {
                LogUtil.LogError("源文件不存在"+ source);
                return ;
            }
            File.Copy(source,dest,overwrite);
        }

        public static void CopyFolder(string strFromPath, string strToPath,string searchPattern, Action<string, string> copyFileCallback = null)
        {
            CopyFolder(strFromPath, strToPath, "", searchPattern, copyFileCallback);
        }

        public static void CopyFolder(string strFromPath, string strToPath,Action<string, string> copyFileCallback)
        {
            CopyFolder(strFromPath, strToPath, "", "*", copyFileCallback);
        }
        public static void CopyFolder(string strFromPath, string strToPath)
        {
            CopyFolder(strFromPath, strToPath, "", "*", null);
        }

        /// <summary>
        /// 创建该文件上级目录
        /// </summary>
        /// <param name="filePath"></param>
        public static void CreadFileLastDirectory(string filePath)
        {
            string strFromPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(strFromPath))
            {
                Directory.CreateDirectory(strFromPath);
            }
        }
        /// <summary>
        /// 文件写入
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="allStr">要写入的str</param>
        /// <param name="isCread">如果该文件不存在是否要创建</param>
        public static void WriteAllText(string filePath,string allStr,bool isCreateNew = false)
        {
            string direName = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(direName)) Directory.CreateDirectory(direName);


            if (!File.Exists(filePath))
            {
                if (isCreateNew)
                {
                    File.Create(filePath).Close();
                }
                else
                {
                    LogUtil.LogError("目标文件不存在:"+ filePath);
                    return;
                }
                
            } 

            File.WriteAllText(filePath,allStr, new UTF8Encoding(false));
        }

        /// <summary>
        /// 将文件转换为类
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static T LoadFileToClass<T>(string FilePath) where T : class
        {
            string path = FilePath;

            try
            {
                if (File.Exists(path))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(path, FileMode.Open);
                    T t = (T)bf.Deserialize(file);
                    file.Close();
                    return t;
                }
                else
                {
                    LogUtil.LogError("该文件不存在");
                    return null;
                }
            }
            catch (System.Exception)
            {
                LogUtil.LogError("加载失败");
                return null;
            }

        }
        /// <summary>
		/// 将类转换为文件保存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="FilePath">路径名</param>
		/// <param name="this_Class">保存的类</param>
		/// <param name="isReplace">如果已经存在同名文件是否替换该文件</param>
		/// <returns></returns>
		public static bool SaveClassToFile<T>(string FilePath, T this_Class, bool isReplace = true) where T : class
        {
            string path = FilePath;

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (File.Exists(path))
                {
                    if (isReplace)
                    {
                        File.Delete(path);
                    }
                    else
                    {
                        return false;
                    }
                }
                FileStream file = File.Create(path);
                bf.Serialize(file, this_Class);
                file.Close();
                return true;
            }
            catch (System.Exception)
            {

                LogUtil.LogError("保存失败");
                return false;
            }


        }


    }
}