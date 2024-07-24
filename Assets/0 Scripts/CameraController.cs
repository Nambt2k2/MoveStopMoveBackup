using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform transformPlayer;
    public Camera camMain;

    public float minX_ArrowEnemyInScreen, maxX_ArrowEnemyInScreen, minY_ArrowEnemyInScreen, maxY_ArrowEnemyInScreen;
    public Vector2 posPlayer;

    void Awake() {
        enabled = false;
    }

    void OnEnable() {
        posPlayer = camMain.WorldToScreenPoint(transformPlayer.position);
        minX_ArrowEnemyInScreen = GamePlaySceneManager.instance.array_enemy[0].img_arrowSelf.GetPixelAdjustedRect().width / 2;
        maxX_ArrowEnemyInScreen = Screen.width - minX_ArrowEnemyInScreen;
        minY_ArrowEnemyInScreen = GamePlaySceneManager.instance.array_enemy[0].img_arrowSelf.GetPixelAdjustedRect().height / 2;
        maxY_ArrowEnemyInScreen = Screen.height - minY_ArrowEnemyInScreen;
    }

    public void DisplayArrowDirectionEnemy(bool b, Enemy enemy) {
        //chuyen toa do enemy word sang screen
        Vector3 tmp = enemy.rectTransformInfo.transform.position;
        if (tmp.z < transform.position.z)
            tmp.z = transform.position.z;

        Vector2 posDirEnemy = camMain.WorldToScreenPoint(tmp);

        //quay huong mui ten den enemy
        Vector2 dir = posDirEnemy - posPlayer;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        enemy.img_arrowSelf.rectTransform.eulerAngles = Vector3.back * angle;

        //info theo enemy
        enemy.obj_info.transform.position = posDirEnemy;

        //mui ten va img level theo enemy
        if (b) {
            enemy.obj_info.SetActive(true);
            if (posDirEnemy.x >= minX_ArrowEnemyInScreen && posDirEnemy.x <= maxX_ArrowEnemyInScreen && posDirEnemy.y >= minY_ArrowEnemyInScreen && posDirEnemy.y < maxY_ArrowEnemyInScreen) {
                enemy.img_arrowSelf.enabled = false;
                enemy.img_level.transform.position = enemy.obj_info.transform.position;
                enemy.txt_name.transform.position = enemy.obj_info.transform.position + Vector3.up * 70;
            } else {
                posDirEnemy.x = Mathf.Clamp(posDirEnemy.x, minX_ArrowEnemyInScreen, maxX_ArrowEnemyInScreen);
                posDirEnemy.y = Mathf.Clamp(posDirEnemy.y, minY_ArrowEnemyInScreen, maxY_ArrowEnemyInScreen);
                enemy.img_arrowSelf.transform.position = posDirEnemy;
                enemy.img_arrowSelf.enabled = true;
                enemy.img_level.transform.position = posDirEnemy - dir.normalized * 60;
            }
        } else
            enemy.obj_info.SetActive(false);
    }
}