using UnityEngine;

public class AudioSourceController : MonoBehaviour {
    public AudioSource audioSource;
    public bool isPaused = false;

    void Update() {
        if ((!audioSource.isPlaying && !isPaused) || PlayerPrefs.GetInt(Constant.SOUND) == 0)
            gameObject.SetActive(false);
    }

    public void PauseAudio() {
        if (audioSource.isPlaying) {
            audioSource.Pause();
            isPaused = true;
        }
    }

    public void ContinueAudio() {
        if (isPaused) {
            audioSource.UnPause();
            isPaused = false;
        }
    }
}
