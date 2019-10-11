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
        string test = "https://firebasestorage.googleapis.com/v0/b/ibegoo-dev.appspot.com/o/assetMedia%2Fbundles%2Fspaceshiptvhead?alt=media&token=8b1cd2ca-04e0-4ca2-8cf0-5aac98c3f847";
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

            GameObject temp = new GameObject();
            foreach (var prefab in (Object[])cd.result)
            {


                if (prefab.GetType() == temp.GetType())
                {
                    temp = Instantiate(prefab) as GameObject;

                    temp.name = "temptemp";
                    Debug.Log(temp.name);
                    Debug.Log(temp.GetComponent<Animator>());
                    Animator animator = temp.GetComponent<Animator>();
                    Debug.Log("loading animator");
                    Debug.Log(animator);
                    Debug.Log("lopad" + Resources.Load("animations/testAnimator"));
                    animator.runtimeAnimatorController = Resources.Load("animations/WomanControl") as RuntimeAnimatorController;
                    if (animator.runtimeAnimatorController != null)// this check eliminiated the warning message
                    { Debug.Log("not null");
                        animator.enabled = true;
                        animator.Play("Dancing");
                    }
                }
                //
            }

        }
    }


    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Time.time);
    }

}