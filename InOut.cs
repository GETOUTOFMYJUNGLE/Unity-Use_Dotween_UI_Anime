using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
        文字,
        變色,
        縮放,
        位移,
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
    [Header("起點定義")]
    [Tooltip("決定起點設置的是絕對的還是相對的，動畫模式為文字時無效")]
    public Type3 TheBegin;

    [Header("起始大小（只有動畫模式為縮放時才有效）")]
    [Tooltip("如果動畫模式是縮放的話，那一開始有多大")]
    public Vector3 begin_sca;

    [Header("位移起點（只有動畫模式為位移時才有效）")]
    [Tooltip("如果動畫模式是位移的話，那起點在哪裡")]
    public Vector3 begin_pos;

    [Header("旋轉起點（只有動畫模式為旋轉時才有效）")]
    [Tooltip("如果動畫模式是旋轉的話，那一開始是幾度")]
    public Vector3 begin_rot;

    [Header("起始顏色（只有動畫模式為變色時才有效）")]
    [Tooltip("如果動畫模式是變色的話，那一開始是甚麼顏色")]
    public Color begin_col;

    [Header("動畫時間")]
    [Tooltip("決定循環一次的時間")]
    [Range(0f, 60f)]
    public float time = 0.1f;

    [Header("隨機改變動畫時間")]
    [Tooltip("決定隨機改變動畫時間可能的最大值與最小值會差幾秒，容易與其他InOut或Floater衝突，請慎用")]
    [Range(0f, 30f)]
    public float The_rand_time = 0f;

    [Header("啟用延遲時間")]
    [Tooltip("可以讓出現的動畫延遲幾秒後再執行")]
    [Range(0f, 60f)]
    public float delay_time = 0f;

    [Header("收回後刪除嗎")]
    [Tooltip("決定收回後是刪除還是放置")]
    public bool Die = true;

    [Header("停在起始嗎")]
    [Tooltip("決定是否停在起始位置")]
    public bool Stopin = false;

    [Header("預設倒帶嗎")]
    [Tooltip("決定是否初次啟用改成收回動畫")]
    public bool Backin = false;

    [Header("預設啟用嗎")]
    [Tooltip("決定是否一開始就要播放動畫，如果啟用收回後就會把物件關閉")]
    public bool Startin = true;

    //XXX為掛載此腳本的物件名稱
    //由別的腳本調用此物件收回的方式：GameObject.Find("XXX").GetComponent<InOut>().Back();
    //由別的腳本調用此物件且不延遲收回的方式：GameObject.Find("XXX").GetComponent<InOut>().NowBack();
    
    string Now_tex;//目前文字
    Color Now_col;//目前色彩
    Vector3 Now_sca;//目前大小
    Vector3 Now_pos;//目前位置
    Vector3 Now_rot;//目前角度
    bool back = false;//是要收回嗎
    bool relatively = false;//用來讓相對模式不會重複動到起始兩次的按鈕
    void Awake()
    {
        //定義目前狀態區
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

        if (GetComponent<Text>() != null)
        {
            Now_tex = GetComponent<Text>().text;
        }
        else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
        {
            if (transform.GetChild(0).GetComponent<Text>() != null)
                Now_tex = transform.GetChild(0).GetComponent<Text>().text;
        }
    }
    void OnEnable()//當他被打開時觸發
    {
        if (Startin)
        {
            //定義目前狀態區
            transform.localScale = Now_sca;
            transform.position = Now_pos;
            transform.eulerAngles = Now_rot;
            if (GetComponent<Image>() != null)
            {
                GetComponent<Image>().color = Now_col;
            }
            else if (GetComponent<Text>() != null)
            {
                GetComponent<Text>().color = Now_col;
            }
            else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
            {
                if (transform.GetChild(0).GetComponent<Image>() != null)
                    transform.GetChild(0).GetComponent<Image>().color = Now_col;
                else if (transform.GetChild(0).GetComponent<Text>() != null)
                    transform.GetChild(0).GetComponent<Text>().color = Now_col;
            }
            if (GetComponent<Text>() != null)
            {
                GetComponent<Text>().text = Now_tex;
            }
            else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
            {
                if (transform.GetChild(0).GetComponent<Text>() != null)
                    transform.GetChild(0).GetComponent<Text>().text = Now_tex;
            }

            Run_Anime();
        }
    }
    public void Run_Anime()
    {
        relatively = false;
        if (delay_time > 0 && !Stopin)//執行動畫
        {
            Stopin = true;
            Anime();
            Stopin = false;
            relatively = true;
            Invoke("Anime", delay_time);
        }
        else if (delay_time > 0 && Stopin)
        {
            Invoke("Anime", delay_time);
        }
        else
        {
            Anime();
        }
        Invoke("BackInClose", delay_time + 0.2f);
    }
    void Anime()
    {
        if (Backin)//如果預設倒帶
        {
            back = true;
        }
        float rand_time = Random.Range(The_rand_time * -1, The_rand_time);//隨機時間
        if (rand_time + time < 0.1f)
            rand_time = (time - 0.1f) * -1;

        //如果有用隨機時間，且同物件有Floater，就由你呼叫
        if (The_rand_time > 0 && GetComponent<Floater>() != null)
        {
            Invoke("CallFloater", time + rand_time);
        }


        Sequence Sequence = DOTween.Sequence();//動畫列表
        Tween[] tween = new Tween[3];
        if (!back)//淡入
        {
            if (TheAnime == Type.文字)
            {
                if (GetComponent<Text>() != null)
                {
                    GetComponent<Text>().text = "";
                    if (!Stopin)
                        tween[2] = Sequence.Insert(0.01f, GetComponent<Text>().DOText(Now_tex, time + rand_time));
                }
                else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                {
                    if (transform.GetChild(0).GetComponent<Text>() != null)
                    {
                        transform.GetChild(0).GetComponent<Text>().text = "";
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, transform.GetChild(0).GetComponent<Text>().DOText(Now_tex, time + rand_time));
                    }
                }
            }
            if (TheAnime == Type.變色)
            {
                if (TheBegin == Type3.相對)
                {
                    if (GetComponent<Image>() != null)
                    {
                        if (!relatively)
                            GetComponent<Image>().color = GetComponent<Image>().color + begin_col;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, GetComponent<Image>().DOColor(Now_col, time + rand_time));
                    }
                    else if (GetComponent<Text>() != null)
                    {
                        if (!relatively)
                            GetComponent<Text>().color = GetComponent<Text>().color + begin_col;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, GetComponent<Text>().DOColor(Now_col, time + rand_time));
                    }
                    else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                    {
                        if (transform.GetChild(0).GetComponent<Image>() != null)
                        {
                            if (!relatively)
                                transform.GetChild(0).GetComponent<Image>().color = transform.GetChild(0).GetComponent<Image>().color + begin_col;
                            if (!Stopin)
                                tween[2] = Sequence.Insert(0.01f, transform.GetChild(0).GetComponent<Image>().DOColor(Now_col, time + rand_time));
                        }
                        else if (transform.GetChild(0).GetComponent<Text>() != null)
                        {
                            if (!relatively)
                                transform.GetChild(0).GetComponent<Text>().color = transform.GetChild(0).GetComponent<Text>().color + begin_col;
                            if (!Stopin)
                                tween[2] = Sequence.Insert(0.01f, transform.GetChild(0).GetComponent<Text>().DOColor(Now_col, time + rand_time));
                        }
                    }
                }
                if (TheBegin == Type3.絕對)
                {
                    if (GetComponent<Image>() != null)
                    {
                        GetComponent<Image>().color = begin_col;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, GetComponent<Image>().DOColor(Now_col, time + rand_time));
                    }
                    else if (GetComponent<Text>() != null)
                    {
                        GetComponent<Text>().color = begin_col;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, GetComponent<Text>().DOColor(Now_col, time + rand_time));
                    }
                    else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                    {
                        if (transform.GetChild(0).GetComponent<Image>() != null)
                        {
                            transform.GetChild(0).GetComponent<Image>().color = begin_col;
                            if (!Stopin)
                                tween[2] = Sequence.Insert(0.01f, transform.GetChild(0).GetComponent<Image>().DOColor(Now_col, time + rand_time));
                        }
                        else if (transform.GetChild(0).GetComponent<Text>() != null)
                        {
                            transform.GetChild(0).GetComponent<Text>().color = begin_col;
                            if (!Stopin)
                                tween[2] = Sequence.Insert(0.01f, transform.GetChild(0).GetComponent<Text>().DOColor(Now_col, time + rand_time));
                        }
                    }
                }
            }
            if (TheAnime == Type.縮放)
            {
                if (TheBegin == Type3.相對)
                {
                    if (!relatively)
                        transform.localScale = transform.localScale + begin_sca;
                    if (!Stopin)
                        tween[2] = Sequence.Insert(0.01f, transform.DOScale(Now_sca, time + rand_time));
                }
                if (TheBegin == Type3.絕對)
                {
                    transform.localScale = begin_sca;
                    if (!Stopin)
                        tween[2] = Sequence.Insert(0.01f, transform.DOScale(Now_sca, time + rand_time));
                }
            }
            if (TheAnime == Type.位移)
            {
                if (TheBegin == Type3.相對)
                {
                    if (GetComponent<RectTransform>() != null)
                    {
                        if (!relatively)
                            GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition + new Vector2(begin_pos.x , begin_pos.y);
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, GetComponent<RectTransform>().DOMove(Now_pos, time + rand_time));
                    }
                    else
                    {
                        if (!relatively)
                            transform.position = transform.position + begin_pos;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, transform.DOMove(Now_pos, time + rand_time));
                    }
                }
                if (TheBegin == Type3.絕對)
                {
                    if (GetComponent<RectTransform>() != null)
                    {
                        GetComponent<RectTransform>().anchoredPosition = begin_pos;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, GetComponent<RectTransform>().DOMove(Now_pos, time + rand_time));
                    }
                    else
                    {
                        transform.position = begin_pos;
                        if (!Stopin)
                            tween[2] = Sequence.Insert(0.01f, transform.DOMove(Now_pos, time + rand_time));
                    }
                }

            }
            if (TheAnime == Type.旋轉)
            {
                if (TheBegin == Type3.相對)
                {
                    if (!relatively)
                        transform.DORotate(transform.eulerAngles + begin_rot, 0);
                    if (!Stopin)
                        tween[2] = Sequence.Insert(0.01f, transform.DORotate(begin_rot, time + rand_time));
                }
                if (TheBegin == Type3.絕對)
                {
                    transform.DORotate(begin_rot, 0);
                    if (!Stopin)
                        tween[2] = Sequence.Insert(0.01f, transform.DORotate(begin_rot, time + rand_time));
                }
            }
        }
        else//淡出
        {
            if (TheAnime == Type.文字)
            {
                if (GetComponent<Text>() != null)
                {
                    if (!Stopin)
                        tween[1] = Sequence.Insert(0f, GetComponent<Text>().DOText("", time + rand_time));
                }
                else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                {
                    if (transform.GetChild(0).GetComponent<Text>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Text>().DOText("", time + rand_time));
                    }
                }
            }
            if (TheAnime == Type.變色)
            {
                if (TheBegin == Type3.相對)
                {
                    if (GetComponent<Image>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, GetComponent<Image>().DOColor(GetComponent<Image>().color - begin_col, time + rand_time));
                    }
                    else if (GetComponent<Text>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, GetComponent<Text>().DOColor(GetComponent<Text>().color - begin_col, time + rand_time));
                    }
                    else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                    {
                        if (transform.GetChild(0).GetComponent<Image>() != null)
                        {
                            if (!Stopin)
                                tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Image>().DOColor(transform.GetChild(0).GetComponent<Image>().color - begin_col, time + rand_time));
                        }
                        else if (transform.GetChild(0).GetComponent<Text>() != null)
                        {
                            if (!Stopin)
                                tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Text>().DOColor(transform.GetChild(0).GetComponent<Text>().color - begin_col, time + rand_time));
                        }
                    }
                }
                if (TheBegin == Type3.絕對)
                {
                    if (GetComponent<Image>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, GetComponent<Image>().DOColor(begin_col, time + rand_time));
                    }
                    else if (GetComponent<Text>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, GetComponent<Text>().DOColor(begin_col, time + rand_time));
                    }
                    else if (GetComponentsInChildren<Transform>(true).Length > 1)//有子物件
                    {
                        if (transform.GetChild(0).GetComponent<Image>() != null)
                        {
                            if (!Stopin)
                                tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Image>().DOColor(begin_col, time + rand_time));
                        }
                        else if (transform.GetChild(0).GetComponent<Text>() != null)
                        {
                            if (!Stopin)
                                tween[1] = Sequence.Insert(0f, transform.GetChild(0).GetComponent<Text>().DOColor(begin_col, time + rand_time));
                        }
                    }
                }
            }
            if (TheAnime == Type.縮放)
            {
                if (TheBegin == Type3.相對)
                {
                    if (!Stopin)
                        tween[1] = Sequence.Insert(0f, transform.DOScale(transform.localScale - begin_sca, time + rand_time));
                }
                if (TheBegin == Type3.絕對)
                {
                    if (!Stopin)
                        tween[1] = Sequence.Insert(0f, transform.DOScale(begin_sca, time + rand_time));
                }
            }
            if (TheAnime == Type.位移)
            {
                if (TheBegin == Type3.相對)
                {
                    if (GetComponent<RectTransform>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, GetComponent<RectTransform>().DOMove(GetComponent<RectTransform>().anchoredPosition - new Vector2(begin_pos.x, begin_pos.y), time + rand_time));
                    }
                    else
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, transform.DOMove(transform.position - begin_pos, time + rand_time));
                    }
                }
                if (TheBegin == Type3.絕對)
                {
                    if (GetComponent<RectTransform>() != null)
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, GetComponent<RectTransform>().DOMove(begin_pos, time + rand_time));
                    }
                    else
                    {
                        if (!Stopin)
                            tween[1] = Sequence.Insert(0f, transform.DOMove(begin_pos, time + rand_time));
                    }
                }
            }
            if (TheAnime == Type.旋轉)
            {
                if (TheBegin == Type3.相對)
                {
                    if (!Stopin)
                        tween[1] = Sequence.Insert(0f, transform.DORotate(transform.eulerAngles - begin_rot, time + rand_time));
                }
                if (TheBegin == Type3.絕對)
                {
                    if (!Stopin)
                        tween[1] = Sequence.Insert(0f, transform.DORotate(begin_rot, time + rand_time));
                } 
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

        if (back && Startin && !Backin)//預設啟用且返回且沒在倒帶就關閉
        {
            gameObject.SetActive(false);
        }
        if (!Backin)
        {
            back = false;
        }

    }
    public void Back()//收回
    {
        back = true;
        Run_Anime();
    }
    public void NowBack()//馬上收回
    {
        back = true;
        delay_time = 0f;
        Run_Anime();
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
    void BackInClose()//把BackIn關掉
    {
        Backin = false;

    }
    void CallFloater()//如果有用隨機時間，且同物件有Floater，就由你呼叫
    {
        if (GetComponent<Floater>().Wait_InOut_Randtime)//只一次
            GetComponent<Floater>().Anime();
    }
}
