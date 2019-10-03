using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using Firebase.Auth;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System;

public class AssetsExporter : MonoBehaviour
{

    public static string databaseURL = "https://ibegoo-dev.firebaseio.com/";
    //public static string databaseURL = "https://late-night-show.firebaseio.com/";

    //Storage Paths and Keys
    public static string modelsStorageURL = "assetMedia/models/";
    public static string propsStorageURL = "assetMedia/props/";
    public static string imagesStorageURL = "assetMedia/images/";
    public static string modelStorageName = "original";

    //Database Paths and Keys
    public static string modelsDBPath = "assets/asset_data/character_model";
    public static string propsDBPath = "assets/asset_data/prop";
    public static string setDesignsDBPath = "assets/asset_data/set_design";

    public static string modelDBNameKey = "name";
    public static string modelDBDescriptionKey = "description";
    public static string modelDBServiceKey = "service";
    public static string modelDBImageKey = "image";
    public static string modelDBImageURLKey = "imageURL";
    public static string modelDBURLKey = "fileURL";
    public static string modelDBBundleURLKey = "bundleURL";

    public static Dictionary<string, JObject> modelsDict = new Dictionary<string, JObject>();

    Dictionary<string, string> thumbnails = new Dictionary<string, string>();
    public GameObject modelsParent;
    bool headShotSpaceAvailable;


    protected void UploadFileTo(string fileName, string fileExt, string filePath, string uploadStoragePath, string fileTag, bool isBundle)
	{
        //Get screenshot and upload image
        StartCoroutine(LoadModelForHeadshot(fileName));

        //Upload Original FBX to DB
        string originalFilePath = filePath + fileExt;
        UploadOriginalFileToStorage(fileName, filePath + fileName +fileExt, uploadStoragePath + AssetsExporter.modelStorageName + fileExt.ToLower(), fileTag);

        //Upload Asset Bundle to DB
        string assetBundleFilePath = filePath.Replace("Assets/Resources", "Assets/AssetBundles") + "/" + fileName.ToLower();
        UploadAssetBundleToStorage(fileName, assetBundleFilePath, uploadStoragePath + fileName.ToLower() , fileTag);

	}

    

    IEnumerator LoadModelForHeadshot(string fileName)
	{
        //Instantiate model from resources
        //GameObject model = (GameObject)Intansciate;
        Debug.Log(fileName);
        yield return new WaitUntil(() => headShotSpaceAvailable == true);
        headShotSpaceAvailable = false;
        GameObject model = (GameObject)Instantiate(Resources.Load(fileName));
        model.transform.SetParent(modelsParent.transform);
        Vector3 modelRotation = Vector3.zero;
        modelRotation.x = model.transform.localRotation.x;
        modelRotation.y = model.transform.localRotation.y + 180f;
        modelRotation.z = model.transform.localRotation.z;
        model.transform.localRotation = Quaternion.Euler(modelRotation);
        yield return new WaitForEndOfFrame();
        ScreenCapture.GrabPixelsOnPostRender(fileName);
        Destroy(model);
        yield return null;

    }

    protected void UploadAssetBundleToStorage(string fileName, string filePath, string uploadStoragePath, string fileTag)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string path = uploadStoragePath;
        // Get a reference to the storage service, using the default Firebase App
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a reference with an initial file path and name
        Firebase.Storage.StorageReference path_reference =
          storage.GetReference(path);

        // Upload the file to the path "images/rivers.jpg"
        path_reference.PutBytesAsync(bytes)
          .ContinueWith((Task<Firebase.Storage.StorageMetadata> task) => {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.Log(task.Exception.ToString());
                  // Uh-oh, an error occurred!
              }
              else
              {
                  // Metadata contains file metadata such as size, content-type, and download URL.
                  Firebase.Storage.StorageMetadata metadata = task.Result;
                  //string download_url = metadata.DownloadUrl.ToString();
                  Debug.Log("Finished uploading...");
                  //Debug.Log("download url = " + download_url);
                  path_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> getURLtask) => {
                      if (!getURLtask.IsFaulted && !getURLtask.IsCanceled)
                      {
                          Debug.Log("Download URL: " + getURLtask.Result);
                          AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey] = getURLtask.Result;
                      }
                  });
              }
          });


    }


    protected void UploadOriginalFileToStorage(string fileName, string filePath, string uploadStoragePath, string fileTag)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string path = uploadStoragePath;
        // Get a reference to the storage service, using the default Firebase App
        Firebase.Storage.FirebaseStorage storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Create a reference with an initial file path and name
        Firebase.Storage.StorageReference path_reference =
          storage.GetReference(path);

        // Upload the file to the path "images/rivers.jpg"
        path_reference.PutBytesAsync(bytes)
          .ContinueWith((Task<Firebase.Storage.StorageMetadata> task) => {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.Log(task.Exception.ToString());
                  // Uh-oh, an error occurred!
              }
              else
              {
                  // Metadata contains file metadata such as size, content-type, and download URL.
                  Firebase.Storage.StorageMetadata metadata = task.Result;
                  //string download_url = metadata.DownloadUrl.ToString();
                  Debug.Log("Finished uploading...");
                  // Fetch the download URL
                  path_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> getURLtask) => {
                      if (!getURLtask.IsFaulted && !getURLtask.IsCanceled)
                      {
                          Debug.Log("Download URL: " + getURLtask.Result);
                          AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey] = getURLtask.Result;
                      }
                  });

              }
          });


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
                string uploadStoragePath = AssetsExporter.modelsStorageURL + fileName + "/";

                AssetsExporter.modelsDict[fileName] = new JObject();
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBNameKey] = fileName;
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBServiceKey] = "iBEGOO Custom 3D Designs";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBDescriptionKey] = fileName + " 3D model";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey] = new JObject();
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey] = "";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey] = "";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey] = "";
                StartCoroutine(WriteToDatabase(fileName));
                UploadFileTo(fileName, fileExt, filePath, uploadStoragePath, "", false);
            }
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        //UploadFileTo("X Bot", "dummy", "", false);
        // Set this before calling into the realtime database.
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(AssetsExporter.databaseURL);

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;



        ExportFiles();
    }

    protected IEnumerator WriteToDatabase(string fileName)
    {
        Debug.Log("yielding 1: " + AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey]);
        Debug.Log("yielding 2: " + AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey]);
        Debug.Log("yielding 3: " + AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey]);

        yield return new WaitUntil
        (() =>
            AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey].ToString() != ""
            &&
            AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey].ToString() != ""
            &&
            AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey].ToString() != ""
        );


        string json = AssetsExporter.modelsDict[fileName].ToString();
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        string key = reference.Child(AssetsExporter.modelsDBPath).Push().Key;
        Debug.Log("generated key: " + key);
        Debug.Log("path: " + AssetsExporter.modelsDBPath + key);
        reference.Child(AssetsExporter.modelsDBPath).Child(key).SetRawJsonValueAsync(json);
        Debug.Log("Finished writing to database: " + AssetsExporter.modelsDict[fileName]);
        //AssetsExporter.modelsDict.Remove(fileName);
        yield return null;
    }


    // Update is called once per frame
    void Update()
    {
        if (modelsParent.transform.childCount == 0)
        {
            headShotSpaceAvailable = true;
        }
    }




    private void Awake()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Firebase.Auth.Credential credential =
    Firebase.Auth.EmailAuthProvider.GetCredential("ah@amirbaradaran.com", "abcd1234");
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
}
