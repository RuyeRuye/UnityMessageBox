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
        Debug.Log("��ȷ��");
    }
    private void cancel()
    {
        Debug.Log("��ȡ��");
    }

    public void ShowConfirmCancelMessageBox()
    {
        MessageBox.Instance.ShowMessageBox("ȷ��ȡ������,�йرհ�ť", confirm, cancel, true);
    }
    public void ShowConfirmMessageBox()
    {
        MessageBox.Instance.ShowMessageBox("ȷ�ϵ���", confirm);
    }
    public void ShowMessageBox()
    {
        MessageBox.Instance.ShowMessageBox("1s����", 1f);
    }
}
