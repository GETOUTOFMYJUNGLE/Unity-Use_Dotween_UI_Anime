using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//使用此腳本前請先在工作檔放入DOTween插件
//在2020時還活著的連結：https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

//使用方式：把此腳本丟到你想讓他用簡單動畫出現或消失的物件上，然後依你的需求調整參數

public class InOut : MonoBehaviour
{
    //用來處理物件出現或消失的簡單動畫的程式
    [System.Serializable]

    public enum Type2
    {
        無,
        由慢到快,
        由快到慢,
        由慢跳到快跳,
        由快跳到慢跳,
        由慢震到快震,
        由快震到慢震,
        亂跳,
    }
    [Header("曲線模式")]
    [Tooltip("決定這物體的緩入緩出時間")]
    public Type2 TheEase;

    public enum Type
    {
        彈跳,
        X軸彈跳,
        Y軸彈跳,
        位移,
        旋轉,
    }
    [Header("動畫模式")]
    [Tooltip("決定這物體的緩入緩出動畫")]
    public Type TheAnime;

    [Header("位移起點（只有動畫模式為位移時才有效）")]
    [Tooltip("如果動畫模式是位移的話，那起點在哪裡")]
    public Vector2 begin_pos;

    [Header("旋轉起點（只有動畫模式為旋轉時才有效）")]
    [Range(0f, 360f)]
    [Tooltip("如果動畫模式是旋轉的話，那一開始是幾度")]
    public float begin_rot;

    [Header("動畫時間")]
    [Tooltip("決定動畫完成的時間")]
    [Range(0.1f, 7f)]
    public float time = 0.1f;

    [Header("收回後刪除嗎")]
    [Tooltip("決定收回後是刪除還是放置")]
    public bool Die = true;

    [Header("預設啟用嗎")]
    [Tooltip("決定是否一開始就要播放動畫")]
    public bool Startin = true;

    //XXX為掛載此腳本的物件名稱
    //由別的腳本調用此物件收回的方式：GameObject.Find("XXX").GetComponent<InOut>().Back();

    Vector3 Now_sca;//目前大小
    Vector3 Now_pos;//目前位置
    float Now_rot;//目前角度
    bool back = false;//是要收回嗎
    void Start()
    {
        Now_sca = transform.localScale;
        Now_pos = transform.position;
        Now_rot = transform.eulerAngles.z;
        if (Startin)
            Anime();
    }
    void Anime()
    {
        Sequence Sequence = DOTween.Sequence();//動畫列表
        Tween[] tween = new Tween[3];
        if (!back)//淡入
        {
            if (TheAnime == Type.彈跳)
            {
                transform.localScale = new Vector3();
                tween[2] = Sequence.Insert(0.01f, transform.DOScale(Now_sca, time));
            }
            if (TheAnime == Type.X軸彈跳)
            {
                transform.localScale = new Vector3(0, transform.localScale.y, transform.localScale.z);
                tween[2] = Sequence.Insert(0.01f, transform.DOScale(Now_sca, time));
            }
            if (TheAnime == Type.Y軸彈跳)
            {
                transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
                tween[2] = Sequence.Insert(0.01f, transform.DOScale(Now_sca, time));
            }
            if (TheAnime == Type.位移)
            {
                if (GetComponent<RectTransform>() != null)
                {
                    GetComponent<RectTransform>().anchoredPosition = begin_pos;
                    tween[2] = Sequence.Insert(0.01f, GetComponent<RectTransform>().DOMove(Now_pos, time));
                }
                else
                {
                    transform.position = begin_pos;
                    tween[2] = Sequence.Insert(0.01f, transform.DOMove(Now_pos, time));
                }
            }
            if (TheAnime == Type.旋轉)
            {
                transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, begin_rot), 0);
                tween[2] = Sequence.Insert(0.01f, transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Now_rot), time));
            }
        }
        else//淡出
        {
            if (TheAnime == Type.彈跳)
            {
                tween[1] = Sequence.Insert(0f, transform.DOScale(new Vector3(0, 0, 0), time));
            }
            if (TheAnime == Type.X軸彈跳)
            {
                tween[1] = Sequence.Insert(0f, transform.DOScale(new Vector3(0, transform.localScale.y, transform.localScale.z), time));
            }
            if (TheAnime == Type.Y軸彈跳)
            {
                tween[1] = Sequence.Insert(0f, transform.DOScale(new Vector3(transform.localScale.y, 0, transform.localScale.z), time));
            }
            if (TheAnime == Type.位移)
            {
                if (GetComponent<RectTransform>() != null)
                {
                    tween[1] = Sequence.Insert(0f, GetComponent<RectTransform>().DOMove(begin_pos, time));
                }
                else
                {
                    tween[1] = Sequence.Insert(0f, transform.DOMove(begin_pos, time));
                }
            }
            if (TheAnime == Type.旋轉)
            {
                tween[1] = Sequence.Insert(0f, transform.DORotate(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, begin_rot), time));
            }
            Invoke("BackEnd", time);
        }

        if (TheEase == Type2.由慢到快)
        {
            tween[1].SetEase(Ease.OutCirc);
            tween[2].SetEase(Ease.InCirc);
        }
        if (TheEase == Type2.由快到慢)
        {
            tween[1].SetEase(Ease.InCirc);
            tween[2].SetEase(Ease.OutCirc);
        }
        if (TheEase == Type2.由慢跳到快跳)
        {
            tween[1].SetEase(Ease.OutBounce);
            tween[2].SetEase(Ease.InBounce);
        }
        if (TheEase == Type2.由快跳到慢跳)
        {
            tween[1].SetEase(Ease.InBounce);
            tween[2].SetEase(Ease.OutBounce);
        }
        if (TheEase == Type2.由慢震到快震)
        {
            tween[1].SetEase(Ease.OutElastic);
            tween[2].SetEase(Ease.InElastic);
        }
        if (TheEase == Type2.由快震到慢震)
        {
            tween[1].SetEase(Ease.InElastic);
            tween[2].SetEase(Ease.OutElastic);
        }
        if (TheEase == Type2.亂跳)
        {
            tween[1].SetEase(Ease.InOutBounce);
            tween[2].SetEase(Ease.InOutBounce);
        }
    }
    public void Back()//收回
    {
        back = true;
        Anime();
    }
    void BackEnd()//收回結束
    {
        if (Die)
        {
            Destroy(gameObject);
        }
        else
        {
            back = false;
        }
    }
}
