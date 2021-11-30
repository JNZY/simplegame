using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{

    [SerializeField] int Agi;
    [SerializeField] int Strength;
    [SerializeField] int Stamina;


    [SerializeField] int level;
    [SerializeField] int experience;
    [SerializeField] int experienceToNextLevel;

    public int getExperience()
    {
        return experience;
    }

    public void setExpDrop(int exp)
    {
        experience = exp;
    }


    public CharacterType characterType;
   
    public enum CharacterType {
        FireWizard,
        Warrior
    }

    public void addExperience(int exp)
    {
        experience += exp;
        Debug.Log("Add " + experience + " exp to the player");
    }
}
        