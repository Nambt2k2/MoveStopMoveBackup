using UnityEngine;
using UnityEngine.UI;

public class ZCCameraController : MonoBehaviour {
    [Header("_____________________________Camera___________________________")]
    public Vector2 posPlayer;
    public Transform transformPlayer;
    public Camera camMain;
    public Image[] array_imgArrowZombie = new Image[16];
    public Image[] array_imgArrowBoss;
    public float minX_ArrowZombieInScreen, maxX_ArrowZombieInScreen, minY_ArrowZombieInScreen, maxY_ArrowZombieInScreen;

    [Header("_____________________________TransparentObject________________")]
    ObjectFader objFader;
    Ray ray;
    RaycastHit hit;
    Vector3 dirToPlayer;

    public void Init() {
        dirToPlayer = transformPlayer.position - transform.position;
        array_imgArrowZombie = new Image[16];
        posPlayer = camMain.WorldToScreenPoint(transformPlayer.position);
        minX_ArrowZombieInScreen = array_imgArrowBoss[0].GetPixelAdjustedRect().width / 2;
        maxX_ArrowZombieInScreen = Screen.width - minX_ArrowZombieInScreen;
        minY_ArrowZombieInScreen = array_imgArrowBoss[0].GetPixelAdjustedRect().height / 2;
        maxY_ArrowZombieInScreen = Screen.height - minY_ArrowZombieInScreen;
    }

    public void ObstaclesFader() {
        ray = new Ray(transform.position, dirToPlayer);
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider == null) return;
            if (hit.collider.gameObject == transformPlayer.gameObject) {
                if (objFader != null) {
                    objFader.DeactiveFade();
                    objFader = null;
                    return;
                }
            } else {
                if ((objFader != null && objFader.gameObject == hit.collider.gameObject) ||
                    hit.collider.gameObject.CompareTag(Tag.Zombie.ToString())) {
                    return;
                }
                if (objFader != null)
                    objFader.DeactiveFade();
                objFader = hit.collider.gameObject.GetComponent<ObjectFader>();
                if (objFader != null) {
                    objFader.ActiveFade();
                }
            }
        }
    }

    public void DisplayArrowDirectionZombie(bool b, ZCZombie zombie, int id) {
        //chuyen toa do zombie word sang screen
        Vector3 tmp = zombie.transform.position;
        if (tmp.z < transform.position.z)
            tmp.z = transform.position.z;

        Vector2 posDirEnemy = camMain.WorldToScreenPoint(tmp);

        //quay huong mui ten den zombie
        Vector2 dir = posDirEnemy - posPlayer;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
        array_imgArrowZombie[id].rectTransform.eulerAngles = Vector3.back * angle;

        //mui ten theo zombie khi player va zombie con song
        if (b) {
            if (posDirEnemy.x >= minX_ArrowZombieInScreen && posDirEnemy.x <= maxX_ArrowZombieInScreen && posDirEnemy.y >= minY_ArrowZombieInScreen && posDirEnemy.y < maxY_ArrowZombieInScreen)
                array_imgArrowZombie[id].enabled = false;
            else {
                posDirEnemy.x = Mathf.Clamp(posDirEnemy.x, minX_ArrowZombieInScreen, maxX_ArrowZombieInScreen);
                posDirEnemy.y = Mathf.Clamp(posDirEnemy.y, minY_ArrowZombieInScreen, maxY_ArrowZombieInScreen);
                array_imgArrowZombie[id].transform.position = posDirEnemy;
                array_imgArrowZombie[id].enabled = true;
            }
        } else {
            array_imgArrowZombie[id].enabled = false;
        }
    }
}
