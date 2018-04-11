using UnityEngine;
using UnityEngine.UI;

public enum TowerColor
{
    White = 0,
    Blue = 1,
    Pink = 2
}

public class NeutralTowerControl : MonoBehaviour
{
    #region 颜色和转动部分
    static Vector3 blue = new Vector3(0f, 210f / 255f, 1f);
    static Vector3 pink = new Vector3(1f, 34f / 255f, 182f / 255f);
    Vector3 FristColor = Vector3.zero;
    Vector3 LastColor = Vector3.zero;
    Vector3 NowColor = Vector3.zero;
    Material NowMaterial = null;
    Transform _Tower = null;
    Vector3 FristRotation;
    Vector3 NowRotation;
    Vector3 LastRotation;
    Quaternion _FristRotation;
    Quaternion _NowRotation;
    float _LastAngle;
    float TheTimeRotationRequire_Now = 0;
    public float TheTimeRotationRequire = 1;
    Transform LeftTower;
    Transform RightTower;
    Transform TargetTower;
    //Transform CameraTransform;
  //  Text number;
    bool IsChangeColor = false;
    public Image RedCircle;
    public Image BlueCircle;
    public Image NowCircle;
    #endregion


    #region 逻辑部分
    float CanAttackDistance = 4f;//中立塔攻击距离
    public static float CanOccuptDistance = 2.8f;//中立塔占领距离
    public TowerColor TowerStatus = TowerColor.White;
    TowerColor BeforeStatus = TowerColor.White;
    public TowerColor RealTimeStatus = TowerColor.White;//士兵存在于占领范围里
    public TowerColor RealTimeColor = TowerColor.White;//占领条的颜色
    public float OccuptTime = 3;//几秒占领完成
    float OccuptTime_Now = 0;
    #endregion


    void Start()
    {
        LeftTower = transform.parent.Find("Tower_Left");
        RightTower = transform.parent.Find("Tower_Right");
        //CameraTransform = GameObject.Find("Camera").transform;
       // number = transform.Find("Number/Text").GetComponent<Text>();
    }



