using System;
using System.Collections.Generic;
using System.Net;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using EventsIP;
using Newtonsoft.Json;
using Org.Json;
using EventsIP.Activities;
using EventsIP.Models;
using EventsIP.Services;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace EventsIP
{
    [Activity(Label = "EventsIP", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, IFacebookCallback, GraphRequest.IGraphJSONObjectCallback
    {
        private ICallbackManager mCallbackManager;
        protected override void OnCreate(Bundle bundle)
        {
            EventsAPIService.GetAllEventsFromApi();

            base.OnCreate(bundle);

            FacebookSdk.SdkInitialize(this.ApplicationContext);

            SetContentView(Resource.Layout.Main);

            Button facebookButton = FindViewById<Button>(Resource.Id.loginbutton);

            if (AccessToken.CurrentAccessToken != null)
            {
                //The user is logged in
                facebookButton.Text = "Log Out";

                RequestToFacebook();

                MoveToEventList();
            }

            mCallbackManager = CallbackManagerFactory.Create();

            LoginManager.Instance.RegisterCallback(mCallbackManager, this);

            facebookButton.Click += (o, e) =>
            {
                if (AccessToken.CurrentAccessToken != null)
                {
                    //The user is logged in
                    facebookButton.Text = "Log Out";
                    LoginManager.Instance.LogOut();
                    facebookButton.Text = "Login";
                }

                else
                {
                    //The user is logged out
                    //LoginManager.Instance.LogInWithReadPermissions(this, new List<string> { "public_profile", "user_friends", "user_events" });
                    LoginManager.Instance.LogInWithReadPermissions(this, new List<string>() { "public_profile", "user_friends", "user_events" });
                    facebookButton.Text = "Log Out";
                }
            };
        }

        public void OnCancel()
        {
            //throw new NotImplementedException();
        }

        public void OnError(FacebookException p0)
        {
            //throw new NotImplementedException();
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            LoginResult loginResult = result as LoginResult;

            RequestToFacebook();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            mCallbackManager.OnActivityResult(requestCode, (int) resultCode, data);

            RequestToFacebook();

            MoveToEventList();
        }

        public void OnCompleted(JSONObject json, GraphResponse response)
        {
            var data = json.ToString();
            EventsReceiveModel result = JsonConvert.DeserializeObject<EventsReceiveModel>(data);

            EventsAPIService.UpdateEvents(result);
        }

        public void RequestToFacebook()
        {
            GraphRequest request = GraphRequest.NewMeRequest(AccessToken.CurrentAccessToken, this);

            Bundle parameters = new Bundle();
            parameters.PutString("fields", "events");

            request.Parameters = parameters;
            request.ExecuteAsync();
        }

        public void MoveToEventList()
        {
            Intent i = new Intent(this, typeof (EventsList));
            this.StartActivity(i);
        }
    }
}

