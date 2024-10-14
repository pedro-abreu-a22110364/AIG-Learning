using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions
{
    public class Teleport : Action
    {
        private const int MANA_COST = 5;
        private const int LEVEL = 2;

        public Teleport(AutonomousCharacter character) : base("Teleport", character) { }

        public override bool CanExecute()
        {
            return Character.baseStats.Mana >= MANA_COST && Character.baseStats.Level >= LEVEL;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            var currentLevel = (int)worldModel.GetProperty(PropertiesName.LEVEL);

            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);

            return currentMana >= MANA_COST && currentLevel >= LEVEL;
        }

        public override void Execute()
        {
            // Teleport to the initial position
            GameManager.Instance.Teleport();
            Debug.Log("Teleport executed: Sir Uthgard teleported to the initial position.");
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var currentMana = (int)worldModel.GetProperty(PropertiesName.MANA);
            worldModel.SetProperty(PropertiesName.MANA, currentMana - MANA_COST); // Deduct 5 mana

            var current = (Vector3)worldModel.GetProperty(PropertiesName.POSITION);
            worldModel.SetProperty(PropertiesName.POSITION, GameManager.Instance.initialPosition);
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            return change;
        }

        public override float GetDuration()
        {
            return 1.0f;
        }

        public override float GetHValue(WorldModel worldModel)
        {
            return -10;
        }
    }
}
