using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DegenCounter : MonoBehaviour
{
    public int degenCount;

    public void catchedDegen()
    {
        degenCount++;
    }
}
