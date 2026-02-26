using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GeneticTrainer : MonoBehaviour
{
    public int PopulationSize = 20;
    public int Generations = 30;
    public int MatchesPerGenome = 10;
    public float MutationRate = 0.15f;

    public List<GeneticGenome> Population = new();
    public GeneticGenome BestGenome;

    const int GENOME_SIZE = 7;

    public GeneticBattleEvaluator Evaluator;

    void Start()
    {
        StartCoroutine(Train());
    }

    void Initialize()
    {
        Population.Clear();

        for (int i = 0; i < PopulationSize; i++)
            Population.Add(new GeneticGenome(GENOME_SIZE));
    }

    IEnumerator Train()
    {
        Initialize();

        for (int g = 0; g < Generations; g++)
        {
            Debug.Log($"===== GENERATION {g} =====");

            foreach (var genome in Population)
            {
                float fitness = 0;

                for (int m = 0; m < MatchesPerGenome; m++)
                {
                    bool finished = false;
                    float result = 0;

                    yield return StartCoroutine(
                        Evaluator.EvaluateGenome(genome, (value) =>
                        {
                            result = value;
                            finished = true;
                        })
                    );

                    fitness += result;
                }

                genome.Fitness = fitness / MatchesPerGenome;
            }

            Population = Population
                .OrderByDescending(x => x.Fitness)
                .ToList();

            BestGenome = Population[0];

            Debug.Log($"BEST FITNESS: {BestGenome.Fitness}");

            Evolve();
        }

        Debug.Log("TRAINING FINISHED");

        Evaluator.SwitchToPlayMode(BestGenome);
    }

    void Evolve()
    {
        List<GeneticGenome> newPop = new();

        // ELITISMO
        newPop.Add(Population[0].Clone());
        newPop.Add(Population[1].Clone());

        while (newPop.Count < PopulationSize)
        {
            var p1 = Tournament();
            var p2 = Tournament();

            var child = Crossover(p1, p2);
            Mutate(child);

            newPop.Add(child);
        }

        Population = newPop;
    }

    GeneticGenome Tournament()
    {
        var a = Population[Random.Range(0, Population.Count)];
        var b = Population[Random.Range(0, Population.Count)];
        return a.Fitness > b.Fitness ? a : b;
    }

    GeneticGenome Crossover(GeneticGenome a, GeneticGenome b)
    {
        GeneticGenome child = new GeneticGenome(a.Weights.Length);

        for (int i = 0; i < child.Weights.Length; i++)
            child.Weights[i] =
                Random.value < 0.5f ? a.Weights[i] : b.Weights[i];

        return child;
    }

    void Mutate(GeneticGenome g)
    {
        for (int i = 0; i < g.Weights.Length; i++)
        {
            if (Random.value < MutationRate)
                g.Weights[i] += Random.Range(-0.4f, 0.4f);
        }
    }
}