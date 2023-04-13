using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AddMenuItem : Editor
{
    [MenuItem("Assets/Create/Weapons/Fist", false, 1)]
    static void AddFist()
    {
        string BaseFist = System.IO.Path.Combine("Assets", "Editor", "Resources", "BaseFist.prefab");
        string Path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(AssetDatabase.GetAssetPath(Selection.activeObject), "NewFist.prefab"));
        AssetDatabase.CopyAsset(BaseFist, Path);
    }
    [MenuItem("Assets/Create/Weapons/Gun", false, 2)]
    static void AddGun()
    {
        string BaseFist = System.IO.Path.Combine("Assets", "Editor", "Resources", "BaseGun.prefab");
        string Path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.Combine(AssetDatabase.GetAssetPath(Selection.activeObject), "NewGun.prefab"));
        AssetDatabase.CopyAsset(BaseFist, Path);
    }
}
