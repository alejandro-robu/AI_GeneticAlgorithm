using System.Collections;
using UnityEngine;

public class GeneticController1on1 : AIController
{
    public enum ActMode
    {
        Learning,
        Play
    }

    public ActMode CurrentMode = ActMode.Learning;

    public GeneticGenome ActiveGenome;

    public override IEnumerator Think()
    {
        //switch (CurrentMode)
        //{
        //    case ActMode.Learning:
        //        if (ActiveGenome == null)
        //        {
        //            RandomAction();
        //            return;
        //        }

        //        var attacks = _player.Attacks;

        //        int index =
        //            ActiveGenome.DecideAttack(GameState, _player.Id);

        //        var chosen = attacks[index];

        //        _attackToDo = ScriptableObject.CreateInstance<Attack>();
        //        _attackToDo.AttackMade = chosen;
        //        _attackToDo.Source = _player;
        //        _attackToDo.Target =
        //            GameState.ListOfPlayers.Players[_player.EnemyId];

        //        Debug.Log($"GENETIC ACTION -> {chosen.name}");
        //        break;
        //    case ActMode.Play:

        //        _attackToDo = ScriptableObject.CreateInstance<Attack>();
        //        _attackToDo.AttackMade = _player.Attacks[0];
        //        _attackToDo.Source = _player;
        //        _attackToDo.Target = GameState.ListOfPlayers.Players[_player.EnemyId];

        //        break;
        //    default:
        //        break;
        //}

        if (ActiveGenome == null)
        {
            RandomAction();
            yield return null;
        }

        var attacks = _player.Attacks;

        int index =
            ActiveGenome.DecideAttack(GameState, _player.Id);

        var chosen = attacks[index];

        _attackToDo = ScriptableObject.CreateInstance<Attack>();
        _attackToDo.AttackMade = chosen;
        _attackToDo.Source = _player;
        _attackToDo.Target =
            GameState.ListOfPlayers.Players[_player.EnemyId];

        Debug.Log($"GENETIC ACTION -> {chosen.name}");
    }

    void RandomAction()
    {
        var att = _player.Attacks[
            Random.Range(0, _player.Attacks.Length)];

        _attackToDo = ScriptableObject.CreateInstance<Attack>();
        _attackToDo.AttackMade = att;
        _attackToDo.Source = _player;
        _attackToDo.Target =
            GameState.ListOfPlayers.Players[_player.EnemyId];
    }
}