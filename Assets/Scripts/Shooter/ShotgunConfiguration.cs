using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ShotgunConfiguration : MonoBehaviour
{
    public float XDegrees;
    public float YDegrees;

    public float Strength;

    public Rigidbody ShotSpherePrefab;
    public Transform ShotPosition;

    public Transform Target;

    public GeneticAlgorithm Genetic;
    public Individual CurrentIndividual;

    [Header("GA Configuration")]
    public int Generations = 20;
    public int PopulationSize = 30;
    public CrossoverType crossoverType;
    public MutationType mutationType;

    private bool _ready;
    private bool _finishedTraining = false;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 50f;

        RandomizeTargetPosition();

        Genetic = new GeneticAlgorithm(
            Generations,
            PopulationSize,
            crossoverType,
            mutationType
        );

        _ready = true;
    }

    void RandomizeTargetPosition()
    {
        Vector3 pos = Target.position;
        pos.x = Random.Range(-5f, 5f);
        pos.y = Random.Range(1f, 4f);
        Target.position = pos;
    }

    public void ShooterConfigure(float xDegrees, float yDegrees, float strength)
    {
        XDegrees = xDegrees;
        YDegrees = yDegrees;
        Strength = strength;
    }

    public void GetResult(float data)
    {
        Debug.Log($"Result {data}");
        CurrentIndividual.fitness = data;
        _ready = true;
    }

    public void FinalResult(float data)
    {
        Debug.Log($"Result {data}");
        CurrentIndividual.fitness = 0;
        _ready = true;
    }

    public void Shot()
    {
        _ready = false;

        transform.eulerAngles = new Vector3(XDegrees, YDegrees,0);
        var shot = Instantiate(ShotSpherePrefab, ShotPosition);
        shot.gameObject.GetComponent<TargetTrigger>().Target = Target;

        shot.gameObject.GetComponent<TargetTrigger>().OnHitCollider += GetResult;

        shot.isKinematic = false;
        var force = transform.up * Strength;
        shot.AddForce(force,ForceMode.Impulse);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _finishedTraining)
        {
            Time.timeScale = 1f;
            //CurrentIndividual = Genetic.GetFittest();
            ShooterConfigure(CurrentIndividual.degree_x, CurrentIndividual.degree_y, CurrentIndividual.strength);
            Shot();
            _ready = false;
        }

        if (_ready && !_finishedTraining)
        {
            CurrentIndividual = Genetic.GetNext();
            if (CurrentIndividual != null)
            {
                ShooterConfigure(CurrentIndividual.degree_x, CurrentIndividual.degree_y, CurrentIndividual.strength);
                Shot();
            }
            else
            {
                CurrentIndividual = Genetic.GetFittest();
                Debug.Log("Entrenamiento Terminado!");
                _ready = false;
                _finishedTraining = true;
            }
        }
    }
}
