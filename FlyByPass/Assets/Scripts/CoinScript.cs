using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CoinScript : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private GameObject collectButton;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private GameObject nextLevelText;
    [SerializeField] private Text textCoinScore;
    [SerializeField] private GameObject textDot_1;
    [SerializeField] private GameObject textDot_2;
    [SerializeField] private PlayerControl playerControl;
    [SerializeField] private RectTransform coinRect;


    public GameObject CollectButton { get => collectButton; set => collectButton = value; }

    void Start()
    {
        CollectButton.SetActive(false);
        nextLevelButton.SetActive(false);
        nextLevelText.SetActive(false);
        textDot_1.SetActive(false);
        textDot_2.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
    }

    public void CreateCoin()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject obj = Instantiate(coinPrefab, transform);
            obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-200, 200), Random.Range(-200, 200));
            obj.transform.DOMove(coinRect.transform.position, 0.5f).SetDelay(Random.Range(0.5f, 0.75f));
            obj.GetComponent<RectTransform>().DOScale(new Vector2(0.5f, 0.5f), 0.5f).SetDelay(Random.Range(0.5f, 0.75f));
            Destroy(obj, 2);
        }
        textCoinScore.text = ((playerControl.StackList.Count * 10) + 50).ToString();
        CollectButton.SetActive(false);
        nextLevelButton.SetActive(true);
    }

    public void NextLevelButton()
    {
        Invoke("NextLevelInvoke", 3);
        nextLevelButton.SetActive(false);
        nextLevelText.SetActive(true);
        StartCoroutine(LoadingDotZero());
        
    }

    public void NextLevelInvoke()
    {
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator LoadingDotZero()
    {
        yield return new WaitForSeconds(0.3f);
        textDot_2.SetActive(false);
        textDot_1.SetActive(false);
        StartCoroutine(LoadingDot());
    }

    IEnumerator LoadingDot()
    {
        yield return new WaitForSeconds(0.3f);
        textDot_2.SetActive(false);
        textDot_1.SetActive(true);
        StartCoroutine(LoadingDot2());
    }

    IEnumerator LoadingDot2()
    {
        yield return new WaitForSeconds(0.3f);
        textDot_1.SetActive(false);
        textDot_2.SetActive(true);
        StartCoroutine(LoadingDotZero());
    }

    public void RestartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
