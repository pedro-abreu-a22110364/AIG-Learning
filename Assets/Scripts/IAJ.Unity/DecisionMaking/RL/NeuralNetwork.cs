using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NeuralNetwork
{
    private int[] layerSizes;
    private List<float[,]> weights;
    private List<float[]> biases;
    private System.Random random;
    private Func<float, float> activationFunction;
    private Func<float, float> activationFunctionDerivative;

    public NeuralNetwork(int[] layerSizes, string activation = "sigmoid")
    {
        this.layerSizes = layerSizes;
        random = new System.Random();

        SetActivationFunction(activation);

        weights = new List<float[,]>();
        biases = new List<float[]>();

        for (int i = 1; i < layerSizes.Length; i++)
        {
            weights.Add(InitializeMatrix(layerSizes[i], layerSizes[i - 1]));
            biases.Add(InitializeArray(layerSizes[i]));
        }
    }

    private void SetActivationFunction(string activation)
    {
        switch (activation.ToLower())
        {
            case "relu":
                activationFunction = ReLU;
                activationFunctionDerivative = ReLUDerivative;
                break;
            case "tanh":
                activationFunction = Tanh;
                activationFunctionDerivative = TanhDerivative;
                break;
            case "sigmoid":
            default:
                activationFunction = Sigmoid;
                activationFunctionDerivative = SigmoidDerivative;
                break;
        }
    }

    private float[,] InitializeMatrix(int rows, int cols)
    {
        float[,] matrix = new float[rows, cols];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                matrix[i, j] = (float)(random.NextDouble() * 2 - 1);
        return matrix;
    }

    private float[] InitializeArray(int size)
    {
        float[] array = new float[size];
        for (int i = 0; i < size; i++)
            array[i] = (float)(random.NextDouble() * 2 - 1);
        return array;
    }

    private float Sigmoid(float z) => 1.0f / (1.0f + Mathf.Exp(-z));
    private float SigmoidDerivative(float z) => Sigmoid(z) * (1 - Sigmoid(z));

    private float ReLU(float z) => Mathf.Max(0, z);
    private float ReLUDerivative(float z) => z > 0 ? 1 : 0;

    private float Tanh(float z) => (float)Math.Tanh(z);
    private float TanhDerivative(float z) => 1 - (float)Math.Pow(Math.Tanh(z), 2);

    public float[] Feedforward(float[] input)
    {
        float[] activations = input;

        for (int i = 0; i < weights.Count; i++)
        {
            float[] z = Add(MatrixVectorProduct(weights[i], activations), biases[i]);

            if (i == weights.Count - 1)
            {
                activations = Softmax(z);
            }
            else
            {
                activations = ApplyActivation(z);
            }
        }

        return activations;
    }

    private float[] Softmax(float[] z)
    {
        float max = Mathf.Max(z);
        float sumExp = 0f;
        float[] softmaxOutput = new float[z.Length];

        for (int i = 0; i < z.Length; i++)
        {
            softmaxOutput[i] = Mathf.Exp(z[i] - max);
            sumExp += softmaxOutput[i];
        }

        for (int i = 0; i < softmaxOutput.Length; i++)
        {
            softmaxOutput[i] /= sumExp;
        }

        return softmaxOutput;
    }

    private float[] ApplyActivation(float[] z)
    {
        float[] activations = new float[z.Length];
        for (int i = 0; i < z.Length; i++)
            activations[i] = activationFunction(z[i]);
        return activations;
    }

    private float[] MatrixVectorProduct(float[,] matrix, float[] vector)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[] result = new float[rows];

        for (int i = 0; i < rows; i++)
        {
            float sum = 0;
            for (int j = 0; j < cols; j++)
                sum += matrix[i, j] * vector[j];
            result[i] = sum;
        }

        return result;
    }

    private float[] Add(float[] vector, float[] bias)
    {
        float[] result = new float[vector.Length];
        for (int i = 0; i < vector.Length; i++)
            result[i] = vector[i] + bias[i];
        return result;
    }

    public void Train(float[] input, float[] target, float learningRate)
    {
        List<float[]> activations = new List<float[]> { input };
        List<float[]> zs = new List<float[]>();

        float[] activation = input;
        for (int i = 0; i < weights.Count; i++)
        {
            float[] z = Add(MatrixVectorProduct(weights[i], activation), biases[i]);
            zs.Add(z);
            activation = ApplyActivation(z);
            activations.Add(activation);
        }

        float[] delta = CostDerivative(activations[^1], target);
        for (int l = weights.Count - 1; l >= 0; l--)
        {
            float[] sp = ApplyActivationDerivative(zs[l]);
            delta = MultiplyElementWise(delta, sp);
            float[,] deltaWeight = OuterProduct(delta, activations[l]);

            for (int i = 0; i < weights[l].GetLength(0); i++)
                for (int j = 0; j < weights[l].GetLength(1); j++)
                    weights[l][i, j] -= learningRate * deltaWeight[i, j];

            for (int i = 0; i < biases[l].Length; i++)
                biases[l][i] -= learningRate * delta[i];

            if (l > 0)
                delta = MatrixVectorProduct(Transpose(weights[l]), delta);
        }
    }

    private float[] ApplyActivationDerivative(float[] z)
    {
        float[] result = new float[z.Length];
        for (int i = 0; i < z.Length; i++)
            result[i] = activationFunctionDerivative(z[i]);
        return result;
    }

    private float[] CostDerivative(float[] output, float[] target)
    {
        float[] result = new float[output.Length];
        for (int i = 0; i < output.Length; i++)
            result[i] = output[i] - target[i];
        return result;
    }

    private float[] MultiplyElementWise(float[] a, float[] b)
    {
        float[] result = new float[a.Length];
        for (int i = 0; i < a.Length; i++)
            result[i] = a[i] * b[i];
        return result;
    }

    private float[,] OuterProduct(float[] a, float[] b)
    {
        float[,] result = new float[a.Length, b.Length];
        for (int i = 0; i < a.Length; i++)
            for (int j = 0; j < b.Length; j++)
                result[i, j] = a[i] * b[j];
        return result;
    }

    private float[,] Transpose(float[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);
        float[,] transposed = new float[cols, rows];
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                transposed[j, i] = matrix[i, j];
        return transposed;
    }

    public int OutputSize => layerSizes[^1];

    public void SaveModel()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "model.dat");

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(layerSizes.Length);
            foreach (var layerSize in layerSizes)
            {
                writer.WriteLine(layerSize);
            }

            // Save weights
            for (int i = 0; i < weights.Count; i++)
            {
                int rows = weights[i].GetLength(0);
                int cols = weights[i].GetLength(1);
                writer.WriteLine(rows);
                writer.WriteLine(cols);
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        writer.Write(weights[i][r, c] + (c < cols - 1 ? "," : ""));
                    }
                    writer.WriteLine();
                }
            }

            // Save biases
            for (int i = 0; i < biases.Count; i++)
            {
                writer.WriteLine(biases[i].Length);
                for (int j = 0; j < biases[i].Length; j++)
                {
                    writer.Write(biases[i][j] + (j < biases[i].Length - 1 ? "," : ""));
                }
                writer.WriteLine();
            }
        }

        Debug.Log($"Model saved to: {filePath}");
    }

    public void LoadModel()
    {
        string filePath = System.IO.Path.Combine(Application.persistentDataPath, "model.dat");

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                // Read the number of layers
                int layerCount = int.Parse(reader.ReadLine());
                layerSizes = new int[layerCount];

                // Read layer sizes
                for (int i = 0; i < layerCount; i++)
                {
                    layerSizes[i] = int.Parse(reader.ReadLine());
                }

                // Read weights
                for (int i = 0; i < layerCount - 1; i++)
                {
                    int rows = int.Parse(reader.ReadLine());
                    int cols = int.Parse(reader.ReadLine());
                    weights[i] = new float[rows, cols];

                    for (int r = 0; r < rows; r++)
                    {
                        string[] values = reader.ReadLine().Split(',');
                        for (int c = 0; c < cols; c++)
                        {
                            weights[i][r, c] = float.Parse(values[c]);
                        }
                    }
                }

                // Read biases
                for (int i = 0; i < layerCount - 1; i++)
                {
                    int biasSize = int.Parse(reader.ReadLine());
                    biases[i] = new float[biasSize];
                    string[] values = reader.ReadLine().Split(',');

                    for (int j = 0; j < biasSize; j++)
                    {
                        biases[i][j] = float.Parse(values[j]);
                    }
                }
            }

            Debug.Log($"Model loaded successfully from: {filePath}");
        }
        else
        {
            Debug.LogError($"Model file not found: {filePath}");
        }
    }

}
