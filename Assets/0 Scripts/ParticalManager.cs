using System.Collections.Generic;
using UnityEngine;

public class ParticalManager : MonoBehaviour {
    public static ParticalManager instance;
    public List<ParticalController> list_particalController;

    public void Init() {
        instance = this;
        DontDestroyOnLoad(this);
        list_particalController = new List<ParticalController>();
    }
}
