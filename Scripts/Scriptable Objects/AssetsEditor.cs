using System.Collections;
using UnityEditor;
using UnityEngine;

namespace CNovell.Editor
{
    public class AssetsEditor
    {

#if UNITY_EDITOR
        [MenuItem("Window/CNovell/Собрать архив")]
        public static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/CNovell/Bundles";
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }
#endif
    }
}
