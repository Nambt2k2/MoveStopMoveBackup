using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZCSceneManager : MonoBehaviour {
    [Header("_____________________________Game_____________________________")]
    public ZCState zcState;
    public static ZCSceneManager instance;
    public ZCPlayer player;
    float timeCountDownSpawnZompie;
    public int numZombieMax;
    public int amountAliveZombie;
    GameObject obj_zombieTmp;
    public ZCZombie zombie;
    public ZCZombie zombieDog;
    public List<ZCZombie> list_zombie = new List<ZCZombie>();
    public ObjectPool<ZCZombie> poolZombieDog = new ObjectPool<ZCZombie>();
    public ObjectPool<ZCZombie> poolZombie = new ObjectPool<ZCZombie>();
    ZCZombie[] arr_zombieTmp;
    public ZCCameraController camController;
    public Vector2[] array_roadSpawn;

    [Header("_____________________________SkillBuffPopUp_____________________________")]
    public ZCSkillBuffController skillBuffController;

    [Header("_____________________________Boss_____________________________")]
    public ZCZombie zombieBoss;
    public ZCZombie[] array_zombieBoss;
    public int[] array_hpBoss;
    public int hpBossCur;

    [Header("_____________________________Ready____________________________")]
    public GameObject obj_readyPlaying;
    public Animation animationReady;
    public Text txtReady;
    public float timeReady;

    [Header("_____________________________WaitClick________________________")]
    public GameObject obj_waitClick;
    public Animation animationWaitClick;

    [Header("_____________________________Pause_____________________________")]
    public GameObject obj_pause;
    public GameObject obj_notChooseSkill;
    public GameObject obj_chooseSkill;
    public Button btn_pause;
    public Image onSound, offSound, onVibration, offVibration;

    [Header("_____________________________GamePlaying______________________")]
    public Animation animationGameZombieCity;
    public AnimationClip[] array_animationClip;
    public GameObject obj_dayProgressEndGame;
    public Text[] array_txtDayZombieProgressEndGame;
    public Text txtAmountAliveZombie;
    public Text txtDayZombie;
    public Image imgArrowToZombie;
    public Transform transTxtKill1Zombie;

    [Header("_____________________________GameWin__________________________")]
    public GameObject obj_gameWin;
    public GameObject[] array_objDayWin;
    public Text txtAnountGoldGameWin, txtX3amountGoldGameWin;
    public Text txtDayZombieGameWin;

    [Header("_____________________________GameOver_________________________")]
    public GameObject obj_gameOver;
    public GameObject[] array_objDayOver;
    public Text txtAmountGoldGameOver, txtX3amountGoldGameOver;

    [Header("_____________________________GameRevive_______________________")]
    public GameObject obj_reviveNow;
    public Animation animationReviveNow;
    public Text txtTimeLoad;
    float timeReviveNow;
    public Button btn_reviveByGold;
    public RectTransform rectTransBtnReviveByAds;

    #region function MonoBehaviour
    void Awake() {
        instance = this;

        poolZombie.objInPool = zombie;
        poolZombieDog.objInPool = zombieDog;
        obj_readyPlaying.SetActive(false);
        obj_waitClick.SetActive(false);

        int idDayTmp = Data.instance.dataPlayer.dayZombieCity;
        amountAliveZombie = numZombieMax = idDayTmp * 10 + Constant.NUM_OBJECT_MAX;
        txtAmountAliveZombie.text = numZombieMax.ToString();
        txtDayZombie.text = "Day " + (idDayTmp + 1).ToString();
        txtDayZombieGameWin.text = "YOU SURVIVED DAY " + (idDayTmp + 1) + "!";
        for (int i = 0; i < 5; i++) {
            array_txtDayZombieProgressEndGame[i].text = "Day " + (idDayTmp / 5 * 5 + i + 1);
        }
        idDayTmp %= 5;
        for (int i = 0; i < 5; i++) {
            array_objDayOver[i].SetActive(false);
            if (i < idDayTmp)
                array_objDayWin[i].SetActive(true);
            else
                array_objDayWin[i].SetActive(false);
        }

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
        skillBuffController.Init();
        player.Init();
        camController.Init();
        enabled = false;
        GameManager.instance.AnimateLoadScene(GameManager.instance.cavasGroupLoadingScene, 40);
    }

    void OnDestroy() {
        AudioManager.instance.list_audioSourceController.Clear();
        //foreach (ObjectPool<ParticalController> p in ParticalManager.instance.poolPartical)
        //    p.list_pooledObjects.Clear();

        if (zombieBoss != null)
            zombieBoss.transform.localScale = Vector3.one;
    }

    void Start() {
        obj_readyPlaying.SetActive(true);
        if ((Data.instance.dataPlayer.dayZombieCity + 1) % 5 == 0) {
            SpawnBoss();
            hpBossCur = array_hpBoss[(Data.instance.dataPlayer.dayZombieCity + 1) / 5];
            zombieBoss.transform.localScale = Vector3.one * (1 + 0.3f * Mathf.Clamp((Data.instance.dataPlayer.dayZombieCity + 1) / 5, 1, 5));
        }
        //random and save skill of next day zombiecity
        Data.instance.dataPlayer.list_idSkillBuffAbility[0] = Random.Range(0, (int)ZCSkillBuffAbility.Wall_Break);
        Data.instance.dataPlayer.list_idSkillBuffAbility[1] = (Data.instance.dataPlayer.list_idSkillBuffAbility[0] + Random.Range(1, (int)ZCSkillBuffAbility.Wall_Break)) % ((int)ZCSkillBuffAbility.Wall_Break + 1);
        GameManager.instance.dataController.SaveGame();
        //load data player truoc khi vao game
        player.ReadyPlayZombieCity();
    }

    void FixedUpdate() {
        if (zcState == ZCState.Pause)
            return;
        //ready play game
        if (obj_readyPlaying.activeSelf) {
            timeReady += Time.deltaTime;
            timeCountDownSpawnZompie += Time.deltaTime;
            if (timeCountDownSpawnZompie > 0.7f) {
                SpawnZombie();
                Vector3 posRandom = player.transform.position;
                posRandom.x += Random.value >= 0.5f ? Random.Range(-3.5f, -1.5f) : Random.Range(2f, 3.5f);
                posRandom.z += Random.value >= 0.5f ? Random.Range(-3.5f, -2f) : Random.Range(1.5f, 3.5f);
                obj_zombieTmp.transform.position = posRandom;
                obj_zombieTmp.transform.eulerAngles = Vector3.up * Random.Range(0, 360);
                timeCountDownSpawnZompie = 0;
            }
            if (timeReady < 3)
                txtReady.text = ((int)(4 - timeReady)).ToString();
            else if (timeReady >= 3 && timeReady < 4)
                txtReady.text = "Go!";
            else if (timeReady >= 4) {
                obj_readyPlaying.SetActive(false);
                obj_waitClick.SetActive(true);
                player.enabled = true;
                player.tranformInfo.gameObject.SetActive(true);
                foreach (ZCZombie z in list_zombie)
                    z.enabled = true;
            }
            return;
        }
        if (player.gameObject.activeSelf && player.enabled) {
            camController.ObstaclesFader();
            player.PlayerAction(Time.deltaTime);
            for (int i = 0; i < list_zombie.Count; i++)
                if (list_zombie[i].gameObject.activeSelf)
                    list_zombie[i].ZombieAction();
            for (int i = 0; i < player.poolWeapon.list_pooledObjects.Count; i++)
                if (player.poolWeapon.list_pooledObjects[i].gameObject.activeSelf)
                    player.poolWeapon.list_pooledObjects[i].WeaponAtkInZombieCity(Time.deltaTime);
        }
        //wait click
        if (obj_waitClick.activeSelf && player.isMove)
            obj_waitClick.SetActive(false);

        //run anmation revive now
        if (zcState == ZCState.ReviveNow) {
            timeReviveNow += Time.deltaTime;
            txtTimeLoad.text = (5 - (int)timeReviveNow).ToString();
            if (timeReviveNow >= 6f) {
                txtTimeLoad.text = 0.ToString();
                animationReviveNow.Stop();
                if (timeReviveNow >= 6.3f)
                    UpdateZCSceneState((int)ZCState.GameOver);
            }
        }

        //spawn zombie
        if (numZombieMax > 0 && zcState == ZCState.Playing) {
            timeCountDownSpawnZompie += Time.deltaTime;
            if (timeCountDownSpawnZompie > 1.2f) {
                SpawnZombie();
                timeCountDownSpawnZompie = 0;
            }
        }

        //arrow follow boss
        if (zombieBoss != null) {
            if (zombieBoss.gameObject.activeSelf)
                camController.DisplayArrowDirectionZombie(zombieBoss.gameObject.activeSelf && player.gameObject.activeSelf, zombieBoss, 0);
            else if (amountAliveZombie == 50) {
                bool isInRoad = false;
                Vector2 tmp = Vector2.zero;
                do {
                    tmp = UnityEngine.Random.insideUnitCircle.normalized * UnityEngine.Random.Range(10f, 20f);
                    tmp = new Vector2(player.transformPlayer.position.x + tmp.x, player.transformPlayer.position.z + tmp.y);
                    for (int i = 0; i < array_roadSpawn.Length / 2; i++) {
                        if (array_roadSpawn[2 * i].x < tmp.x && tmp.x < array_roadSpawn[2 * i + 1].x && array_roadSpawn[2 * i].y < tmp.y && tmp.y < array_roadSpawn[2 * i + 1].y) {
                            zombieBoss.transform.position = new Vector3(tmp.x, zombieBoss.transform.position.y, tmp.y);
                            isInRoad = true;
                            break;
                        }
                    }
                } while (!isInRoad);
                camController.array_imgArrowZombie[0] = camController.array_imgArrowBoss[0];
                camController.array_imgArrowBoss[0].color = zombieBoss.skinMeshRen_Body.material.color;
                zombieBoss.gameObject.SetActive(true);
                camController.DisplayArrowDirectionZombie(zombieBoss.gameObject.activeSelf && player.gameObject.activeSelf, zombieBoss, 0);
            }
        }

        //arrow follow zombie when num zombie <= 15
        if (arr_zombieTmp == null && amountAliveZombie == 15) {
            arr_zombieTmp = new ZCZombie[16];
            int i = 1;
            foreach (ZCZombie z in list_zombie)
                if (z.gameObject.activeSelf) {
                    camController.array_imgArrowZombie[i] = Instantiate(imgArrowToZombie, animationGameZombieCity.transform);
                    camController.array_imgArrowZombie[i].color = z.skinMeshRen_Body.material.color;
                    arr_zombieTmp[i] = z;
                    i++;
                }
        }
        if (amountAliveZombie <= 15)
            for (int i = 0; i < 16; i++) {
                if (arr_zombieTmp[i] == null || arr_zombieTmp[i] == zombieBoss)
                    continue;

                camController.DisplayArrowDirectionZombie(arr_zombieTmp[i].gameObject.activeSelf && player.gameObject.activeSelf, arr_zombieTmp[i], i);
            }
    }
    #endregion
    void SpawnZombie() {
        if (Data.instance.dataPlayer.dayZombieCity >= 9 && Random.value > 0.7f) {
            if (poolZombieDog.GetPooledObjectNotInstantiate() == null) {
                zombieDog = poolZombieDog.GetPooledObject();
                list_zombie.Add(zombieDog);
            } else
                zombieDog = poolZombieDog.GetPooledObject();
            zombieDog.skinMeshRen_Body.material.color = GameManager.instance.array_color[Random.Range(0, GameManager.instance.array_color.Length)];
            zombieDog.transformPlayer = player.transform;
            obj_zombieTmp = zombieDog.gameObject;
        } else {
            if (poolZombie.GetPooledObjectNotInstantiate() == null) {
                zombie = poolZombie.GetPooledObject();
                list_zombie.Add(zombie);
            } else
                zombie = poolZombie.GetPooledObject();
            zombie.skinMeshRen_Body.material.color = GameManager.instance.array_color[Random.Range(0, GameManager.instance.array_color.Length)];
            zombie.transformPlayer = player.transform;
            obj_zombieTmp = zombie.gameObject;
        }
        bool isInRoad = false;
        Vector2 tmp = Vector2.zero;
        do {
            tmp = Random.insideUnitCircle.normalized * Random.Range(7f, 20f);
            tmp = new Vector2(player.transformPlayer.position.x + tmp.x, player.transformPlayer.position.z + tmp.y);
            for (int i = 0; i < array_roadSpawn.Length / 2; i++) {
                if (array_roadSpawn[2 * i].x < tmp.x && tmp.x < array_roadSpawn[2 * i + 1].x && array_roadSpawn[2 * i].y < tmp.y && tmp.y < array_roadSpawn[2 * i + 1].y) {
                    obj_zombieTmp.transform.position = new Vector3(tmp.x, player.transformPlayer.position.y, tmp.y);
                    isInRoad = true;
                    break;
                }
            }
        } while (!isInRoad);
        obj_zombieTmp.SetActive(true);
        numZombieMax -= 1;
    }
    public void SpawnBoss() {
        zombieBoss = Instantiate(array_zombieBoss[Random.Range(0, array_zombieBoss.Length)]);
        list_zombie.Add(zombieBoss);
        zombieBoss.skinMeshRen_Body.material.color = GameManager.instance.array_color[Random.Range(0, GameManager.instance.array_color.Length)];
        zombieBoss.transformPlayer = player.transform;
        obj_zombieTmp = zombieBoss.gameObject;
        numZombieMax -= 1;
    }

    #region btnEvent
    public void BtnLoadSceneHome() {
        GameManager.instance.LoadScene(NameScene.MoveStopMove.ToString());
    }
    public void BtnLoadNextDayZombieCity() {
        GameManager.instance.UpdateGameState((int)GameState.SceneGameZombieCity);
    }
    public void BtnContinueGame() {
        animationGameZombieCity.Play(array_animationClip[0].name);
        btn_pause.interactable = true;
        foreach (ZCZombie z in list_zombie)
            if (z.gameObject.activeSelf) {
                z.animator.speed = 1;
                z.agent.speed = zombie.moveSpeed;
            }
        foreach (ZCAnimationTxt a in player.poolTxtKill1Zombie.list_pooledObjects)
            if (a.gameObject.activeSelf)
                a.anim[a.anim.clip.name].speed = 1;
        foreach (AudioSourceController a in AudioManager.instance.list_audioSourceController)
            if (a.gameObject.activeSelf)
                a.ContinueAudio();
        //foreach (ObjectPool<ParticalController> p in ParticalManager.instance.poolPartical)
        //    foreach (ParticalController q in p.list_pooledObjects)
        //        if (q.gameObject.activeSelf)
        //            q.ContinueParticle();

        player.particalShield.Play();
        player.animator.speed = 1;
        animationReady[animationReady.clip.name].speed = 1;
        animationWaitClick[animationWaitClick.clip.name].speed = 1;
        player.animationTxtLevelUp[player.animationTxtLevelUp.clip.name].speed = 1;
        foreach (ZCAnimationTxt a in player.poolTxtKill1Zombie.list_pooledObjects)
            a.anim[a.anim.clip.name].speed = 1;

        UpdateZCSceneState((int)ZCState.Playing);
    }
    public void BtnClaimGoldEndGame(int id) {
        Data.instance.dataPlayer.AmountGold += player.level * (int)Mathf.Pow(3, id);
        GameManager.instance.dataController.SaveGame();
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
        player.RevivePlayer();
        player.isRevive = true;
        foreach (ZCZombie z in list_zombie) {
            z.agent.enabled = true;
            z.enabled = true;
        }
        UpdateZCSceneState((int)ZCState.Playing);
        animationGameZombieCity.Play(array_animationClip[0].name);
        btn_pause.interactable = true;
    }
    #endregion


    #region function state Game manager
    public void UpdateZCSceneState(int idNewState) {
        ZCState newState = (ZCState)idNewState;
        switch (newState) {
            case ZCState.Pause:
                animationGameZombieCity.Play();
                btn_pause.interactable = false;
                animationReady[animationReady.clip.name].speed = 0;
                obj_waitClick.SetActive(false);

                foreach (ZCZombie z in list_zombie)
                    if (z.gameObject.activeSelf) {
                        z.animator.speed = 0;
                        z.agent.speed = 0;
                    }
                foreach (ZCAnimationTxt a in player.poolTxtKill1Zombie.list_pooledObjects)
                    if (a.gameObject.activeSelf)
                        a.anim[a.anim.clip.name].speed = 0;
                foreach (AudioSourceController a in AudioManager.instance.list_audioSourceController)
                    if (a.gameObject.activeSelf)
                        a.PauseAudio();
                //foreach (ObjectPool<ParticalController> p in ParticalManager.instance.poolPartical)
                //    foreach (ParticalController q in p.list_pooledObjects)
                //        if (q.gameObject.activeSelf)
                //            q.PauseParticle();

                player.particalShield.Pause();
                player.animator.speed = 0;
                player.animationTxtLevelUp[player.animationTxtLevelUp.clip.name].speed = 0;
                foreach (ZCAnimationTxt a in player.poolTxtKill1Zombie.list_pooledObjects)
                    a.anim[a.anim.clip.name].speed = 0;

                break;
            case ZCState.GameWin:
                animationGameZombieCity.Play();
                btn_pause.interactable = false;
                player.gameObject.SetActive(false);
                foreach (WeaponPlayer w in player.poolWeapon.list_pooledObjects)
                    if (w.gameObject.activeSelf)
                        w.gameObject.SetActive(false);

                camController.camMain.fieldOfView = 30;
                player.transformAvatar.transform.rotation = Quaternion.Euler(Vector3.up * 180);
                player.UpdateAnimation(StateAnimationCharacter.Win);
                obj_dayProgressEndGame.SetActive(true);
                array_objDayWin[Data.instance.dataPlayer.dayZombieCity % 5].SetActive(true);
                txtAnountGoldGameWin.text = player.level.ToString();
                txtX3amountGoldGameWin.text = (player.level * 3).ToString();
                Data.instance.dataPlayer.dayZombieCity++;
                GameManager.instance.dataController.SaveGame();
                break;
            case ZCState.GameOver:
                if (player.isRevive) {
                    animationGameZombieCity.Play();
                    btn_pause.interactable = false;
                }
                obj_dayProgressEndGame.SetActive(true);
                array_objDayOver[Data.instance.dataPlayer.dayZombieCity % 5].SetActive(true);
                txtAmountGoldGameOver.text = player.level.ToString();
                txtX3amountGoldGameOver.text = (player.level * 3).ToString();
                break;
            case ZCState.ReviveNow:
                animationGameZombieCity.Play();
                btn_pause.interactable = false;
                if (obj_waitClick.activeSelf)
                    obj_waitClick.SetActive(false);
                if (Data.instance.dataPlayer.AmountGold < 150) {
                    btn_reviveByGold.gameObject.SetActive(false);
                    rectTransBtnReviveByAds.anchoredPosition = Vector2.up * rectTransBtnReviveByAds.anchoredPosition.y;
                }
                break;
        }
        zcState = newState;
        obj_pause.SetActive(newState == ZCState.Pause);
        obj_gameWin.SetActive(newState == ZCState.GameWin);
        obj_gameOver.SetActive(newState == ZCState.GameOver);
        obj_reviveNow.SetActive(newState == ZCState.ReviveNow);
    }
    #endregion

    public void OnOffSound() {
        onSound.enabled = !onSound.enabled;
        offSound.enabled = !offSound.enabled;
        PlayerPrefs.SetInt(Constant.SOUND, onSound.enabled ? 1 : 0);
    }
    public void OnOffVibration() {
        onVibration.enabled = !onVibration.enabled;
        offVibration.enabled = !offVibration.enabled;
        PlayerPrefs.SetInt(Constant.VIBRATION, onVibration.enabled ? 1 : 0);
    }
}
