using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{


    public int RowNum = 7;
    public int ColumnNum = 10;
    public GameObject candy;

    public GameObject controller;
    //二维数组
    public ArrayList arrayList;


    //声音
    public AudioClip swapClip;
    public AudioClip explodeClip;
    public AudioClip matchClip;
    public AudioClip wrongClip;

    // Use this for initialization
    void Start()
    {
        arrayList = new ArrayList();
        Create();
        //第一次消除
        WhileCheckMatches();

    }

    void WhileCheckMatches()
    {
        if (CheckMatches())
        {
            RemoveMatch();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Create()
    {

        for (int i = 0; i < RowNum; i++)
        {
            ArrayList list = new ArrayList();
            for (int j = 0; j < ColumnNum; j++)
            {
                Candy c = AddCandy(i, j);
                list.Add(c);

            }
            arrayList.Add(list);


        }
    }
    //得到二维数组的值
    private Candy GetCandy(int rowIndex, int columnIndex)
    {
        ArrayList tmp = arrayList[rowIndex] as ArrayList;
        Candy c = tmp[columnIndex] as Candy;
        return c;
    }
    //设置二维数组的值
    private void SetCandy(int rowIndex, int columnIndex, Candy c)
    {
        ArrayList tmp = arrayList[rowIndex] as ArrayList;
        tmp[columnIndex] = c;
    }

    private Candy AddCandy(int i, int j)
    {
        GameObject obj = Instantiate(candy) as GameObject;
        obj.transform.parent = controller.transform;
        Candy c = obj.GetComponent<Candy>();
        c.rowIndx = i;
        c.columnIndex = j;
        c.UpdatePosition();
        c.controler = this;
        return c;
    }
    private Candy firstCandy;
    //交换位置
    public void Select(Candy candy)
    {

        if (firstCandy == null)
        {
            firstCandy = candy;
            firstCandy.Selected = true;
        }
        else
        {
            firstCandy.Selected = false;
            if (Mathf.Abs(candy.rowIndx - firstCandy.rowIndx) + Mathf.Abs(candy.columnIndex - firstCandy.columnIndex) == 1)
            {
                //使用协程  等待.4秒在进行消除
                StartCoroutine(ExChangeByCoroutine(firstCandy, candy));
            }
            else {

                audio.PlayOneShot(wrongClip);
            }
            firstCandy = null;
           
        }

    }

    IEnumerator ExChangeByCoroutine(Candy firstCandy, Candy candy)
    {
        //检测并消除
        Exchange(firstCandy, candy);
       yield return new WaitForSeconds(0.4f);
        //检测并消除
        if (CheckMatches())
        {
            RemoveMatch();
        }
        else
        {
            Exchange(firstCandy, candy);
        }
    }
    void Exchange(Candy c1, Candy c2)
    {
        audio.PlayOneShot(swapClip);

        Candy temp = c2;
        SetCandy(c1.rowIndx, c1.columnIndex, c2);
        SetCandy(temp.rowIndx, temp.columnIndex, c1);

        int rowIndex = c1.rowIndx;
        c1.rowIndx = c2.rowIndx;
        c2.rowIndx = rowIndex;

        int cloumnIndex = c1.columnIndex;
        c1.columnIndex = c2.columnIndex;
        c2.columnIndex = cloumnIndex;

        c1.TweenToPosition();
        c2.TweenToPosition();
        print("交换成功");

    }

    private void AddEffect(Vector3 pos)
    {
        Instantiate(Resources.Load("Prefabs/Explosion2"), pos, Quaternion.identity);
        //振屏
        CameraShake.shakeFor(0.2f, 0.2f);
    }

    private void Remove(Candy c)
    {
        AddEffect(c.transform.position);
        audio.PlayOneShot(explodeClip);

        c.Dispose();
        // move up candy down
        int columnIndex = c.columnIndex;
        for (int rowIndex = c.rowIndx + 1; rowIndex < RowNum; rowIndex++)
        {
            Candy c2 = GetCandy(rowIndex, columnIndex);
            c2.rowIndx--;
            //c2.UpdatePosition();
            c2.TweenToPosition();
            SetCandy(rowIndex - 1, columnIndex, c2);
        }
        // add new to top
        Candy newC = AddCandy(RowNum - 1, columnIndex);
        newC.rowIndx = RowNum;
        newC.UpdatePosition();
        newC.rowIndx--;
        newC.TweenToPosition();
        SetCandy(RowNum - 1, columnIndex, newC);
    }

    //检测有无可疑消除的  无消除返回false
    public bool CheckMatches()
    {
     
        return CheckHorizontalMathches() || CheckVerticalMathches();
    }
    //检测水平的
    public bool CheckHorizontalMathches()
    {
        bool result = false;
        // int rowIndex = RowNum - 1;
        for (int rowIndex = 0; rowIndex < RowNum; rowIndex++)
        {
            //ColumnNum-2  这里得减2 不然下面加2会超出数组限制
            for (int columnIndex = 0; columnIndex < ColumnNum - 2; columnIndex++)
            {
                if ((GetCandy(rowIndex, columnIndex).type == GetCandy(rowIndex, columnIndex + 1).type) &&
                   (GetCandy(rowIndex, columnIndex + 2).type == GetCandy(rowIndex, columnIndex + 1).type))
                {
                    audio.PlayOneShot(matchClip);
                    result = true;
                    Debug.Log(rowIndex + ":: " + columnIndex + " " + (columnIndex + 1) + " " + (columnIndex + 2));
                    AddMatch(GetCandy(rowIndex, columnIndex));
                    AddMatch(GetCandy(rowIndex, columnIndex + 1));
                    AddMatch(GetCandy(rowIndex, columnIndex + 2));
                }
            }
        }
        return result;
    }
    private ArrayList matches;
    //添加要消除的对象
    private void AddMatch(Candy c)
    {
        if (matches == null)
        {
            matches = new ArrayList();

        }
        int index = matches.IndexOf(c);//判断是否存在该对象
        if (index == -1)
        {
            matches.Add(c);
        }

    }

    private void RemoveMatch()
    {
        Candy temp;
        for (int i = 0; i < matches.Count; i++)
        {
            temp = matches[i] as Candy;

            Remove(temp);
        }
        matches = new ArrayList();
        //移除的时候 也有动画
        StartCoroutine(WaitAndCheck());

    }
    IEnumerator WaitAndCheck()
    {
        yield return new WaitForSeconds(0.5f);
        if (CheckMatches())
        {
            RemoveMatch();
        }

    }
    //检测垂直的
    public bool CheckVerticalMathches()
    {
        bool result = false;
        for (int i = 0; i < ColumnNum; i++)
        {
            //ColumnNum-2  这里得减2 不然下面加2会超出数组限制
            for (int rowIndex = 0; rowIndex < RowNum - 2; rowIndex++)
            {
                if (GetCandy(rowIndex, i).type == GetCandy(rowIndex + 1, i).type && GetCandy(rowIndex + 1, i).type == GetCandy(rowIndex + 2, i).type)
                {
                    audio.PlayOneShot(matchClip);
                    result = true;
                    //print("检测到要消除的");
                    //如果发现有三个一样的
                    AddMatch(GetCandy(rowIndex, i));
                    AddMatch(GetCandy(rowIndex + 1, i));
                    AddMatch(GetCandy(rowIndex + 2, i));
                }
            }
        }
        return result;
    }
}
