using System.Collections.Generic;
using UnityEngine;
using Antymology.Terrain;

namespace Antymology.Agents
{
    public class EvolutionManager : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject workerPrefab;
        public GameObject queenPrefab;

        [Header("Population")]
        public int workersPerEval = 12;

        [Header("NN Shape")]
        public int inputSize = 6;
        public int hiddenSize = 8;
        public int outputSize = 6;

        [Header("Evolution")]
        public int populationSize = 12;      // number of queen brains per generation
        public int elites = 3;               // top N kept
        public float mutationRate = 0.10f;
        public float mutationStrength = 0.25f;

        [Header("Evaluation")]
        public float evalSeconds = 25f;

        System.Random rng;
        float timer;

        int generation = 0;
        int currentIndex = 0;

        List<NeuralNetwork> population = new();
        List<int> scores = new();

        List<GameObject> spawned = new();
        int bestScoreEver = 0;

        void Start()
        {
            rng = new System.Random(12345);

            // init population
            population.Clear();
            for (int i = 0; i < populationSize; i++)
                population.Add(new NeuralNetwork(inputSize, hiddenSize, outputSize, rng));

            scores = new List<int>(new int[populationSize]);

            StartGeneration();
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer >= evalSeconds)
            {
                EndEvaluation();
            }
        }

        void StartGeneration()
        {
            generation++;
            currentIndex = 0;
            Debug.Log($"=== Generation {generation} ===");
            StartEvaluationForCurrent();
        }

        void StartEvaluationForCurrent()
        {
            timer = 0f;

            // Clear world count
            WorldManager.Instance.ResetNestCount();

            // Destroy previous ants
            CleanupSpawned();

            // Spawn queen with current brain
            Vector3 spawn = new Vector3(20, 150, 20); // safe high spawn; AntBase/Queen will settle
            var qObj = Instantiate(queenPrefab, spawn, Quaternion.identity);
            spawned.Add(qObj);

            var queen = qObj.GetComponent<QueenAnt>();
            queen.SetBrain(population[currentIndex].Clone());

            // Spawn workers (simple random ants)
            for (int i = 0; i < workersPerEval; i++)
            {
                Vector3 pos = new Vector3(22 + i, 150, 22);
                var w = Instantiate(workerPrefab, pos, Quaternion.identity);
                spawned.Add(w);
            }

            Debug.Log($"Eval start: gen {generation}, candidate {currentIndex+1}/{populationSize}");
        }

        void EndEvaluation()
        {
            int nestScore = WorldManager.Instance.NestCount;
            scores[currentIndex] = nestScore;
            if (nestScore > bestScoreEver) bestScoreEver = nestScore;

            Debug.Log($"Eval end: gen {generation}, candidate {currentIndex+1}/{populationSize}, nests={nestScore}, bestEver={bestScoreEver}");

            currentIndex++;
            if (currentIndex >= populationSize)
            {
                EvolvePopulation();
                StartGeneration();
            }
            else
            {
                StartEvaluationForCurrent();
            }
        }

        void EvolvePopulation()
        {
            // sort indices by score desc
            List<int> idx = new();
            for (int i = 0; i < populationSize; i++) idx.Add(i);
            idx.Sort((a, b) => scores[b].CompareTo(scores[a]));

            // keep elites
            List<NeuralNetwork> next = new();
            for (int e = 0; e < elites; e++)
                next.Add(population[idx[e]].Clone());

            // fill rest with mutated copies of elites
            while (next.Count < populationSize)
            {
                var parent = next[rng.Next(elites)].Clone();
                parent.Mutate(rng, mutationRate, mutationStrength);
                next.Add(parent);
            }

            population = next;
            Debug.Log($"Evolved population. Top score gen {generation}: {scores[idx[0]]}");
        }

        void CleanupSpawned()
        {
            for (int i = 0; i < spawned.Count; i++)
                if (spawned[i] != null) Destroy(spawned[i]);
            spawned.Clear();
        }
    }
}
