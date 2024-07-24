using UnityEngine;

public class WeaponEnemy : MonoBehaviour {
    [Header("_____________________________Weapon___________________________")]
    public float timeLife;
    public float timeLifeCur;
    public Enemy enemy;
    public Vector3 dir;
    public MeshRenderer meshrenWeapon;
    public TypeAtk typeAtk;
    public bool isWall;

    void OnEnable() {
        timeLifeCur = 0;
        isWall = false;
    }

    public void WeaponAtk(float deltaTime) {
        if (timeLifeCur < timeLife) {
            if (!isWall)
                Atk(typeAtk);
            else
                if (typeAtk == TypeAtk.Return && timeLifeCur > enemy.timeFlyBack)
                    gameObject.SetActive(false);
            timeLifeCur += deltaTime;
        } else
            gameObject.SetActive(false);
    }

    void Atk(TypeAtk typeAtk) {
        switch (typeAtk) {
            case TypeAtk.Rotation:
                enemy.Atk(transform, dir);
                break;
            case TypeAtk.Straight:
                enemy.Atk(transform, dir, timeLifeCur);
                break;
            case TypeAtk.Return:
                enemy.Atk(transform, dir, timeLifeCur, enemy.timeFlyBack, enemy.transfomEnemy.position);
                break;
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag(Tag.Character.ToString()) && other.gameObject != enemy.gameObject) {
            //ParticalManager.instance.PlayPartical(ParticalName.ParticalLevelUp, enemyManager.transform.position,enemyManager.enemyAvatarManager.skinMeshRen_body.material.color);
            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
            enemy.LevelUp();
            if (other.gameObject == GamePlaySceneManager.instance.player.gameObject) {
                //ParticalManager.instance.PlayPartical(ParticalName.ParticalWeaponHit, other.transform.position,BasicGameSceneManager.instance.playerManager.playerAvatarManager.skinMeshRen_body.material.color);
                GamePlaySceneManager.instance.player.UpdateAnimation(StateAnimationCharacter.Dead);
                GamePlaySceneManager.instance.gamePlayUI.txt_nameEnemyKillPlayer.text = enemy.txt_name.text;
                GamePlaySceneManager.instance.gamePlayUI.txt_nameEnemyKillPlayer.color = enemy.skinMeshRen_body.material.color;
                if (!GamePlaySceneManager.instance.player.isRevive)
                    GamePlaySceneManager.instance.UpdateGamePlayState((int)GamePlayState.ReviveNow);
                else
                    GamePlaySceneManager.instance.UpdateGamePlayState((int)GamePlayState.GameOver);
                return;
            }
            //ParticalManager.instance.PlayPartical(ParticalName.ParticalWeaponHit, other.transform.position,other.GetComponent<EnemyManager>().enemyAvatarManager.skinMeshRen_body.material.color);
            GamePlaySceneManager.instance.gamePlayUI.txt_numAlive.text = "Alive: " + GamePlaySceneManager.instance.amountAliveCharacter;
            GamePlaySceneManager.instance.amountAliveCharacter--;
        }
    }
}
