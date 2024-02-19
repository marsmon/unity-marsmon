
using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace YF
{
    public static class FileUtil
    {
        public const string AssetsFolderName = "Assets";

        public static string FormatToUnityPath(string path)
        {
            return path.Replace("\\", "/");
        }

        public static string FormatToSysFilePath(string path)
        {
            return path.Replace("/", "\\");
        }
        
        public static string FullPathToAssetPath(string full_path)
        {
            full_path = FormatToUnityPath(full_path);
            if (!full_path.StartsWith(Application.dataPath))
            {
                return null;
            }
            string ret_path = full_path.Replace(Application.dataPath, "");
            return AssetsFolderName + ret_path;
        }

        public static string GetFileExtension(string path)
        {
            return Path.GetExtension(path).ToLower();
        }

        public static string[] GetSpecifyFilesInFolder(string path, string[] extensions = null, bool exclude = false)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return null;
            }

            if (extensions == null)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            }
            else if (exclude)
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => !extensions.Contains(GetFileExtension(f))).ToArray();
            }
            else
            {
                return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                    .Where(f => extensions.Contains(GetFileExtension(f))).ToArray();
            }
        }

        public static string[] GetSpecifyFilesInFolder(string path, string pattern)
        {
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
        }

        public static string[] GetAllFilesInFolder(string path)
        {
            return GetSpecifyFilesInFolder(path);
        }

        public static string[] GetAllDirsInFolder(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories);
        }

        public static byte[] ReadByteArray(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile) || !File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static byte[] SafeReadAllBytes(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllBytes(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllBytes failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        public static string[] SafeReadAllLines(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllLines(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllLines failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        public static string SafeReadAllText(string inFile)
        {
            try
            {
                if (string.IsNullOrEmpty(inFile))
                {
                    return null;
                }

                if (!File.Exists(inFile))
                {
                    return null;
                }

                File.SetAttributes(inFile, FileAttributes.Normal);
                return File.ReadAllText(inFile);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(string.Format("SafeReadAllText failed! path = {0} with err = {1}", inFile, ex.Message));
                return null;
            }
        }

        public static void CreateFile(string path, string info)
        {
            FileInfo t = new FileInfo(path);
            if (t.Exists)
            {
                return;
            }

            StreamWriter sw = t.CreateText();
            sw.WriteLine(info);
            sw.Close();
            sw.Dispose();
        }

        public static string LoadFile(string path)
        {
            StreamReader streamReader = null;

            try
            {
                streamReader = File.OpenText(path);
                
                return streamReader.ReadToEnd();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
            finally
            {
                streamReader.Close();
            }
        }

        public static void WriteFile(string path, string content)
        {
            FileStream fileStream = null;
            StreamWriter streamWriter = null;

            try
            {
                fileStream = new FileStream(path, FileMode.OpenOrCreate);
                streamWriter = new StreamWriter(fileStream);
                streamWriter.Write(content);
            }
            finally
            {
                streamWriter.Close();
                fileStream.Close();
            }
        }

        public static void CheckFileAndCreateDirWhenNeeded(string filePath, bool clearDir = false)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            FileInfo file_info = new FileInfo(filePath);
            DirectoryInfo dir_info = file_info.Directory;

            if (clearDir) {
                SafeClearDir(dir_info.FullName);
                
            } else {
                if (!dir_info.Exists)
                {
                    Directory.CreateDirectory(dir_info.FullName);
                }
            }
        }

        public static void CheckDirAndCreateWhenNeeded(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath))
            {
                return;
            }

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }

        public static bool SafeWriteAllBytes(string outFile, byte[] outBytes)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllBytes(outFile, outBytes);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeWriteAllBytes failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        public static bool SafeWriteAllLines(string outFile, string[] outLines)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllLines(outFile, outLines);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeWriteAllLines failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        public static bool SafeWriteAllText(string outFile, string text)
        {
            try
            {
                if (string.IsNullOrEmpty(outFile))
                {
                    return false;
                }

                CheckFileAndCreateDirWhenNeeded(outFile);
                if (File.Exists(outFile))
                {
                    File.SetAttributes(outFile, FileAttributes.Normal);
                }
                File.WriteAllText(outFile, text);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeWriteAllText failed! path = {0} with err = {1}", outFile, ex.Message));
                return false;
            }
        }

        public static void CreateDictionary(string path)
        {
            Directory.CreateDirectory(path);
        }

        public static void DeleteDictionaryContent(string path)
        {
            foreach (string d in Directory.GetFileSystemEntries(path))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    {
                        fi.Attributes = FileAttributes.Normal;
                    }

                    File.Delete(d);//直接删除其中的文�? 
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        //递归删除子文件夹
                        DeleteDictionaryContent(d1.FullName);
                    }

                    Directory.Delete(d);
                }
            }
        }
        
        public static void DeleteDirectory(string dirPath)
        {
            string[] files = Directory.GetFiles(dirPath);
            string[] dirs = Directory.GetDirectories(dirPath);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(dirPath, false);
        }

        public static void InnerSafeCopyDir(string source, string destination,string searchPattern="*") {
            DirectoryInfo directory = new DirectoryInfo(source);
            FileInfo[] files = directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
            for(int i=0;i<files.Length;i++) {
                FileInfo tFile = files[i];
                // UnityEngine.Debug.Log("copy from {0} to {1}", Path.Combine(source, tFile.Name), Path.Combine(destination, tFile.Name));
                SafeCopyFile(Path.Combine(source, tFile.Name), Path.Combine(destination, tFile.Name));
            }

            DirectoryInfo[] directorys = directory.GetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
            for(int i=0;i<directorys.Length;i++) {
                DirectoryInfo tDirectory = directorys[i];
                InnerSafeCopyDir(Path.Combine(source, tDirectory.Name), Path.Combine(destination, tDirectory.Name),searchPattern);
            }
        }

        public static bool SafeCopyDir(string source, string destination,string searchPattern="*") {
            try
            {
                InnerSafeCopyDir(source, destination,searchPattern);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeCopyDir failed! source = {0} destination = {1} with err = {2}", source, destination, ex.Message));
                return false;
            }
        }

        public static bool SafeClearDir(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }
                Directory.CreateDirectory(folderPath);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeClearDir failed! path = {0} with err = {1}", folderPath, ex.Message));
                return false;
            }
        }

        public static bool SafeDeleteDir(string folderPath)
        {
            try
            {
                if (string.IsNullOrEmpty(folderPath))
                {
                    return true;
                }

                if (Directory.Exists(folderPath))
                {
                    DeleteDirectory(folderPath);
                }
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeDeleteDir failed! path = {0} with err: {1}", folderPath, ex.Message));
                return false;
            }
        }

        public static bool SafeDeleteFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    return true;
                }

                if (!File.Exists(filePath))
                {
                    return true;
                }
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeDeleteFile failed! path = {0} with err: {1}", filePath, ex.Message));
                return false;
            }
        }

        public static bool SafeRenameFile(string sourceFileName, string destFileName)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceFileName))
                {
                    return false;
                }

                if (!File.Exists(sourceFileName))
                {
                    return true;
                }
                File.SetAttributes(sourceFileName, FileAttributes.Normal);
                CheckFileAndCreateDirWhenNeeded(destFileName);
                File.Move(sourceFileName, destFileName);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeRenameFile failed! path = {0} with err: {1}", sourceFileName, ex.Message));
                return false;
            }
        }

        public static bool SafeCopyFile(string fromFile, string toFile)
        {
            try
            {
                if (string.IsNullOrEmpty(fromFile))
                {
                    return false;
                }

                if (!File.Exists(fromFile))
                {
                    return false;
                }
                CheckFileAndCreateDirWhenNeeded(toFile);
                if (File.Exists(toFile))
                {
                    File.SetAttributes(toFile, FileAttributes.Normal);
                }
                File.Copy(fromFile, toFile, true);
                return true;
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError(string.Format("SafeCopyFile failed! formFile = {0}, toFile = {1}, with err = {2}",
                    fromFile, toFile, ex.Message));
                return false;
            }
        }

        public static bool SafeCopyFileMobile(string fromUrl, string toFilePath, Action<float> progressCallback, Action completeCallback)
        {
            try {
                UnityWebRequest uwr = new UnityWebRequest(fromUrl, UnityWebRequest.kHttpVerbGET);
                uwr.downloadHandler = new DownloadHandlerFile(toFilePath);
                uwr.SendWebRequest();

                while(!uwr.isDone) {
                    if (progressCallback != null) {
                        progressCallback(uwr.downloadProgress);
                    }
                }

                if (completeCallback != null) {
                    completeCallback();
                }

                return true;
            } catch (System.Exception ex) {
                UnityEngine.Debug.LogError(string.Format("SafeCopyFileMobile failed! fromUrl = {0}, toFilePath = {1}, with err = {2}",
                    fromUrl, toFilePath, ex.Message));
                return false;
            }
        }

        public static void CopyFolder(string srcPath, string tarPath)
        {
            if (!Directory.Exists(srcPath))
            {
                Directory.CreateDirectory(srcPath);
            }

            if (!Directory.Exists(tarPath))
            {
                Directory.CreateDirectory(tarPath);
            }

            CopyAllFile(srcPath, tarPath);

            string[] directionName = Directory.GetDirectories(srcPath);
            foreach (string dirPath in directionName)
            {
                string directionPathTemp = tarPath + "\\" + dirPath.Substring(srcPath.Length + 1);
                CopyFolder(dirPath, directionPathTemp);
            }
        }

        public static void CopyAllFile(string srcPath, string tarPath)
        {
            string[] filesList = Directory.GetFiles(srcPath);
            foreach (string f in filesList)
            {
                string fTarPath = tarPath + "\\" + f.Substring(srcPath.Length + 1);
                if (File.Exists(fTarPath))
                {
                    File.Copy(f, fTarPath, true);
                }
                else
                {
                    File.Copy(f, fTarPath);
                }
            }
        }

        public static bool CopyFile(string pathFrom, string pathTo)
        {
            if (!File.Exists(pathFrom))
            {
                return false;
            }

            File.Copy(pathFrom, pathTo, true);

            return true;
        }
    }
}