using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Present;

public class PresentWrapperWindow : EditorWindow
{
    static List<AssetInfo> _assetList;

    #region Constructor
    public PresentWrapperWindow()
    {
        if (_assetList == null)
        {
            _assetList = new List<AssetInfo>();
        }

        foreach (var guid in Selection.assetGUIDs)
        {
            string assetFullPath = Path.Combine(Util.ProjectFullPath, AssetDatabase.GUIDToAssetPath(guid));
            var assetInfo = new AssetInfo(guid, assetFullPath);
            if (assetInfo.IsDirectory())
            {
                var allFiles = Util.GetAllFiles(assetInfo.FullPath);
                foreach (var fullPath in allFiles)
                {
                    string fileGuid = AssetDatabase.AssetPathToGUID(Util.GetRelativePath(fullPath));

                    if (!string.IsNullOrEmpty(fileGuid))
                        AddAsset(fileGuid, fullPath);
                }
            }
            else
            {
                AddAsset(guid, assetInfo);
            }
        }
                
        // Log for debug.
        foreach (var asset in _assetList)
            Debug.Log(asset.ToString());
    }
    #endregion

    #region Private methods
    private void AddAsset(string guid, string fullPath)
    {
        AddAsset(guid, new AssetInfo(guid, fullPath));
    }

    private void AddAsset(string guid, AssetInfo asset)
    {
        if (_assetList.Find(item => guid == item.Guid) == null)
            _assetList.Add(asset);
    }
    #endregion

    #region Unity methods
    void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load letter"))
        {
            EditorUtility.OpenFilePanel("Open letter", "", "letter");
        }
        if (GUILayout.Button("Save letter"))
        {
            EditorUtility.OpenFilePanel("Save letter", "", "letter");
        }
        if (GUILayout.Button("Clear cache"))
        {
            _assetList.Clear();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Export"))
        {
            string savePath = EditorUtility.SaveFilePanel("Save present", "", "", Util.PresentExtension);

            if (_assetList.Count >= 1)
            {
                List<string> targetFiles = new List<string>();
                foreach (var asset in _assetList)
                    targetFiles.Add(asset.FullPath);
                PresentCompression.Compress(savePath, targetFiles.ToArray());
            }
            else
                EditorUtility.DisplayDialog("Error", "Nothing to select files", "OK");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(PresentString.String["exportInfo"], MessageType.Info);
        EditorGUILayout.Separator();
        EditorGUILayout.HelpBox("Selected files", MessageType.None);
        foreach (var asset in _assetList)
        {
            if (!asset.IsDirectory())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(asset.FileName);
                asset.Version = EditorGUILayout.TextField(asset.Version, GUILayout.MaxWidth(40));
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndVertical();

        DragAndDropFile();
    }

    private void DragAndDropFile()
    {
        Event currentEvent = Event.current;
        EventType currentEventType = currentEvent.type;

        switch (currentEventType)
        {
            case EventType.DragUpdated:
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                currentEvent.Use();
                break;
            case EventType.DragPerform:
                DragAndDrop.AcceptDrag();
                foreach (var path in DragAndDrop.paths)
                    AddAsset(AssetDatabase.AssetPathToGUID(path), Path.Combine(Util.ProjectFullPath, path));
                currentEvent.Use();
                break;
        }
    }
    #endregion
}