using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions
{
    public class GetManaPotion : WalkToTargetAndExecuteAction
    {
        private const int MANA_RESTORE_AMOUNT = 10;

        public GetManaPotion(AutonomousCharacter character, GameObject target)
            : base("GetManaPotion", character, target)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return Character.baseStats.Mana < Character.baseStats.MaxMana;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
            var maxMana = (int)worldModel.GetProperty(PropertiesName.MAXMANA);
            return currentMana < maxMana;
        }

        public override void Execute()
        {
            base.Execute();
            GameManager.Instance.GetManaPotion(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            worldModel.SetProperty(PropertiesName.MANA, MANA_RESTORE_AMOUNT);

            // Disable the target object in the simulation so it can't be reused
            worldModel.SetProperty(this.Target.name, false);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change -= goal.InsistenceValue * 0.2f;
            }
            else if (goal.Name == AutonomousCharacter.BE_QUICK_GOAL)
            {
                change += this.Duration;
            }

            return change;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
            var maxMana = (int)worldModel.GetProperty(PropertiesName.MAXMANA);

            // If mana is low, this action should be prioritized. Otherwise, deprioritize it
            return currentMana / (float)maxMana * 0.5f + base.GetHValue(worldModel) * 0.5f;
        }
    }
}

