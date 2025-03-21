namespace Common.Data
{
    public enum EnemyMovement
    {
        Approach = 0,
        KeepDistance,
        RandomWalk,
        NoMove
    }

    public enum EnemyTriggerType
    {
        OnEnterRange = 1,
        OnDamaged,
        OnInterval,
        OnDeath
    }
    public class EnemySkillDefine
    {
        public EnemyMovement Movement { get; set; }
        public EnemyTriggerType TriggerConditions { get; set; }
    }
}