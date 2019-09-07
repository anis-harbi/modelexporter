// Attach this script to a Camera
//Also attach a GameObject that has a Renderer (e.g. a cube) in the Display field
//Press the space key in Play mode to capture

using System;
using UnityEngine;

public class ScreenCapture : MonoBehaviour
{

	private void Update()
	{

	}


    public static string GrabPixelsOnPostRender()
	{
		//Create a new texture with the width and height of the screen
		Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		//Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
		texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
		texture.Apply();
		byte[] bytes = texture.EncodeToPNG();
		return(Convert.ToBase64String(bytes));
	}

}