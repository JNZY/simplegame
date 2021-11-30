using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelWindow : MonoBehaviour
{

    private Text levelText;
    private Image experienceBarImage;
    private LevelSystem levelSystem;
    private void Awake()
    {
        levelText = transform.Find("LevelText").GetComponent<Text>();
        experienceBarImage = transform.Find("ExperienceBar").Find("bar").GetComponent<Image>();

    }

    private void SetExperienceBarSize(float experienceNormalized)
    {
        experienceBarImage.fillAmount = experienceNormalized;
    }

    private void SetLevelNumber(int levelNumber)
    {
        levelText.text = "" + (levelNumber + 1); 
    }

    public void setLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;

        //starting values
        SetLevelNumber(levelSystem.getLevel());
        SetExperienceBarSize(levelSystem.getExperienceNormalized());

        //subscribe to event
        levelSystem.OnExperienceChanged += LevelSystem_OnExperienceChanged;
        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;

    }

    private void LevelSystem_OnLevelChanged(object sender, System.EventArgs e)
    {
        SetLevelNumber(levelSystem.getLevel());
    }

    private void LevelSystem_OnExperienceChanged(object sender, System.EventArgs e)
    {
        SetExperienceBarSize(levelSystem.getExperienceNormalized());
    }



   
}
