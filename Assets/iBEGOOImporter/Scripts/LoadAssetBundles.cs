using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoadAssetBundles : MonoBehaviour
{
     void Start()
    {
        StartCoroutine(GetAssetBundle());
    }


    // Start is called before the first frame update
    string baseBundleURL = @"https://raw.githubusercontent.com/guotata1996/AssetsBundleTest/master/UnityBundles/";
    public string assetName;
    AssetBundle bundle;

    public Text bundleNameInput;

    IEnumerator GetAssetBundle() {
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle("https://firebasestorage.googleapis.com/v0/b/ibegoo-dev.appspot.com/o/assetMedia%2Fmodels%2Fmodel%2Fmodel?alt=media&token=960399c1-b73c-4e56-be72-1a1ca7d2b73f");
        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            bundle = DownloadHandlerAssetBundle.GetContent(www);
            var prefabs = bundle.LoadAllAssets();
            foreach(var prefab in prefabs){
                Instantiate(prefab);
            }
        }
    }

    public void UpdateBundle()
    {
        StartCoroutine(GetAssetBundle());
    }

}