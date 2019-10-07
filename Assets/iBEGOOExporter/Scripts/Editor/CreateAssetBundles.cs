using UnityEditor;
using System.IO;
using UnityEngine;

public class CreateAssetBundles
{
	[MenuItem("Assets/Build AssetBundles")]
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