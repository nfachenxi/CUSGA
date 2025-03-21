using System.Collections.Generic;

namespace Common.Data
{
    public enum EnemyForm
    {
        Ground = 0,
        Fly = 1,
    }

    public enum EnemyAttackType
    {
        NoRemote = 0,
        Remote = 1,
    }
    
    public class EnemyDefine
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public float HP { get; set; }
        public float ATK { get; set; }  
        public float SPD { get; set; }
        public EnemyForm Form { get; set; }
        public EnemyAttackType AttackType { get; set; }
        public float Range { get; set; }
        public List<int> Skill {  get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
    }
}
