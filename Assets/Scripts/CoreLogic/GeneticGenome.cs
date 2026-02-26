using UnityEngine;

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
    // DECIDE ATTACK
    // ======================================================
    public int DecideAttack(GameState state, int playerId)
    {
        var players = state.ListOfPlayers.Players;

        var self = players[playerId];
        var enemy = players[self.EnemyId];

        float[] scores = new float[self.Attacks.Length];
        float sum = 0f;

        for (int i = 0; i < self.Attacks.Length; i++)
        {
            var atk = self.Attacks[i];

            // si no tienes energía, score = 0
            if (self.Energy < atk.Energy)
            {
                scores[i] = 0f;
            }
            else
            {
                scores[i] = Mathf.Max(0f, EvaluateSingleAttack(self, enemy, atk));
            }

            sum += scores[i];
        }

        // fallback si ningún ataque es posible
        if (sum == 0f) return 1;

        // elegir probabilísticamente para no quedarse siempre con Rest
        float rnd = Random.value * sum;
        float acc = 0f;
        for (int i = 0; i < scores.Length; i++)
        {
            acc += scores[i];
            if (acc >= rnd) return i;
        }

        return 0; // fallback
    }

    // ======================================================
    // EVALUACIÓN DE UN ATAQUE INDIVIDUAL
    // ======================================================
    float EvaluateSingleAttack(PlayerInfo self,
                               PlayerInfo enemy,
                               AttackInfo atk)
    {
        float[] f = new float[FEATURE_COUNT];

        // -------- FEATURES --------
        f[0] = self.HP / self.InitialHP;               // vida propia %
        f[1] = 1f - (enemy.HP / enemy.InitialHP);     // daño al enemigo (invertido)
        float expectedDamage = (atk.MinDam + atk.MaxDam) / 2f;
        f[2] = expectedDamage / 20f;                  // daño normalizado
        f[3] = atk.HitChance;                          // precisión
        f[4] = 1f - (atk.Energy / 10f);               // coste energía invertido
        f[5] = expectedDamage / 20f;                  // repetir daño como feature adicional

        // -------- DOT PRODUCT --------
        float value = 0f;
        for (int i = 0; i < FEATURE_COUNT; i++)
            value += f[i] * Weights[i];

        return value;
    }



}