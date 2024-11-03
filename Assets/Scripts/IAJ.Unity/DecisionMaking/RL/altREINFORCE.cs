using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class altREINFORCE
{
    public NeuralNetwork policyNetwork;
    private float learningRate;
    private System.Random random;
    private List<float> rewards = new List<float>();
    private List<float[]> episodeStates = new List<float[]>();
    private List<int> episodeActions = new List<int>();
    private float epsilon;
    private float minEpsilon = 0.01f;
    private float epsilonDecay = 0.995f;
    private float discountFactor = 0.99f;

    public altREINFORCE(int[] layerSizes, float learningRate = 0.05f, float initialEpsilon = 1.0f)
    {
        this.learningRate = learningRate;
        policyNetwork = new NeuralNetwork(layerSizes, "tanh");
        random = new System.Random();
        this.epsilon = initialEpsilon;
    }

    public int SelectAction(float[] state)
    {
        if (random.NextDouble() < epsilon)
        {
            return random.Next(policyNetwork.OutputSize);
        }
        else
        {
            float[] actionProbabilities = policyNetwork.Feedforward(state);
            float randomValue = (float)random.NextDouble();
            float cumulativeProbability = 0f;

            for (int i = 0; i < actionProbabilities.Length; i++)
            {
                cumulativeProbability += actionProbabilities[i];
                if (randomValue < cumulativeProbability)
                {
                    episodeStates.Add(state);
                    episodeActions.Add(i);
                    return i;
                }
            }

            return Array.IndexOf(actionProbabilities, actionProbabilities.Max());
        }
    }

    public void StoreReward(float reward)
    {
        rewards.Add(reward);
    }

    public void ResetRewards()
    {
        rewards.Clear();
        episodeStates.Clear();
        episodeActions.Clear();
    }

    public void UpdatePolicy()
    {
        List<float> returns = CalculateReturns();
        NormalizeReturns(returns);

        for (int t = 0; t < rewards.Count; t++)
        {
            float[] state = episodeStates[t];
            int action = episodeActions[t];
            float returnT = returns[t];

            Train(state, action, returnT);
        }

        epsilon = Math.Max(minEpsilon, epsilon * epsilonDecay);
    }

    private List<float> CalculateReturns()
    {
        List<float> returns = new List<float>();
        float cumulativeReward = 0f;

        for (int t = rewards.Count - 1; t >= 0; t--)
        {
            cumulativeReward = rewards[t] + discountFactor * cumulativeReward;
            returns.Insert(0, cumulativeReward);
        }

        return returns;
    }

    private void NormalizeReturns(List<float> returns)
    {
        float meanReturn = returns.Average();
        float stdDevReturn = (float)Math.Sqrt(returns.Average(v => Math.Pow(v - meanReturn, 2)));

        for (int i = 0; i < returns.Count; i++)
        {
            returns[i] = (returns[i] - meanReturn) / (stdDevReturn + 1e-5f);
        }
    }

    private void Train(float[] state, int action, float returnT)
    {
        float[] actionProbabilities = policyNetwork.Feedforward(state);
        float actionProbability = actionProbabilities[action];
        float logProbability = Mathf.Log(actionProbability);

        float[] target = new float[actionProbabilities.Length];
        target[action] = logProbability * returnT;

        policyNetwork.Train(state, target, learningRate);
    }
}
