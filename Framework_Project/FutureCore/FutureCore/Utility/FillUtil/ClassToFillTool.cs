/****************************************************
    文件：SaveSvc.cs
	作者：承清
    邮箱: 2728285639@qq.com
    日期：2020/9/14   9:21
	功能：类转文件工具
*****************************************************/
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace FutureCore
{
	/// <summary>
	/// 类转文件工具
	/// </summary>
	public static class ClassToFillTool
	{
		/// <summary>
		/// 路径
		/// </summary>
		public enum FolderPath
		{
			StreamingAssetsPath,
			AssetsPath,
		}

		/// <summary>
		/// 将文件转换为类
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="FolderPath">文件路径</param>
		/// <param name="FolderName">文件名</param>
		/// <returns></returns>
		public static T LoadFileToClass<T>(FolderPath FolderPath, string FolderName) where T:class
		{
		    string path = FolderPathToPath(FolderPath);

			if (path == string.Empty) return null;
			path += "/" + FolderName;

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
					LogUtil.Log("加载失败,该文件不存在");
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
		/// 将文件转换为类
		/// </summary>
		/// <typeparam name="T">类型</typeparam>
		/// <param name="FolderPath">文件路径</param>
		/// <param name="FolderName">文件名</param>
		/// <returns></returns>
		public static T LoadFileToClass<T>(string FolderPath, string FolderName) where T : class
		{
			string path = FolderPath;

			if (path == string.Empty) return null;
			path += "/" + FolderName;

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
					Debug.LogError("该文件不存在");
					return null;
				}
			}
			catch (System.Exception)
			{
				Debug.LogError("加载失败");
				return null;
			}

		}

		/// <summary>
		/// 将类转换为文件保存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="FolderPath">路径名</param>
		/// <param name="FolderName">将保存的文件名</param>
		/// <param name="this_Class">保存的类</param>
		/// <param name="isReplace">如果已经存在同名文件是否替换该文件</param>
		/// <returns></returns>
		public static bool SaveClassToFile<T>(FolderPath FolderPath, string FolderName,T this_Class,bool isReplace=true) where T:class
		{
			string path = FolderPathToPath(FolderPath);

			if (path == string.Empty) return false;
			path += "/" + FolderName;

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

				Debug.LogError("保存失败");
				return false;
			}


		}

		/// <summary>
		/// 将类转换为文件保存
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="FolderPath">路径名</param>
		/// <param name="FolderName">将保存的文件名</param>
		/// <param name="this_Class">保存的类</param>
		/// <param name="isReplace">如果已经存在同名文件是否替换该文件</param>
		/// <returns></returns>
		public static bool SaveClassToFile<T>(string FolderPath, string FolderName, T this_Class, bool isReplace = true) where T : class
		{
			string path = FolderPath;

			if (path == string.Empty) return false;
			path += "/" + FolderName;

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

				Debug.LogError("保存失败");
				return false;
			}


		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		/// <param name="FolderPath">文件夹路径</param>
		/// <param name="FolderName">文件夹名字</param>
		/// <returns></returns>
		public static bool CreateFolder(FolderPath FolderPath, string FolderName)
		{
			string path = FolderPathToPath(FolderPath);

			if (path == string.Empty) return false;

			path += "/" + FolderName;

			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
					return true;
				}
				else
				{
					Debug.Log("创建失败,已有同名文件夹");
					return false;
				}
			}
			catch (System.Exception)
			{
				Debug.LogError("创建失败");
				return false;
			}



		}

		private static string FolderPathToPath(FolderPath FolderPath)
		{
			string path = string.Empty;

			switch (FolderPath)
			{
				case FolderPath.StreamingAssetsPath:
#if UNITY_EDITOR
					path = Application.streamingAssetsPath;
#elif UNITY_IOS
					path = Application.persistentDataPath ;
#endif
#if UNITY_ANDROID
					path = Application.persistentDataPath;
#endif

					break;
				case FolderPath.AssetsPath:
					path = Application.dataPath;
					break;
			}

			return path;
		}

		/// <summary>
		/// 创建文件夹
		/// </summary>
		/// <param name="FolderPath">文件夹路径</param>
		/// <param name="FolderName">文件夹名字</param>
		/// <returns></returns>
		public static bool CreateFolder(string FolderPath, string FolderName)
		{

			string path = FolderPath;

			if (path == string.Empty) return false;

			path += "/" + FolderName;

			try
			{
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
					return true;
				}
				else
				{
					Debug.LogError("创建失败,已有同名文件夹");
					return false;
				}
			}
			catch (System.Exception)
			{
				Debug.LogError("创建失败");
				return false;
			}



		}


		public static bool SaveTxt(string txt, FolderPath path, string txtName)
		{
			string pathStr = FolderPathToPath(path)+"/"+ txtName;
			try
			{
				File.WriteAllText(pathStr, txt, Encoding.UTF8);
#if UNITY_EDITOR
				UnityEditor.AssetDatabase.Refresh();
#endif
			}
			catch (System.Exception)
			{
				return false;
			}
			return true;
		}

		public static string ReadTxt(FolderPath path, string txtName)
		{
			string pathStr = FolderPathToPath(path) + "/" + txtName;
			string txtStr = string.Empty;
			LogUtil.Log(pathStr);
			try
			{
				txtStr = File.ReadAllText(pathStr, Encoding.UTF8);
			}
			catch (System.Exception e)
			{
				LogUtil.LogError(e);
			}
			
			return txtStr;

		}

	}
	
}

