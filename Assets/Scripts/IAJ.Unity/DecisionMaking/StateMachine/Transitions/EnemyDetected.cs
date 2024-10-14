using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class EnemyDetected : Transition
    {
        private AutonomousCharacter enemy;
        public Monster agent;

        public EnemyDetected(Monster agent, Vector3 patrolPoint1, Vector3 patrolPoint2)
        {
            // Find all Orcs in the scene with the tag "Orc"
            var orcs = GameObject.FindGameObjectsWithTag("Orc");
            List<NPC> orcList = new List<NPC>();

            // Loop through the orcs and ensure they have the NPC component
            foreach (var orcObject in orcs)
            {
                NPC orc = orcObject.GetComponent<NPC>();
                if (orc != null && orc is Orc) // Ensure the object is an Orc
                {
                    orcList.Add(orc);
                }
            }

            this.agent = agent;
            this.enemy = GameManager.Instance.Character;

            if (agent is Orc)
            {
                TargetState = new PursuitShout(agent, enemy, GameManager.Instance.Character.gameObject.transform.position, patrolPoint1, patrolPoint2);
            }
            else
            {
                TargetState = new Pursuit(agent, enemy, patrolPoint1, patrolPoint2);
            }

            Actions = new List<IAction> { new Shout(agent, orcList, GameManager.Instance.Character.gameObject.transform.position, patrolPoint1, patrolPoint2) };

        }

        public override bool IsTriggered()
        {
            return (Vector3.Distance(agent.transform.position, enemy.transform.position) <= agent.stats.AwakeDistance);
        }
    }
}