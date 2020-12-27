using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//使用此腳本前請先在工作檔放入DOTween插件
//在2020時還活著的連結：https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

//使用方式：把此腳本丟到你想讓他用簡單動畫循環漂浮的物件上，然後依你的需求調整參數

public class Floater : MonoBehaviour
{
    //用來處理循環漂浮UI的程式
    [System.Serializable]
    public enum Type
    {
        上下飄浮,
        下上飄浮,
        左右飄浮,
        右左飄浮,
        上下壓扁,
        蹺蹺板,
    }
    [Header("行為模式")]
    [Tooltip("決定這物體的行為")]
    public Type behavior;

    public enum Type2
    {
        無,
        由慢到快,
        由快到慢,
        由慢跳到快跳,
        由快跳到慢跳,
    }
    [Header("曲線模式")]
    [Tooltip("決定這物體的緩入緩出")]
    public Type2 TheEase;

    [Header("浮動力道")]
    [Tooltip("決定浮動的力道")]
    [Range(0f, 500f)]
    public float power = 0f;

    [Header("浮動週期")]
    [Tooltip("決定浮動一次的時間")]
    [Range(0.1f, 10f)]
    public float time = 0.1f;

    float Nowz;//目前角度

    void Start()
    {
        Nowz = transform.eulerAngles.z;
        Anime();
    }
    void Anime()
    {
        Sequence Sequence = DOTween.Sequence();//動畫列表
        Tween[] tween = new Tween[3];
        if (behavior == Type.上下飄浮)
        {
            tween[1] = Sequence.Insert(0f, transform.DOMoveY(transform.position.y + power, time));
            tween[2] = Sequence.Insert(time, transform.DOMoveY(transform.position.y, time));
        }
        if (behavior == Type.下上飄浮)
        {
            tween[1] = Sequence.Insert(0f, transform.DOMoveY(transform.position.y - power, time));
            tween[2] = Sequence.Insert(time, transform.DOMoveY(transform.position.y, time));
        }
        if (behavior == Type.上下壓扁)
        {
            tween[1] = Sequence.Insert(0f, transform.DOScaleY(transform.localScale.y - (transform.localScale.y * (power / 500)), time));
            tween[2] = Sequence.Insert(time, transform.DOScaleY(transform.localScale.y, time));
        }
        if (behavior == Type.左右飄浮)
        {
            tween[1] = Sequence.Insert(0f, transform.DOMoveX(transform.position.x - power, time));
            tween[2] = Sequence.Insert(time, transform.DOMoveX(transform.position.x, time));
        }
        if (behavior == Type.右左飄浮)
        {
            tween[1] = Sequence.Insert(0f, transform.DOMoveX(transform.position.x + power, time));
            tween[2] = Sequence.Insert(time, transform.DOMoveX(transform.position.x, time));
        }
        if (behavior == Type.蹺蹺板)
        {
            tween[1] = Sequence.Insert(0, transform.DORotate(new Vector3(0, 0, Nowz + (power * 0.5f * 0.25f)), time));
            tween[2] = Sequence.Insert(time, transform.DORotate(new Vector3(0, 0, Nowz - (power * 1f * 0.25f)), time));
        }

        if (TheEase == Type2.由快到慢)
        {
            tween[1].SetEase(Ease.OutCirc);
            tween[2].SetEase(Ease.InCirc);
        }
        if (TheEase == Type2.由慢到快)
        {
            tween[1].SetEase(Ease.InCirc);
            tween[2].SetEase(Ease.OutCirc);
        }
        if (TheEase == Type2.由快跳到慢跳)
        {
            tween[1].SetEase(Ease.OutBounce);
            tween[2].SetEase(Ease.InBounce);
        }
        if (TheEase == Type2.由慢跳到快跳)
        {
            tween[1].SetEase(Ease.InBounce);
            tween[2].SetEase(Ease.OutBounce);
        }
        Invoke("Anime", time * 2);
    }
}
