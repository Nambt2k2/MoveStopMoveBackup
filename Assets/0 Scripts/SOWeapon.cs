using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Data/Weapon")]
public class SOWeapon : ScriptableObject {
    public int id;
    public string nameWeapon;
    public int cost;
    public int numAttributeBuff;
    public AttributeBuff attribute;
    public int timeLifeOrigin;
}
