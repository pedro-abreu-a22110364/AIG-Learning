using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions
{
    public class LayOnHands : Action
    {
        private const int MANA_COST = 7;
        private const int LEVEL = 2;

        public LayOnHands(AutonomousCharacter character) : base("LayOnHands", character) { }

        public override bool CanExecute()
        {
            var currentLevel = Character.baseStats.Level;

            var currentHP = Character.baseStats.HP;

            var maxHP = Character.baseStats.MaxHP;

            var currentMana = Character.baseStats.Mana;

            return currentMana >= MANA_COST && currentLevel >= LEVEL && currentHP < maxHP;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            var currentLevel = (int)worldModel.GetProperty(PropertiesName.LEVEL);

            var currentHP = (int)worldModel.GetProperty(PropertiesName.HP);

            var maxHP = (int)worldModel.GetProperty(PropertiesName.MAXHP);

            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);

            return currentMana >= MANA_COST && currentLevel >= LEVEL && currentHP < maxHP;
        }

        public override void Execute()
        {
            GameManager.Instance.LayOnHands();
            Debug.Log("Lay On Hands executed: Full HP restored.");
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            worldModel.SetProperty(PropertiesName.HP, Character.baseStats.MaxHP);

            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
            worldModel.SetProperty(PropertiesName.MANA, currentMana - MANA_COST); // Deduct 5 mana
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            return change;
        }

        public override float GetDuration()
        {
            return 2.0f;
        }
        public override float GetHValue(WorldModel worldModel)
        {
            var currentHP = (int)worldModel.GetProperty(PropertiesName.HP);
            var maxHP = (int)worldModel.GetProperty(PropertiesName.MAXHP);

            return currentHP / maxHP * 0.5f;
        }
    }
}
