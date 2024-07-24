public enum NameScene { Loading, MoveStopMove, ZombieCity }
public enum GameState { SceneHome, SceneGamePlay, SceneGameZombieCity }
public enum GamePlayState { Playing, GameWin, GameOver, ReviveNow }
public enum ZCState { Playing, Pause, GameWin, GameOver, ReviveNow }
public enum ZCSkillBuffAbility {
    Attack_Behind, Bullet_Plus,
    Continuous, Cross_Attack, Diagonal, Gold_Miner, Move_Fast,
    Piercing, Revive, Triple_Arrow, Wall_Break,
    Blade_Circle, Chasing_Weapon, Growing_Bullet, Start_Bigger
}
public enum StateAnimationZombie { Walk, Run, Win }
public enum StateAnimationCharacter { Idle, Run, Ulti, Win, Dance, Attack, Dead }
public enum TypeAtk { Rotation, Straight, Return }
public enum AttributeBuff { Range, Gold, MoveSpeed, AtkSpeed }
public enum TabSkin { Hair, Pant, Shield, Set }
public enum TypeBuySkin { DontBuy, BuyOnceTime, UseBuyOnceTime, Buy }
public enum Tag { Character, Zombie, Building }
public enum AudioName { ThrowWeapon, WeaponHit }
public enum ParticalName { ParticalWeaponHit, ParticalLevelUp }
