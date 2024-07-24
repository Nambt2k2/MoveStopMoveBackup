using UnityEngine;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Analytics;
using Firebase.Messaging;
using Firebase.RemoteConfig;

public class FireBaseManager : MonoBehaviour {
    public static FireBaseManager instance;
    public FirebaseApp app;
    public bool isRemoteConfigComplete;

    void OnDestroy() {
        FirebaseMessaging.TokenReceived -= OnTokenReceived;
        FirebaseMessaging.MessageReceived -= OnMessageReceived;
    }

    public void Init() {
        instance = this;
        DontDestroyOnLoad(this);
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(CalledAfterSomeTime);
    }

    void CalledAfterSomeTime(Task<DependencyStatus> task) {
        var dependencyStatus = task.Result;
        if (dependencyStatus == DependencyStatus.Available) {
            app = FirebaseApp.DefaultInstance;
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            FirebaseAnalytics.LogEvent("EventInit", "Content", "Complete");
            FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            FirebaseRemoteConfig.DefaultInstance.FetchAsync(System.TimeSpan.Zero).ContinueWith(FetchComplete);
        } else
            Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
    }

    public void LogSceneGame(string sceneGame) {
        FirebaseAnalytics.LogEvent("EventSceneGame", "Name", sceneGame);
    }

    public void OnTokenReceived(object sender, TokenReceivedEventArgs token) {
        Debug.Log("Received Registration Token: " + token.Token);
    }

    public void OnMessageReceived(object sender, MessageReceivedEventArgs e) {
        Debug.Log("Received a new message from: " + e.Message.From);
    }

    void FetchComplete(Task fetchTask) {
        if (!fetchTask.IsCompleted) {
            Debug.LogError("Retrieval hasn't finished.");
            return;
        }

        var remoteConfig = FirebaseRemoteConfig.DefaultInstance;
        var info = remoteConfig.Info;
        if (info.LastFetchStatus != LastFetchStatus.Success) {
            Debug.LogError($"{nameof(FetchComplete)} was unsuccessful\n{nameof(info.LastFetchStatus)}: {info.LastFetchStatus}");
            return;
        }

        remoteConfig.ActivateAsync().ContinueWithOnMainThread(task => {
            Debug.Log($"Remote data loaded and ready for use. Last fetch time {info.FetchTime}.");
            var values = remoteConfig.AllValues;
            values.TryGetValue("config_isBackupData", out var configValue);
            if (configValue.BooleanValue == false)
                GameManager.instance.isBackupData = false;
            else {
                GameManager.instance.isBackupData = true;
                Data.instance.dataPlayer = GameManager.instance.dataController.LoadDataBackup();
                isRemoteConfigComplete = true;
            }
        });
    }
}
