using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GG : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest("https://firebasestorage.googleapis.com/v0/b/late-night-show.appspot.com/o/assetMedia%2Fmodels%2FX%20Bot%2Foriginal.fbx?alt=media&token=4131f3b2-8924-4ae5-8f20-09e35d7cf365"));
	}

    // Update is called once per frame
    void Update()
    {
        if (GameObject.Find("EX(Clone)") == null )
        {
            GameObject newCharacter = (GameObject)Instantiate(Resources.Load("EX"));
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                //GameObject newCharacter = (GameObject)Instantiate(webRequest.downloadHandler.data);
                ByteArrayToFile("/Resources/EX.fbx",webRequest.downloadHandler.data);
            }



        }

    }


    public bool ByteArrayToFile(string fileName, byte[] byteArray)
    {
        try
        {
            using (var fs = new FileStream(Application.dataPath+  fileName, FileMode.Create, FileAccess.Write))
            {
                Debug.Log("writing to file to " + Application.dataPath + fileName);
                fs.Write(byteArray, 0, byteArray.Length);
                Debug.Log("file downloaded");
                
                return true;
            }
           
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught in process: {0}", ex);
            return false;
        }
    }

}
