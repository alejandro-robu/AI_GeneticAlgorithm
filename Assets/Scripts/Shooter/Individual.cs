using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[Serializable]
public class Individual: IComparable<Individual>
{
    public float degree_x;
    public float degree_y;

    public float strength;

    public float fitness;

    public Individual(float dx, float dy, float s)
    {
        fitness = +1000f;
        degree_x = dx;
        degree_y = dy;
        strength = s;
    }

    public int CompareTo(Individual other)
    {
        return fitness.CompareTo(other.fitness);
    }
}
