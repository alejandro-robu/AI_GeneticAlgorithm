//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;


//public class GeneticController1on1 : AIController
//{
//    public void Start()
//    {
//        Debug.Log("Start Genetic");
//    }


//    protected override void Think()
//    {
//        _attackToDo = ScriptableObject.CreateInstance<Attack>();
//        _attackToDo.AttackMade = _player.Attacks[0];
//        _attackToDo.Source = _player;
//        _attackToDo.Target = GameState.ListOfPlayers.Players[_player.EnemyId];

//    }
//}


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

    //protected override void Think()
    //{
    //    if (ActiveGenome == null)
    //    {
    //        RandomAction();
    //        return;
    //    }

    //    ChooseBestAttack();
    //}

    public override void Think()
    {
        _attackToDo = null;

        Debug.Log($"Thinking Mode: {Mode}");

        if (Mode == AIMode.Genetic)
        {
            ExpectGenetic();
        }
        else
        {
            ExpectMiniMax();
        }
    }

    void ChooseBestAttack()
    {
        float bestScore = float.MinValue;
        AttackInfo bestAttack = null;

        foreach (var attack in _player.Attacks)
        {
            if (_player.Energy < attack.Energy)
                continue;

            float score = EvaluateAttack(attack);

            if (score > bestScore)
            {
                bestScore = score;
                bestAttack = attack;
            }
        }

        _attackToDo = ScriptableObject.CreateInstance<Attack>();
        _attackToDo.AttackMade = bestAttack;
        _attackToDo.Source = _player;
        _attackToDo.Target =
            GameState.ListOfPlayers.Players[_player.EnemyId];
    }

    float EvaluateAttack(AttackInfo attack)
    {
        float expectedDamage =
            ((attack.MinDam + attack.MaxDam) * 0.5f)
            * attack.HitChance;

        float myHP =
            GameState.ListOfPlayers.Players[_player.Id].HP;

        float enemyHP =
            GameState.ListOfPlayers.Players[_player.EnemyId].HP;

        var w = ActiveGenome.Weights;

        return
            w[0] * expectedDamage +
            w[1] * attack.HitChance +
            w[2] * (-attack.Energy) +
            w[3] * myHP +
            w[4] * (-enemyHP);
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