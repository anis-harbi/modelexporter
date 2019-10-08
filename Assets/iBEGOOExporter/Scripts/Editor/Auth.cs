using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Auth 
{
    public static void LoginFirebase(string emailInputvalue, string passwordInputvalue)
    {
        Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;

        Firebase.Auth.Credential credential =
Firebase.Auth.EmailAuthProvider.GetCredential(emailInputvalue, passwordInputvalue);
        auth.SignInWithCredentialAsync(credential).ContinueWith(task => {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync was unsuccessful.");
                return;
            }
            else
            {
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    newUser.DisplayName, newUser.UserId);
              

            }

        });

    }
}
