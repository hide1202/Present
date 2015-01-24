using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class LetterReader
{
    public static List<AssetInfo> ToAssetInfoList(string xml)
    {
        var result = new List<AssetInfo>();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNodeList assetList = doc.SelectNodes("AssetInfos/Asset");
        foreach (XmlNode node in assetList)
            result.Add(ReadAssetInfo(node));

        return result;
    }

    private static AssetInfo ReadAssetInfo(XmlNode assetNode)
    {
        var propDic = new Dictionary<string, string>();

        foreach (XmlNode child in assetNode.ChildNodes)
            propDic.Add(child.Name, child.InnerText);
        
        return new AssetInfo(propDic["Guid"], propDic["FullPath"])
        {
            Version = propDic["Version"],
            State = (AssetInfoState)System.Enum.Parse(typeof(AssetInfoState), propDic["State"])
        };
    }
}
