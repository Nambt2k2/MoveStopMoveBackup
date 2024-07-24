using UnityEngine;
using UnityEngine.AI;

public class ZCZombie : MonoBehaviour {
    public float moveSpeed = 1.5f;
    public Transform transformPlayer;
    public Animator animator;
    public NavMeshAgent agent;
    public SkinnedMeshRenderer skinMeshRen_Body;
    void Awake() {
        agent.speed = moveSpeed = Constant.MOVE_SPEED_BEGIN - 0.2f;
        UpdateAnimation(StateAnimationZombie.Walk);
        if (ZCSceneManager.instance.obj_readyPlaying.activeSelf)
            enabled = false;
    }

    void OnEnable() {
        UpdateAnimation(StateAnimationZombie.Run);
    }

    public void ZombieAction() {
        agent.SetDestination(transformPlayer.position);
    }

    public void UpdateAnimation(StateAnimationZombie newState) {
        for (int i = 0; i <= (int)StateAnimationZombie.Win; i++) {
            StateAnimationZombie stateTmp = (StateAnimationZombie)i;
            animator.SetBool(stateTmp.ToString(), newState == stateTmp);
        }
    }
}
