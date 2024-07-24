using UnityEngine;
using UnityEngine.UI;

public class ZCAnimationTxt : MonoBehaviour {
    public Text txt;
    public Animation anim;

    public void DeactiveGameobject() {//event animation
        gameObject.SetActive(false);
    }
}
