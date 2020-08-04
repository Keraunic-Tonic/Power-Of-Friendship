using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSelect : MonoBehaviour
{

    public void ChooseScene()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
