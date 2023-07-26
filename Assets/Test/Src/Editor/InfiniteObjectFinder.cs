//using Assets.Src.Aspects.Visibility;
//using System.Linq;
//using UNet2;
//using UnityEditor;
//using UnityEngine;

//public static class InfiniteObjectFinder
//{
//    [MenuItem("Build/Find Infinite Assets")]
//    public static void Find()
//    {
//        var assets = AssetDatabase.FindAssets("t: GameObject");
//        var q = from assetGUID in assets
//                let assetPath = AssetDatabase.GUIDToAssetPath(assetGUID)
//                let assetObj = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath)
//                where assetObj != null
//                where PrefabUtility.GetPrefabType(assetObj) == PrefabType.Prefab
//                let id = assetObj.GetComponent<NetworkIdentity>()
//                where id != null && !id.m_ServerOnly
//                where assetObj.GetComponent<ColonyProximityChecker>() == null
//                select assetObj;

//        foreach(var obj in q)
//        {
//            Debug.LogFormat(obj, "Infinite Object: {0}", AssetDatabase.GetAssetPath(obj));
//        }
//    }

//}
