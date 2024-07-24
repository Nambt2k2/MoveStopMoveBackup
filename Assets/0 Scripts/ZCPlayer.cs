using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ZCPlayer : MonoBehaviour {
    [Header("_____________________________Weapon___________________________")]
    public WeaponPlayer weaponPlayerTmp;
    public GameObject obj_weapon, obj_weaponHold;
    public ObjectPool<WeaponPlayer> poolWeapon = new ObjectPool<WeaponPlayer>();
    public float timeLifeWeapon;
    public float timeFlyBack;
    public Transform transformWeaponHold;
    public MeshRenderer meshrenWeaponHold;

    [Header("_____________________________PlayerSkin_______________________")]
    public SkinnedMeshRenderer skinMeshRen_body;
    public SkinnedMeshRenderer skinMeshRen_pant;
    public Transform transHair, transShield, transWingSet, transTailSet, transWeaponDefSet;

    [Header("_____________________________LevelUpUI________________________")]
    public Text TxtLevelUp;
    public Animation animationTxtLevelUp;
    public ZCAnimationTxt animationTxTKill1Zombie;
    public ObjectPool<ZCAnimationTxt> poolTxtKill1Zombie = new ObjectPool<ZCAnimationTxt>();
    public int[] array_levelUp;

    [Header("_____________________________Buff_____________________________")]
    public GameObject obj_circleRangeAtk;
    public RawImage[] array_iconShield;

    [Header("_____________________________Player___________________________")]
    public Transform transformPlayer;
    public int level;
    public bool canAtk;
    public bool isBeginAtk;
    public bool isMove;
    public bool isRevive;
    public bool isShield;
    public GameObject obj_shield;
    public ParticleSystem particalShield;
    float angleTmp;
    Vector3 directionPlayer;
    bool inRangeAtk;
    public float angleBulletBetweenBullet;
    public float rangeAtk;
    public float moveSpeed;
    public int amoutShield;
    public int maxBullet;
    public float timeCoutDownShield;
    public bool doneCountDownAtk;
    public int timeCountDownAtk;
    public ZCCameraController camController;
    public int idSkillChoosed;

    [Header("_____________________________PlayerInfo_______________________")]
    public Transform tranformInfo;
    public Text txt_name;
    public Text txt_level;
    public Image imgLevel;

    [Header("_____________________________Zombies__________________________")]
    public GameObject obj_isChooseAtk;
    Vector3 posZombie, directionZombie;
    List<float> list_distanceToZombie = new List<float>();

    [Header("_____________________________Animation________________________")]
    public Animator animator;
    public Transform transformAvatar;
    public StateAnimationCharacter stateAnim = StateAnimationCharacter.Idle;

    #region MonoBehaviour function
    void OnDestroy() {
        poolWeapon.objInPool.timeLife = GameManager.instance.dataController.FindWeaponDataById(Data.instance.dataPlayer.idWeaponCur).timeLifeOrigin;
    }

    async void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag(Tag.Zombie.ToString())) {
            if (isShield)
                return;
            if (amoutShield > 0) {
                amoutShield--;
                array_iconShield[amoutShield].gameObject.SetActive(false);
                obj_shield.SetActive(true);
                isShield = true;
                return;
            }
            gameObject.SetActive(false);
            UpdateAnimation(StateAnimationCharacter.Dead);
            foreach (ZCZombie z in ZCSceneManager.instance.list_zombie)
                if (z.gameObject.activeInHierarchy) {
                    z.agent.enabled = false;
                    z.enabled = false;
                    z.UpdateAnimation(StateAnimationZombie.Win);
                }
            foreach (WeaponPlayer w in ZCSceneManager.instance.player.poolWeapon.list_pooledObjects)
                if (w.gameObject.activeSelf)
                    w.gameObject.SetActive(false);

            if (idSkillChoosed == (int)ZCSkillBuffAbility.Revive) {
                idSkillChoosed = -1;
                await Task.Delay(2250);
                RevivePlayer();
                foreach (ZCZombie z in ZCSceneManager.instance.list_zombie)
                    if (z.gameObject.activeInHierarchy) {
                        z.agent.enabled = true;
                        z.enabled = true;
                    }
                return;
            }
            if (!isRevive)
                ZCSceneManager.instance.UpdateZCSceneState((int)ZCState.ReviveNow);
            else
                ZCSceneManager.instance.UpdateZCSceneState((int)ZCState.GameOver);

        }
    }
    #endregion

    public void Init() {
        //load vu khi
        meshrenWeaponHold = Instantiate(GameManager.instance.array_meshrenWeaponHold[Data.instance.dataPlayer.idWeaponCur], transformWeaponHold);
        obj_weaponHold = meshrenWeaponHold.gameObject;
        meshrenWeaponHold.materials = GameManager.instance.array_meshrenSkinWeapon[Data.instance.dataPlayer.idSkinWeaponCur + Data.instance.dataPlayer.idWeaponCur * 5].sharedMaterials;
        weaponPlayerTmp = poolWeapon.objInPool = GameManager.instance.array_weaponPlayer[Data.instance.dataPlayer.idWeaponCur];
        poolWeapon.objInPool.meshrenWeapon.materials = meshrenWeaponHold.materials;
        //load buff
        for (int i = 0; i < Data.instance.dataPlayer.levelBuffShield; i++)
            array_iconShield[i].gameObject.SetActive(true);
        obj_circleRangeAtk.transform.localScale *= (Data.instance.dataPlayer.levelBuffRange * 0.05f + 1);
        timeLifeWeapon = weaponPlayerTmp.timeLife *= (Data.instance.dataPlayer.levelBuffRange * 0.05f + 1);
        if (weaponPlayerTmp.typeAtk == TypeAtk.Return) {
            timeLifeWeapon /= timeLifeWeapon;
            timeLifeWeapon *= (Data.instance.dataPlayer.levelBuffRange * 0.05f + 1);
        }
        camController.camMain.fieldOfView *= (Data.instance.dataPlayer.levelBuffRange * 0.04f + 1);
        UseSkinCur();//load skin
        //load info
        txt_name.text = Data.instance.dataPlayer.namePlayer;
        txt_name.color = imgLevel.color = skinMeshRen_body.material.color;
        tranformInfo.gameObject.SetActive(false);
        UpdateAnimation(StateAnimationCharacter.Idle);
    }

    public void ReadyPlayZombieCity() {
        maxBullet = Data.instance.dataPlayer.levelBuffMaxBullet + 2;
        rangeAtk = Constant.RANGE_ATK_BEGIN * (Data.instance.dataPlayer.levelBuffRange * 0.05f + 1);
        moveSpeed = Constant.MOVE_SPEED_BEGIN * (Data.instance.dataPlayer.levelBuffSpeed * 0.1f + 1);
        amoutShield = Data.instance.dataPlayer.levelBuffShield;
        animationTxTKill1Zombie.txt.text = "+1";

        if (idSkillChoosed == (int)ZCSkillBuffAbility.Move_Fast)
            moveSpeed += 0.3f;
        else if (idSkillChoosed == (int)ZCSkillBuffAbility.Gold_Miner)
            animationTxTKill1Zombie.txt.text = "+2";

        if (ZCSceneManager.instance.zombieBoss != null)
            ZCSceneManager.instance.zombieBoss.agent.speed = moveSpeed;
    }

    public async void PlayerAction(float deltaTime) {
        Move();
        CheckRangeAtk(ZCSceneManager.instance.list_zombie);

        if (!isMove && inRangeAtk && obj_weaponHold.activeSelf && doneCountDownAtk) {
            canAtk = true;
        }

        UpdateState();
        UpdateAnimation(stateAnim);

        if (isBeginAtk && doneCountDownAtk) {
            CalcAtkWithAbility(idSkillChoosed);
            //AudioManager.instance.PlayAudio(AudioName.ThrowWeapon);
            isBeginAtk = false;
            doneCountDownAtk = false;
            await Task.Delay(timeCountDownAtk);
            doneCountDownAtk = true;
        }

        if (isShield) {
            timeCoutDownShield += deltaTime;
            if (timeCoutDownShield >= 1.5f) {
                isShield = false;
                obj_shield.SetActive(false);
                timeCoutDownShield = 0;
            }
        }
    }

    void Move() {
        directionPlayer.x = JoystickCustom.Horizontal();
        directionPlayer.z = JoystickCustom.Vertical();

        if (directionPlayer.x != 0 || directionPlayer.z != 0) {
            transformPlayer.Translate(moveSpeed * Time.deltaTime * directionPlayer);
            transformPlayer.position = new Vector3(Mathf.Clamp(transformPlayer.position.x, -33.8f, 85.6f), transformPlayer.position.y, Mathf.Clamp(transformPlayer.position.z, -57.7f, 73.8f));
            angleTmp = Mathf.Atan2(directionPlayer.x, directionPlayer.z) * Mathf.Rad2Deg;
            transformAvatar.eulerAngles = Vector3.up * angleTmp;
            isMove = true;
        } else {
            isMove = false;
        }
    }
    #region atk
    float CheckRangeAtk(List<ZCZombie> list_zombie) {
        for (int i = 0; i < list_zombie.Count; i++) {
            if (list_distanceToZombie.Count < list_zombie.Count && list_distanceToZombie.Count <= i)
                if (list_zombie[i].gameObject.activeSelf)
                    list_distanceToZombie.Add((list_zombie[i].transform.position - transform.position).sqrMagnitude);
                else
                    list_distanceToZombie.Add(Constant.DISTANCE_WHEN_DIE);
            else
                if (list_zombie[i].gameObject.activeSelf)
                list_distanceToZombie[i] = (list_zombie[i].transform.position - transform.position).sqrMagnitude;
            else
                list_distanceToZombie[i] = Constant.DISTANCE_WHEN_DIE;
        }
        if (list_distanceToZombie.Count == 0)
            return 0;
        float min = list_distanceToZombie[0];
        for (int i = 1; i < list_zombie.Count; i++)
            if (min > list_distanceToZombie[i])
                min = list_distanceToZombie[i];
        if (min > rangeAtk * rangeAtk) {
            inRangeAtk = false;
            obj_isChooseAtk.SetActive(false);
        }

        for (int i = 0; i < list_zombie.Count; i++) {
            obj_isChooseAtk.SetActive(false);
            if (min == list_distanceToZombie[i] && min < rangeAtk * rangeAtk) {
                inRangeAtk = true;
                posZombie = list_zombie[i].transform.position;
                obj_isChooseAtk.transform.position = posZombie;
                obj_isChooseAtk.SetActive(true);
                break;
            }
        }
        return min;
    }

    public void RotaionPlayerToAtk() {
        directionZombie = posZombie - transform.position;
        angleTmp = Mathf.Atan2(directionZombie.x, directionZombie.z) * Mathf.Rad2Deg;
        transformAvatar.eulerAngles = Vector3.up * angleTmp;
    }
    void CalcAtkWithAbility(int idAbility) {
        switch (idAbility) {
            case (int)ZCSkillBuffAbility.Attack_Behind:
                CalcAtkNormal();
                CalcAtkBehind();
                break;
            case (int)ZCSkillBuffAbility.Bullet_Plus:
                CalcAtkBulletPlus();
                break;
            case (int)ZCSkillBuffAbility.Continuous:
                CalcAtkNormal();
                CalcAtkContinouns();
                break;
            case (int)ZCSkillBuffAbility.Cross_Attack:
                CalcAtkNormal();
                CalcAtkCross();
                break;
            case (int)ZCSkillBuffAbility.Diagonal:
                CalcAtkDiagonal();
                break;
            case (int)ZCSkillBuffAbility.Triple_Arrow:
                CalcAtkTripleArrow();
                break;
            default:
                CalcAtkNormal();
                break;
        }
    }

    void CalcAtkNormal() {
        CalcDirToAtkEnemy(directionZombie, Vector3.zero, 0);
        for (int i = 1; i < maxBullet; i++) {
            if (level < array_levelUp[i - 1])
                break;
            CalcDirToAtkEnemy(directionZombie, i % 2 == 0 ? Vector3.up * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet : Vector3.down * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet, 0);
        }
    }
    void CalcAtkBehind() {
        CalcDirToAtkEnemy(directionZombie, Vector3.up * 180, 0);
    }

    void CalcAtkBulletPlus() {
        CalcDirToAtkEnemy(directionZombie, Vector3.zero, 0);
        CalcDirToAtkEnemy(directionZombie, Vector3.down * angleBulletBetweenBullet, 0);
        for (int i = 2; i < maxBullet + 1; i++) {
            if (level < array_levelUp[i - 2])
                break;
            CalcDirToAtkEnemy(directionZombie, i % 2 == 0 ? Vector3.up * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet : Vector3.down * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet, 0);
        }
    }

    void CalcAtkContinouns() {
        CalcDirToAtkEnemy(directionZombie, Vector3.zero, 200);
        for (int i = 1; i < maxBullet; i++) {
            if (level < array_levelUp[i - 1])
                break;
            CalcDirToAtkEnemy(directionZombie, i % 2 == 0 ? Vector3.up * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet : Vector3.down * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet, 200);
        }
    }
    void CalcAtkCross() {
        CalcDirToAtkEnemy(directionZombie, Vector3.up * 90, 0);
        CalcDirToAtkEnemy(directionZombie, Vector3.up * -90, 0);
    }

    void CalcAtkDiagonal() {
        CalcDirToAtkEnemy(directionZombie, Vector3.zero, 0);
        int amountBulletTmp = 0;
        for (int i = 1; i < maxBullet; i++) {
            if (level < array_levelUp[i - 1])
                break;
            amountBulletTmp = i;
        }
        for (int i = 1; i <= amountBulletTmp; i++)
            CalcDirToAtkEnemy(directionZombie, Vector3.up * 360 / (amountBulletTmp + 1) * i, 0);
    }
    void CalcAtkTripleArrow() {
        CalcDirToAtkEnemy(directionZombie, Vector3.zero, 0);
        CalcDirToAtkEnemy(directionZombie, Vector3.up * angleBulletBetweenBullet * 3, 0);
        CalcDirToAtkEnemy(directionZombie, Vector3.down * angleBulletBetweenBullet * 3, 0);
        for (int i = 1; i < maxBullet; i++) {
            if (level < array_levelUp[i - 1])
                break;
            if (i < 5) {
                CalcDirToAtkEnemy(directionZombie, i % 2 == 0 ? Vector3.up * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet : Vector3.down * ((int)(i / 2.1f) + 1) * angleBulletBetweenBullet, 0);
                continue;
            }
            CalcDirToAtkEnemy(directionZombie, i % 2 == 0 ? Vector3.up * ((int)(i / 2.1f) + 2) * angleBulletBetweenBullet : Vector3.down * ((int)(i / 2.1f) + 2) * angleBulletBetweenBullet, 0);
        }
    }

    async void CalcDirToAtkEnemy(Vector3 dir, Vector3 v3, int time) {
        await Task.Delay(time);
        Quaternion q = Quaternion.Euler(v3);
        SpawnWeaponPlayer();
        weaponPlayerTmp.dir = q * dir;
        weaponPlayerTmp.dir.y = 0;
        obj_weapon.transform.position = transform.position + Vector3.up * 0.15f + weaponPlayerTmp.dir.normalized / 5;
        if (weaponPlayerTmp.dir.z > 0)
            obj_weapon.transform.position += Vector3.forward * 0.05f;
        else if (weaponPlayerTmp.dir.z < 0)
            obj_weapon.transform.position += Vector3.back * 0.05f;
        obj_weapon.SetActive(true);
    }

    public void Atk(Transform transform, Vector3 dir) {
        transform.Rotate(Vector3.forward * Constant.SPEED_ATK_ROTATION * Time.deltaTime);
        transform.position += dir.normalized * Time.deltaTime * Constant.SPEED_FLY_ATK;
    }

    public void Atk(Transform transform, Vector3 dir, float timeLifeCur) {
        if (timeLifeCur == 0)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + 180, transform.eulerAngles.z);
        transform.position += dir.normalized * Time.deltaTime * Constant.SPEED_FLY_ATK;
    }

    public void Atk(Transform transform, Vector3 dir, float timeLifeCur, float timeFlyBack, Vector3 posCharacter) {
        if (timeLifeCur > timeFlyBack) {
            dir = posCharacter - transform.position;
            dir.y = 0;
            if (dir.sqrMagnitude < 0.03)
                transform.gameObject.SetActive(false);
        }
        transform.Rotate(Vector3.forward * Constant.SPEED_ATK_ROTATION * Time.deltaTime);
        transform.position += dir.normalized * Time.deltaTime * Constant.SPEED_FLY_ATK;
    }

    public void SpawnWeaponPlayer() {
        weaponPlayerTmp = poolWeapon.GetPooledObject();
        obj_weapon = weaponPlayerTmp.gameObject;
        weaponPlayerTmp.playerZC = this;
        if (weaponPlayerTmp.typeAtk == TypeAtk.Return)
            timeFlyBack = timeLifeWeapon;
        else
            weaponPlayerTmp.timeLife = timeLifeWeapon;
    }
    #endregion
    public void RevivePlayer() {
        gameObject.SetActive(true);
        canAtk = isBeginAtk = false;
        amoutShield = Data.instance.dataPlayer.levelBuffShield;
        for (int i = 0; i < amoutShield; i++)
            array_iconShield[i].gameObject.SetActive(true);
        do
            transformPlayer.position = new Vector3(Random.Range(22.4f, 29), transformPlayer.position.y, Random.Range(0, 36));
        while (CheckRangeAtk(ZCSceneManager.instance.list_zombie) < 32);
    }

    void UseSkinCur() {
        int idSkinCur = Data.instance.dataPlayer.idSkinCur;
        if (idSkinCur < 0)
            return;
        switch (Data.instance.dataPlayer.idTabSkinCur) {
            case (int)TabSkin.Hair:
                Instantiate(GameManager.instance.array_objHair[idSkinCur], transHair).SetActive(true);
                break;
            case (int)TabSkin.Pant:
                skinMeshRen_pant.material = GameManager.instance.array_matPant[idSkinCur];
                break;
            case (int)TabSkin.Shield:
                Instantiate(GameManager.instance.array_objShield[idSkinCur], transShield).SetActive(true);
                break;
            case (int)TabSkin.Set:
                switch (idSkinCur) {
                    case 0:
                        skinMeshRen_body.material.color = GameManager.instance.array_color[0];
                        skinMeshRen_pant.material.color = GameManager.instance.array_color[0];
                        Instantiate(GameManager.instance.array_objHair[3], transHair).SetActive(true);
                        Instantiate(GameManager.instance.array_objSet[0], transTailSet).SetActive(true);
                        Instantiate(GameManager.instance.array_objSet[1], transWingSet).SetActive(true);
                        break;
                    case 1:
                        skinMeshRen_body.material.color = GameManager.instance.array_color[2];
                        skinMeshRen_pant.material.color = GameManager.instance.array_color[2];
                        Instantiate(GameManager.instance.array_objHair[4], transHair).SetActive(true);
                        Instantiate(GameManager.instance.array_objSet[2], transWeaponDefSet).SetActive(true);
                        Instantiate(GameManager.instance.array_objSet[3], transWingSet).SetActive(true);
                        break;
                }
                break;
        }
    }

    void UpdateState() {
        if (isMove)
            stateAnim = StateAnimationCharacter.Run;
        else {
            stateAnim = StateAnimationCharacter.Idle;
            if (canAtk)
                stateAnim = StateAnimationCharacter.Attack;
        }
    }

    public void UpdateAnimation(StateAnimationCharacter newState) {
        for (int i = 0; i <= (int)StateAnimationCharacter.Dead; i++) {
            StateAnimationCharacter stateTmp = (StateAnimationCharacter)i;
            animator.SetBool(stateTmp.ToString(), newState == stateTmp);
        }
    }
}