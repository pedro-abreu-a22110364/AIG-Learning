using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine.AI;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree;
using Assets.Scripts.IAJ.Unity.DecisionMaking.BehaviorTree.BehaviourTrees;
using Assets.Scripts.IAJ.Unity.Formations;
using System.Collections.Generic;
using static GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.StateMachine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.NPCs
{

    public class Orc : Monster
    {

        public GameObject AlertSprite;

        public Vector3 pos1 { get; set; }
        public Vector3 pos2 { get; set; }

        public Orc()
        {
            this.stats.Type = "Orc";
            this.stats.XPvalue = 8;
            this.stats.AC = 14;
            this.baseStats.HP = 15;
            this.DmgRoll = () => RandomHelper.RollD10() + 2;
            this.stats.SimpleDamage = 6;
            this.stats.AwakeDistance = 15;
            this.stats.WeaponRange = 3;
        }

        public override void InitializeStateMachine()
        {
            GetPatrolPositions();

            Debug.Log(agent.name + ": Patrol point 1 - " + pos1 + "; Patrol point 2 - " + pos2);

            this.StateMachine = new StateMachine(new Patrol(this, pos1, pos2, true));

            //this.StateMachine = new StateMachine(new Sleep(this));
        }

        public override void InitializeBehaviourTree()
        {
            var gameObjs = GameObject.FindGameObjectsWithTag("Orc");

            this.BehaviourTree = new BasicTree(this, Target);
        }

        public void GetPatrolPositions()
        {
            var patrols = GameObject.FindGameObjectsWithTag("Patrol");

            float pos = float.MaxValue;
            GameObject closest = null;
            foreach (var p in patrols)
            {
                int children = p.transform.childCount;
                for (int i = 0; i < children; ++i)
                {
                    if (Vector3.Distance(this.agent.transform.position, p.transform.GetChild(i).position) < pos)
                    {
                        pos = Vector3.Distance(this.agent.transform.position, p.transform.GetChild(i).position);
                        closest = p;
                    }
                }

            }

            this.pos1 = closest.transform.GetChild(0).position;
            this.pos2 = closest.transform.GetChild(1).position;
        }

    }
}
