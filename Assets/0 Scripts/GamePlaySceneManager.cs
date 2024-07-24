using UnityEngine;
using System.Threading.Tasks;

public class GamePlaySceneManager : MonoBehaviour {
    [Header("_____________________________Game_____________________________")]
    public GamePlayState gamePlayState;
    public static GamePlaySceneManager instance;
    public int amountAliveCharacter, amountCharacterSpawned;
    public CameraController camController;
    public Player player;
    public Enemy[] array_enemy;
    public WeaponEnemy[] array_weaponEnemy;
    public Transform[] array_transCharacter;
    public string[] array_nameEnemy;
    public GamePlayUI gamePlayUI;

    #region function MonoBehaviour
    void OnDestroy() {
        AudioManager.instance.list_audioSourceController.Clear();
        //foreach (ObjectPool<ParticalController> p in ParticalManager.instance.poolPartical)
        //    p.list_pooledObjects.Clear();
    }
    void Update() {
        if (gamePlayUI.obj_waitClick.activeSelf && player.isMove)//wait click
            gamePlayUI.obj_waitClick.SetActive(false);
        //enemy hanh dong
        for (int i = 1; i < Constant.NUM_CHARACTER_1TURN; i++) {
            //display arrow follow enamy
            camController.DisplayArrowDirectionEnemy(player.gameObject.activeSelf && array_enemy[i - 1].gameObject.activeSelf, array_enemy[i - 1]);
            if (array_enemy[i - 1].gameObject.activeSelf)
                array_enemy[i - 1].EnemyAction(Time.deltaTime);
            else {
                if (amountAliveCharacter >= 1 && array_enemy[i - 1].stateAnim != StateAnimationCharacter.Win)
                    array_enemy[i - 1].UpdateAnimation(StateAnimationCharacter.Dead);
                //respawn enemy da chet qua 5 giay
                if (!array_enemy[i - 1].gameObject.activeSelf && amountCharacterSpawned <= Constant.NUM_OBJECT_MAX - Constant.NUM_CHARACTER_1TURN) {
                    array_enemy[i - 1].countDownSpawnEnemy += Time.deltaTime;
                    if (array_enemy[i - 1].countDownSpawnEnemy > 5) {
                        array_enemy[i - 1].ReSpwan();
                        amountCharacterSpawned++;
                        array_enemy[i - 1].countDownSpawnEnemy = 0;
                    }
                }
            }
            for (int j = 0; j < array_enemy[i - 1].poolWeapon.list_pooledObjects.Count; j++) {
                if (array_enemy[i - 1].poolWeapon.list_pooledObjects[j].gameObject.activeSelf)
                    array_enemy[i - 1].poolWeapon.list_pooledObjects[j].WeaponAtk(Time.deltaTime);
            }
        }
        //player hanh dong
        if (player.gameObject.activeSelf && !gamePlayUI.obj_setting.activeSelf)
            player.PlayerAction();
        for (int i = 0; i < player.poolWeapon.list_pooledObjects.Count; i++)
            if (player.poolWeapon.list_pooledObjects[i].gameObject.activeSelf)
                player.poolWeapon.list_pooledObjects[i].WeaponAtkInGamePlay(Time.deltaTime);
        //logic game over
        if (gamePlayState == GamePlayState.ReviveNow) {
            gamePlayUI.timeReviveNow += Time.deltaTime;
            gamePlayUI.txtTimeLoad.text = (5 - (int)gamePlayUI.timeReviveNow).ToString();
            if (gamePlayUI.timeReviveNow >= 6f) {
                gamePlayUI.txtTimeLoad.text = 0.ToString();
                gamePlayUI.animationReviveNow.Stop();
                if (gamePlayUI.timeReviveNow >= 6.3f)
                    UpdateGamePlayState((int)GamePlayState.GameOver);
            }
        }
        //logic enemy win
        if (amountAliveCharacter == 1 && !player.gameObject.activeSelf) {
            for (int i = 1; i < Constant.NUM_CHARACTER_1TURN; i++) {
                if (array_enemy[i - 1].gameObject.activeSelf) {
                    array_enemy[i - 1].UpdateAnimation(StateAnimationCharacter.Win);
                    array_enemy[i - 1].stateAnim = StateAnimationCharacter.Win;
                    array_enemy[i - 1].gameObject.SetActive(false);
                    break;
                }
            }
            player.gameObject.SetActive(false);
            player.UpdateAnimation(StateAnimationCharacter.Dead);
            if (player.isRevive)
                enabled = false;
            return;
        }
        //logic game win
        if (amountAliveCharacter < 1 && player.gameObject.activeSelf)
            UpdateGamePlayState((int)GamePlayState.GameWin);
    }
    #endregion

    public void Init() {
        GameManager.instance.gamePlaySceneManager = this;
        //random ten enemy
        int idBegin = Random.Range(0, array_nameEnemy.Length);
        for (int i = 0; i < array_enemy.Length; i++)
            array_enemy[i].txt_name.text = array_nameEnemy[(idBegin + i) % array_nameEnemy.Length];
        //random skin and weapon
        for (int i = 0; i < array_enemy.Length; i++)
            array_enemy[i].skinMeshRen_body.material.color = GameManager.instance.array_color[i + 3];
        int amountEnemyUseSet = Random.Range(0, 3);
        int idBeginEnemyUseSet = Random.Range(0, array_enemy.Length);
        for (int i = 0; i < amountEnemyUseSet; i++)
            array_enemy[(i + idBeginEnemyUseSet) % array_enemy.Length].RandomSkinAndWeapon((int)TabSkin.Set, i);
        for (int i = 0; i < array_enemy.Length; i++) {
            if (idBeginEnemyUseSet <= i && i < idBeginEnemyUseSet + amountEnemyUseSet)
                continue;
            array_enemy[i].RandomSkinAndWeapon(Random.Range(0, (int)TabSkin.Set), 0);
        }
        if (array_enemy[array_enemy.Length - 1].skinMeshRen_body.material.color == GameManager.instance.array_color[GameManager.instance.array_color.Length - 1])
            array_enemy[array_enemy.Length - 1].txt_level.color = Color.black;
    }

