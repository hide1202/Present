using UnityEngine;
using System.Collections;
using UnityEditor;

public class PresentDevelopTools : MonoBehaviour
{
    [MenuItem("Present Dev/Print debug")]
    public static void PrintDebug()
    {
        Debug.Log(Present.Util.TempFullPath);
        Debug.Log(Present.Util.GetRelativePath("E:/Programming/Unity Projects\\Present/Assets\\Present/Editor/Common/LetterReader.cs"));
    }
}
