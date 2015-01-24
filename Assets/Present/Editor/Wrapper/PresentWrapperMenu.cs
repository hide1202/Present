using UnityEngine;
using UnityEditor;

public class PresentWrapperMenu
{
    [MenuItem("Assets/Present/Export Package")]
    public static void ExportPresentPackage()
    {
        var window = EditorWindow.GetWindow<PresentWrapperWindow>("Present wrapper", true);
        window.minSize = new Vector2(450, 300);
        window.Show();
    }

    [MenuItem("Assets/Present/Import Package")]
    public static void ImportPresentPackage()
    {
        string openPath = EditorUtility.OpenFilePanel("Open present", "", Present.Util.PresentExtension);
        PresentCompression.Decompress(openPath);
        AssetDatabase.Refresh();
    }
}