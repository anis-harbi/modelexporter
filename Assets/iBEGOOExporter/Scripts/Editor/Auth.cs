using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Auth 
{
    public static bool success;


    public static void LoginFirebase(string emailInputvalue, string passwordInputvalue)
    {

        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;


        Firebase.Auth.Credential credential =
Firebase.Auth.EmailAuthProvider.GetCredential(emailInputvalue, passwordInputvalue);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                //Debug.LogError("SignInWithCredentialAsync was unsuccessful.");
                success = false;
                return;
            }
            else
            {
                success = true;
                Firebase.Auth.FirebaseUser newUser = task.Result;
                //Debug.LogFormat("User signed in successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);
              

            }

        });

    }


    public static void LogoutFirebase()
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.SignOut();
    }

    public static bool IsUserSignedIN()
    {
        Firebase.Auth.FirebaseUser user;
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
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
            //Debug.Log(userName + " signed in...");
            return true;

        }
        else
        {
            //Debug.Log("User not signed in...");
            return false;
        }
    }


}
