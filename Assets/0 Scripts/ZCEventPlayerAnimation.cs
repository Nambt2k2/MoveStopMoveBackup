using UnityEngine;

public class ZCEventPlayerAnimation : MonoBehaviour {
    public ZCPlayer playerZC;

    public void Event_SetFalseCanAtk() {
        playerZC.canAtk = false;
    }

    public void Event_RotaionToAtk() {
        playerZC.RotaionPlayerToAtk();
    }

    public void Event_IsBeginAtk() {
        playerZC.isBeginAtk = true;
        playerZC.obj_weaponHold.SetActive(false);
    }

    public void Event_ActiveWeaponHold() {
        playerZC.obj_weaponHold.SetActive(true);
    }

    public void Event_DeactiveWeaponHold() {
        playerZC.obj_weaponHold.SetActive(false);
    }
}
