// Attach this script to a Camera
//Also attach a GameObject that has a Renderer (e.g. a cube) in the Display field
//Press the space key in Play mode to capture

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{

	private void Update()
	{

	}


    public static string GrabPixelsOnPostRender(string fileName)
	{
		//Create a new texture with the width and height of the screen
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		//Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
		texture.Apply();
		byte[] bytes = texture.EncodeToPNG();
        string path = AssetsExporter.imagesStorageURL + fileName + ".png";
        Debug.Log("images path: " + path);
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
                          AssetsExporter.modelsDict[fileName][AssetsExporter.modelDBImageKey][AssetsExporter.modelDBImageURLKey] = getURLtask.Result;

                      }
                  });

              }
          });


        return (Convert.ToBase64String(bytes));
	}

}