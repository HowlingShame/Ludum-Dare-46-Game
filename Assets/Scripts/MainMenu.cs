using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Scene]
    public string       m_RunScene;

    //////////////////////////////////////////////////////////////////////////
    public void StartGame()
    {
        SceneManager.LoadScene(m_RunScene);
    }

}
