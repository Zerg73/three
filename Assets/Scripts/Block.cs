using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Block : MonoBehaviour
{
    public int num;
    public int no;
    public bool lig;
    public bool ok;

    private GameObject gm;
    private GameObject text;
    private GameObject pl;
    private GameObject okicon;
    void Awake()
    {
        lig=false;
        ok=false;
        SetValue();
        gm = GameObject.Find("GameManager");
        pl=transform.Find("Panel").gameObject;
        okicon=transform.Find("ok").gameObject;
        if (gm == null)
        {
            Debug.Log("GM丢失，出Bug了！");
        }
    }

    public void SetValue(){
        num = Random.Range(0, 8);
    }
    void OnEnable()
    {
        
    }
   

    void Update()
    {
       
        gameObject.GetComponent<Image>().color = ColorSet(num);
        pl.SetActive(lig);
        okicon.SetActive(ok&&gm.GetComponent<GameManager>().istip);
        text = transform.Find("Text").gameObject;
    }

    // public void Speak(){
    //     Debug.Log("我是"+no+"，我被点击了。");
    // }

    public void Click()
    {
        lig=!lig;
        gm.GetComponent<GameManager>().GetClick(no);
    }


    Color ColorSet(int n)
    {
        switch (n)
        {
            case 0:
                return Color.red;
            case 1:
                return new Vector4(1f, 0.5f, 0, 1);
            case 2:
                return Color.yellow;
            case 3:
                return Color.green;
            case 4:
                return Color.cyan;
            case 5:
                return Color.blue;
            case 6:
                return Color.magenta;
            case 7:
                return Color.white;
            default:
                return Color.clear;
        }
    }
}
