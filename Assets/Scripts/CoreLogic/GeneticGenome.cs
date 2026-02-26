using UnityEngine;

[System.Serializable]
public class GeneticGenome
{
    public float[] Weights;
    public float Fitness;

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
    // DECIDE ATTACK (SOFTMAX SELECTION)
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

            if (self.Energy < atk.Energy)
            {
                scores[i] = 0f;
                continue;
            }

            float val = EvaluateSingleAttack(self, enemy, atk);

            Debug.Log($"ATTACK {atk} SCORE: {val}");

            // evita valores negativos dominantes
            scores[i] = Mathf.Exp(val);
            sum += scores[i];
        }

        // fallback seguro
        if (sum <= 0f)
            return Random.Range(0, self.Attacks.Length);

        float rnd = Random.value * sum;
        float acc = 0f;

        for (int i = 0; i < scores.Length; i++)
        {
            acc += scores[i];
            if (acc >= rnd)
                return i;
        }

        return 0;
    }

    // ======================================================
    // EVALUACIÓN GENÉTICA REAL
    // ======================================================
    float EvaluateSingleAttack(PlayerInfo self,
                               PlayerInfo enemy,
                               AttackInfo atk)
    {
        float expectedDamage =
            ((atk.MinDam + atk.MaxDam) * 0.5f)
            * atk.HitChance;

        float myHPpct = self.HP / self.InitialHP;
        float enemyHPpct = enemy.HP / enemy.InitialHP;

        float damageRelative =
            expectedDamage / enemy.InitialHP;

        float energyCost =
            atk.Energy / 10f;

        float restPenalty = 0f;

        if (atk.Energy < 0f) // es REST o similar
        {
            // si ya tienes energía suficiente, es mala decisión
            if (self.Energy > self.InitialEnergy * 0.5f)
                restPenalty = 1f;
        }

        float score =
              Weights[0] * damageRelative
            + Weights[1] * atk.HitChance
            + Weights[2] * (-energyCost)
            + Weights[3] * myHPpct
            + Weights[4] * (1f - enemyHPpct)
            - Weights[5] * restPenalty;

        //float score =
        //      damageRelative
        //    + atk.HitChance
        //    + (-energyCost)
        //    + myHPpct
        //    + (1f - enemyHPpct)
        //    - restPenalty;

        // BONUS DE FINALIZACIÓN (MUY IMPORTANTE)
        if (enemy.HP - expectedDamage <= 0)
            score += Weights[5];


        return score;
    }



}