using UnityEngine;
using UnityEditor;

public class MyWindow : EditorWindow
{
    string email = "";
    string password = "";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.24f;

    // Add menu named "My Window" to the Window menu
    [MenuItem("iBEGOO/Sign in")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        MyWindow window = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Sing in with your iBEGOO Creator Credentials", EditorStyles.boldLabel);
        email = EditorGUILayout.TextField("Email", email);
        password = EditorGUILayout.TextField("Password", password);

        if (GUILayout.Button("Sign in"))
        {
            Debug.Log("Clicked Button " + email + password);
            Auth.LoginFirebase(email,password);

        }
        //groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        //myBool = EditorGUILayout.Toggle("Toggle", myBool);
        //myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        //EditorGUILayout.EndToggleGroup();
    }
}