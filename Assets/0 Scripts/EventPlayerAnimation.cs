using UnityEngine;

public class EventPlayerAnimation : MonoBehaviour {
    public Player player;

    public void Event_SetFalseCanAtk() {
        player.canAtk = false;
    }

    public void Event_RotaionToAtk() {
        player.RotaionPlayerToAtk();
    }

    public void Event_IsBeginAtk() {
        player.isBeginAtk = true;
        player.obj_weaponHold.SetActive(false);
    }

    public void Event_ActiveWeaponHold() {
        player.obj_weaponHold.SetActive(true);
    }

    public void Event_DeactiveWeaponHold() {
        player.obj_weaponHold.SetActive(false);
    }

}
