using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Game.NPCs;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class PursuitShout : IState
    {
        private Monster agent;
        public AutonomousCharacter Target { get; set; }
        private Vector3 shoutPosition;
        public Vector3 PatrolPoint1 { get; set; }
        public Vector3 PatrolPoint2 { get; set; }

        public PursuitShout(Monster agent, AutonomousCharacter target, Vector3 shoutPosition, Vector3 patrolPoint1, Vector3 patrolPoint2)
        {
            this.agent = agent;
            this.Target = target;
            this.shoutPosition = shoutPosition;
            this.PatrolPoint1 = patrolPoint1;
            this.PatrolPoint2 = patrolPoint2;
        }

        public List<IAction> GetEntryActions()
        {
            // Start moving to the shout position
            Debug.Log(agent.name + " is responding to the shout.");
            return new List<IAction>();
        }

        public List<IAction> GetActions()
        {
            // Move to the shout position
            return new List<IAction> { new MoveTo(agent, shoutPosition) };
        }

        public List<IAction> GetExitActions()
        {
            Debug.Log(agent.name + " has responded to the shout.");
            return new List<IAction>();
        }

        public List<Transition> GetTransitions()
        {
            // Transition back to patrol after reaching the shout point
            return new List<Transition>
            {
                new ToMeleeCombat(agent,Target),
                new EnemyDetected(agent, this.PatrolPoint1, this.PatrolPoint2),
                new ReachedShoutPoint(agent, shoutPosition, this.PatrolPoint1, this.PatrolPoint2)
            };
        }
    }
}
