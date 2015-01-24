using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public abstract class PresentXmlReader : IEnumerable<XmlNode>
{
    protected XmlDocument _xmlDoc = null;
    protected Dictionary<string, XmlNodeList> _loadedDataDic;

    protected PresentXmlReader()
    {
        var stringXml = Resources.Load<TextAsset>(FileName) as TextAsset;

        if (stringXml == null)
            throw new System.IO.FileNotFoundException(string.Format("Failed to load {0}", FileName));

        _xmlDoc = new XmlDocument();
        _xmlDoc.LoadXml(stringXml.text);

        _loadedDataDic = new Dictionary<string,XmlNodeList>();
    }

    protected PresentXmlReader(params string[] xPaths) : this()
    {
        foreach(var xPath in xPaths)
            LoadPath(xPath);
    }

    protected void LoadPath(string xPath)
    {
        _loadedDataDic.Add(xPath, _xmlDoc.SelectNodes(xPath));
    }

    protected abstract string FileName { get; }

    protected XmlNode FindNodeForKey(string key, string value)
    {
        foreach (var dataList in _loadedDataDic.Values)
            for (int i = 0; i < dataList.Count; i++)
                if (dataList[i].Attributes[key].Value == value)
                    return dataList[i];
        return null;
    }

    protected string GetInnerText(string key, string value)
    {
        var node = FindNodeForKey(key, value);
        if (node != null)
            return node.InnerText;
        return string.Empty;
    }

    public IEnumerator<XmlNode> GetEnumerator()
    {
        foreach(var nodeList in _loadedDataDic.Values)
            for(int i = 0; i < nodeList.Count; i++)
                yield return nodeList[i];
    }

    IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}