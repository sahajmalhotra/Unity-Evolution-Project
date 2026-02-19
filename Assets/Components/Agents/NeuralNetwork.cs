using System;
using UnityEngine;

namespace Antymology.Agents
{
    [Serializable]
    public class NeuralNetwork
    {
        public int inputSize, hiddenSize, outputSize;

        // Flattened weights: W1 is inputSize*hiddenSize, W2 is hiddenSize*outputSize
        public float[] w1, b1, w2, b2;

        public NeuralNetwork(int input, int hidden, int output, System.Random rng)
        {
            inputSize = input; hiddenSize = hidden; outputSize = output;
            w1 = new float[input * hidden];
            b1 = new float[hidden];
            w2 = new float[hidden * output];
            b2 = new float[output];
            Init(rng);
        }

        void Init(System.Random rng)
        {
            for (int i = 0; i < w1.Length; i++) w1[i] = Rand(rng, -0.5f, 0.5f);
            for (int i = 0; i < b1.Length; i++) b1[i] = Rand(rng, -0.1f, 0.1f);
            for (int i = 0; i < w2.Length; i++) w2[i] = Rand(rng, -0.5f, 0.5f);
            for (int i = 0; i < b2.Length; i++) b2[i] = Rand(rng, -0.1f, 0.1f);
        }

        static float Rand(System.Random rng, float a, float b) =>
            (float)(a + (b - a) * rng.NextDouble());

        static float Tanh(float x) => (float)Math.Tanh(x);

        public float[] Forward(float[] input)
        {
            float[] hidden = new float[hiddenSize];
            float[] output = new float[outputSize];

            for (int h = 0; h < hiddenSize; h++)
            {
                float sum = b1[h];
                for (int i = 0; i < inputSize; i++)
                    sum += input[i] * w1[i * hiddenSize + h];
                hidden[h] = Tanh(sum);
            }

            for (int o = 0; o < outputSize; o++)
            {
                float sum = b2[o];
                for (int h = 0; h < hiddenSize; h++)
                    sum += hidden[h] * w2[h * outputSize + o];
                output[o] = Tanh(sum);
            }

            return output;
        }

        public NeuralNetwork Clone()
        {
            var nn = (NeuralNetwork)MemberwiseClone();
            nn.w1 = (float[])w1.Clone();
            nn.b1 = (float[])b1.Clone();
            nn.w2 = (float[])w2.Clone();
            nn.b2 = (float[])b2.Clone();
            return nn;
        }

        public void Mutate(System.Random rng, float rate, float strength)
        {
            MutArr(rng, w1, rate, strength);
            MutArr(rng, b1, rate, strength);
            MutArr(rng, w2, rate, strength);
            MutArr(rng, b2, rate, strength);
        }

        static void MutArr(System.Random rng, float[] arr, float rate, float strength)
        {
            for (int i = 0; i < arr.Length; i++)
                if (rng.NextDouble() < rate)
                    arr[i] += Rand(rng, -strength, strength);
        }
    }
}
