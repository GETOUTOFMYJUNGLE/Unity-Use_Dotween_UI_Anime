using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//使用此腳本前請先在工作檔放入DOTween插件
//在2020時還活著的連結：https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676

//使用方式：把此腳本丟到你想讓他用簡單動畫循環漂浮的物件上，然後依你的需求調整參數

public class Floater : MonoBehaviour
{
    //用來處理循環UI動畫的程式
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
    [Tooltip("決定這物體的緩入緩出")]
    public Type2 TheEase;

    public enum Type
    {
        變色,
        位移,
        縮放,
        旋轉,
    }
    [Header("動畫模式")]
    [Tooltip("決定這物體的動畫行為")]
    public Type TheAnime;

    public enum Type3
    {
        絕對,
        相對,
    }
    [Header("終點定義")]
    [Tooltip("決定端點設置的是絕對的還是相對的")]
    public Type3 TheBegin;

    [Header("終點顏色")]
    [Tooltip("如果動畫模式是變色的話，那最終是甚麼顏色")]
    public Color end_col;

    [Header("終點位置")]
    [Tooltip("如果動畫模式是位移的話，那最終要在哪裡")]
    public Vector3 end_pos;

    [Header("終點角度")]
    [Tooltip("如果動畫模式是旋轉的話，那最終是幾度")]
    public Vector3 end_rot;

    [Header("終點大小")]
    [Tooltip("如果動畫模式是縮放的話，那最終是多大")]
    public Vector3 end_sca;

    [Header("動畫時間")]
    [Tooltip("決定浮動一次的時間")]
    [Range(0.1f, 30f)]
    public float time = 0.1f;

    [Header("隨機改變動畫時間")]
    [Tooltip("決定隨機改變動畫時間可能的最大值與最小值會差幾秒，容易與其他InOut或Floater衝突，請慎用")]
    [Range(0f, 30f)]
    public float The_rand_time = 0f;

