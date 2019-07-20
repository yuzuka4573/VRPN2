using Firebase.Auth;
using System.Threading.Tasks;
using UnityEngine;


namespace VRPN2.Auth
{
    public class VRPN2Auth
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        public FirebaseUser User { get; private set; }
        /// <summary>
        /// Login function for Firebase
        /// </summary>
        /// <param name="email"> user email section</param>
        /// <param name="password">user password section</param>
        public async Task FirebaseUserSignIn(string email, string password)
        {
            await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
                {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                        return;
                    }
                    if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        return;
                    }
                    //set user data
                    User = task.Result;
                    Debug.Log("User signed in successfully:" + User.UserId);
                });


        }

        /// <summary>
        /// Create User function for Firebase
        /// </summary>
        /// <param name="email"> user email section</param>
        /// <param name="password">user password section</param>
        public async Task FirebaseUserSingUp(string email, string password)
        {

            await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
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
                Firebase.Auth.FirebaseUser newUser = task.Result;
                Debug.Log("Firebase user created successfully: " + newUser.UserId);
            });

        }

        /// <summary>
        /// logout the current User
        /// </summary>
        public void FirebaseUserSignOut()
        {
            auth.SignOut();
            User = null;
        }

    }
}