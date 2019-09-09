using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;

public class AssetsExporter : MonoBehaviour
{
    Dictionary<string, string> thumbnails = new Dictionary<string, string>();
    public GameObject modelsParent;
    bool headShotSpaceAvailable;


    protected void UploadFileTo(string fileName, string uploadURL, string fileTag, bool isBundle)
	{
        if (isBundle) { string filePath = "Assets/AssetBundles"; } else { string filePath = "Assets/Resources"; }

		StartCoroutine(LoadModelForHeadshot(fileName));
	}

    IEnumerator LoadModelForHeadshot(string modelName)
	{
        //Instantiate model from resources
        //GameObject model = (GameObject)Intansciate;
        Debug.Log(modelName);
        yield return new WaitUntil(() => headShotSpaceAvailable == true);
        headShotSpaceAvailable = false;
        GameObject model = (GameObject)Instantiate(Resources.Load(modelName));
        model.transform.SetParent(modelsParent.transform);
        Vector3 modelRotation = Vector3.zero;
        modelRotation.x = model.transform.localRotation.x;
        modelRotation.y = model.transform.localRotation.y + 180f;
        modelRotation.z = model.transform.localRotation.z;
        model.transform.localRotation = Quaternion.Euler(modelRotation);
        yield return new WaitForEndOfFrame();
        Debug.Log(ScreenCapture.GrabPixelsOnPostRender());
        yield return new WaitForEndOfFrame();
        Destroy(model);
        //StartCoroutine(GetModelThumbnail(model));

    }

	IEnumerator GetModelThumbnail(GameObject model)
	{
        // We should only read the screen buffer after rendering is complete
        yield return new WaitForEndOfFrame();
        Debug.Log(ScreenCapture.GrabPixelsOnPostRender());
        yield return new WaitForEndOfFrame();
        Destroy(model);
       

		
	}


    protected void ExportFiles()
    {
        var info = new DirectoryInfo("Assets/Resources");
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo)
        {
            
            //end of directory path, start of file name
            int fileNamePos = file.ToString().LastIndexOf("/", System.StringComparison.CurrentCulture);
            //end of file name, start of file extension
            int fileExtPos = file.ToString().LastIndexOf(".", System.StringComparison.CurrentCulture);

            //parent directory with trailing slash
            string filePath = file.ToString().Substring(0, fileNamePos + 1);
            //isolated file name
            string fileName = file.ToString().Substring(fileNamePos + 1, fileExtPos - filePath.Length);
            //extension with "."
            string fileExt = file.ToString().Substring(fileExtPos);

            if (fileExt == ".fbx" || fileExt == ".FBX" || fileExt == ".OBJ" || fileExt == ".obj") 
             
            {
                Debug.Log("Found 3D Model File: " + fileName);
                UploadFileTo(fileName, "dummy", "", false);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        //UploadFileTo("X Bot", "dummy", "", false);
        // Set this before calling into the realtime database.
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://YOUR-FIREBASE-APP.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

        ExportFiles();
    }

    // Update is called once per frame
    void Update()
    {
        if (modelsParent.transform.childCount == 0)
        {
            headShotSpaceAvailable = true;
        }
       
    }
}
