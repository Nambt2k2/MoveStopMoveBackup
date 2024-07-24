using UnityEngine;

public class WeaponPlayer : MonoBehaviour {
    [Header("_____________________________Weapon___________________________")]
    public Player player;
    public ZCPlayer playerZC;
    public float timeLife, timeLifeCur;
    public Vector3 dir;
    public MeshRenderer meshrenWeapon;
    public TypeAtk typeAtk;
    public bool isWall;

    void OnEnable() {
        timeLifeCur = 0;
        isWall = false;
    }

    void OnTriggerEnter(Collider other) {
        if (GamePlaySceneManager.instance != null) {
            if (other.CompareTag(Tag.Character.ToString()) && other.gameObject != player.gameObject) {
                //ParticalManager.instance.PlayPartical(ParticalName.ParticalWeaponHit, transform.position,other.GetComponent<EnemyManager>().enemyAvatarManager.skinMeshRen_body.material.color);
                //ParticalManager.instance.PlayPartical(ParticalName.ParticalLevelUp, playerManager.transform.position,playerManager.playerAvatarManager.skinMeshRen_body.material.color);
                gameObject.SetActive(false);
                other.gameObject.SetActive(false);
                player.rectTransformInfo.anchoredPosition += Vector2.up * 10;
                player.rangeAtk *= Constant.ZOOM_LEVEL_UP;
                player.timeLifeWeapon *= Constant.ZOOM_LEVEL_UP;
                player.transformPlayer.localScale *= Constant.ZOOM_LEVEL_UP;
                player.localScaleWeapon *= Constant.ZOOM_LEVEL_UP;
                player.level += 1;
                player.txt_level.text = player.level.ToString();
                GamePlaySceneManager.instance.amountAliveCharacter--;
                GamePlaySceneManager.instance.gamePlayUI.txt_numAlive.text = "Alive: " + GamePlaySceneManager.instance.amountAliveCharacter;
            }
        } else {
            OnTriggerInZombieCity(other);
        }
    }

