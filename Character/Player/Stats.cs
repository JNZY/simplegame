using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stats : MonoBehaviour
{
    [SerializeField] int playerLevel;
    [SerializeField] int exp;
    [SerializeField] int expToNextLevel;

    public Text levelText;

    private void Start()
    {
        levelText.text = levelText.ToString();
    }

    public void levelUp()
    {
        playerLevel++;
    }

}
