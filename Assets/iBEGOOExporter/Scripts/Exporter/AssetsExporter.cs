﻿using System.Collections;
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
using UnityEngine.UI;
using UnityEditor;

public class AssetsExporter : MonoBehaviour
{
    Firebase.Auth.FirebaseAuth auth;
    Firebase.Auth.FirebaseUser user;

    bool readyToUpload = false;
    bool startedUploading = false;
    bool isFinished = false;

    Firebase.Storage.FirebaseStorage storage;

    public GameObject success;

    // static string databaseURL = "https://ibegoo-dev.firebaseio.com/";
    public static string databaseURL = "https://late-night-show.firebaseio.com/";

    //Storage Paths and Keys
    public static string bundlesStorageUrl = "assetMedia/bundles/";
    public static string modelsStorageURL = "assetMedia/models/";
    public static string propsStorageURL = "assetMedia/props/";
    public static string setsStorageURL = "assetMedia/sets/";
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


    string[] resourcesSubPaths = { "models", "props", "sets" };

    Dictionary<string, string> thumbnails = new Dictionary<string, string>();
    public GameObject modelsParent;
    public GameObject status;
    public GameObject slider;
    bool headShotSpaceAvailable;
    Dictionary<int, bool> headshotSpaceAvailibilty = new Dictionary<int, bool>();


    int progress = 0;
    int total = 1;
    int fileCount = 0;
    int writesCount = 0;


    private void Awake()
    {


    }

