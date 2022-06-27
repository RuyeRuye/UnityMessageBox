using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxTestUse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void confirm()
    {
        Debug.Log("已确定");
    }
    private void cancel()
    {
        Debug.Log("已取消");
    }

    public void ShowConfirmCancelMessageBox()
    {
        MessageBox.Instance.ShowMessageBox("确认取消弹窗,有关闭按钮", confirm, cancel, true);
    }
    public void ShowConfirmMessageBox()
    {
        MessageBox.Instance.ShowMessageBox("确认弹窗", confirm);
    }
    public void ShowMessageBox()
    {
        MessageBox.Instance.ShowMessageBox("1s弹窗", 1f);
    }
}
