using Firebase.Extensions;
using Firebase.Firestore;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseFirestoreController : MonoBehaviour
{
    internal static FirebaseFirestoreController instance;

    FirebaseFirestore db;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    internal void ConnectToDatabase()
    {
        db = FirebaseFirestore.DefaultInstance;
    }

    internal void CreateData()
    {
        DocumentReference docRef = db.Collection("users").Document("alovelace");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "First", "Ada" },
            { "Last", "Lovelace" },
            { "Born", 1815 },
        };
        docRef.SetAsync(user).ContinueWithOnMainThread(task => {
            Debug.Log("Added data to the alovelace document in the users collection.");
        });
    }

    internal void ReadData()
    {
        CollectionReference usersRef = db.Collection("users");
        usersRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                Debug.Log(String.Format("User: {0}", document.Id));
                Dictionary<string, object> documentDictionary = document.ToDictionary();
                Debug.Log(String.Format("First: {0}", documentDictionary["First"]));
                if (documentDictionary.ContainsKey("Middle"))
                {
                    Debug.Log(String.Format("Middle: {0}", documentDictionary["Middle"]));
                }

                Debug.Log(String.Format("Last: {0}", documentDictionary["Last"]));
                Debug.Log(String.Format("Born: {0}", documentDictionary["Born"]));
            }

            Debug.Log("Read all data from the users collection.");
        });
    }

    internal void UpdateData()
    {
        DocumentReference docRef = db.Collection("users").Document("alovelace");
        Dictionary<string, object> user = new Dictionary<string, object>
        {
            { "First", "Abba" },
            { "Last", "Lovelace" },
            { "Born", 1815 },
        };

        docRef.SetAsync(user).ContinueWithOnMainThread(task => {
            Debug.Log("Updated data in the alovelace document in the users collection.");
        });
    }

    internal void DeleteData()
    {
        db.Collection("users").Document("alovelace").DeleteAsync().ContinueWithOnMainThread(task=>
        {
            Debug.Log("Deleted data in the alovelace document");
        });
    }
}
