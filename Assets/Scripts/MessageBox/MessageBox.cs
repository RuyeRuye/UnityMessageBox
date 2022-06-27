using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 消息提示框
/// </summary>
public class MessageBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MessageBox Instance;

    [Tooltip("消息提示框")]
    [SerializeField] private RectTransform box;

    [Tooltip("文字内容")]
    [SerializeField] private Text content;

    [Tooltip("确认按钮")]
    [SerializeField] private Button confirmButton;

    [Tooltip("取消按钮")]
    [SerializeField] private Button cancelButton;

    [Tooltip("关闭按钮")]
    [SerializeField] private Button closeButton;

    [Tooltip("弹出用时")]
    [SerializeField] private float moveTime=0.3f;

    [Header("速度变化曲线")][Tooltip("从(0,0)至(1,1)的曲线，斜率表示瞬时速度，如果Box包含CanvasGroup,则同时改变透明度")]
    [SerializeField] private AnimationCurve speedCurve=AnimationCurve.EaseInOut(0,0,1,1);
    private float startY;
    private CanvasGroup canvasGroup;
    bool isPointerInside = false;
    public bool isShow;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    // Start is called before the first frame update
    private void Start()
    {
        closeButton.gameObject.SetActive(false);
        box.gameObject.SetActive(false);
        startY = -box.position.y-box.rect.height*0.5f;
        canvasGroup = box.GetComponent<CanvasGroup>();
        //Debug.Log(box.localPosition.y);
        //Debug.Log(box.position.y);

    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
    }

    // Update is called once per frame
    private void Update()
    {
        //Debug.Log(isPointerInside);
    }
    #region 外部调用方法
    /// <summary>
    /// 弹出确认取消提示框
    /// </summary>
    /// <param name="text"></param>
    /// <param name="confirmAction"></param>
    /// <param name="cancelAction"></param>
    /// <param name="isUseClose"></param>
    public void ShowMessageBox(string text, UnityAction confirmAction,UnityAction cancelAction,bool isUseClose)
    {
        ShowMessageBoxConfirmCancel(moveTime, confirmAction, cancelAction, isUseClose, text);
        isShow = true;
    }
    /// <summary>
    /// 弹出确认提示框
    /// </summary>
    /// <param name="text"></param>
    /// <param name="confirmAction"></param>
    public void ShowMessageBox(string text, UnityAction confirmAction)
    {
        ShowMessageBoxConfirm(moveTime, confirmAction, text);
        isShow = true;
    }
    /// <summary>
    /// 弹出提示框
    /// </summary>
    /// <param name="text"></param>
    /// <param name="waitTime"></param>
    public void ShowMessageBox(string text, float waitTime)
    {
        StopAllCoroutines();
        StartCoroutine(ShowMessageBoxTimeIE(moveTime, waitTime, text));
        isShow = true;
    }
    /// <summary>
    /// 隐藏弹出框
    /// </summary>
    public void HideMessageBox()
    {
        StopAllCoroutines();
        StartCoroutine(HideMessageBoxIE(moveTime));
    }
    #endregion
    private IEnumerator HideMessageBoxIE(float time)
    {
        float count = 0;
        float lastY = box.localPosition.y;
        float lastAlpha = 1;
        if (canvasGroup != null) lastAlpha = canvasGroup.alpha;
        while (count < time)
        {
            count += Time.unscaledDeltaTime;
            //box.localPosition = new Vector2(box.localPosition.x, Mathf.Lerp(lastY, startY, count / time));
            box.localPosition = new Vector2(box.localPosition.x, Mathf.Lerp(lastY, startY, speedCurve.Evaluate(count / time)));
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(lastAlpha, 0, speedCurve.Evaluate(count / time));
            yield return 0;
        }
        box.localPosition = new Vector2(box.localPosition.x, startY);
        box.gameObject.SetActive(false);
        isShow = false;
    }
    private IEnumerator ShowMessageBoxIE(float time)
    {
        transform.SetAsLastSibling();
        box.gameObject.SetActive(true);
        if (canvasGroup != null)    canvasGroup.alpha = 0;
        float count = 0;
        while (count < time)
        {
            count += Time.unscaledDeltaTime;
            //box.localPosition = new Vector2(box.localPosition.x, Mathf.Lerp(startY, 0, count / time));
            box.localPosition = new Vector2(box.localPosition.x, Mathf.Lerp(startY, 0, speedCurve.Evaluate(count / time)));
            if (canvasGroup != null) canvasGroup.alpha = Mathf.Lerp(0, 1, speedCurve.Evaluate(count / time));
            yield return 0;
        }
        box.localPosition = new Vector2(box.localPosition.x, 0);
        if (canvasGroup != null) canvasGroup.alpha = 1;
    }
    private IEnumerator ShowMessageBoxTimeIE(float time,float waitTime,string text)
    {
        closeButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
        content.text = text;
        yield return StartCoroutine(ShowMessageBoxIE(time));
        float count = 0;
        while (count < waitTime)
        {
            count += Time.unscaledDeltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                if (!isPointerInside)
                {
                    HideMessageBox();
                    yield break;
                }
                else
                {
                    waitTime += 1f;
                }
            }
            yield return 0;
        }
        //yield return  new WaitForSecondsRealtime(waitTime);
        HideMessageBox();
    }

    private void ShowMessageBoxConfirm(float time,UnityAction confirmAction,string text)
    {
        closeButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        EnableButtons();
        confirmButton.onClick.RemoveAllListeners();
        if (confirmAction != null)
        {
            confirmButton.onClick.AddListener(confirmAction);
        }
        confirmButton.onClick.AddListener(DisableButtons);
        confirmButton.onClick.AddListener(HideMessageBox);
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(DisableButtons);
        cancelButton.onClick.AddListener(HideMessageBox);
        content.text = text;
        StopAllCoroutines();
        StartCoroutine(ShowMessageBoxIE(time));
    }
    private void ShowMessageBoxConfirmCancel(float time, UnityAction confirmAction,UnityAction cancelAction,bool isUseClose, string text)
    {
        closeButton.gameObject.SetActive(isUseClose);
        cancelButton.gameObject.SetActive(true);
        confirmButton.gameObject.SetActive(true);
        EnableButtons();
        confirmButton.onClick.RemoveAllListeners();
        if (confirmAction != null)
        {
            confirmButton.onClick.AddListener(confirmAction);
        }
        confirmButton.onClick.AddListener(DisableButtons);
        confirmButton.onClick.AddListener(HideMessageBox);
        cancelButton.onClick.RemoveAllListeners();
        if (cancelAction != null)
        {
            cancelButton.onClick.AddListener(cancelAction);
        }
        cancelButton.onClick.AddListener(DisableButtons);
        cancelButton.onClick.AddListener(HideMessageBox);
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(DisableButtons);
        closeButton.onClick.AddListener(HideMessageBox);
        content.text = text;

        StopAllCoroutines();
        StartCoroutine(ShowMessageBoxIE(time));
    }
    private void DisableButtons()
    {
        confirmButton.interactable = false;
        cancelButton.interactable = false;
        confirmButton.interactable = false;
    }
    private void EnableButtons()
    {
        confirmButton.interactable = true;
        cancelButton.interactable = true;
        confirmButton.interactable = true;
    }
}
