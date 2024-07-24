using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour {
    [Header("_____________________________Weapon___________________________")]
    public WeaponEnemy weaponEnemyTmp;
    public GameObject obj_weapon, obj_weaponHold;
    public ObjectPool<WeaponEnemy> poolWeapon = new ObjectPool<WeaponEnemy>();
    public Vector3 localScaleWeapon;
    public Vector3 localScaleWeaponOrigin;
    public float timeLifeWeapon;
    public float timeFlyBack;
    public int idWeaponCur;
    public Transform transformWeaponHold;
    public MeshRenderer meshrenWeaponHold;

    [Header("_____________________________EnemySkin________________________")]
    public SkinnedMeshRenderer skinMeshRen_body;
    public SkinnedMeshRenderer skinMeshRen_pant;
    public Transform transHair, transShield, transWingSet, transTailSet, transWeaponDefSet;

    [Header("_____________________________Enemy____________________________")]
    public int level;
    public bool canAtk;
    public bool isBeginAtk;
    public float rangeAtk, moveSpeed;
    public Transform transfomEnemy;
    float angleTmp;
    public bool isMove;
    public float timeMove;
    bool inRangeAtk;
    Vector3 dirTemp;
    bool isMoveTmp;
    float timeTmp;
    public float countDownSpawnEnemy;

    [Header("_____________________________EnemyInfo________________________")]
    public RectTransform rectTransformInfo;
    public GameObject obj_info;
    public Text txt_name;
    public Text txt_level;
    public Image img_level;
    public Image img_arrowSelf;

    [Header("_____________________________Enemys___________________________")]
    public float[] array_distance;
    public Vector3 posEnemy, directionEnemy;

    [Header("_____________________________Animation________________________")]
    public Animator animator;
    public Transform transformAvatar;
    public StateAnimationCharacter stateAnim = StateAnimationCharacter.Idle;

    void Awake() {
        rangeAtk = Constant.RANGE_ATK_BEGIN;
        moveSpeed = Constant.MOVE_SPEED_BEGIN;
        array_distance = new float[Constant.NUM_CHARACTER_1TURN];
        poolWeapon.objInPool = weaponEnemyTmp;
        localScaleWeaponOrigin = localScaleWeapon = GamePlaySceneManager.instance.array_weaponEnemy[idWeaponCur].transform.localScale;
        timeLifeWeapon = weaponEnemyTmp.timeLife;
        if (weaponEnemyTmp.typeAtk == TypeAtk.Return)
            timeLifeWeapon /= timeLifeWeapon;
        UpdateAnimation(StateAnimationCharacter.Idle);
    }

    void OnDestroy() {
        poolWeapon.objInPool.timeLife = GameManager.instance.dataController.FindWeaponDataById(idWeaponCur).timeLifeOrigin;
    }

    public void EnemyAction(float deltaTime) {
        if (isMoveTmp) {
            angleTmp = Mathf.Atan2(dirTemp.x, dirTemp.z) * Mathf.Rad2Deg;
            transformAvatar.eulerAngles = Vector3.up * angleTmp;
            transfomEnemy.position += Time.deltaTime * moveSpeed * dirTemp.normalized;
            isMove = true;
            timeTmp += deltaTime;
            if (timeTmp > 0.8f) {
                isMoveTmp = false;
                isMove = false;
                timeTmp = 0;
            }
        } else
            Move();
        CheckRangeAtk();
        if (!isMove && inRangeAtk && obj_weaponHold.activeSelf)
            canAtk = true;
        if (isBeginAtk) {
            CalcDirToAtkEnemy(Vector3.zero);
            isBeginAtk = false;
            if (Random.value > 0.05f)
                DirTmpBeforeAtk();
        }
        UpdateState();
        UpdateAnimation(stateAnim);
    }

    void Move() {
        timeMove += Time.deltaTime;
        if (timeMove > 2 || inRangeAtk) {
            timeMove = 0;
            isMove = false;
            return;
        }
        if (timeMove > 0.4f && timeMove <= 2f) {
            directionEnemy = posEnemy - transform.position;
            angleTmp = Mathf.Atan2(directionEnemy.x, directionEnemy.z) * Mathf.Rad2Deg;
            transformAvatar.eulerAngles = Vector3.up * angleTmp;
            transfomEnemy.position += Time.deltaTime * moveSpeed * directionEnemy.normalized;
            isMove = true;
        }
    }

    async void DirTmpBeforeAtk() {
        await Task.Delay(200);
        dirTemp = Random.insideUnitCircle;
        dirTemp.z = dirTemp.y;
        dirTemp.y = 0;
        isMoveTmp = true;
    }

    public void ReSpwan() {
        gameObject.SetActive(true);
        do
            transfomEnemy.position = new Vector3(Random.Range(-15, 15), 50.1f, Random.Range(-15, 15));
        while ((transfomEnemy.position - GamePlaySceneManager.instance.player.transformPlayer.position).sqrMagnitude < 32);
        transfomEnemy.localScale = Vector3.one;
        localScaleWeapon = localScaleWeaponOrigin;
        timeLifeWeapon = 1;
        rectTransformInfo.anchoredPosition = Vector2.up * 1.1f;
        rangeAtk = Constant.RANGE_ATK_BEGIN;
        level = 0;
        txt_level.text = level.ToString();
        for (int i = 0; i < Mathf.Clamp(Random.Range(GamePlaySceneManager.instance.player.level - 2, GamePlaySceneManager.instance.player.level + 4), 0, GamePlaySceneManager.instance.player.level + 4); i++) {
            LevelUp();
        }
    }
    public void LevelUp() {
        rectTransformInfo.anchoredPosition += Vector2.up * 0.01f;
        rangeAtk *= Constant.ZOOM_LEVEL_UP;
        transfomEnemy.localScale *= Constant.ZOOM_LEVEL_UP;
        localScaleWeapon *= Constant.ZOOM_LEVEL_UP;
        timeLifeWeapon *= Constant.ZOOM_LEVEL_UP;
        level += 1;
        txt_level.text = level.ToString();
    }

    void CheckRangeAtk() {
        for (int i = 0; i < Constant.NUM_CHARACTER_1TURN; i++)
            if (GamePlaySceneManager.instance.array_transCharacter[i].gameObject.activeSelf && (GamePlaySceneManager.instance.array_transCharacter[i].position - transform.position).sqrMagnitude > 0)
                array_distance[i] = (GamePlaySceneManager.instance.array_transCharacter[i].position - transform.position).sqrMagnitude;
            else
                array_distance[i] = Constant.DISTANCE_WHEN_DIE;
        float min = array_distance[0];
        for (int i = 1; i < Constant.NUM_CHARACTER_1TURN; i++)
            if (min > array_distance[i])
                min = array_distance[i];
        for (int i = 0; i < Constant.NUM_CHARACTER_1TURN; i++)
            if (min == array_distance[i]) {
                if (min < rangeAtk * rangeAtk)
                    inRangeAtk = true;
                posEnemy = GamePlaySceneManager.instance.array_transCharacter[i].position;
                break;
            }
        if (min > rangeAtk * rangeAtk) {
            inRangeAtk = false;
        }
    }

    void CalcDirToAtkEnemy(Vector3 v3) {
        Quaternion q = Quaternion.Euler(v3);
        SpawnWeaponPlayer();
        weaponEnemyTmp.dir = q * (posEnemy - transform.position);
        weaponEnemyTmp.dir.y = 0;
        obj_weapon.transform.position = transform.position + Vector3.up * 0.5f + weaponEnemyTmp.dir.normalized / 8;
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
        weaponEnemyTmp = poolWeapon.GetPooledObject();
        obj_weapon = weaponEnemyTmp.gameObject;
        weaponEnemyTmp.enemy = this;
        weaponEnemyTmp.meshrenWeapon.materials = meshrenWeaponHold.materials;
        obj_weapon.transform.localScale = localScaleWeapon;
        if (weaponEnemyTmp.typeAtk == TypeAtk.Return)
            timeFlyBack = timeLifeWeapon;
        else
            weaponEnemyTmp.timeLife = timeLifeWeapon;
    }

    public void RotaionEnemyToAtk() {
        directionEnemy = posEnemy - transform.position;
        angleTmp = Mathf.Atan2(directionEnemy.x, directionEnemy.z) * Mathf.Rad2Deg;
        transformAvatar.eulerAngles = Vector3.up * angleTmp;
    }

    //animation
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

    public int RandomSkinAndWeapon(int idTabSkin, int idSet) {
        int temp = idWeaponCur = Random.Range(0, GamePlaySceneManager.instance.array_weaponEnemy.Length);
        weaponEnemyTmp = GamePlaySceneManager.instance.array_weaponEnemy[temp];

        meshrenWeaponHold = Instantiate(GameManager.instance.array_meshrenWeaponHold[temp], transformWeaponHold);
        obj_weaponHold = meshrenWeaponHold.gameObject;
        meshrenWeaponHold.materials = GameManager.instance.array_meshrenSkinWeapon[temp * 5 + Random.Range(1, 4)].sharedMaterials;

        switch (idTabSkin) {
            case (int)TabSkin.Hair:
                Instantiate(GameManager.instance.array_objHair[Random.Range(0, GameManager.instance.array_objHair.Length - 1)], transHair).SetActive(true);
                break;
            case (int)TabSkin.Pant:
                skinMeshRen_pant.material = GameManager.instance.array_matPant[Random.Range(0, GameManager.instance.array_matPant.Length - 1)];
                break;
            case (int)TabSkin.Shield:
                Instantiate(GameManager.instance.array_objShield[Random.Range(0, GameManager.instance.array_objShield.Length - 1)], transShield).SetActive(true);
                break;
            case (int)TabSkin.Set:
                switch (idSet) {
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
        img_arrowSelf.color = txt_name.color = img_level.color = skinMeshRen_body.material.color;
        return temp;
    }
}