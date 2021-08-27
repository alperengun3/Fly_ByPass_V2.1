using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject MainScene;
    [SerializeField] private GameObject level_1;
    [SerializeField] private GameObject level_2;
    [SerializeField] private GameObject level_3;

    private void Start()
    {
        MainScene.SetActive(true);
        level_1.SetActive(true);
    }
    void Level_1()
    {
        MainScene.SetActive(false);
        level_1.SetActive(true);
    }

    void Level_2()
    {
        level_1.SetActive(false);
        level_2.SetActive(true);
    }

    void Level_3()
    {
        level_2.SetActive(false);
        level_3.SetActive(true);
    }
    public void NextScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
