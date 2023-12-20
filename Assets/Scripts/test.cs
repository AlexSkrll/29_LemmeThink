using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject teza;
    private TezaController tezaController;
    private void Start()
    {
        tezaController = teza.GetComponent<TezaController>();
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(tezaController.attackPoint.position, tezaController.attackRange);

    }

}

