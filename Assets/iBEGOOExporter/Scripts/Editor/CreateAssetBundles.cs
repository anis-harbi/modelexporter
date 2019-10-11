using UnityEditor;
using System.IO;
using UnityEngine;

public class CreateAssetBundles
{
	[MenuItem("iBEGOO/Build Asset Bundles")]
	static void BuildAllAssetBundles()
	{
		string assetBundleDirectory = "Assets/iBEGOOExporter/AssetBundles";
		if (!Directory.Exists(assetBundleDirectory))
		{
			Directory.CreateDirectory(assetBundleDirectory);
		}
		BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.iOS);
        AssetDatabase.Refresh();
    }


}