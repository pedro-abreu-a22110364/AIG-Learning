using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class Pursuit : IState
    {

        public Monster Agent { get; set; }
        public AutonomousCharacter Target { get; set; }

        public float maximumRange { get; set; }
        public Vector3 PatrolPoint1 { get; set; }
        public Vector3 PatrolPoint2 { get; set; }

        public Pursuit(Monster agent, AutonomousCharacter target, Vector3 patrolPoint1, Vector3 patrolPoint2)
        {
            this.Agent = agent;
            this.Target = target;
            this.PatrolPoint1 = patrolPoint1;
            this.PatrolPoint2 = patrolPoint2;
        }

        public List<IAction> GetEntryActions() { Debug.Log(Agent.name + "is pursuing"); return new List<IAction>(); }

        public List<IAction> GetActions()
        { return new List<IAction> { new MoveTo(Agent, Target.transform.position)}; }

        public List<IAction> GetExitActions() { Debug.Log(Agent.name + "is no longer pursuing"); return new List<IAction>(); }

        public List<Transition> GetTransitions()
        {
            return new List<Transition> 
            { 
                new ToMeleeCombat(Agent,Target), 
                new LostEnemy(Agent, Target, PatrolPoint1, PatrolPoint2)
            };
        }
    } 
}
