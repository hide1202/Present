using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Present
{
    public static class Util
    {
        static Util()
        {
            ProjectFullPath = Application.dataPath.Replace("Assets", string.Empty);
        }

        public static string ProjectFullPath { get; private set; }
        public static string PresentExtension { get { return "present"; } }
        
        public static List<string> GetAllFiles(string dirPath)
        {
            if (string.IsNullOrEmpty(dirPath))
                throw new System.ArgumentException("dirPath is null or empty!");

            List<string> result = new List<string>(System.IO.Directory.GetFiles(dirPath));

            var dirs = System.IO.Directory.GetDirectories(dirPath);
            foreach (var dir in dirs)
            {
                result.AddRange(GetAllFiles(dir));
            }

            return result;
        }

        public static string GetRelativePath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                throw new System.ArgumentException("fullPath is null or empty!");

            return fullPath.Substring(fullPath.IndexOf("Assets/"));
        }
    }
}