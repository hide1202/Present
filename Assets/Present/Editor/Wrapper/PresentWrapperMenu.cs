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

    [MenuItem("Present/Compress files")]
    public static void CompressTest()
    {
        PresentCompression.Compress( "D:\\Result.7z", "D:\\TestFile.txt", "D:\\TestFile2.txt");
    }

    [MenuItem("Present/Decompress files")]
    public static void DecompressTest()
    {
        PresentCompression.Decompress("D:\\Result.7z");
    }
}