using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadAssetBundles : MonoBehaviour
{

    public IEnumerator GetAssetBundle(string bundleUrl) {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            yield return null;
        }
        else
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            Object[] prefabs = bundle.LoadAllAssets();
            yield return prefabs;
        }
    }

    public IEnumerator DownloadAssetBundle(string bundleUrl)
    {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleUrl);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //bundle is cached
        }
    }

    public void DownloadAssetBundles(List<string> bundleUrls)
    {
        foreach (string bundleUrl in bundleUrls)
        {
            StartCoroutine(DownloadAssetBundle(bundleUrl));
        }
    }

}