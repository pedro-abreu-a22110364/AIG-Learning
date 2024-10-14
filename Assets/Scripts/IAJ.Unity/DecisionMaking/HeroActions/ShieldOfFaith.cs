using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions
{
    public class ShieldOfFaith : Action
    {
        private const int SHIELD_AMOUNT = 5;
        private const int MANA_COST = 5;

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith", character)
        {
        }

        public override bool CanExecute()
        {
            return Character.baseStats.Mana >= MANA_COST; 
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
            return currentMana >= MANA_COST;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.ShieldOfFaith();
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            // Set the shield value in the world model
            worldModel.SetProperty(PropertiesName.ShieldHP, SHIELD_AMOUNT);

            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
            worldModel.SetProperty(PropertiesName.MANA, currentMana - MANA_COST); // Deduct 5 mana
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            return change;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            // This action should be prioritized if the character has low HP
            var currentHP = (int)worldModel.GetProperty(PropertiesName.HP);
            var shieldHP = (int)worldModel.GetProperty(PropertiesName.ShieldHP);

            // If HP is low, this action should have higher priority
            return (currentHP < 5) ? 10.0f : 0.0f; 
        }
    }
}
