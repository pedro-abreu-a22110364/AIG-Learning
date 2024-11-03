using RL;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.HeroActions.Action;


namespace Assets.Scripts.IAJ.Unity.DecisionMaking
{
    public class QLearning
    {
        private List<Action> actions { get; set; }
        private List<TQLState> states { get; set; }
        private int maxEpisodes { get; set; }
        private int maxSteps { get; set; }
        private float learningRate { get; set; }
        private float discountRate { get; set; }
        private float epsilon {  get; set; }
        private float  epsilonDecay { get; set; }
        private float epsilonMin {  get; set; }
        public float[,] qTable { get; set; }

        public bool inProgress { get; set; }
        public float totalProcessedTime { get; set; }
        public Action bestAction { get; private set; }

        public QLearning(List<Action> actions, List<TQLState> states, int maxEpisodes, int maxSteps, float learningRate, float discountRate, float epsilon, float epsilonDecay, float minEpsilon)
        {
            this.actions = actions;
            this.states = states;
            this.maxEpisodes = maxEpisodes;
            this.maxSteps = maxSteps;
            this.learningRate = learningRate;
            this.discountRate = discountRate;
            this.epsilon = epsilon;
            this.epsilonDecay = epsilonDecay;
            this.epsilonMin = epsilonMin;

            this.qTable = new float[states.Count, actions.Count];
        }

        public void InitializeQLearning()
        {
            this.inProgress = true;
            this.totalProcessedTime = 0.0f;
            this.bestAction = null;
        }

        public int ChooseAction(int stateIndex)
        {
            if (UnityEngine.Random.value < epsilon)
            {
                return UnityEngine.Random.Range(0, actions.Count);
            }
            else
            {
                int bestActionIndex = 0;
                float maxQValue = qTable[stateIndex, 0];

                for (int i = 1; i < actions.Count; i++)
                {
                    if (qTable[stateIndex, i] > maxQValue)
                    {
                        maxQValue = qTable[stateIndex, i];
                        bestActionIndex = i;
                    }
                }
                return bestActionIndex;
            }
        }

        public void UpdateQValue(int stateIndex, int actionIndex, int nextStateIndex, float reward)
        {
            float maxQNext = qTable[nextStateIndex, 0];
            for (int i = 1; i < actions.Count; i++)
            {
                maxQNext = Math.Max(maxQNext, qTable[nextStateIndex, i]);
            }

            qTable[stateIndex, actionIndex] =
                (1 - learningRate) * qTable[stateIndex, actionIndex] +
                learningRate * (reward + discountRate * maxQNext);
        }

        public void Train()
        {
            for (int episode = 0; episode < maxEpisodes; episode++)
            {
                int currentStateIndex = UnityEngine.Random.Range(0, states.Count);
                for (int step = 0; step < maxSteps; step++)
                {
                    int action = ChooseAction(currentStateIndex);

                    float reward = GetRewardForAction(currentStateIndex, actions[action]);
                    int nextStateIndex = GetNextState(currentStateIndex, actions[action]);

                    UpdateQValue(currentStateIndex, action, nextStateIndex, reward);

                    currentStateIndex = nextStateIndex;

                    epsilon = Mathf.Max(epsilonMin, epsilon * epsilonDecay);
                }
            }

            inProgress = false;
        }

        private float GetRewardForAction(int stateIndex, Action action)
        {
            // Implement logic to obtain the reward for taking an action in a state
            return 0.0f;
        }

        private int GetNextState(int currentStateIndex, Action action)
        {
            // Implement logic to obtain the next state index after taking an action
            return currentStateIndex;
        }
    }
}