    void OnTriggerInZombieCity(Collider other) {
        if (ZCSceneManager.instance.zcState == ZCState.Pause)
            return;
        if (other.CompareTag(Tag.Building.ToString())) {
            isWall = true;
            if (ZCSceneManager.instance.player.idSkillChoosed == (int)ZCSkillBuffAbility.Wall_Break)
                isWall = false;
        }
        if (other.CompareTag(Tag.Zombie.ToString())) {
            if (ZCSceneManager.instance.player.idSkillChoosed != (int)ZCSkillBuffAbility.Piercing)
                gameObject.SetActive(false);
            //AudioManager.instance.PlayAudio(AudioName.WeaponHit);
            //ParticalManager.instance.PlayPartical(ParticalName.ParticalWeaponHit, transform.position,other.GetComponent<ZCZombieManager>().skinMeshRen_Body.material.color);
            if (ZCSceneManager.instance.zombieBoss != null && other.gameObject == ZCSceneManager.instance.zombieBoss.gameObject) {
                ZCSceneManager.instance.hpBossCur--;
                if (ZCSceneManager.instance.hpBossCur == 0) {
                    other.gameObject.SetActive(false);
                    ZCSceneManager.instance.amountAliveZombie--;
                    playerZC.level += 1;
                    if (ZCSceneManager.instance.player.idSkillChoosed == (int)ZCSkillBuffAbility.Gold_Miner)
                        playerZC.level += 1;
                    ZCSceneManager.instance.txtAmountAliveZombie.text = ZCSceneManager.instance.amountAliveZombie.ToString();
                    playerZC.txt_level.text = playerZC.level.ToString();
                    if (playerZC.level <= playerZC.array_levelUp[playerZC.maxBullet - 2])
                        for (int i = 1; i < playerZC.maxBullet; i++) {
                            if (playerZC.level < playerZC.array_levelUp[i - 1])
                                break;
                            if (playerZC.level == playerZC.array_levelUp[i - 1]) {
                                playerZC.TxtLevelUp.gameObject.SetActive(true);
                                //ParticalManager.instance.PlayPartical(ParticalName.ParticalLevelUp, playerManagerZC.transform.position,playerManagerZC.playerAvatarManagerZC.skinMeshRen_body.material.color);
                                break;
                            }
                        }
                    if (ZCSceneManager.instance.amountAliveZombie == 0)
                        ZCSceneManager.instance.UpdateZCSceneState((int)ZCState.GameWin);
                    foreach (ZCAnimationTxt a in playerZC.poolTxtKill1Zombie.list_pooledObjects)
                        if (!a.gameObject.activeSelf) {
                            a.gameObject.SetActive(true);
                            return;
                        }
                    playerZC.poolTxtKill1Zombie.list_pooledObjects.Add(Instantiate(playerZC.animationTxTKill1Zombie,
                        ZCSceneManager.instance.animationGameZombieCity.transform));
                }
                for (int i = Mathf.Clamp((Data.instance.dataPlayer.dayZombieCity + 1) / 5, 0, 5); i >= 0; i--)
                    if (ZCSceneManager.instance.hpBossCur < ZCSceneManager.instance.array_hpBoss[i])
                        ZCSceneManager.instance.zombieBoss.transform.localScale = Vector3.one * (1 + 0.3f * i);
                    else
                        break;
            } else {
                other.gameObject.SetActive(false);
                ZCSceneManager.instance.amountAliveZombie--;
                playerZC.level += 1;
                if (ZCSceneManager.instance.player.idSkillChoosed == (int)ZCSkillBuffAbility.Gold_Miner)
                    playerZC.level += 1;
                ZCSceneManager.instance.txtAmountAliveZombie.text = ZCSceneManager.instance.amountAliveZombie.ToString();
                playerZC.txt_level.text = playerZC.level.ToString();
                if (playerZC.level <= playerZC.array_levelUp[playerZC.maxBullet - 2])
                    for (int i = 1; i < playerZC.maxBullet; i++) {
                        if (playerZC.level < playerZC.array_levelUp[i - 1])
                            break;
                        if (playerZC.level == playerZC.array_levelUp[i - 1]) {
                            playerZC.TxtLevelUp.gameObject.SetActive(true);
                            //ParticalManager.instance.PlayPartical(ParticalName.ParticalLevelUp, playerManagerZC.transform.position,playerManagerZC.playerAvatarManagerZC.skinMeshRen_body.material.color);
                            break;
                        }
                    }
                if (ZCSceneManager.instance.amountAliveZombie == 0)
                    ZCSceneManager.instance.UpdateZCSceneState((int)ZCState.GameWin);
                foreach (ZCAnimationTxt a in playerZC.poolTxtKill1Zombie.list_pooledObjects)
                    if (!a.gameObject.activeSelf) {
                        a.gameObject.SetActive(true);
                        return;
                    }
                playerZC.poolTxtKill1Zombie.list_pooledObjects.Add(Instantiate(playerZC.animationTxTKill1Zombie,
                    ZCSceneManager.instance.transTxtKill1Zombie));
            }
        }
    }

    public void WeaponAtkInGamePlay(float deltaTime) {
        if (timeLifeCur < timeLife) {
            if (!isWall)
                if (player.canUlti) {
                    Atk(TypeAtk.Straight);
                    transform.localScale += Mathf.Clamp(timeLifeCur * 2, 0, 10f) * Vector3.one;
                } else
                    Atk(typeAtk);
            else
                if (typeAtk == TypeAtk.Return && timeLifeCur > player.timeFlyBack)
                    gameObject.SetActive(false);
            timeLifeCur += deltaTime;
        } else
            gameObject.SetActive(false);
    }

    public void WeaponAtkInZombieCity(float deltaTime) {
        if (timeLifeCur < timeLife) {
            if (!isWall)
                Atk();
            else
                if (typeAtk == TypeAtk.Return && timeLifeCur > playerZC.timeFlyBack)
                    gameObject.SetActive(false);
            timeLifeCur += deltaTime;
        } else
            gameObject.SetActive(false);
    }

    void Atk(TypeAtk typeAtk) {
        switch (typeAtk) {
            case TypeAtk.Rotation:
                player.Atk(transform, dir);
                break;
            case TypeAtk.Straight:
                player.Atk(transform, dir, timeLifeCur);
                break;
            case TypeAtk.Return:
                player.Atk(transform, dir, timeLifeCur, player.timeFlyBack, player.transformPlayer.position);
                break;
        }
    }

    void Atk() {
        switch (typeAtk) {
            case TypeAtk.Rotation:
                playerZC.Atk(transform, dir);
                break;
            case TypeAtk.Straight:
                playerZC.Atk(transform, dir, timeLifeCur);
                break;
            case TypeAtk.Return:
                playerZC.Atk(transform, dir, timeLifeCur, playerZC.timeFlyBack, playerZC.transformPlayer.position);
                break;
        }
    }
}
