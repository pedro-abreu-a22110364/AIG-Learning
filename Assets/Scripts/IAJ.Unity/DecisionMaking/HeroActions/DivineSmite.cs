using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private const int MANA_COST = 2;
        private int xpChange; 

        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite", character, target)
        {
            this.xpChange = target.tag.Equals("Skeleton") ? 3 : 0; // Set appropriate XP gain for defeating skeletons
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_LEVEL_GOAL)
            {
                change += -xpChange; // Deduct XP for using a smite
            }

            return change;
        }

        public override void Execute()
        {
            base.Execute();
            if (CanExecute())
            {
                GameManager.Instance.DivineSmite(this.Target);
            }
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            if (this.Target.tag.Equals("Skeleton"))
            {
                // Deactivate object
                worldModel.SetProperty(this.Target.name, false);

                // Gain XP
                int currentXP = (int)worldModel.GetProperty(PropertiesName.XP);
                worldModel.SetProperty(PropertiesName.XP, currentXP + this.xpChange);

                // Deduct mana cost
                int currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
                worldModel.SetProperty(PropertiesName.MANA, currentMana - MANA_COST);
            }
        }

        public override bool CanExecute()
        {
            return base.CanExecute() && Character.baseStats.Mana >= MANA_COST && this.Target.tag.Equals("Skeleton");
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            return base.CanExecute(worldModel) && (int)worldModel.GetProperty(PropertiesName.MANA) >= MANA_COST && this.Target.tag.Equals("Skeleton");
        }

        public override float GetHValue(WorldModel worldModel)
        {
            int xp = (int)worldModel.GetProperty(PropertiesName.XP);
            int level = (int)worldModel.GetProperty(PropertiesName.LEVEL);

            return base.GetHValue(worldModel) * 0.5f + ((float)Math.Min(xpChange, 1)) * 0.5f;
        }
    }
}
