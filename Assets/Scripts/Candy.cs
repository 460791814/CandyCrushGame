using UnityEngine;
using System.Collections;

public class Candy : MonoBehaviour {
    public int rowIndx = 0;
    public int columnIndex = 0;
    public float xOff = -4.5f;
    public float yOff = -3f;

    public int candyTypeNum = 6;//显示类型的个数

    public GameObject[] objs;
    public GameObject bg;
    public int type;


    public GameController controler;

    private SpriteRenderer sprite;
    //选中
    public bool Selected {
        set {
            if (sprite != null)
            {
                sprite.color = value ? Color.blue : Color.white;
            }
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //鼠标按下    IPhone不能用
    void OnMouseDown()
    {
        controler.Select(this);
    }
    void AddRanage()
    {
       if (bg != null) return;
       type=Random.Range(0, Mathf.Min(candyTypeNum,objs.Length));
       bg = Instantiate(objs[type]) as GameObject;
       bg.transform.parent = transform;
       sprite = bg.GetComponent<SpriteRenderer>();
    }
    public void TweenToPosition()
    {
      
        AddRanage();
       iTween.MoveTo(this.gameObject,iTween.Hash("x",columnIndex + xOff,"y",rowIndx + yOff,"time",0.3f));
    }
    public void UpdatePosition()
    {
        
        AddRanage();
        transform.position = new Vector3(columnIndex + xOff, rowIndx + yOff, 0);
    }
    //销毁自身
    public void Dispose()
    {
        controler = null;
        Destroy(bg.gameObject);
        Destroy(this.gameObject);
    }
}
