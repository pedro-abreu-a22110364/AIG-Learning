using Assets.Scripts.Game;
using Assets.Scripts.Game.NPCs;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine
{
    class Shout : IAction
    {
        private AutonomousCharacter enemy;
        private NPC sourceOrc; // The Orc that shouts
        private List<NPC> nearbyOrcs; // The other Orcs that hear the shout
        private Vector3 shoutPosition; // The position from where the shout originates
        public Vector3 PatrolPoint1 { get; set; }
        public Vector3 PatrolPoint2 { get; set; }

        public Shout(NPC orc, List<NPC> orcList, Vector3 position, Vector3 patrolPoint1, Vector3 patrolPoint2)
        {
            this.sourceOrc = orc;
            this.nearbyOrcs = orcList;
            this.shoutPosition = position;
            this.PatrolPoint1 = patrolPoint1;
            this.PatrolPoint2 = patrolPoint2;

            this.enemy = GameManager.Instance.Character;
        }

        public void Execute()
        {
            if(sourceOrc is not Orc) { return; }

            // The source Orc makes the shout
            Debug.Log(sourceOrc.name + " shouted from " + shoutPosition);

            // All nearby Orcs move toward the shout position
            foreach (var orc in nearbyOrcs)
            {
                if (orc != sourceOrc && orc is Orc respondingOrc)
                {
                    //Debug.Log(orc.name + " is moving toward the shout at " + shoutPosition);
                    //orc.StartPathfinding(shoutPosition); // Move to the shout position

                    // Ensure the patrol points are calculated
                    respondingOrc.GetPatrolPositions();

                    // Access the patrol points from the responding orc
                    Vector3 patrolPoint1 = respondingOrc.pos1; 
                    Vector3 patrolPoint2 = respondingOrc.pos2;

                    // Create the PursuitShout state using the patrol points from the responding orc
                    var responseState = new PursuitShout(respondingOrc, this.enemy, shoutPosition, patrolPoint1, patrolPoint2);
                    respondingOrc.StateMachine.CurrentState = responseState; // Transition to PursuitShout

                }
            }
        }
    }
}
