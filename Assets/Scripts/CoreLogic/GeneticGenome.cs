using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public class GeneticGenome
{
    public float[] Weights;
    public float Fitness;

    // Nº de features usadas por ataque
    public const int FEATURE_COUNT = 6;

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

    // ======================================================
    // CEREBRO GENÉTICO
    // ======================================================
    public int DecideAttack(GameState state, int playerId)
    {
        var players = state.ListOfPlayers.Players;

        var self = players[playerId];
        var enemy = players[self.EnemyId];

        float bestScore = float.MinValue;
        int bestIndex = 0;

        for (int i = 0; i < self.Attacks.Length; i++)
        {
            var atk = self.Attacks[i];

            if (self.Energy < atk.Energy)
                continue;

            float score = EvaluateAttack(self, enemy, atk);

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    // ======================================================
    // Evaluación lineal
    // ======================================================
    float EvaluateAttack(PlayerInfo self,
                         PlayerInfo enemy,
                         AttackInfo atk)
    {
        float[] f = new float[FEATURE_COUNT];

        // -------- FEATURES --------
        f[0] = self.HP / self.InitialHP;        // vida propia %
        f[1] = enemy.HP / enemy.InitialHP;      // vida enemigo %
        f[2] = atk.MinDam / 10f;                // daño mínimo
        f[3] = atk.MaxDam / 10f;                // daño máximo
        f[4] = atk.HitChance;                   // precisión
        f[5] = atk.Energy / 10f;                // coste energía

        // -------- DOT PRODUCT --------
        float value = 0f;

        for (int i = 0; i < FEATURE_COUNT; i++)
            value += f[i] * Weights[i];

        return value;
    }



    
}