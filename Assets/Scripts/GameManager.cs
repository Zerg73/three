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

    bool b;
    void Awake()
    {

    }
    void OnEnable()
    {
        Init();
    }
    void Start()
    {

    }

    void Update()
    {
        if (score >= highScore)
        {
            highScore=score;
            PlayerPrefs.SetInt("Score", highScore);
        }
        highScoreText.text = "HighScore: " + highScore;
        scoreText.text = "Score: " + score;
        string a = "剩余移动方法： " + answer;
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

    public void Restart(){
        score=0;
        ReloadButton();
        t1 = 0; t2 = 0; t3 = 0; t4 = 0;
    }

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

    IEnumerator Waiting(float n)
    {
        falling = true;
        yield return new WaitForSeconds(n);
        falling = false;
    }

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
                if (row[i][j] == row[i][j - 1])
                {
                    rowline++;
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
                if (col[i][j] == col[i][j - 1])
                {
                    colline++;
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



    void Falldown()
    {
        falling = true;
        for (int i = 0; i < size; i++)
        {
            Fall(ref col[i]);
        }
        Csplice();
    }



    void Fall(ref int[] input)
    {
        int j = size - 1;
        for (int i = size - 1; i >= 0; i--)
        {
            if (input[i] != 10)
            {
                Swap(ref input[i], ref input[j--]);
            }
            else
            {

            }
        }
    }
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
    void Fill()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
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
    public void GetClick(int no)
    {


        if (!oneSelected)
        {
            noa = no;
            oneSelected = true;
        }
        else
        {
            nob = no;
            if (noa != nob)
                Exchange();
            box[Mathf.FloorToInt(noa / size), noa % size].GetComponent<Block>().lig = false;
            box[Mathf.FloorToInt(nob / size), nob % size].GetComponent<Block>().lig = false;
            oneSelected = false;
        }
    }
    void Exchange()
    {
        ischange = false;

        if (Mathf.Abs(nob - noa) == 1 || Mathf.Abs(nob - noa) == size)
        {
            Swap(ref col[noa % size][Mathf.FloorToInt(noa / size)], ref col[nob % size][Mathf.FloorToInt(nob / size)]);
            Swap(ref row[Mathf.FloorToInt(noa / size)][noa % size], ref row[Mathf.FloorToInt(nob / size)][nob % size]);

            Check();
        }
        else
        {
        }
    }



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
