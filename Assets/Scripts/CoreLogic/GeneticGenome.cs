using UnityEngine;

[System.Serializable]
public class GeneticGenome
{
    public float[] Weights;
    public float Fitness;

    public GeneticGenome(int size)
    {
        Weights = new float[size];

        for (int i = 0; i < size; i++)
            Weights[i] = Random.Range(-1f, 1f);
    }

    public GeneticGenome Clone()
    {
        GeneticGenome g = new GeneticGenome(Weights.Length);
        Weights.CopyTo(g.Weights, 0);
        g.Fitness = Fitness;
        return g;
    }
}