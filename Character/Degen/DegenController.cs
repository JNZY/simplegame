using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DegenController : MonoBehaviour
{

    public bool isCatch;

    public void CatchDegen()
    {
        isCatch = true;
        Debug.Log("Degen is now catched....");
        Destroy(gameObject);
    }
}
