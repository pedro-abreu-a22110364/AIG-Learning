using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class REINFORCE
{
    public NeuralNetwork policyNetwork;
    private float learningRate;
    private System.Random random;
    private List<float> rewards = new List<float>(); // Store rewards for each episode
    private List<float[]> episodeStates = new List<float[]>(); // Store states from each episode
    private List<int> episodeActions = new List<int>(); // Store actions taken in each episode

    public REINFORCE(int[] layerSizes, float learningRate = 0.05f)
    {
        this.learningRate = learningRate;
        policyNetwork = new NeuralNetwork(layerSizes, "tanh");
        random = new System.Random();
    }

    public int SelectAction(float[] state)
    {
        // Get action probabilities only for executable actions
        float[] actionProbabilities = policyNetwork.Feedforward(state);

        // Sample action based on probabilities
        float randomValue = (float)random.NextDouble();
        float cumulativeProbability = 0f;

        for (int i = 0; i < actionProbabilities.Length; i++)
        {
            cumulativeProbability += actionProbabilities[i];
            if (randomValue < cumulativeProbability)
            {
                // Store state and action for policy update later
                episodeStates.Add(state);
                episodeActions.Add(i);
                return i;
            }
        }

        // Fallback to the most probable action if something goes wrong
        return Array.IndexOf(actionProbabilities, actionProbabilities.Max());
    }



    public void StoreReward(float reward)
    {
        rewards.Add(reward);
    }

    public void ResetRewards()
    {
        rewards.Clear();
        episodeStates.Clear(); // Clear states for the next episode
        episodeActions.Clear(); // Clear actions for the next episode
    }

    public void UpdatePolicy()
    {
        // Calculate returns (cumulative discounted rewards)
        float discountFactor = 0.99f;
        List<float> returns = new List<float>();
        float cumulativeReward = 0f;

        for (int t = rewards.Count - 1; t >= 0; t--)
        {
            cumulativeReward = rewards[t] + discountFactor * cumulativeReward;
            returns.Insert(0, cumulativeReward);
        }

        NormalizeReturns(returns);

        for (int t = 0; t < rewards.Count; t++)
        {
            float[] state = episodeStates[t];
            int action = episodeActions[t];
            float returnT = returns[t];

            Train(state, action, returnT);
        }
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