    Vector3 Now_sca;//目前大小
    Vector3 Now_pos;//目前位置
    Vector3 Now_rot;//目前角度
    Color Now_col;//目前色彩
    void Start()
    {
        Now_sca = transform.localScale;
        Now_pos = transform.position;
        Now_rot = transform.eulerAngles;
        if (GetComponent<Image>() != null)
        {
            Now_col = GetComponent<Image>().color;
        }
        else if (GetComponent<Text>() != null)
        {
            Now_col = GetComponent<Text>().color;
        }
        else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
        {
            if (transform.GetChild(0).GetComponent<Image>() != null)
                Now_col = transform.GetChild(0).GetComponent<Image>().color;
            else if (transform.GetChild(0).GetComponent<Text>() != null)
                Now_col = transform.GetChild(0).GetComponent<Text>().color;
        }


        //換算區
        if (TheBegin == Type3.絕對 && GetComponent<RectTransform>() != null)//自動換算
        {
            end_pos = new Vector3(end_pos.x + Screen.width / 2, end_pos.y + Screen.height / 2, end_pos.z);
        }
        
        //如果你有InOut就要避免衝突
        if (GetComponent<InOut>() != null)
        {
            Invoke("Anime", GetComponent<InOut>().time + GetComponent<InOut>().delay_time);
        }
        else//沒有就直接開始
            Anime();
    }
    void Anime()
    {
        float rand_time = Random.Range(The_rand_time * -1, The_rand_time);
        if (rand_time + time < 0.1f)
            rand_time = (time - 0.1f) * -1;

        Sequence Sequence = DOTween.Sequence();//動畫列表
        Tween[] tween = new Tween[3];
        if (TheAnime == Type.變色)
        {
            if (TheBegin == Type3.相對)
            {
                if (GetComponent<Image>() != null)
                {
                    tween[1] = Sequence.Insert(0f, GetComponent<Image>().DOColor(Now_col + end_col, time + rand_time));
                    tween[2] = Sequence.Insert(time + rand_time, GetComponent<Image>().DOColor(Now_col, time + rand_time));
                }
                else if (GetComponent<Text>() != null)
                {
                    tween[1] = Sequence.Insert(0f, GetComponent<Text>().DOColor(Now_col + end_col, time + rand_time));
                    tween[2] = Sequence.Insert(time + rand_time, GetComponent<Text>().DOColor(Now_col, time + rand_time));
                }
                else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                {
                    if (transform.GetChild(0).GetComponent<Image>() != null)
                    {
                        tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Image>().DOColor(Now_col + end_col, time + rand_time));
                        tween[2] = Sequence.Insert(time + rand_time, transform.GetChild(0).GetComponent<Image>().DOColor(Now_col, time + rand_time));
                    }
                    else if (transform.GetChild(0).GetComponent<Text>() != null)
                    {
                        tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Text>().DOColor(Now_col + end_col, time + rand_time));
                        tween[2] = Sequence.Insert(time + rand_time, transform.GetChild(0).GetComponent<Text>().DOColor(Now_col, time + rand_time));
                    }
                }
            }
            if (TheBegin == Type3.絕對)
            {
                if (GetComponent<Image>() != null)
                {
                    tween[1] = Sequence.Insert(0f, GetComponent<Image>().DOColor(end_col, time + rand_time));
                    tween[2] = Sequence.Insert(time + rand_time, GetComponent<Image>().DOColor(Now_col, time + rand_time));
                }
                else if (GetComponent<Text>() != null)
                {
                    tween[1] = Sequence.Insert(0f, GetComponent<Text>().DOColor(end_col, time + rand_time));
                    tween[2] = Sequence.Insert(time + rand_time, GetComponent<Text>().DOColor(Now_col, time + rand_time));
                }
                else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                {
                    if (transform.GetChild(0).GetComponent<Image>() != null)
                    {
                        tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Image>().DOColor(end_col, time + rand_time));
                        tween[2] = Sequence.Insert(time + rand_time, transform.GetChild(0).GetComponent<Image>().DOColor(Now_col, time + rand_time));
                    }
                    else if (transform.GetChild(0).GetComponent<Text>() != null)
                    {
                        tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Text>().DOColor(end_col, time + rand_time));
                        tween[2] = Sequence.Insert(time + rand_time, transform.GetChild(0).GetComponent<Text>().DOColor(Now_col, time + rand_time));
                    }
                }
            }
        }
        if (TheAnime == Type.位移)
        {
            if (TheBegin == Type3.相對)
            {
                tween[1] = Sequence.Insert(0f, transform.DOMove(Now_pos + end_pos, time + rand_time));
                tween[2] = Sequence.Insert(time + rand_time, transform.DOMove(Now_pos, time + rand_time));
            }
            if (TheBegin == Type3.絕對)
            {
                tween[1] = Sequence.Insert(0f, transform.DOMove(end_pos, time + rand_time));
                tween[2] = Sequence.Insert(time + rand_time, transform.DOMove(Now_pos, time + rand_time));
            }
        }
        if (TheAnime == Type.縮放)
        {
            if (TheBegin == Type3.相對)
            {
                tween[1] = Sequence.Insert(0f, transform.DOScale(Now_sca + end_sca, time + rand_time));
                tween[2] = Sequence.Insert(time + rand_time, transform.DOMove(Now_sca, time + rand_time));
            }
            if (TheBegin == Type3.絕對)
            {
                tween[1] = Sequence.Insert(0f, transform.DOScale(end_sca, time + rand_time));
                tween[2] = Sequence.Insert(time + rand_time, transform.DOMove(Now_sca, time + rand_time));
            }
        }
        if (TheAnime == Type.旋轉)
        {
            if (TheBegin == Type3.相對)
            {
                tween[1] = Sequence.Insert(0, transform.DORotate(Now_rot + end_rot, time + rand_time));
                tween[2] = Sequence.Insert(time + rand_time, transform.DORotate(Now_rot, time + rand_time));
            }
            if (TheBegin == Type3.絕對)
            {
                tween[1] = Sequence.Insert(0, transform.DORotate(end_rot, time + rand_time));
                tween[2] = Sequence.Insert(time + rand_time, transform.DORotate(Now_rot, time + rand_time));
            }
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

        Invoke("Anime", (time + rand_time) * 2);
    }
}
