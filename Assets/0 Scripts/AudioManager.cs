using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;
    public List<AudioSourceController> list_audioSourceController;

    public void Init() {
        instance = this;
        DontDestroyOnLoad(this);
        list_audioSourceController = new List<AudioSourceController>();
    }
}
