using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Firebase.Messaging;
using Firebase.RemoteConfig;

public class GameManager : MonoBehaviour {
    [Header("_____________________________Game_____________________________")]
    public GameState gameState;
    public static GameManager instance;

    [Header("_____________________________Manager__________________________")]
    public AudioManager audioManager;
    public ParticalManager particalManager;
    public FireBaseManager fireBaseManager;
    public HomeSceneManager homeSceneManager;
    public GamePlaySceneManager gamePlaySceneManager;
    public ZCSceneManager zombieCitySceneManager;

    [Header("_____________________________Data_____________________________")]
    public Data data;
    public DataController dataController;
    public Material mat_pantOrigin, mat_bodyOrigin;
    public Color[] array_color;
    public WeaponPlayer[] array_weaponPlayer;
    public Material[] array_matPant;
    public GameObject[] array_objHair, array_objShield, array_objSet;
    public MeshRenderer[] array_meshrenWeaponHold;
    public MeshRenderer[] array_meshrenSkinWeapon;
    public bool isBackupData;

    [Header("_____________________________LoadingScene_____________________")]
    public CanvasGroup cavasGroupLoadingBegin;
    public CanvasGroup cavasGroupLoadingScene;
    public Animation animationLoadScene;
    public FPSCounter fpsCounter;

    async void Awake() {
        //cai dat fps = 60
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        fpsCounter.Init();
        //khoi tao cac manager
        instance = this;
        DontDestroyOnLoad(this);
        DontDestroyOnLoad(cavasGroupLoadingScene.gameObject);
        audioManager.Init();
        particalManager.Init();
        //tai du lieu
        dataController = new DataController();
        data.Init();
        Data.instance.dataPlayer = dataController.LoadGame();
        //khoi tao fire base
        fireBaseManager.Init();
        for (int i = 0; i < 10; i++) {
            await Task.Delay(1000);
            if (FireBaseManager.instance.isRemoteConfigComplete)
                break;
        }
        if (!FireBaseManager.instance.isRemoteConfigComplete)
            isBackupData = false;
        SceneManager.LoadSceneAsync(NameScene.MoveStopMove.ToString());
        AnimateLoadScene(cavasGroupLoadingBegin, 40);
    }

    public void UpdateGameState(int idNewState) {
        GameState newState = (GameState)idNewState;
        switch (newState) {
            case GameState.SceneHome:
                GoogleAdsManager.instance.LoadAd();
                if (FireBaseManager.instance.app != null)
                    FireBaseManager.instance.LogSceneGame(GameState.SceneHome.ToString());
                HomeSceneManager.instance.Init();
                GamePlaySceneManager.instance.Init();
                homeSceneManager.obj_home.SetActive(newState == GameState.SceneHome);
                break;
            case GameState.SceneGamePlay:
                GoogleAdsManager.instance.CreateBannerView();
                GoogleAdsManager.instance.LoadAd();
                if (FireBaseManager.instance.app != null)
                    FireBaseManager.instance.LogSceneGame(GameState.SceneGamePlay.ToString());
                UseSkinOnceTime();
                GamePlaySceneManager.instance.InitGamePlay();
                homeSceneManager.obj_home.SetActive(newState == GameState.SceneHome);
                break;
            case GameState.SceneGameZombieCity:
                LoadScene(NameScene.ZombieCity.ToString());
                GoogleAdsManager.instance.CreateBannerView();
                GoogleAdsManager.instance.LoadAd();
                if (FireBaseManager.instance.app != null)
                    FireBaseManager.instance.LogSceneGame(NameScene.ZombieCity.ToString());
                break;
        }
        gameState = newState;
    }

    public async void LoadScene(string sceneName) {
        AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(sceneName);
        sceneLoad.allowSceneActivation = false;
        cavasGroupLoadingScene.gameObject.SetActive(true);
        //chay animation khi loadscene
        for (int i = 0; i < 100; i++) {
            await Task.Delay((int)(1000 - sceneLoad.progress * 1000));
            if (sceneLoad.progress == 0.9f) {
                sceneLoad.allowSceneActivation = true;
                break;
            }
        }
        //dung animation de sang AnimateLoadScene
        animationLoadScene.Stop(animationLoadScene.clip.name);
    }

    public void LoadSceneHome() {
        SceneManager.LoadScene(NameScene.MoveStopMove.ToString());
    }

    public async void AnimateLoadScene(CanvasGroup canvas, int timeDelay) {
        for (int i = 1; i < 11; i++) {
            await Task.Delay(timeDelay);
            if (canvas == null) {
                return;
            }
            canvas.alpha = Mathf.Lerp(canvas.alpha, 1 - i * 0.1f, 1f);
        }
        canvas.gameObject.SetActive(false);
        canvas.alpha = 1;
    }

    public void UseSkinOnceTime() {
        int idTabSkinCur = Data.instance.dataPlayer.idTabSkinCur;
        int idSkinCur = Data.instance.dataPlayer.idSkinCur;

        if (idSkinCur < 0) {
            return;
        }
        switch (idTabSkinCur) {
            case (int)TabSkin.Hair:
                if (Data.instance.dataPlayer.list_hairBought[idSkinCur] == (int)TypeBuySkin.BuyOnceTime) {
                    Data.instance.dataPlayer.list_hairBought[idSkinCur] = (int)TypeBuySkin.UseBuyOnceTime;
                    Data.instance.dataPlayer.idSkinCur = -1;
                    dataController.SaveGame();
                }
                break;
            case (int)TabSkin.Pant:
                if (Data.instance.dataPlayer.list_pantBought[idSkinCur] == (int)TypeBuySkin.BuyOnceTime) {
                    Data.instance.dataPlayer.list_pantBought[idSkinCur] = (int)TypeBuySkin.UseBuyOnceTime;
                    Data.instance.dataPlayer.idSkinCur = -1;
                    dataController.SaveGame();
                }
                break;
            case (int)TabSkin.Shield:
                if (Data.instance.dataPlayer.list_shieldBought[idSkinCur] == (int)TypeBuySkin.BuyOnceTime) {
                    Data.instance.dataPlayer.list_shieldBought[idSkinCur] = (int)TypeBuySkin.UseBuyOnceTime;
                    Data.instance.dataPlayer.idSkinCur = -1;
                    dataController.SaveGame();
                }
                break;
            case (int)TabSkin.Set:
                if (Data.instance.dataPlayer.list_setBought[idSkinCur] == (int)TypeBuySkin.BuyOnceTime) {
                    Data.instance.dataPlayer.list_setBought[idSkinCur] = (int)TypeBuySkin.UseBuyOnceTime;
                    Data.instance.dataPlayer.idSkinCur = -1;
                    dataController.SaveGame();
                }
                break;
        }
    }
}