    void Update()
    {
        if (MainGame.isPause)
            return;
        RatationTower();

     
        


    }
    void FixedUpdate()
    {
        if (!MainGame.isPlay)
            return;
        if (MainGame.isPause)
            return;
        ii();
        iii();
        iiii();

    }
    void ii()
    {
        var LeftSoldiers = MainGame.CharacterSystem.GetLeftList();
        var RightSoldiers = MainGame.CharacterSystem.GetRightList();
        bool HaveLeftSoldier = false;
        bool HaveRightSoldier = false;
        //foreach(var LeftSoldier in LeftSoldiers)        
        //    if (!HaveLeftSoldier)            
        //        if(IsCanOccupt(Distance(LeftSoldier.transform.position, transform.position)))                
        //            HaveLeftSoldier = true;

        //判断是否有士兵在中立塔的占领范围内
        foreach (var LeftSoldier in LeftSoldiers)
            if (!HaveLeftSoldier && IsCanOccupt(Distance(LeftSoldier.transform.position, transform.position)))
                HaveLeftSoldier = true;
        foreach (var RightSoldier in RightSoldiers)
            if (!HaveRightSoldier && IsCanOccupt(Distance(RightSoldier.transform.position, transform.position)))
                HaveRightSoldier = true;
        if (HaveLeftSoldier && HaveRightSoldier) RealTimeStatus = TowerColor.White;
        if (!HaveLeftSoldier && !HaveRightSoldier) RealTimeStatus = TowerColor.White;
        if (HaveLeftSoldier && !HaveRightSoldier) RealTimeStatus = TowerColor.Blue;
        if (!HaveLeftSoldier && HaveRightSoldier) RealTimeStatus = TowerColor.Pink;
        // Debug.Log("HaveLeftSoldier:" + HaveLeftSoldier.ToString() + "          HaveRightSoldier:" + HaveRightSoldier.ToString());
    }
    void iii()
    {
        if (TowerStatus == TowerColor.White)
        {
            if (RealTimeStatus == TowerColor.Blue)
            {
                if (RealTimeColor != TowerColor.Pink)
                {
                    OccuptTime_Now += Time.fixedDeltaTime;
                    RealTimeColor = TowerColor.Blue;
                    if (OccuptTime_Now >= OccuptTime)
                    {
                        OccuptTime_Now = OccuptTime;
                        TowerStatus = TowerColor.Blue;
                    }
                }
                if (RealTimeColor == TowerColor.Pink)
                {
                    OccuptTime_Now -= Time.fixedDeltaTime;
                    if (OccuptTime_Now <= 0)
                    {
                        OccuptTime_Now = 0;
                        RealTimeColor = TowerColor.Blue;
                    }
                }


            }
            if (RealTimeStatus == TowerColor.Pink)
            {
                if (RealTimeColor != TowerColor.Blue)
                {
                    OccuptTime_Now += Time.fixedDeltaTime;
                    RealTimeColor = TowerColor.Pink;
                    if (OccuptTime_Now >= OccuptTime)
                    {
                        OccuptTime_Now = OccuptTime;
                        TowerStatus = TowerColor.Pink;
                    }
                }
                if (RealTimeColor == TowerColor.Blue)
                {
                    OccuptTime_Now += Time.fixedDeltaTime;
                    if (OccuptTime_Now <= 0)
                    {
                        OccuptTime_Now = 0;
                        RealTimeColor = TowerColor.Pink;
                    }
                }


            }
            if (RealTimeStatus == TowerColor.White)
            {
                if (RealTimeColor != TowerColor.White)
                {
                    OccuptTime_Now -= Time.fixedDeltaTime;
                    if (OccuptTime_Now <= 0)
                    {
                        RealTimeColor = TowerColor.White;
                        OccuptTime_Now = 0;
                    }
                }

            }
        }

        if (TowerStatus == TowerColor.Blue)
        {
            if (RealTimeStatus != TowerColor.Pink)
            {
                OccuptTime_Now += Time.fixedDeltaTime;
                OccuptTime_Now = Mathf.Min(OccuptTime_Now, OccuptTime);
            }

            if (RealTimeStatus == TowerColor.Pink)
            {
                OccuptTime_Now -= Time.fixedDeltaTime;
                if (OccuptTime_Now <= 0)
                {
                    OccuptTime_Now = Mathf.Max(OccuptTime_Now, 0);
                    TowerStatus = TowerColor.White;
                    RealTimeColor = TowerColor.White;
                }

            }
        }
        if (TowerStatus == TowerColor.Pink)
        {
            if (RealTimeStatus != TowerColor.Blue)
            {
                OccuptTime_Now += Time.fixedDeltaTime;
                OccuptTime_Now = Mathf.Min(OccuptTime_Now, OccuptTime);
            }

            if (RealTimeStatus == TowerColor.Blue)
            {
                OccuptTime_Now -= Time.fixedDeltaTime;
                if (OccuptTime_Now <= 0)
                {
                    OccuptTime_Now = Mathf.Max(OccuptTime_Now, 0);
                    TowerStatus = TowerColor.White;
                    RealTimeColor = TowerColor.White;
                }

            }
        }
    }
    void iiii()
    {
        if (TowerStatus != BeforeStatus)
        {
            TowerChangeColor(TowerStatus);
            BeforeStatus = TowerStatus;
        }
        if (TowerStatus == TowerColor.Blue)
        {
            TowerBeingOccupied(TowerColor.White, TowerColor.Blue, OccuptTime_Now / OccuptTime);
        }
        if (TowerStatus == TowerColor.Pink)
        {
            TowerBeingOccupied(TowerColor.White, TowerColor.Pink, OccuptTime_Now / OccuptTime);
        }
        if (TowerStatus == TowerColor.White)
        {
            if (RealTimeColor == TowerColor.Blue)
                TowerBeingOccupied(TowerColor.White, TowerColor.Blue, OccuptTime_Now / OccuptTime);
            if (RealTimeColor == TowerColor.Pink)
                TowerBeingOccupied(TowerColor.White, TowerColor.Pink, OccuptTime_Now / OccuptTime);

        }



    }

    public string GetTowerStatus()
    {
        if (TowerStatus == TowerColor.Blue)
            return "L";
        if (TowerStatus == TowerColor.Pink)
            return "R";
        else return null;
    }

