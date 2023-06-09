using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.UI;
using TMPro;

public class MenuAnimator : MonoBehaviour
{
    public GameObject logo;
    public Image logoImage;
    public Image fadeImage;
    public TMP_Text playText;
    public TMP_Text optionsText;
    public TMP_Text quitText;
    public TMP_Text helpText;
    public TMP_Text creditsText;

    private void Start()
    {
        StartSequence();
    }

    void StartSequence()
    {
        var logoStart = DOTween.Sequence();
        logoStart.Append(fadeImage.transform.DOScale(new Vector3(40, 40, 40), 1.5f));
        logoStart.Append(logo.transform.DOMove(new Vector3(1000, 8000, 0), 0.5f).From());
        logoStart.Insert(1.75f, logo.transform.DOScale(new Vector3(2f, 0.2f, 1), 0.25f));
        logoStart.Append(logo.transform.DOScale(new Vector3(1, 1, 1), 0.25f));

        logoStart.Insert(1.5f, logoImage.DOFade(1, 0.3f));
     
        logoStart.Append(logo.transform.DOScale(1.1f, 2).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo));

        logoStart.Insert(4.5f, playText.DOFade(1, 0.5f));
        logoStart.Insert(4.5f, optionsText.DOFade(1, 0.5f));
        logoStart.Insert(4.5f, quitText.DOFade(1, 0.5f));
        logoStart.Insert(4.5f, helpText.DOFade(1, 0.5f));
        logoStart.Insert(4.5f, creditsText.DOFade(1, 0.5f));
        //logoStart.Insert(0, logo.transform.DOPunchScale(new Vector3(-0.8f, 0.8f, 1), 0.25f, 1, 1));
        //logoStart.Insert(0.25f, logo.transform.DOPunchScale(new Vector3(0, -0.5f, 1), 0.25f, 1, 1));
    }
}
