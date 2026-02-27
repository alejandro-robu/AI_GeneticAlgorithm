using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CrossoverType
{
    OnePoint,
    Average,
    Uniform
}

public enum MutationType
{
    RandomReset,
    Gaussian,
    Adaptive
}

[Serializable]
public class GeneticAlgorithm
{
    public List<Individual> population;

    private int _currentIndex;

    public int CurrentGeneration;
    public int MaxGenerations;

    private CrossoverType crossoverType;
    private MutationType mutationType;

    public string Summary;

    public GeneticAlgorithm(int generations, int populationSize,
        CrossoverType crossover,
        MutationType mutation)
    {
        MaxGenerations = generations;
        crossoverType = crossover;
        mutationType = mutation;

        GenerateRandomPopulation(populationSize);
        Summary = "Generation,Fitness\n";
    }

    void GenerateRandomPopulation(int size)
    {
        population = new List<Individual>();

        for (int i = 0; i < size; i++)
        {
            population.Add(
                new Individual(
                    Random.Range(0f, 90f),
                    Random.Range(-45f, 45f),
                    Random.Range(0f, 12f)
                )
            );
        }

        StartGeneration();
    }

    public Individual GetFittest()
    {
        population.Sort();
        return population[0];
    }

    void StartGeneration()
    {
        _currentIndex = 0;
        CurrentGeneration++;
    }

    public Individual GetNext()
    {
        if (_currentIndex >= population.Count)
        {
            EndGeneration();

            if (CurrentGeneration >= MaxGenerations)
            {
                Debug.Log(Summary);
                return null;
            }

            StartGeneration();
        }

        return population[_currentIndex++];
    }

    void EndGeneration()
    {
        population.Sort();

        Summary += $"{CurrentGeneration},{population[0].fitness}\n";

        Crossover();
        Mutation();
    }

    void Crossover()
    {
        Individual p1 = population[0];
        Individual p2 = population[1];

        Individual c1 = null;
        Individual c2 = null;

        switch (crossoverType)
        {
            case CrossoverType.OnePoint:
                c1 = new Individual(p1.degree_x, p1.degree_y, p2.strength);
                c2 = new Individual(p2.degree_x, p2.degree_y, p1.strength);
                break;

            case CrossoverType.Average:
                float deg_x = (p1.degree_x + p2.degree_x) * 0.5f;
                float deg_y = (p1.degree_y + p2.degree_y) * 0.5f;
                float str = (p1.strength + p2.strength) * 0.5f;

                c1 = new Individual(deg_x, deg_y, str);
                c2 = new Individual(deg_x, deg_y, str);
                break;

            case CrossoverType.Uniform:
                float d1_x = Random.value < 0.5f ? p1.degree_x : p2.degree_x;
                float d1_y = Random.value < 0.5f ? p1.degree_y : p2.degree_y;
                float s1 = Random.value < 0.5f ? p1.strength : p2.strength;

                float d2_x = Random.value < 0.5f ? p1.degree_x : p2.degree_x;
                float d2_y = Random.value < 0.5f ? p1.degree_y : p2.degree_y;
                float s2 = Random.value < 0.5f ? p1.strength : p2.strength;

                c1 = new Individual(d1_x, d1_y, s1);
                c2 = new Individual(d2_x, d2_y, s2);
                break;
        }

        population.RemoveAt(population.Count - 1);
        population.RemoveAt(population.Count - 1);

        population.Add(c1);
        population.Add(c2);
    }

    void Mutation()
    {
        foreach (var ind in population)
        {
            switch (mutationType)
            {
                case MutationType.RandomReset:
                    if (Random.value < 0.02f)
                        ind.degree_x = Random.Range(0f, 90f);

                    if (Random.value < 0.02f)
                        ind.degree_y = Random.Range(-45f, 45f);

                    if (Random.value < 0.02f)
                        ind.strength = Random.Range(0f, 12f);
                    break;

                case MutationType.Gaussian:
                    if (Random.value < 0.2f)
                        ind.degree_x += Random.Range(-5f, 5f);

                    if (Random.value < 0.2f)
                        ind.degree_y += Random.Range(-5f, 5f);

                    if (Random.value < 0.2f)
                        ind.strength += Random.Range(-1f, 1f);
                    break;

                case MutationType.Adaptive:
                    float rate = Mathf.Lerp(
                        0.1f,
                        0.01f,
                        (float)CurrentGeneration / MaxGenerations
                    );

                    if (Random.value < rate)
                        ind.degree_x = Random.Range(0f, 90f);

                    if (Random.value < rate)
                        ind.degree_y = Random.Range(-45f, 45f);

                    if (Random.value < rate)
                        ind.strength = Random.Range(0f, 12f);
                    break;
            }

            ind.degree_x = Mathf.Clamp(ind.degree_x, 0f, 90f);
            ind.degree_y = Mathf.Clamp(ind.degree_y, -45f, 45f);
            ind.strength = Mathf.Clamp(ind.strength, 0f, 12f);
        }
    }
}

