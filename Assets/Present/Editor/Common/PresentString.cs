using UnityEngine;
using System.Collections;
using System.Xml;

public class PresentString : PresentXmlReader
{
    protected PresentString(string xPath) : base(xPath) { }

    private static PresentString _uniqueInstance = null;
    public static PresentString String
    {
        get
        {
#if !PRESENT_DEV
            if (_uniqueInstance == null)
#endif
                _uniqueInstance = new PresentString("strings/string");
            return _uniqueInstance;
        }
    }

    protected override string FileName { get { return "PresentString"; } }
    public string this[string key] { get { return GetInnerText("key", key); } }
}