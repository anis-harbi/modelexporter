using UnityEngine;
using UnityEditor;

public class iBEGOO : EditorWindow
{
    string email = "";
    string password = "";


    bool signinReady;
    bool invalidCredentials;

    // Add menu named "My Window" to the Window menu
    [MenuItem("iBEGOO/Sign in", false, 0)]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        iBEGOO window = (iBEGOO)EditorWindow.GetWindow(typeof(iBEGOO));
        window.Show();
    }

    void OnGUI()
    {


        if (!Auth.IsUserSignedIN()) 
        {
            GUILayout.Label("Sing in with your iBEGOO Creator Credentials", EditorStyles.boldLabel);
            email = EditorGUILayout.TextField("Email", email);
            password = EditorGUILayout.TextField("Password", password);


            if (GUI.Button(new Rect(10, 70, 140, 20), "Sign in"))
            {

                Auth.LoginFirebase(email, password);
                if (Auth.success)
                {
                    invalidCredentials = false;
                }
                else
                {
                    invalidCredentials = true;
                }

            }


            if (invalidCredentials)
            {
                GUI.Label(new Rect(170, 72, 140, 20), "Invalid Credentials!");
            }

        }

        else
        {
            if (Auth.IsUserSignedIN())
            {
                GUILayout.Label("Already Signed In!", EditorStyles.boldLabel);

                if (GUI.Button(new Rect(10, 30, 140, 20), "Sign out"))
                {
                    invalidCredentials = false;
                    Auth.LogoutFirebase();
                }




            }
        }

    }
}