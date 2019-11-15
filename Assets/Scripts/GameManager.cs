using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject block;
    public GameObject groundPanel;
    // public Text test;
    public Text scoreText;
    public Text highScoreText;
    public Text testText;
    public Text timeText;
    public GameObject reloadButton;
    public GameObject tipButton;

    public int size = 9;

    //下面这俩变量将用来检测消除以及保存棋盘上的块。
    private GameObject[,] box;
    private int[][] row, col;
    private int score;
    private int highScore;
    private bool oneSelected;
    private bool falling;
    private bool ischange;
    public bool istip;
    private int noa, nob;
    private int answer;
    float t1 = 0, t2 = 0, t3 = 0, t4 = 0;
    void OnEnable()
    {
        Init();
    }
    void Update()
    {
        //最高分更新
        if (score >= highScore)
        {
            highScore=score;
            PlayerPrefs.SetInt("Score", highScore);
        }
        highScoreText.text = "HighScore: " + highScore;
        scoreText.text = "Score: " + score;
        string a = "剩余移动方法： " + answer;
        //当无法交换的时候显示更新按钮
        if (answer == 0)
        {
            reloadButton.gameObject.SetActive(true);
        }
        else
        {
            reloadButton.gameObject.SetActive(false);
        }
        testText.text = a;
        ShowTime();
        DeathCheck();

    }
    //重新加载游戏（其实就是重置了分数和时间，顺便随机一哈
    public void Restart(){
        score=0;
        ReloadButton();
        t1 = 0; t2 = 0; t3 = 0; t4 = 0;
    }
    //重新随机，显而易见
    public void ReloadButton()
    {
        int s = score;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                box[i, j].GetComponent<Block>().SetValue();
            }
        }
        Check();
        score = s;
    }
    //用于切换是否显示提示
    public void SwitchTipButton()
    {
        istip = !istip;
        if (istip)
        {
            tipButton.transform.Find("Text").GetComponent<Text>().text = "关闭提示";
        }
        else
        {
            tipButton.transform.Find("Text").GetComponent<Text>().text = "显示提示";
        }
    }
    //时间显示相关函数
    void ShowTime()
    {
        t1 = t1 + Time.deltaTime;
        if (t1 >= 60)
        {
            t2++;
            t1 = 0;
        }
        if (t2 >= 60f)
        {
            t3 += 1f;
            t2 = 0f;
        }
        if (t3 >= 24f)
        {
            t4 += 1f;
            t3 = 0f;
        }
        timeText.text = Mathf.FloorToInt(t4).ToString() +
        " : " + Mathf.FloorToInt(t3).ToString() + " : " +
        Mathf.FloorToInt(t2).ToString() + " : " + Mathf.FloorToInt(t1).ToString() + "\n";
    }
    //初始化，加载出 size × size 个物体，初始布局
    void Init()
    {
        int no = 0;
        testText.text = "";
        oneSelected = false;
        falling = false;
        ischange = false;
        istip = false;
        score = 0;
        highScore = PlayerPrefs.GetInt("Score", 0);
        box = new GameObject[size, size];
        row = new int[size][];
        col = new int[size][];
        for (int i = 0; i < size; i++)
        {
            row[i] = new int[size];
            col[i] = new int[size];
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject temp = Instantiate(block, groundPanel.transform.position, groundPanel.transform.rotation);
                temp.transform.SetParent(groundPanel.transform);
                temp.GetComponent<Block>().no = no++;
                box[i, j] = temp;
            }
        }
        Cut();
        Check();
        DeathCheck();
        while (answer == 0)
        {
            ReloadButton();
        }
        score = 0;
        ischange = false;
    }
    //用于检测当前是否凑齐了消除条件
    void Check()
    {
        ischange = false;
        int rowline;
        int colline;

        for (int i = 0; i < size; i++)
        {
            rowline = 1;
            colline = 1;

            for (int j = 1; j < size; j++)
            {
                //当该行存在存在两个相连的时候，继续判断是否能达到3个
                if (row[i][j] == row[i][j - 1])
                {
                    rowline++;
                    //如果判断到了末尾且连续达到3个或以上就开始消除（及将值设置为预设的10）
                    if (j == size - 1 && rowline >= 3)
                    {
                        ischange = true;
                        do
                        {
                            row[i][j - rowline + 1] = 10;
                            score++;
                        } while (--rowline > 0);
                    }
                }
                else
                {
                    //这个是和前面的颜色不同，但之前相连超过3个，就将前面的消除。
                    if (rowline >= 3)
                    {
                        ischange = true;
                        do
                        {
                            row[i][j - rowline] = 10;
                            score++;
                        } while (--rowline > 0);
                    }
                    rowline = 1;
                }
                /* *************************************************************************/
                //和上面对应，这是列
                if (col[i][j] == col[i][j - 1])
                {
                    colline++;
                    //参考上面
                    if (j == size - 1 && colline >= 3)
                    {
                        ischange = true;
                        do
                        {
                            col[i][j - colline + 1] = 10;
                            score++;
                        } while (--colline > 0);
                    }
                }
                else
                {
                    //同上
                    if (colline >= 3)
                    {
                        ischange = true;
                        do
                        {
                            col[i][j - colline] = 10;
                            score++;
                        } while (--colline > 0);
                    }
                    colline = 1;
                }
            }
        }

        //如果发生了交换的情况，就更新数组，下落并填充，继续Check直到3连结束
        if (ischange)
        {
            Splice();
            Falldown();
            Fill();
            Check();
        }
        else
            Cut();

        // ischange=false;
    }



    //此函数将块的值（颜色）分配给计算用的两个数组。
    void Cut()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                row[i][j] = box[i, j].GetComponent<Block>().num;
                col[j][i] = box[i, j].GetComponent<Block>().num;
            }
        }
    }
    //和上面那个函数反过来。
    void Splice()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (row[i][j] == col[j][i])
                {
                    box[i, j].GetComponent<Block>().num = row[i][j];
                }
                else
                {
                    box[i, j].GetComponent<Block>().num = 10;
                }
            }
        }
        Cut();
    }
    //单独将col数组更新至box数组
    void Csplice()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                box[i, j].GetComponent<Block>().num = col[j][i];
            }
        }
        Cut();
    }
    //单独将row数组更新至box数组
    void Rsplice()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                box[i, j].GetComponent<Block>().num = row[i][j];
            }
        }
        Cut();
    }


    //处理下落
    void Falldown()
    {
        falling = true;
        for (int i = 0; i < size; i++)
        {
            Fall(ref col[i]);
        }
        Csplice();
    }


    //对每一列进行下落处理，归并到Falldown()函数中
    //这个函数的实现我是想起了一道算法题：将数组中的0全部置于末尾，其余元素顺序不变。这个函数的实现就是将10全部放到前面
    void Fall(ref int[] input)
    {
        int j = size - 1;
        for (int i = size - 1; i >= 0; i--)
        {
            if (input[i] != 10)
            {
                Swap(ref input[i], ref input[j--]);
            }
        }
    }
    //交换函数，不解释了
    void Swap(ref int a, ref int b)
    {
        if (a == b)
        {
            return;
        }
        a ^= b;
        b ^= a;
        a ^= b;
    }
    //填充函数，被消除后就填充啦
    void Fill()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //当前版本，10即为“消除”
                if (col[i][j] == 10)
                {
                    col[i][j] = Random.Range(0, 8);
                }
                else
                {
                    break;
                }
            }
        }
        Csplice();
        falling = false;
    }
    //接受鼠标点击事件，达到两次就检测是否可以移动消除
    public void GetClick(int no)
    {
        //检测是否是第一次点击
        if (!oneSelected)
        {
            noa = no;
            oneSelected = true;
        }
        //不然就是第二次，然后重置标志变量
        else
        {
            nob = no;
            //只要两次点击不在同一位置，就启动exchange函数
            if (noa != nob)
                Exchange();
            box[Mathf.FloorToInt(noa / size), noa % size].GetComponent<Block>().lig = false;
            box[Mathf.FloorToInt(nob / size), nob % size].GetComponent<Block>().lig = false;
            oneSelected = false;
        }
    }
    //如果上一个函数满足条件，就运行这个函数来交换
    void Exchange()
    {
        ischange = false;
        //限定：只有上下或者左右相邻的两个元素才可进行交换
        if (Mathf.Abs(nob - noa) == 1 || Mathf.Abs(nob - noa) == size)
        {
            Swap(ref col[noa % size][Mathf.FloorToInt(noa / size)], ref col[nob % size][Mathf.FloorToInt(nob / size)]);
            Swap(ref row[Mathf.FloorToInt(noa / size)][noa % size], ref row[Mathf.FloorToInt(nob / size)][nob % size]);
            Check();
        }
    }


    //用于检测当前棋盘是否还可以交换
    void DeathCheck()
    {
        answer = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                box[i, j].GetComponent<Block>().ok = false;
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //我决定画个图
                //
                // X O X X O X
                // O X 1 2 X O
                // X O X X O X
                //
                //思路是：以1为起点，假如2的位置没有越界，那么在12相连的基础上，能达成消除的就6种O的位置，逐一判断是否越界，下面那个是列的，90°倒转思考。
                //两个相邻的情况
                if (j + 1 < size && row[i][j] == row[i][j + 1])
                {
                    if ((i - 1 >= 0 && j - 1 >= 0) && row[i][j] == row[i - 1][j - 1])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 1].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((i + 1 < size && j + 2 < size) && row[i][j] == row[i + 1][j + 2])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 1].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((i - 1 >= 0 && j + 2 < size) && row[i][j] == row[i - 1][j + 2])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 1].GetComponent<Block>().ok = true;
                        answer++;

                    }
                    if ((i + 1 < size && j - 1 >= 0) && row[i][j] == row[i + 1][j - 1])
                    {

                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 1].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((j - 2 >= 0) && row[i][j] == row[i][j - 2])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 1].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((j + 3 < size) && row[i][j] == row[i][j + 3])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 1].GetComponent<Block>().ok = true;
                        answer++;
                    }
                }
                //
                // X O X
                // 1 X 2
                // X O X
                //
                //隔一个的情况
                if (j + 2 < size && row[i][j] == row[i][j + 2])
                {
                    if (i - 1 >= 0 && row[i][j] == row[i - 1][j + 1])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 2].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if (i + 1 < size && row[i][j] == row[i + 1][j + 1])
                    {
                        box[i, j].GetComponent<Block>().ok = true;
                        box[i, j + 2].GetComponent<Block>().ok = true;
                        answer++;
                    }
                }

                //纵向
                //两个相邻的情况
                if (j + 1 < size && col[i][j] == col[i][j + 1])
                {
                    if ((i - 1 >= 0 && j - 1 >= 0) && col[i][j] == col[i - 1][j - 1])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 1, i].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((i + 1 < size && j + 2 < size) && col[i][j] == col[i + 1][j + 2])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 1, i].GetComponent<Block>().ok = true;
                        answer++;

                    }
                    if ((i - 1 >= 0 && j + 2 < size) && col[i][j] == col[i - 1][j + 2])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 1, i].GetComponent<Block>().ok = true;
                        answer++;

                    }
                    if ((i + 1 < size && j - 1 >= 0) && col[i][j] == col[i + 1][j - 1])
                    {

                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 1, i].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((j - 2 >= 0) && col[i][j] == col[i][j - 2])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 1, i].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if ((j + 3 < size) && col[i][j] == col[i][j + 3])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 1, i].GetComponent<Block>().ok = true;
                        answer++;
                    }
                }

                //隔一个的情况
                if (j + 2 < size && col[i][j] == col[i][j + 2])
                {
                    if (i - 1 >= 0 && col[i][j] == col[i - 1][j + 1])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 2, i].GetComponent<Block>().ok = true;
                        answer++;
                    }
                    if (i + 1 < size && col[i][j] == col[i + 1][j + 1])
                    {
                        box[j, i].GetComponent<Block>().ok = true;
                        box[j + 2, i].GetComponent<Block>().ok = true;
                        answer++;
                    }
                }

            }
        }

    }
}
