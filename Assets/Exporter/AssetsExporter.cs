using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetsExporter : MonoBehaviour
{
    Dictionary<string, string> thumbnails = new Dictionary<string, string>();


    protected void UploadFileTo(string fileName, string uploadURL, string fileTag, bool isBundle)
	{
        if (isBundle) { string filePath = "Assets/AssetBundles"; } else { string filePath = "Assets/Resources"; }

		LoadModelForHeadshot("X Bot");
	}

    protected void LoadModelForHeadshot(string modelName)
	{
		//Instantiate model from resources
		//GameObject model = (GameObject)Intansciate;
		GameObject model = (GameObject)Instantiate(Resources.Load(modelName));
        Vector3 modelRotation = Vector3.zero;
        modelRotation.x = model.transform.localRotation.x;
        modelRotation.y = model.transform.localRotation.y + 180f;
        modelRotation.z = model.transform.localRotation.z;
        model.transform.localRotation = Quaternion.Euler(modelRotation);
        
        StartCoroutine(GetModelThumbnail(model));

    }

	IEnumerator GetModelThumbnail(GameObject model)
	{
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();
        ScreenCapture.GrabPixelsOnPostRender();
        Destroy(model);
		
	}


    // Start is called before the first frame update
    void Start()
    {
		UploadFileTo("X Bot", "dummy", "", false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
