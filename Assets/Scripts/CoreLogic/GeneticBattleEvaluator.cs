using System;
using System.Collections;
using UnityEngine;

public class GeneticBattleEvaluator : MonoBehaviour
{
    public GeneticController1on1 GeneticAI;
    public GameLogic GameLogic;
    public AIController OpponentAI;

    float startHP_AI;
    float startHP_Enemy;

    public IEnumerator EvaluateGenome(
    GeneticGenome genome,
    Action<float> onFinished)
    {
        GeneticAI.ActiveGenome = genome;

        // ---- RESET ----
        yield return StartCoroutine(ResetBattle());

        startHP_AI =
            GeneticAI.GameState.ListOfPlayers.Players[
                GeneticAI.Body.Info.Id].HP;

        startHP_Enemy =
            GeneticAI.GameState.ListOfPlayers.Players[
                GeneticAI.Body.Info.EnemyId].HP;

        // ---- RUN ----
        yield return StartCoroutine(RunBattle());

        // ---- FITNESS ----
        float fitness = CalculateFitness();

        onFinished?.Invoke(fitness);

        yield return null;
    }

    IEnumerator RunBattle()
    {
        while (!BattleFinished())
            yield return null;
    }

    bool BattleFinished()
    {
        var players =
            GeneticAI.GameState.ListOfPlayers.Players;

        return players[0].HP <= 0 ||
               players[1].HP <= 0;
    }

    float CalculateFitness()
    {
        var players = GeneticAI.GameState.ListOfPlayers.Players;

        float aiHP = players[0].HP;
        float enemyHP = players[1].HP;

        float damageDealt = startHP_Enemy - enemyHP;
        float damageTaken = startHP_AI - aiHP;

        float fitness = 0f;

        // daño al enemigo (principal objetivo)
        fitness += damageDealt * 2.0f;

        // supervivencia
        fitness += aiHP * 1.0f;

        // penalización por daño recibido
        fitness -= damageTaken * 1.5f;

        // bonus por victoria
        //if (enemyHP <= 0)
        //    fitness += 500f;

        // penalización por derrota
        if (aiHP <= 0)
            fitness -= 500f;

        return fitness;
    }

    //IEnumerator ResetBattle()
    //{
    //    GeneticAI.StopAllCoroutines();
    //    OpponentAI.StopAllCoroutines();

    //    yield return null;

    //    GeneticAI.GameState.ResetState();

    //    yield return null;


    //    var firstPlayer =
    //        GeneticAI.GameState.ListOfPlayers.Players[0];
    //    Debug.Log($"Starting new battle, first player is: {firstPlayer}");

    //    GeneticAI.OnGameTurnChange(firstPlayer);
    //    OpponentAI.OnGameTurnChange(firstPlayer);
    //}

    IEnumerator ResetBattle()
    {
        yield return null;

        GeneticAI.GameState.ResetState();

        GameLogic.ResetLogic();

        yield return null;
    }

    public void SwitchToPlayMode(GeneticGenome best)
    {
        Debug.Log("SWITCHING TO PLAY MODE");

        GeneticAI.ActiveGenome = best;
        GeneticAI.CurrentMode =
            GeneticController1on1.ActMode.Play;
    }
}