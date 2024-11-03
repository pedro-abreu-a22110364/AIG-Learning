using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Utils;
using Assets.Scripts.Game;
using System;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.DecisionMaking.ForwardModel;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace RL
{
    public class TQLState
    {
        public string QHP {  get; set; }
        public int QLevel { get; set; }
        public int QMoney { get; set; }

        public TQLState(string qHP, int qLevel, int qMoney) 
        {
            this.QHP = qHP;
            this.QLevel = qLevel;
            this.QMoney = qMoney;
        }
    }
}