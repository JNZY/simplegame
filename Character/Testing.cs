using Game.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    [SerializeField] private LevelWindow levelWindow;
    private void Awake()
    {
        handleDeadMonster();
    }

    public void handleDeadMonster()
    {
      
       LevelSystem ls = new LevelSystem();

       Debug.Log(ls.getLevel());
       ls.addExperience(50);
       Debug.Log(ls.getLevel());
       ls.addExperience(0);
       Debug.Log(ls.getLevel());


       levelWindow.setLevelSystem(ls);
      
    }
}
