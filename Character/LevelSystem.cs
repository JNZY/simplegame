using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelSystem 
{

    public event EventHandler OnExperienceChanged;
    public event EventHandler OnLevelChanged;


    private int level;
    private int experience;
    private int experienceToNextLevel;

    public LevelSystem()
    {
        level = 1;
        experience = 0;
        experienceToNextLevel = 100;
    }

    public void addExperience(int amount)
    {
        experience += amount;
        if(experience >= experienceToNextLevel)
        {
            level++;
            experience -= experienceToNextLevel;

            if (OnLevelChanged != null) OnLevelChanged(this, EventArgs.Empty);
        }


        if (OnExperienceChanged != null) OnExperienceChanged(this, EventArgs.Empty);
    }

    public int getLevel()
    {
        return level;
    }

    public float getExperienceNormalized()
    {
        return (float)experience / experienceToNextLevel;
    }

}