    public async void InitGamePlay() {
        gamePlayUI = Instantiate(gamePlayUI);
        gamePlayUI.obj_joyStick.SetActive(false);
        for (int i = 0; i < array_enemy.Length; i++) {
            array_enemy[i].obj_info.transform.SetParent(gamePlayUI.obj_gamePlay.transform);
            array_enemy[i].obj_info.transform.SetSiblingIndex(i);
        }
        if (PlayerPrefs.GetInt(Constant.SOUND) != 0) {
            gamePlayUI.onSound.enabled = true;
            gamePlayUI.offSound.enabled = false;
        } else {
            gamePlayUI.onSound.enabled = false;
            gamePlayUI.offSound.enabled = true;
        }
        if (PlayerPrefs.GetInt(Constant.VIBRATION) != 0) {
            gamePlayUI.onVibration.enabled = true;
            gamePlayUI.offVibration.enabled = false;
        } else {
            gamePlayUI.onVibration.enabled = false;
            gamePlayUI.offVibration.enabled = true;
        }
        HomeSceneManager.instance.animationCam.Play(HomeSceneManager.instance.array_animationClip[2].name);
        gamePlayUI.btn_setting.interactable = false;
        await Task.Delay(1000);
        player.camController.enabled = true;
        enabled = true;
        foreach (Transform transCharacter in array_transCharacter) {
            transCharacter.gameObject.SetActive(true);
        }
        gamePlayUI.obj_joyStick.SetActive(true);
        gamePlayUI.obj_waitClick.SetActive(true);
        gamePlayUI.btn_setting.interactable = true;
    }

    #region function state game play
    public void UpdateGamePlayState(int idNewState) {
        GamePlayState newState = (GamePlayState)idNewState;
        switch (newState) {
            case GamePlayState.GameWin:
                if (gamePlayUI.obj_setting.activeSelf)
                    gamePlayUI.obj_setting.SetActive(false);
                else {
                    gamePlayUI.animationGamePlay.Play();
                    gamePlayUI.btn_setting.interactable = false;
                }
                camController.camMain.fieldOfView = 20;
                player.gameObject.SetActive(false);
                player.UpdateAnimation(StateAnimationCharacter.Win);
                for (int i = 1; i < Constant.NUM_CHARACTER_1TURN; i++) {
                    camController.DisplayArrowDirectionEnemy(player.gameObject.activeSelf, array_enemy[i - 1]);
                    if (!array_transCharacter[i].gameObject.activeSelf) {
                        array_enemy[i - 1].UpdateAnimation(StateAnimationCharacter.Dead);
                        array_enemy[i - 1].img_level.gameObject.SetActive(false);
                        array_enemy[i - 1].txt_name.enabled = false;
                    }
                }
                enabled = false;

                gamePlayUI.txt_goldReward.text = (player.level * 2).ToString();
                gamePlayUI.txt_namePlayer.text = player.txt_name.text;
                gamePlayUI.obj_reward.SetActive(true);

                Data.instance.dataPlayer.AmountGold += player.level * 2;
                GameManager.instance.dataController.SaveGame();
                break;
            case GamePlayState.GameOver:
                if (gamePlayUI.obj_setting.activeSelf) {
                    gamePlayUI.obj_setting.SetActive(false);
                } else if (player.isRevive) {
                    gamePlayUI.animationGamePlay.Play();
                    gamePlayUI.btn_setting.interactable = false;
                }

                gamePlayUI.txt_goldReward.text = (player.level * 2).ToString();
                gamePlayUI.txt_numRank.text = "# " + amountAliveCharacter.ToString();
                gamePlayUI.obj_reward.SetActive(true);

                Data.instance.dataPlayer.AmountGold += player.level * 2;
                GameManager.instance.dataController.SaveGame();
                break;
            case GamePlayState.ReviveNow:
                if (gamePlayUI.obj_setting.activeSelf)
                    gamePlayUI.obj_setting.SetActive(false);
                else {
                    gamePlayUI.animationGamePlay.Play();
                    gamePlayUI.btn_setting.interactable = false;
                }
                if (gamePlayUI.obj_waitClick.activeSelf)
                    gamePlayUI.obj_waitClick.SetActive(false);
                if (Data.instance.dataPlayer.AmountGold < 150) {
                    gamePlayUI.btn_reviveByGold.gameObject.SetActive(false);
                    gamePlayUI.rectTransBtnReviveByAds.anchoredPosition = Vector2.up * gamePlayUI.rectTransBtnReviveByAds.anchoredPosition.y;
                }
                break;
        }
        gamePlayState = newState;
        gamePlayUI.obj_gameWin.SetActive(newState == GamePlayState.GameWin);
        gamePlayUI.obj_gameOver.SetActive(newState == GamePlayState.GameOver);
        gamePlayUI.obj_reviveNow.SetActive(newState == GamePlayState.ReviveNow);
    }
    #endregion
}
