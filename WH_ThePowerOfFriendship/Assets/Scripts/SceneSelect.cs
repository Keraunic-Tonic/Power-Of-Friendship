using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;

public class SceneSelect : MonoBehaviour
{

    public void ChooseScene()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "SceneSelectButton")
        {
            SceneManager.LoadScene("SceneSelection");
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "AidaNeoButton")
        {
            SceneManager.LoadScene("AidaNeoScene");
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "BlazeEganButton")
        {
            SceneManager.LoadScene("BlazeEganScene");
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "VVButton")
        {
            SceneManager.LoadScene("AtTheDoor");
        }
        else if (EventSystem.current.currentSelectedGameObject.name == "EndGameButton")
        {
            SceneManager.LoadScene("EndingScene");
        }
        else
        {
            SceneManager.LoadScene("DahliaRayScene");
        }
    }
}
