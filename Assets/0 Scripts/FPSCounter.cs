using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {
    static FPSCounter instance;
    public Text fpsText;
    float deltaTime;
    TimeCounter timeCounter = new TimeCounter(2);

    public void Init() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this);
            gameObject.SetActive(true);
        }
    }

    void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        if (timeCounter.TimeCounterComplete(Time.deltaTime)) {
            fpsText.text = string.Format("FPS: {0:0.0}", 1.0f / deltaTime);
        }
    }
}
