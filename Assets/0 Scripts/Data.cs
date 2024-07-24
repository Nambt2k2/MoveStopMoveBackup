using UnityEngine;

public class Data : MonoBehaviour {
    public static Data instance;
    public DataPlayer dataPlayer;
    public SOWeapon[] array_weaponData;
    public SOSet[] array_setData;

    public void Init() {
        instance = this;
        DontDestroyOnLoad(this);
    }
}
