using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace Present
{
    public static class Util
    {
        static Util()
        {
            /*
             * 'Application.dataPath' is full path of Assets directory.
             * Path.GetDirectoryName(path) returns parent's full path, if argument is directory's path.
             * So Path.GetDirectoryName(Application.dataPath) returns project's path.
             */
            ProjectFullPath = Path.GetDirectoryName(Application.dataPath);
            TempFullPath = Path.Combine(ProjectFullPath, "Temp");
        }

        public static string ProjectFullPath { get; private set; }
        public static string TempFullPath { get; private set; }
        public static string PresentExtension { get { return "present"; } }

        public static List<string> GetAllFiles(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                throw new ArgumentException("dirPath is null or empty!");

            List<string> result = new List<string>(Directory.GetFiles(dirPath));

            var dirs = Directory.GetDirectories(dirPath);
            Array.ForEach<string>(dirs, dir => { result.AddRange(GetAllFiles(dir)); });

            return result;
        }

        public static string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentException("fullPath is null or empty!");

            fullPath = ConvertEscapePath(fullPath);
            int assetStartIndex = fullPath.IndexOf("/Assets/");
            
            if(assetStartIndex < 0 || assetStartIndex >= fullPath.Length)
                throw new IndexOutOfRangeException("Not found Assets directory!");

            return fullPath.Substring(assetStartIndex + 1);
        }

        public static string FullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path is null or empty!");
            return Path.GetFullPath(path);
        }

        public static string ConvertEscapePath(string fullPath) { return fullPath.Replace("\\", "/"); }

        public static bool IsContainsPath(string superPath, string subPath)
        {
            if (string.IsNullOrEmpty(superPath))    throw new ArgumentException("superPath is null or empty!");
            if (string.IsNullOrEmpty(subPath))      throw new ArgumentException("subPath is null or empty!");

            return FullPath(superPath).Contains(FullPath(subPath));
        }        
    }
}