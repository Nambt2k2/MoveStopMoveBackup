using UnityEngine;

[CreateAssetMenu(fileName = "New Set", menuName = "Data/Set")]
public class SOSet : ScriptableObject {
    public int id;
    public int numAttributeBuff;
    public AttributeBuff attribute;
}

