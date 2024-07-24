using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class HomeSceneManager : MonoBehaviour {
    [Header("_____________________________Home_____________________________")]
    public GamePlaySceneManager gamePlaySceneManager;
    public static HomeSceneManager instance;
    public Animation animationCam, animationHome;
    public AnimationClip[] array_animationClip;

    [Header("_____________________________HomeUI___________________________")]
    public Text txt_amountGoldHome;
    public InputField inputfield_namePlayer;
    public RawImage onSound, offSound, onVibration, offVibration;
    public GameObject obj_home;
    public ShopSkin shopSkin;
    public ShopWeapon shopWeapon;
    public ShopWeapon shopWeaponPrefab;
    public bool isActiveBtn;

    async void Awake() {
        instance = this;
        GamePlaySceneManager.instance = gamePlaySceneManager;
        if (GameManager.instance.gameState == GameState.SceneGameZombieCity)
            GameManager.instance.AnimateLoadScene(GameManager.instance.cavasGroupLoadingScene, 40);
        else if (GameManager.instance.gameState == GameState.SceneGamePlay)
            for (int i = 0; i < 10; i++) {
                if (GamePlaySceneManager.instance.enabled == true)
                    await Task.Delay(1000);
                else
                    break;
            }
        GameManager.instance.UpdateGameState((int)GameState.SceneHome);
    }

    public void Init() {
        GameManager.instance.homeSceneManager = this;
        if (PlayerPrefs.GetInt(Constant.SOUND) != 0) {
            onSound.enabled = true;
            offSound.enabled = false;
        } else {
            onSound.enabled = false;
            offSound.enabled = true;
        }
        if (PlayerPrefs.GetInt(Constant.VIBRATION) != 0) {
            onVibration.enabled = true;
            offVibration.enabled = false;
        } else {
            onVibration.enabled = false;
            offVibration.enabled = true;
        }
        inputfield_namePlayer.text = Data.instance.dataPlayer.namePlayer;
        txt_amountGoldHome.text = Data.instance.dataPlayer.AmountGold.ToString();
        shopSkin.InitDataShopSkin();
        shopSkin.UseSkinCur();
        GamePlaySceneManager.instance.player.UseWeaponCur();
    }

    public void BtnLoadSceneGamePlay() {
        if (!isActiveBtn) {
            isActiveBtn = true;
            GameManager.instance.UpdateGameState((int)GameState.SceneGamePlay);
        }
    }

    public void BtnLoadSceneZombieCity() {
        if (!isActiveBtn) {
            isActiveBtn = true;
            GameManager.instance.UpdateGameState((int)GameState.SceneGameZombieCity);
        }
    }

    public void BtnSaveDataBackup() {
        GameManager.instance.dataController.SaveDataBackup();
    }

    public async void Btn_ActivePopupShopWeapon() {
        if (shopWeapon == null) {
            shopWeapon = Instantiate(shopWeaponPrefab);
            shopWeapon.InitDataShopWeapon();
            shopWeapon.array_meshrenWeaponHold[Data.instance.dataPlayer.idWeaponCur] = GamePlaySceneManager.instance.player.meshrenWeaponHold;
        }
        if (!isActiveBtn) {
            isActiveBtn = true;
            animationCam.Play();
            animationHome.Play();
            GamePlaySceneManager.instance.player.transformAvatar.gameObject.SetActive(false);
            await Task.Delay(250);
            shopWeapon.gameObject.SetActive(true);
        }
    }

    public void SaveNamePlayer() {//btn
        Data.instance.dataPlayer.namePlayer = inputfield_namePlayer.text;
        GameManager.instance.dataController.SaveGame();
    }

    public void OnOffSound() {//btn
        onSound.enabled = !onSound.enabled;
        offSound.enabled = !offSound.enabled;
        PlayerPrefs.SetInt(Constant.SOUND, onSound.enabled ? 1 : 0);
    }

    public void OnOffVibration() {//btn
        onVibration.enabled = !onVibration.enabled;
        offVibration.enabled = !offVibration.enabled;
        PlayerPrefs.SetInt(Constant.VIBRATION, onVibration.enabled ? 1 : 0);
    }

    public void BtnBuffGold() {
        if (GameManager.instance.isBackupData) {
            txt_amountGoldHome.text = (Data.instance.dataPlayer.AmountGold += 1000).ToString();
            GameManager.instance.dataController.SaveGame();
        }
    }
}
