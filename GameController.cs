using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] string signInEmail;
    [SerializeField] string signInPassword;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Register a new user
            FirebaseAuthentication.instance.Register(signInEmail, signInPassword);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            //Sign in
            FirebaseAuthentication.instance.SignIn(signInEmail, signInPassword);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            //sign out
            FirebaseAuthentication.instance.SignOut();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            //create data
            FirebaseFirestoreController.instance.CreateData();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //read data
            FirebaseFirestoreController.instance.ReadData();
        }
    }
}
