using System;
using System.Collections.Generic;
using UnityEngine;

public class REINFORCE
{
    private NeuralNetwork policyNetwork;
    private float learningRate;
    private System.Random random;

    public REINFORCE(int[] layerSizes, float learningRate = 0.01f)
    {
        this.learningRate = learningRate;
        policyNetwork = new NeuralNetwork(layerSizes);
        random = new System.Random();
    }

    public int SelectAction(float[] state)
    {
        // Get action probabilities from the policy network
        float[] actionProbabilities = policyNetwork.Feedforward(state);

        // Sample action based on probabilities
        float randomValue = (float)random.NextDouble();
        float cumulativeProbability = 0f;

        for (int i = 0; i < actionProbabilities.Length; i++)
        {
            cumulativeProbability += actionProbabilities[i];
            if (randomValue < cumulativeProbability)
            {
                return i;
            }
        }

        return actionProbabilities.Length - 1; // fallback to the last action
    }

    public void Train(List<Tuple<float[], int, float>> episode)
    {
        // Calculate the return (cumulative discounted reward) for each step
        List<float> returns = CalculateReturns(episode);

        // Update the policy network based on the policy gradient
        for (int t = 0; t < episode.Count; t++)
        {
            float[] state = episode[t].Item1;
            int action = episode[t].Item2;
            float G = returns[t]; // Return (cumulative reward)

            // Get action probabilities and log probability of the chosen action
            float[] actionProbabilities = policyNetwork.Feedforward(state);
            float actionProbability = actionProbabilities[action];
            float logProbability = Mathf.Log(actionProbability);

            // Gradient ascent on log-probability * return
            float[] target = new float[actionProbabilities.Length];
            target[action] = logProbability * G;

            // Update policy network
            policyNetwork.Train(state, target, learningRate);
        }
    }

    private List<float> CalculateReturns(List<Tuple<float[], int, float>> episode, float gamma = 0.99f)
    {
        // Compute cumulative rewards from each timestep
        List<float> returns = new List<float>();
        float G = 0f;

        for (int t = episode.Count - 1; t >= 0; t--)
        {
            G = episode[t].Item3 + gamma * G;
            returns.Insert(0, G); // insert at beginning to maintain order
        }

        return returns;
    }
}

