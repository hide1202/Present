using UnityEngine;
using System.Collections;
using System.Xml;
using System.Reflection;
using System.IO;
using Present;

public class LetterWriter
{
    public static string Save(string fileName, AssetInfo[] assetInfoList)
    {
        string path = Path.Combine(Util.TempFullPath, fileName.EndsWith(".letter") ? fileName : fileName + ".letter");
        using (StreamWriter writer = new StreamWriter(path))
            writer.Write(ToXml(assetInfoList));
        return path;
    }

    static string ToXml(AssetInfo[] assetInfoList)
    {
        string result = string.Empty;

        XmlDocument doc = new XmlDocument();
        doc.AppendChild(doc.CreateNode(XmlNodeType.Element, "AssetInfos", null));

        foreach (var asset in assetInfoList)
        {
            var node = CreateAssetNode(doc, asset);
            doc.FirstChild.AppendChild(node);
        }

        var sWriter = new System.Text.StringBuilder();

        XmlWriterSettings setting = new XmlWriterSettings() { 
            Indent = true,
            NewLineHandling = NewLineHandling.Replace,
            IndentChars = " ",
            NewLineChars = "\r\n",
            Encoding = new System.Text.UTF8Encoding(false)
        };
        
        using (var writer = XmlWriter.Create(sWriter, setting))
        {
            doc.WriteTo(writer);
            result = sWriter.ToString();

            using (var fileWriter = new StreamWriter(Path.Combine(Present.Util.TempFullPath, "temporary.letter")))
                fileWriter.Write(result);
        }

        return result;
    }

    static XmlNode CreateAssetNode(XmlDocument doc, AssetInfo asset) 
    {
        var result = doc.CreateNode(XmlNodeType.Element, "Asset", null);

        PropertyInfo[] propInfos = typeof(AssetInfo).GetProperties();

        foreach (var propInfo in propInfos)
        {
            var node = doc.CreateElement(propInfo.Name);
            object @value = propInfo.GetValue(asset, null);
            node.InnerText = @value == null ? "null" : @value.ToString();
            result.AppendChild(node);
        }

        return result;
    }
}
