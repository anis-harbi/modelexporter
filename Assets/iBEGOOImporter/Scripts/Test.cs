using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<string> urlsy;
    public GameObject gam;
    // Start is called before the first frame update
    void Start()
    {
        string test = "vdfvfdvdf   fdvd   https://firebasestorage.googleapis.com/v0/b/ibegoo-dev.appspot.com/o/assetMedia%2Fbundles%2Fprop?alt=media&token=a9c340de-bc0e-43b3-9ecb-e2eaa9fb0ad2    ";
        List<string> urls = Parser.GetUrls(test);
        urls = Parser.GetUrlsContaining(urls, "firebase");
        urlsy = urls;
        if (urls.Count != 0)
        {
            foreach (string url in urls)
            {
                Debug.Log(url);
            }
        }

        gam.GetComponent<LoadAssetBundles>().DownloadAssetBundles(urls);
        StartCoroutine(WaitAndDo());
    }

    IEnumerator WaitAndDo()
    {
        yield return new WaitForSeconds(5f);
        foreach (string u in urlsy)
        {


            CoroutineWithData cd = new CoroutineWithData(this, gam.GetComponent<LoadAssetBundles>().GetAssetBundle(u));
            yield return cd.coroutine;
            foreach (var prefab in (Object[])cd.result)
            {
                Instantiate(prefab);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
