using UnityEngine;
using System.Collections;
using System;
using SevenZip.Compression.LZMA;
using System.IO;
using System.Collections.Generic;

public class PresentCompression
{
    private static void Log(string format, params object[] @params)
    { 
#if PRESENT_DEV
        Debug.Log(string.Format(format, @params));
#endif
    }

    private static string ToRelativePath(string absolutePath)
    {
        if (!absolutePath.Contains(Application.dataPath))
            throw new ArgumentException("absolutePath not contains Application.dataPath!");
        string result = absolutePath.Replace(Application.dataPath, string.Empty);
        if (result[0] == '\\' || result[0] == '/') result = result.Substring(1);
        return result;
    }

    private static string ToAbsolutePath(string relativePath)
    {
        return Path.Combine(Application.dataPath, relativePath);
    }

    public static void Compress(string outputFilePath, params string[] fileList)
    {
        List<FileStream> inStreamList = new List<FileStream>();
        foreach (var path in fileList)
            inStreamList.Add(new FileStream(path, FileMode.Open));

        if (File.Exists(outputFilePath)) File.Delete(outputFilePath);
        FileStream outStream = new FileStream(outputFilePath, FileMode.CreateNew);
        Encoder encoder = new Encoder();
        encoder.WriteCoderProperties(outStream);

        outStream.Write(BitConverter.GetBytes(inStreamList.Count), 0, 4);
        foreach (var inStream in inStreamList)
        {
            Log("[inStream][Name:{0}][Length:{1}]", inStream.Name, inStream.Length);
            Log("[inStream][Length:{0}]", inStream.Name.Length);

            string filePath = ToRelativePath(inStream.Name);
            byte[] inStreamFileNameLength = BitConverter.GetBytes(filePath.Length);
            byte[] inStreamFileName = System.Text.Encoding.Default.GetBytes(filePath);

            outStream.Write(inStreamFileNameLength, 0, 4);
            outStream.Write(inStreamFileName, 0, filePath.Length);
            outStream.Write(BitConverter.GetBytes(inStream.Length), 0, 8);
        }

        foreach (var inStream in inStreamList)
        {
            encoder.Code(inStream, outStream, inStream.Length, -1, new Progress());
        }

        inStreamList.ForEach(stream => stream.Dispose());
        outStream.Dispose();
    }

    struct Info { public string filePath; public long fileLength; }
    internal static void Decompress(string sourceFileName)
    {
        FileStream inStream = new FileStream(sourceFileName, FileMode.Open);

        Decoder decoder = new Decoder();

        byte[] properties = new byte[5];
        inStream.Read(properties, 0, 5);

        byte[] buffer = new byte[2048];
        inStream.Read(buffer, 0, 4);
        int fileCount = BitConverter.ToInt32(buffer, 0);

        List<Info> infoList = new List<Info>();
        for (int i = 0; i < fileCount; i++)
        {
            Array.Clear(buffer, 0, buffer.Length);
            inStream.Read(buffer, 0, 4);
            int filePathLength = BitConverter.ToInt32(buffer, 0);

            Array.Clear(buffer, 0, buffer.Length);
            inStream.Read(buffer, 0, filePathLength);
            string filePath = System.Text.Encoding.Default.GetString(buffer, 0, filePathLength);
            filePath = ToAbsolutePath(filePath);

            Array.Clear(buffer, 0, buffer.Length);
            inStream.Read(buffer, 0, 8);
            long fileLength = BitConverter.ToInt64(buffer, 0);

            Log("[FileInfo][Path:{0}][StringLen:{2}][Length:{1}]", filePath, fileLength,filePath.Length);
            Log("[FileInfo][StringLen:{0}]", filePath.Length);
            Log(Path.GetFileName(filePath));

            Info info = new Info();
            info.filePath = filePath;
            info.fileLength = fileLength;

            infoList.Add(info);
        }

        foreach (var fileInfo in infoList)
        {
            if (File.Exists(fileInfo.filePath)) File.Delete(fileInfo.filePath);
            using (FileStream outStream = new FileStream(fileInfo.filePath, FileMode.CreateNew))
            {
                decoder.SetDecoderProperties(properties);
                decoder.Code(inStream, outStream, inStream.Length, fileInfo.fileLength, new Progress());
            }
        }

        inStream.Dispose();
    }

    public class Progress : SevenZip.ICodeProgress
    {
        public void SetProgress(long inSize, long outSize)
        {
            PresentCompression.Log("[inSize:{0}]:[outSize:{1}]", inSize, outSize);
        }
    }
}