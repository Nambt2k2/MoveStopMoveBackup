public class TimeCounter {
    float time;
    float timeTarget;
    public bool isPause;

    public TimeCounter(float timeTarget) {
        this.timeTarget = timeTarget;
    }

    public bool TimeCounterComplete(float deltaTime) {
        if (!isPause) {
            if (time < timeTarget) {
                time += deltaTime;
                return false;
            } else {
                time = 0;
                return true;
            }
        }
        return false;
    }
}
