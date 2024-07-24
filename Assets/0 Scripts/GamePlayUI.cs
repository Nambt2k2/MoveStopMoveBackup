using Firebase.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour {
    [Header("_____________________________WaitClick________________________")]
    public GameObject obj_waitClick;

    [Header("_____________________________Setting__________________________")]
    public GameObject obj_setting;
    public Button btn_setting;
    public RawImage onSound, offSound, onVibration, offVibration;

    [Header("_____________________________GamePlaying______________________")]
    public Animation animationGamePlay;
    public AnimationClip[] array_animationClip;
    public GameObject obj_gamePlay;
    public GameObject obj_joyStick;
    public Text txt_numAlive;

    [Header("_____________________________GameWin__________________________")]
    public GameObject obj_gameWin;
    public Text txt_namePlayer;

    [Header("_____________________________GameOver_________________________")]
    public GameObject obj_gameOver;
    public Text txt_numRank;
    public Text txt_nameEnemyKillPlayer;

    [Header("_____________________________Reward___________________________")]
    public GameObject obj_reward;
    public Text txt_goldReward;

    [Header("_____________________________GameRevive_______________________")]
    public GameObject obj_reviveNow;
    public Animation animationReviveNow;
    public Text txtTimeLoad;
    public float timeReviveNow;
    public Button btn_reviveByGold;
    public RectTransform rectTransBtnReviveByAds;

    public void BtnActiveSetting() {
        if (obj_waitClick.activeSelf) {
            obj_waitClick.SetActive(false);
        }
        animationGamePlay.Play();
        GamePlaySceneManager.instance.player.UpdateAnimation(StateAnimationCharacter.Idle);
        btn_setting.interactable = false;
        obj_setting.SetActive(true);
    }
    public void BtnDeactiveSetting() {
        animationGamePlay.Play(array_animationClip[0].name);
        btn_setting.interactable = true;
        obj_setting.SetActive(false);
    }
    public void BtnRevivePlayer(bool buyByGold) {
        if (buyByGold == true) {
            if (Data.instance.dataPlayer.AmountGold >= 150) {
                Data.instance.dataPlayer.AmountGold -= 150;
                GameManager.instance.dataController.SaveGame();
            }
        } else {
            //ads
        }
        GamePlaySceneManager.instance.player.RevivePlayer();
        foreach (Enemy e in GamePlaySceneManager.instance.array_enemy) {
            if (e.stateAnim == StateAnimationCharacter.Win) {
                e.gameObject.SetActive(true);
                break;
            }
        }
        GamePlaySceneManager.instance.UpdateGamePlayState((int)GamePlayState.Playing);
        animationGamePlay.Play(array_animationClip[0].name);
        btn_setting.interactable = true;
        obj_joyStick.SetActive(true);
    }

    public void BtnDeactiveRevive() {
        GamePlaySceneManager.instance.UpdateGamePlayState((int)GamePlayState.GameOver);
    }

    public void BtnRewardGoldAds() {
        Data.instance.dataPlayer.AmountGold += GamePlaySceneManager.instance.player.level * 4;
        GameManager.instance.dataController.SaveGame();
    }

    public void BtnLoadSceneHome() {
        GameManager.instance.LoadSceneHome();
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
}
