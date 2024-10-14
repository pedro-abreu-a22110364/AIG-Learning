using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class Patrol : IState
    {
        public Monster Agent { get; set; }
        public Vector3 PatrolPoint1 { get; set; }
        public Vector3 PatrolPoint2 { get; set; }
        public bool movingToPoint1 { get; set; }

        public Patrol(Monster agent, Vector3 patrolPoint1, Vector3 patrolPoint2, bool movingToPoint1)
        {
            this.Agent = agent;
            this.PatrolPoint1 = patrolPoint1;
            this.PatrolPoint2 = patrolPoint2;
            this.movingToPoint1 = movingToPoint1;
        }

        public List<IAction> GetEntryActions()
        {
            Debug.Log(Agent.name + " started patrolling.");
            return new List<IAction>();
        }

        public List<IAction> GetActions()
        {
            var currentTarget = movingToPoint1 ? PatrolPoint1 : PatrolPoint2;
            //Debug.Log(Agent.name + " is moving towards " + (movingToPoint1 ? "PatrolPoint1" : "PatrolPoint2"));
            return new List<IAction> { new MoveTo(Agent, currentTarget) };
        }

        public List<IAction> GetExitActions()
        {
            Debug.Log(Agent.name + " is stopping patrol.");
            return new List<IAction>();
        }

        public List<Transition> GetTransitions()
        {
            return new List<Transition>
            {
                new EnemyDetected(Agent, PatrolPoint1, PatrolPoint2),
                new ReachedPatrolPoint(Agent, PatrolPoint1, PatrolPoint2, movingToPoint1)
            };
        }
    }
}

