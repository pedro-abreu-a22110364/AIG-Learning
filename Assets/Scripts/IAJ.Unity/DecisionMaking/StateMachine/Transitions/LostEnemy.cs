using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class LostEnemy : Transition
    {
        private NPC enemy;
        public Monster agent;

        public LostEnemy(Monster agent, NPC enemy, Vector3 patrolPoint1, Vector3 patrolPoint2)
        {
            this.agent = agent;
            this.enemy = GameManager.Instance.Character;
            TargetState = new Patrol(agent, patrolPoint1, patrolPoint2, true);
            Actions = new List<IAction>();
        }

        public override bool IsTriggered()
        {
            return !(Vector3.Distance(agent.transform.position, enemy.transform.position) <= agent.stats.AwakeDistance * 2.0f);
        }
    }
}