using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
    [Header("_____________________________Weapon___________________________")]
    public WeaponPlayer weaponPlayerTmp;
    public GameObject obj_weapon, obj_weaponHold;
    public ObjectPool<WeaponPlayer> poolWeapon = new ObjectPool<WeaponPlayer>();
    public Vector3 localScaleWeapon;
    public Vector3 localScaleWeaponOrigin;
    public float timeLifeWeapon;
    public float timeFlyBack;
    public Transform transformWeaponHold;
    public MeshRenderer meshrenWeaponHold;

    [Header("_____________________________PlayerSkin_______________________")]
    public SkinnedMeshRenderer skinMeshRen_body;
    public SkinnedMeshRenderer skinMeshRen_pant;
    public GameObject[] array_objHair, array_objShield, array_objSet;

    [Header("_____________________________Player___________________________")]
    public int level;
    public bool canAtk;
    public bool isBeginAtk;
    public float rangeAtk, moveSpeed;
    public Transform transformPlayer;
    float angleTmp;
    Vector3 directionPlayer;
    public bool isMove;
    public bool isRevive;
    public bool canUlti;
    bool inRangeAtk;
    public bool doneCountDownAtk;
    public int timeCountDownAtk;

    [Header("_____________________________PlayerInfo_______________________")]
    public RectTransform rectTransformInfo;
    public Text txt_name;
    public Text txt_level;
    public Image img_level;

    [Header("_____________________________Enemys___________________________")]
    public GameObject obj_isChooseAtk;
    public float[] array_distance;
    public Vector3 posEnemy, directionEnemy;

    [Header("_____________________________Camera___________________________")]
    public CameraController camController;

    [Header("_____________________________Animation________________________")]
    public Animator animator;
    public Transform transformAvatar;
    public StateAnimationCharacter stateAnim;

    void Awake() {
        rangeAtk = Constant.RANGE_ATK_BEGIN;
        moveSpeed = Constant.MOVE_SPEED_BEGIN;
        txt_name.text = Data.instance.dataPlayer.namePlayer;
        img_level.color = txt_name.color = skinMeshRen_body.material.color;
        weaponPlayerTmp = GameManager.instance.array_weaponPlayer[Data.instance.dataPlayer.idWeaponCur];
        weaponPlayerTmp.meshrenWeapon.materials = meshrenWeaponHold.materials;
        poolWeapon.objInPool = weaponPlayerTmp;
        array_distance = new float[Constant.NUM_CHARACTER_1TURN - 1];
        localScaleWeaponOrigin = localScaleWeapon = GameManager.instance.array_weaponPlayer[Data.instance.dataPlayer.idWeaponCur].transform.localScale;
        timeLifeWeapon = weaponPlayerTmp.timeLife;
        if (weaponPlayerTmp.typeAtk == TypeAtk.Return)
            timeLifeWeapon /= timeLifeWeapon;
        UpdateAnimation(StateAnimationCharacter.Idle);
    }

    void OnDestroy() {
        poolWeapon.objInPool.timeLife = GameManager.instance.dataController.FindWeaponDataById(Data.instance.dataPlayer.idWeaponCur).timeLifeOrigin;
    }

    public async void PlayerAction() {
        Move();
        CheckRangeAtk();
        if (!isMove && inRangeAtk && obj_weaponHold.activeSelf && doneCountDownAtk)
            canAtk = true;
        UpdateState();
        UpdateAnimation(stateAnim);
        if (isBeginAtk && doneCountDownAtk) {
            CalcDirToAtkEnemy(Vector3.zero);
            //AudioManager.instance.PlayAudio(AudioName.ThrowWeapon);
            isBeginAtk = false;
            doneCountDownAtk = false;
            await Task.Delay(timeCountDownAtk);
            doneCountDownAtk = true;
        }
    }

    void Move() {
        directionPlayer.x = JoystickCustom.Horizontal();
        directionPlayer.z = JoystickCustom.Vertical();

        if (directionPlayer.x != 0 || directionPlayer.z != 0) {
            transformPlayer.Translate(moveSpeed * Time.deltaTime * directionPlayer);
            transformPlayer.position = new Vector3(Mathf.Clamp(transformPlayer.position.x, -18, 18), transformPlayer.position.y, Mathf.Clamp(transformPlayer.position.z, -18, 18));
            angleTmp = Mathf.Atan2(directionPlayer.x, directionPlayer.z) * Mathf.Rad2Deg;
            transformAvatar.eulerAngles = Vector3.up * angleTmp;
            isMove = true;
        } else {
            isMove = false;
        }
    }

    public void RevivePlayer() {
        gameObject.SetActive(true);
        canAtk = isBeginAtk = false;
        isRevive = true;
        do {
            transformPlayer.position = new Vector3(Random.Range(-15, 15), 50.1f, Random.Range(-15, 15));
        } while (CheckRangeAtk() < 32);
    }

    float CheckRangeAtk() {
        for (int i = 1; i < Constant.NUM_CHARACTER_1TURN; i++)
            if (GamePlaySceneManager.instance.array_transCharacter[i].gameObject.activeSelf)
                array_distance[i - 1] = (GamePlaySceneManager.instance.array_transCharacter[i].position - transform.position).sqrMagnitude;
            else
                array_distance[i - 1] = Constant.DISTANCE_WHEN_DIE;
        float min = array_distance[0];
        for (int i = 1; i < Constant.NUM_CHARACTER_1TURN - 1; i++)
            if (min > array_distance[i])
                min = array_distance[i];
        if (min > rangeAtk * rangeAtk) {
            inRangeAtk = false;
            obj_isChooseAtk.SetActive(false);
            return min;
        }
        for (int i = 1; i < Constant.NUM_CHARACTER_1TURN; i++) {
            obj_isChooseAtk.SetActive(false);
            if (min == array_distance[i - 1]) {
                inRangeAtk = true;
                posEnemy = GamePlaySceneManager.instance.array_transCharacter[i].position;
                obj_isChooseAtk.transform.position = posEnemy;
                obj_isChooseAtk.SetActive(true);
                break;
            }
        }
        return min;
    }

    void CalcDirToAtkEnemy(Vector3 v3) {
        Quaternion q = Quaternion.Euler(v3);
        SpawnWeaponPlayer();
        weaponPlayerTmp.dir = q * (posEnemy - transform.position);
        weaponPlayerTmp.dir.y = 0;
        obj_weapon.transform.position = transform.position + Vector3.up * 0.5f + weaponPlayerTmp.dir.normalized / 8;
        obj_weapon.SetActive(true);
    }

    public void SpawnWeaponPlayer() {
        weaponPlayerTmp = poolWeapon.GetPooledObject();
        weaponPlayerTmp.player = this;
        obj_weapon = weaponPlayerTmp.gameObject;
        obj_weapon.transform.localScale = localScaleWeapon;
        if (weaponPlayerTmp.typeAtk == TypeAtk.Return)
            timeFlyBack = timeLifeWeapon;
        else
            weaponPlayerTmp.timeLife = timeLifeWeapon;
    }

    public void RotaionPlayerToAtk() {
        directionEnemy = posEnemy - transform.position;
        angleTmp = Mathf.Atan2(directionEnemy.x, directionEnemy.z) * Mathf.Rad2Deg;
        transformAvatar.eulerAngles = Vector3.up * angleTmp;
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

    public void UseWeaponCur() {
        meshrenWeaponHold = Instantiate(GameManager.instance.array_meshrenWeaponHold[Data.instance.dataPlayer.idWeaponCur], transformWeaponHold);
        meshrenWeaponHold.materials = GameManager.instance.array_meshrenSkinWeapon[Data.instance.dataPlayer.idSkinWeaponCur + Data.instance.dataPlayer.idWeaponCur * 5].sharedMaterials;
        obj_weaponHold = meshrenWeaponHold.gameObject;
        obj_weaponHold.SetActive(true);
    }
}