    // Start is called before the first frame update
    void Start()
    {
        success.SetActive(false);
        InitializeFirebase();
        headshotSpaceAvailibilty[0] = true;
        slider.SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {

        if (!isFinished)
        {
            slider.GetComponent<Slider>().value = ((float)progress / (float)total);
        }



        if (readyToUpload && !startedUploading)
        {
            //Set to false to stop update calls...
            startedUploading = true;

            int subPathsCount = resourcesSubPaths.Length;
            

            foreach (string subPath in resourcesSubPaths)
            {
                ExportFiles(subPath);
            }

        }

        user = auth.CurrentUser;
        if (user != null)
        {
            string userName = user.DisplayName;
            string email = user.Email;
            System.Uri photo_url = user.PhotoUrl;
            // The user's Id, unique to the Firebase project.
            // Do NOT use this value to authenticate with your backend     server, if you
            // have one; use User.TokenAsync() instead.
            string uid = user.UserId;

            readyToUpload = true;

        }
        else
        {


        }



    }


    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {

        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);


        // Get a reference to the storage service, using the default Firebase App
        storage = Firebase.Storage.FirebaseStorage.DefaultInstance;
        // Set this before calling into the realtime database.
        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl(AssetsExporter.databaseURL);

    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {

                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }



    protected void ExportFiles(string subPath)
    {
        var info = new DirectoryInfo("Assets/iBEGOOExporter/Resources/" + subPath);
        var fileInfo = info.GetFiles("*.*", SearchOption.AllDirectories);


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


            if (fileExt.ToLower().Contains(".fbx"))

            {
                total += 3;
               
                string uploadStoragePath = "";
                string storageFileName = fileName;
                if (subPath == "models") { uploadStoragePath = AssetsExporter.modelsStorageURL + fileName + "/"; storageFileName = AssetsExporter.modelStorageName; }
                if (subPath == "props") { uploadStoragePath = AssetsExporter.propsStorageURL; }
                if (subPath == "sets") { uploadStoragePath = AssetsExporter.setsStorageURL; }

                AssetsExporter.modelsDict[fileName] = new JObject();
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBNameKey] = fileName;
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBServiceKey] = "iBEGOO Custom 3D Designs";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBDescriptionKey] = fileName + " 3D model";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey] = new JObject();
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey] = "";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey] = "";
                AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey] = "";
                StartCoroutine(WriteToDatabase(fileName, subPath));

                string pathInResources = "";
                string[] stringSeparators = new string[] { "Assets/iBEGOOExporter/Resources/" };
                string[] splittingResult = filePath.Split(stringSeparators, StringSplitOptions.None);
                pathInResources = splittingResult[1];

                UploadFileTo(fileName, fileExt, filePath, pathInResources + fileName, uploadStoragePath, storageFileName, fileCount);
                fileCount += 1;
                headshotSpaceAvailibilty[fileCount] = false;
            }
        }



    }

    protected void UploadFileTo(string fileName, string fileExt, string filePath, string subPath, string uploadStoragePath, string storageFileName,  int fileCount)
    {
        //Get screenshot and upload image
        StartCoroutine(LoadModelForHeadshot(fileName, subPath, fileCount));

        //Upload Original FBX to DB
        string originalFilePath = filePath + fileExt;

        UploadOriginalFileToStorage(fileName, filePath + fileName + fileExt, uploadStoragePath + storageFileName + fileExt.ToLower());

        //Upload Asset Bundle to DB
        string[] stringSeparators = new string[] { "Assets/iBEGOOExporter/Resources" };
        string[] splittingResult = filePath.Split(stringSeparators, StringSplitOptions.None);
        string assetBundleFilePath = splittingResult[0] + "Assets/iBEGOOExporter/AssetBundles" + "/" + fileName.ToLower();
        string assetBundleStoragePath = AssetsExporter.bundlesStorageUrl;
        UploadAssetBundleToStorage(fileName, assetBundleFilePath, assetBundleStoragePath + fileName.ToLower());

    }

    protected void GrabPixelsOnPostRender(string fileName)
    {
        //Create a new texture with the width and height of the screen
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        texture.Apply();
        byte[] bytes = texture.EncodeToPNG();
        string path = AssetsExporter.imagesStorageURL + fileName + ".png";

        // Create a reference with an initial file path and name
        Firebase.Storage.StorageReference path_reference =
          storage.GetReference(path);

        // Upload the file to the path "images/rivers.jpg"
        path_reference.PutBytesAsync(bytes)
          .ContinueWith((Task<Firebase.Storage.StorageMetadata> task) => {
              if (task.IsFaulted || task.IsCanceled)
              {
                  Debug.Log(task.Exception.ToString());
                  Debug.Log("task failed: " + path + " " + fileName);
                  // Uh-oh, an error occurred!
              }
              if (task.IsCanceled)
              {
                  Debug.Log("task was canceled");
              }
              else
              {
                  // Metadata contains file metadata such as size, content-type, and download URL.
                  Firebase.Storage.StorageMetadata metadata = task.Result;
                  //string download_url = metadata.DownloadUrl.ToString();
                  Debug.Log(fileName + " Finished uploading...");
                  progress += 1;
                  // Fetch the download URL
                  path_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> getURLtask) => {
                      if (!getURLtask.IsFaulted && !getURLtask.IsCanceled)
                      {
                          AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey] = getURLtask.Result;
                      }
                  });

              }
          });

    }

    IEnumerator LoadModelForHeadshot(string fileName, string subPath, int count)
    {

        yield return new WaitUntil(() => headshotSpaceAvailibilty[count] == true);

        GameObject model = (GameObject)Instantiate(Resources.Load(subPath));
        model.transform.SetParent(modelsParent.transform);
        model.transform.localPosition = new Vector3 (0f, -0.5f, 0f);
        Vector3 modelRotation = Vector3.zero;
        modelRotation.x = model.transform.localRotation.x;
        modelRotation.y = model.transform.localRotation.y + 180f;
        modelRotation.z = model.transform.localRotation.z;
        model.transform.localRotation = Quaternion.Euler(modelRotation);
        model.transform.localScale *= 1.5f;
        yield return new WaitForEndOfFrame();
        GrabPixelsOnPostRender(fileName);
        yield return new WaitForEndOfFrame();
        Destroy(model);
        headshotSpaceAvailibilty[count + 1] = true;


    }

    protected void UploadAssetBundleToStorage(string fileName, string filePath, string uploadStoragePath)
    {
        if (!File.Exists(filePath))
        {
            status.SetActive(true);
            status.GetComponent<Text>().text = "Missing Asset Bundles!";
            return;
        }
        byte[] bytes = File.ReadAllBytes(filePath);
        string path = uploadStoragePath;

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
                  Debug.Log(fileName + " Finished uploading...");
                  progress += 1;
                  //Debug.Log("download url = " + download_url);
                  path_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> getURLtask) => {
                      if (!getURLtask.IsFaulted && !getURLtask.IsCanceled)
                      {
                          AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey] = getURLtask.Result;
                          Debug.Log(getURLtask.Result);
                      }
                  });
              }
          });


    }

    protected void UploadOriginalFileToStorage(string fileName, string filePath, string uploadStoragePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string path = uploadStoragePath;

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
                  Debug.Log(fileName + " Finished uploading...");
                  progress += 1;
                  // Fetch the download URL
                  path_reference.GetDownloadUrlAsync().ContinueWith((Task<Uri> getURLtask) => {
                      if (!getURLtask.IsFaulted && !getURLtask.IsCanceled)
                      {
                          AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey] = getURLtask.Result;
                      }
                  });

              }
          });


    }

    protected IEnumerator WriteToDatabase(string fileName, string resourceSubPath)
    {
        yield return new WaitUntil
        (() =>
            AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBBundleURLKey].ToString() != ""
            &&
            AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey].ToString() != ""
            &&
            AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBURLKey].ToString() != ""
        );

        string DBref = "";
        if (resourceSubPath == "models") { DBref = AssetsExporter.modelsDBPath; }
        if (resourceSubPath == "props") { DBref = AssetsExporter.propsDBPath; }
        if (resourceSubPath == "sets") { DBref = AssetsExporter.setDesignsDBPath; }

        string json = AssetsExporter.modelsDict[fileName].ToString();
        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
        string key = reference.Child(DBref).Push().Key;
        key = fileName;
        reference.Child(DBref).Child(key).SetRawJsonValueAsync(json);
        writesCount += 1;
        //AssetsExporter.modelsDict.Remove(fileName);
        if (writesCount == fileCount)
        {
            isFinished = true;
            status.SetActive(true);
            slider.GetComponent<Slider>().value = 1f;
            //status.GetComponent<Text>().text = "finished uploading assets";
            success.SetActive(true);

        }

        yield return null;
    }




}
