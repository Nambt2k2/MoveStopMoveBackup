using UnityEngine;

public class EventEnemyAimation : MonoBehaviour {
    public Enemy enemy;

    public void Event_SetFalseCanAtk() {
        enemy.canAtk = false;
    }
    public void Event_RotaionToAtk() {
        enemy.RotaionEnemyToAtk();
    }
    public void Event_IsBeginAtk() {
        enemy.isBeginAtk = true;
        enemy.obj_weaponHold.SetActive(false);
    }
    public void Event_ActiveWeaponHold() {
        enemy.obj_weaponHold.SetActive(true);
    }
    public void Event_DeactiveWeaponHold() {
        enemy.obj_weaponHold.SetActive(false);
    }
}
