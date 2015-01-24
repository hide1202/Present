using System.IO;

public class AssetInfo
{
    public AssetInfo(string guid, string fullPath) { Guid = guid; FullPath = fullPath; }

    public string Guid { get; private set; }
    public string FullPath { get; private set; }
    public string RelativePath { get { return FullPath.Substring(FullPath.IndexOf("Assets/")); } }
    public string FileName { get { return Path.GetFileName(FullPath); } }
    public string FolderPath { get { if (IsDirectory())return FullPath; return Path.GetDirectoryName(FullPath); } }
    private FileAttributes FileAttribute { get { return File.GetAttributes(FullPath); } }

    public string Version { get; set; }
    public AssetInfoState State { get; set; }

    public bool IsDirectory() { return (FileAttribute & FileAttributes.Directory) == FileAttributes.Directory; }

    public override string ToString()
    {
        return string.Format("[Asset info]:[Guid:{0}]:[FullPath:{1}]:[FileName:{2}]:[FolderPath:{3}]", Guid, FullPath, FileName, FolderPath);
    }
}