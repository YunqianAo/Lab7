using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class AgenteML : Agent
{
    [SerializeField]
    private float _fuerzaMovimiento = 200;

    [SerializeField]
    private Transform _target;

    public bool _training = true;

    private Rigidbody _rb;

    public override void Initialize()
    {
        _rb = GetComponent<Rigidbody>();
        if (!_training) MaxStep = 0;
    }

    public override void OnEpisodeBegin()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;

        MoverPosicionInicial();
    }

    /// <summary>
    /// El vectorAction sirve para construir un vector de desplazamiento
    /// [0]: X.
    /// [1]: Z.
    /// </summary>
    /// <param name="vectorAction"></param>

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Accede a las acciones continuas
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        // Construye el vector de movimiento y aplica la fuerza
        Vector3 movimiento = new Vector3(moveX, 0f, moveZ);
        _rb.AddForce(movimiento * _fuerzaMovimiento);
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        //calcular cuanto queda hacia el objetivo
        Vector3 alObjetivo = _target.position - transform.position;
        //Un vector ocupa 3 observaciones
        sensor.AddObservation(alObjetivo.normalized);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Obtén las acciones continuas como referencia
        float[] continuousActions = actionsOut.ContinuousActions.Array;

        // Control manual con teclado
        continuousActions[0] = Input.GetAxis("Horizontal");
        continuousActions[1] = Input.GetAxis("Vertical");
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("target"))
        {
            if(_training)
            {
                AddReward(1f);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("target"))
        {
            if (_training)
            {
                AddReward(0.5f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("borders"))
        {
            if(_training)
            {
                AddReward(-0.1f);
            }
        }
    }

    private void MoverPosicionInicial()
    {
        bool posicionEncontrada = false;
        int intentos = 100;
        Vector3 posicionPotencial = Vector3.zero;

        while (!posicionEncontrada || intentos >= 0)
        {
            intentos--;
            posicionPotencial = new Vector3(transform.parent.position.x + UnityEngine.Random.Range(-4f, 4f), 0.555f, transform.parent.position.x + UnityEngine.Random.Range(-4f, 4f));

            Collider[] colliders = Physics.OverlapSphere(posicionPotencial, 0.05f);
            if (colliders.Length == 0)
            {
                transform.position = posicionPotencial;
                posicionEncontrada = true;
            }
        }
    }

}
