using System.Collections;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public PlayerList PlayerList;
    public GameState GameState;

    public GameEvent EndGameEvent;
    public AttackResultEvent AttackResult;
    public PlayerEvent ChangeTurnEvent;

    private int _count;

    public void ResetLogic()
    {
        StopAllCoroutines();

        _count = 0;
        GameState.TurnNumber = 1;
        GameState.IsFinished = false;

        StartCoroutine(StartBattle());
    }

    IEnumerator StartBattle()
    {
        yield return null;
        ChangeTurn();
    }

    public void ChangeTurn()
    {
        StartCoroutine(ChangeTurnNextFrame());
    }

    IEnumerator ChangeTurnNextFrame()
    {
        yield return null;

        var next = _count;
        _count = (_count + 1) % PlayerList.Players.Count;

        GameState.CurrentPlayer = PlayerList.Players[next];
        GameState.TurnNumber++;   // ⭐⭐⭐ CLAVE

        ChangeTurnEvent.Raise(GameState.CurrentPlayer);
    }

    bool EndGameTest()
    {
        if (PlayerList.Players.Any(p => p.HP <= 0))
        {
            GameState.IsFinished = true;
            EndGameEvent.Raise();
            return true;
        }
        return false;
    }

    public void OnAttackDone(Attack att)
    {
        var result = ScriptableObject.CreateInstance<AttackResult>();
        result.Attack = att;

        if (att.Source.Energy >= att.AttackMade.Energy &&
            Dice.PercentageChance() <= att.AttackMade.HitChance)
        {
            result.IsHit = true;
            result.Damage =
                Dice.RangeRoll(att.AttackMade.MinDam,
                               att.AttackMade.MaxDam + 1);

            att.Target.HP -= result.Damage;
        }

        att.Source.Energy -= att.AttackMade.Energy;

        AttackResult.Raise(result);

        if (!EndGameTest())
            ChangeTurn();
    }
}