using UnityEngine;

public class ParticalController : MonoBehaviour {
    public ParticleSystem particle;
    public bool isPaused = false;
    public bool isEnd = false;
    public float timeParticle;
    public float playbackTime;
    public Color color;
    TimeCounter timeCounter;

    void OnEnable() {
        ParticleSystem.MainModule main = particle.main;
        timeParticle = main.startLifetime.constant;
        main.startColor = color;
        timeCounter = new TimeCounter(timeParticle);
    }

    void Update() {
        if (timeCounter.TimeCounterComplete(Time.deltaTime) && !isPaused)
            gameObject.SetActive(false);
    }

    public void PauseParticle() {
        if (particle.isPlaying) {
            particle.Pause();
            timeCounter.isPause = true;
            isPaused = true;
        }
    }

    public void ContinueParticle() {
        if (isPaused) {
            particle.Play();
            timeCounter.isPause = false;
            isPaused = false;
        }
    }
}
