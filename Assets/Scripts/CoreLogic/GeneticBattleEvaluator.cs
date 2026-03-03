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

        yield return StartCoroutine(ResetBattle());

        startHP_AI =
            GeneticAI.GameState.ListOfPlayers.Players[
                GeneticAI.Body.Info.Id].HP;

        startHP_Enemy =
            GeneticAI.GameState.ListOfPlayers.Players[
                GeneticAI.Body.Info.EnemyId].HP;

        yield return StartCoroutine(RunBattle());

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

        fitness += damageDealt;
        fitness -= damageTaken;

        return fitness;
    }
    IEnumerator ResetBattle()
    {
        yield return null;

        GeneticAI.GameState.ResetState();

        GameLogic.ResetLogic();

        yield return null;
    }

    IEnumerator FinalizeTraining()
    {
        yield return null;

        GeneticAI.GameState.ResetState();

        GameLogic.ResetLogicAfterTraining();

        GameLogic.GameState.RightPlayerIsHuman = true;

        yield return null;
    }

    public void SwitchToPlayMode(GeneticGenome best)
    {
        Debug.Log("SWITCHING TO PLAY MODE");


        GeneticAI.ActiveGenome = best;
        GeneticAI.CurrentMode =
            GeneticController1on1.ActMode.Play;

        StopAllCoroutines();
        StartCoroutine(FinalizeTraining());
    }
}