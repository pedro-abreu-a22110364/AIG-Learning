using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine;
using System.Collections.Generic;
using UnityEngine;

class ReachedShoutPoint : Transition
{
    private Monster agent;
    private Vector3 shoutPosition;

    public ReachedShoutPoint(Monster agent, Vector3 shoutPosition, Vector3 patrolPoint1, Vector3 patrolPoint2)
    {
        this.agent = agent;
        this.shoutPosition = shoutPosition;
        TargetState = new Patrol(agent, patrolPoint1, patrolPoint2, true); // Or another state
        Actions = new List<IAction>();
    }

    public override bool IsTriggered()
    {
        // Check if the orc reached the shout point
        //Debug.Log(agent.name + " reached shout point");
        return Vector3.Distance(agent.transform.position, shoutPosition) < 7.0f; // Threshold for "reached"
    }
}