    #region 距离判断函数
    bool IsCanOccupt(float Distance)
    {
        return CanOccuptDistance > Distance;
    }
    bool IsCanAttack(float Distance)
    {
        return CanAttackDistance > Distance;
    }
    float Distance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2);
    }
    #endregion

    #region 改变颜色和转动的函数
    void TowerChangeColor(TowerColor towerColor)
    {
        switch (towerColor)
        {
            case TowerColor.White:
                LastColor = Vector3.one;
                break;
            case TowerColor.Blue:
                LastColor = blue;
                break;
            case TowerColor.Pink:
                LastColor = pink;
                break;
            default:
                Debug.Log("没有输入正确的ENUM值");
                break;
        }
        Transform Shield = gameObject.transform.Find("Tower_Base/Shield_Big");
        if (Shield == null)
        {
            Debug.Log("你选错了GameObject");
            return;
        }
        NowMaterial = Shield.GetComponent<MeshRenderer>().material;
        FristColor = new Vector3(NowMaterial.color.r, NowMaterial.color.g, NowMaterial.color.b);
        _Tower = gameObject.transform;
        _FristRotation = _Tower.localRotation;

        if (towerColor == TowerColor.Blue)
        {

            TargetTower = RightTower;
            _LastAngle = LastAngle(_Tower.position, TargetTower.position);
            LastRotation = new Vector3(0, _LastAngle, 0);
        }
        else if (towerColor == TowerColor.Pink)
        {
            TargetTower = LeftTower;
            _LastAngle = LastAngle(_Tower.position, TargetTower.position);
            LastRotation = new Vector3(0, _LastAngle, 0);
        }
        else
        {
            TargetTower = null;
            LastRotation = Vector3.zero;
        }

        IsChangeColor = true;
        TheTimeRotationRequire_Now = 0;
    }

    float LastAngle(Vector3 _from, Vector3 _to)
    {
        Vector3 from_to = _to - _from;
        from_to.Normalize();
        float Angle = Mathf.Atan2(from_to.x, from_to.z) * Mathf.Rad2Deg;
        return Angle;
    }

    void TowerBeingOccupied(TowerColor FromColor, TowerColor ToColor, float t)
    {
        Vector3 fromColor = Vector3.zero, toColor = Vector3.zero, nowColor = Vector3.zero;
        switch (FromColor)
        {
            case TowerColor.White:
                fromColor = Vector3.one;
                break;
            case TowerColor.Blue:
                fromColor = blue;
                break;
            case TowerColor.Pink:
                fromColor = pink;
                break;
            default:
                Debug.Log("FromColor没有输入正确的ENUM值");
                break;
        }
        switch (ToColor)
        {
            case TowerColor.White:
                toColor = Vector3.one;
                break;
            case TowerColor.Blue:
                toColor = blue;
                break;
            case TowerColor.Pink:
                toColor = pink;
                break;
            default:
                Debug.Log("ToColor没有输入正确的ENUM值");
                break;
        }
        nowColor = Vector3.Slerp(fromColor, toColor, t);
        Transform Tower_Top_Deco = gameObject.transform.Find("Tower_Top/Tower_Top_Deco");
        Tower_Top_Deco.GetComponent<MeshRenderer>().material.color = new Color(nowColor.x, nowColor.y, nowColor.z);

        if(FromColor== TowerColor.White)
        {
            if(ToColor== TowerColor.Blue&&RedCircle.enabled==true)
            {
                RedCircle.enabled = false;
                BlueCircle.enabled = true;
                NowCircle = BlueCircle;
            }
            if (ToColor == TowerColor.Pink && BlueCircle.enabled == true)
            {
                RedCircle.enabled = true;
                BlueCircle.enabled = false;
                NowCircle = RedCircle;

            }
        }
        else
        {
            if(FromColor== TowerColor.Blue)
            {
                RedCircle.enabled = false;
                BlueCircle.enabled = true;
                NowCircle = BlueCircle;
            }
            else
            {
                RedCircle.enabled = true;
                BlueCircle.enabled = false;
                NowCircle = RedCircle;
            }
        }
        if (NowCircle != null)
        {
            NowCircle.fillAmount = OccuptTime_Now / OccuptTime;
            NowCircle.color=new Color(NowCircle.color.r, NowCircle.color.g, NowCircle.color.b, OccuptTime_Now / OccuptTime);
        }



        //number.transform.parent.rotation = Quaternion.Euler(-90, 0, -180);
        //number.color = new Color(nowColor.x, nowColor.y, nowColor.z);
        //number.text = (OccuptTime_Now * 100 / OccuptTime).ToString("#0");
        //if (OccuptTime_Now * 100 / OccuptTime < 1.3f)
        //    number.text = "0";
        //if (number.text == "100")
        //{
        //    number.text = "99";
        //}

    }

    void RatationTower()
    {
        if (IsChangeColor)
        {
            if (TheTimeRotationRequire_Now <= TheTimeRotationRequire)
            {
                TheTimeRotationRequire_Now += Time.deltaTime;
                NowColor = Vector3.Slerp(FristColor, LastColor, TheTimeRotationRequire_Now / TheTimeRotationRequire);
                _NowRotation = Quaternion.Slerp(_FristRotation, Quaternion.Euler(LastRotation), TheTimeRotationRequire_Now / TheTimeRotationRequire);
                NowMaterial.color = new Color(NowColor.x, NowColor.y, NowColor.z);
                _Tower.transform.localRotation = _NowRotation;
            }
            if (TheTimeRotationRequire_Now > TheTimeRotationRequire) IsChangeColor = false;
        }
    }
    #endregion

    public void reset()
    {
        TowerStatus = TowerColor.White;
        BeforeStatus = TowerColor.White;
        RealTimeStatus = TowerColor.White;
        RealTimeColor = TowerColor.White;
        TowerChangeColor(TowerColor.White);
        OccuptTime_Now = 0;
        TowerBeingOccupied(TowerColor.White, TowerColor.White, 0);
        
    }
}
