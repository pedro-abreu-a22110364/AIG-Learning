using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class ReachedPatrolPoint : Transition
    {
        private Monster agent;
        private Vector3 patrolPoint1;
        private Vector3 patrolPoint2;

        public ReachedPatrolPoint(Monster agent, Vector3 patrolPoint1, Vector3 patrolPoint2, bool movingToPoint1)
        {
            this.agent = agent;
            this.patrolPoint1 = patrolPoint1;
            this.patrolPoint2 = patrolPoint2;
            TargetState = new Patrol(agent, patrolPoint1, patrolPoint2, movingToPoint1);
            Actions = new List<IAction>();
        }

        public override bool IsTriggered()
        {
            // Access the current target via the IsMovingToPoint1 getter
            var patrolState = (Patrol)agent.StateMachine.CurrentState;
            var currentTarget = patrolState.movingToPoint1 ? patrolPoint1 : patrolPoint2;

            // Check if agent reached the current patrol point
            bool reached = Vector3.Distance(agent.transform.position, currentTarget) < 2.0f;

            if (reached)
            {
                //Debug.Log(agent.name + " reached patrol point and switched to the next patrol point.");

                // Switch patrol point when the current target is reached
                ((Patrol)TargetState).movingToPoint1 = !((Patrol)TargetState).movingToPoint1;
            }

            return reached;
        }
    }
}
