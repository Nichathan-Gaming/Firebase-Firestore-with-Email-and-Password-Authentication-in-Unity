using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseAuthentication : MonoBehaviour
{
    internal static FirebaseAuthentication instance;

    FirebaseAuth auth;
    FirebaseUser user;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    internal bool IsVerified
    {
        get
        {
            return user != null && user.IsEmailVerified;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        EnableAuthentication();
    }

    void EnableAuthentication()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    internal void Register(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            user = task.Result;
            user.SendEmailVerificationAsync();
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                user.DisplayName, user.UserId);

            FirebaseFirestoreController.instance.ConnectToDatabase();
        });
    }

    internal void SignIn(string email, string password)
    {
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            // Firebase user has been created.
            user = task.Result;
            Debug.LogFormat("Firebase user signed in successfully: {0} ({1})",
                user.DisplayName, user.UserId);

            FirebaseFirestoreController.instance.ConnectToDatabase();
        });
    }

    internal void SignOut()
    {
        auth.SignOut();
    }

    internal void ResetPassword(string email)
    {
        auth.SendPasswordResetEmailAsync(email);
    }

    internal void ChangeEmail(string email, UnityAction<bool> callback)
    {
        user.UpdateEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                callback(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                callback(false);
                return;
            }

            Debug.Log("Firebase user updated their email.");
            callback(true);
        });
    }

    internal void ChangePassword(string password, UnityAction<bool> callback)
    {
        user.UpdatePasswordAsync(password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                callback(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                callback(false);
                return;
            }

            Debug.Log("Firebase user updated their password.");
            callback(true);
        });
    }

    internal void UpdateUserPhotoURL(string url, UnityAction<bool> callback)
    {
        UserProfile userProfile = new()
        {
            PhotoUrl = new System.Uri(url),
            DisplayName = user.DisplayName
        };

        UpdateProfile(userProfile, callback);
    }

    internal void UpdateUserDisplayName(string displayName, UnityAction<bool> callback)
    {
        UserProfile userProfile = new()
        {
            PhotoUrl = user.PhotoUrl,
            DisplayName = displayName
        };

        UpdateProfile(userProfile, callback);
    }

    void UpdateProfile(UserProfile profile, UnityAction<bool> callback)
    {
        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                callback(false);
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                callback(false);
                return;
            }

            Debug.Log("Firebase user updated their profile.");
            callback(true);
        });
    }
}
