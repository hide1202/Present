using Present;
using SevenZip.Compression.LZMA;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using StringBuilder = System.Text.StringBuilder;
using Encoding = System.Text.Encoding;

public static class PresentCompression
{
    static void Debug(string format, params object[] @params)
    { 
#if PRESENT_DEV
        UnityEngine.Debug.Log(string.Format(format, @params));
#endif
    }

    static string ToRelativePath(string absolutePath)
    {
        if (!Util.IsContainsPath(absolutePath, Application.dataPath))
            throw new ArgumentException("absolutePath does not contains Application.dataPath!");
        
        StringBuilder result = new StringBuilder(Util.FullPath(absolutePath));
        result.Replace(Util.FullPath(Application.dataPath), string.Empty);
        if (result[0] == '\\' || result[0] == '/')  result.Remove(0, 1);
        return result.ToString();
    }

    static string ToAbsolutePath(string relativePath)
    {
        return Path.Combine(Application.dataPath, relativePath);
    }

    public static void Compress(string outputFilePath, params string[] inputFilePaths)
    {
        List<FileStream> inStreamList = new List<FileStream>();
        FileStream outStream = null;

        try
        {
            foreach (var path in inputFilePaths)
                inStreamList.Add(new FileStream(path, FileMode.Open));

            if (File.Exists(outputFilePath)) File.Delete(outputFilePath);
            outStream = new FileStream(outputFilePath, FileMode.CreateNew);

            Encoder encoder = new Encoder();
            encoder.WriteCoderProperties(outStream);

            outStream.Write(BitConverter.GetBytes(inStreamList.Count), 0, 4);
            foreach (var inStream in inStreamList)
            {
                string filePath = ToRelativePath(inStream.Name);
                byte[] inStreamFileNameLength = BitConverter.GetBytes(filePath.Length);
                byte[] inStreamFileName = Encoding.Default.GetBytes(filePath);

                Debug("[inStream][Name:{0}][NameLength:{1}][Length:{2}]",
                    filePath, filePath.Length, inStream.Length);

                outStream.Write(inStreamFileNameLength, 0, sizeof(Int32));
                outStream.Write(inStreamFileName, 0, filePath.Length);
                outStream.Write(BitConverter.GetBytes(inStream.Length), 0, sizeof(Int64));
            }

            foreach (var inStream in inStreamList)
                encoder.Code(inStream, outStream, inStream.Length, -1, new CodeProgress());
        }
        finally
        {
            inStreamList.ForEach(stream => { if (stream != null) stream.Dispose(); });
            if (outStream != null) outStream.Dispose();
        }
    }

    struct OutputFileInfo { public string filePath; public long fileLength; }
    internal static void Decompress(string sourceFileName)
    {
        using (FileStream inStream = new FileStream(sourceFileName, FileMode.Open))
        {
            Decoder decoder = new Decoder();

            byte[] properties = new byte[5];
            inStream.Read(properties, 0, 5);

            decoder.SetDecoderProperties(properties);

            byte[] buffer = new byte[2048];
            inStream.Read(buffer, 0, 4);
            int fileCount = BitConverter.ToInt32(buffer, 0);

            List<OutputFileInfo> infoList = new List<OutputFileInfo>();
            for (int i = 0; i < fileCount; i++)
            {
                Array.Clear(buffer, 0, buffer.Length);
                inStream.Read(buffer, 0, sizeof(Int32));
                int filePathLength = BitConverter.ToInt32(buffer, 0);

                Array.Clear(buffer, 0, buffer.Length);
                inStream.Read(buffer, 0, filePathLength);
                string filePath = Encoding.Default.GetString(buffer, 0, filePathLength);
                filePath = ToAbsolutePath(filePath);
                
                Array.Clear(buffer, 0, buffer.Length);
                inStream.Read(buffer, 0, sizeof(Int64));
                long fileLength = BitConverter.ToInt64(buffer, 0);

                Debug("[FileInfo][Path:{0}][StringLen:{2}][Length:{1}]", 
                    filePath, fileLength, filePath.Length);

                OutputFileInfo info = new OutputFileInfo();
                info.filePath = filePath;
                info.fileLength = fileLength;

                infoList.Add(info);
            }

            foreach (var info in infoList)
            {
                if (File.Exists(info.filePath)) File.Delete(info.filePath);

                FileInfo fileInfo = new FileInfo(info.filePath);
                if (!fileInfo.Directory.Exists)
                    MakeDirectory(fileInfo.Directory.FullName);
                
                using (FileStream outStream = new FileStream(info.filePath, FileMode.CreateNew))
                    decoder.Code(inStream, outStream, inStream.Length, info.fileLength, new CodeProgress());
            }
        }
    }

    static void MakeDirectory(string dirPath)
    {
        DirectoryInfo dirInfo = new DirectoryInfo(dirPath);

        if (!dirInfo.Exists)
        {
            if (!dirInfo.Parent.Exists)
                MakeDirectory(dirInfo.Parent.FullName);
            Directory.CreateDirectory(dirPath);
        }
    }

    class CodeProgress : SevenZip.ICodeProgress
    {
        public void SetProgress(long inSize, long outSize)
        {
            PresentCompression.Debug("[inSize:{0}]:[outSize:{1}]", inSize, outSize);
        }
    }
}