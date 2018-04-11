using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SoldierAI : MonoBehaviour
{

    NavMeshAgent nav = null;
    Animation ani = null;
    ParticleSystem WeaponEffect=null;
    Transform tra = null;
    Transform enemyTower = null;
    SoldierAI target = null;
    GameObject HPCanvas = null;
    Slider HP = null;
    Text AttributePanel = null;//属性面板
    public float CanSeeDistance = 5f;
    public float AttackDistance = 3.8f;//这个可以升级武器时增加！
    float HP_MAX, HP_NOW, Attack;//血量，攻击力
    public bool NeedToDie = false;
    public float MoveSpeed = 2f;

    static float Attack_Time = 1f;//每次攻击的间隔时间
    float Attack_Time_Now = Attack_Time/5;



    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        ani = GetComponent<Animation>();
        WeaponEffect = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/weapon-point/Effect").GetComponent<ParticleSystem>();
        SetAniSpeed();
        tra = GetComponent<Transform>();
        SetEnemyTower();//设置敌方基地塔
        SetHPSlider();
        transform.localRotation = Quaternion.LookRotation(enemyTower.position - transform.position);
    }


    void Update()
    {
        if (!MainGame.isPlay)
            return;
        if (MainGame.isPause)
        {
            if (ani["stand"].speed == 1)
            {
                ani["stand"].speed = 0;
                ani["move"].speed = 0;
                ani["attack"].speed = 0;
            }
            return;
        }
        else
        {
            if (ani["stand"].speed ==0)            
                SetAniSpeed();        
        }
           
        UpdateHPSlider();
    }
    private void FixedUpdate()
    {
        if (!MainGame.isPlay)
            return;
        if (MainGame.isPause)
            return;
        AttackTarget();
    }



    void SetAniSpeed()
    {
        ani["stand"].speed = 1;
        ani["move"].speed = MoveSpeed * 0.4f;
        ani["attack"].speed = Attack_Time * 0.8f;

    }//设置移动速度会同时修改动作的播放速度

    void SetRotation(Transform target = null)//保持站立并转身
    {
        Vector3 t;
        if (target == null)
            t = Quaternion.LookRotation(enemyTower.position - transform.position).eulerAngles;
        else
            t = Quaternion.LookRotation(target.position - transform.position).eulerAngles;
        t = new Vector3(0, t.y, 0);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(t), 1);
    }

    public void SetHPAndAttack(float HP, float AttackPower)
    {
        HP_MAX = HP;
        HP_NOW = HP_MAX;
        Attack = AttackPower;

    }

    public void BeAttack(float theHP)
    {
       // Debug.Log("BeAttack: " + Time.time);
        if (HP_NOW > theHP) HP_NOW -= theHP;
        else
        {
            if (!MainGame.CharacterSystem.DeleteLeftList(this))
                MainGame.CharacterSystem.DeleteRightList(this);
            Destroy(gameObject);
        }

    }

    public void AttackTarget()
    {




        float MinDistance = 999;
        SoldierAI _target = null;
        if (gameObject.tag == "Left_Soldier")
        {
            foreach (var Enemy in MainGame.CharacterSystem.GetRightList())
            {
                float t = MinDistance;
                MinDistance = Mathf.Min(MinDistance, Distance(gameObject.transform.position, Enemy.transform.position));
                if (t != MinDistance)
                    _target = Enemy;
            }

        }
        else
        {
            foreach (var Enemy in MainGame.CharacterSystem.GetLeftList())
            {
                float t = MinDistance;
                MinDistance = Mathf.Min(MinDistance, Distance(gameObject.transform.position, Enemy.transform.position));
                if (t != MinDistance)
                    _target = Enemy;
            }
        }
        if (IsCanSee(MinDistance))
        {
            SetRotation(_target.transform);//看着目标
            if (IsCanAttack(MinDistance))
            {
                ani.Play("attack");
               
                Attack_Time_Now -= Time.fixedDeltaTime;
                if (Attack_Time_Now > Time.fixedDeltaTime)
                    return;
                WeaponEffect.Stop();
                WeaponEffect.Play();
                Attack_Time_Now = Attack_Time;
                _target.BeAttack(Attack);//攻击
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, _target.transform.position, MoveSpeed * Time.deltaTime);//朝Target移动
                ani.Play("move");
            }

        }
        else if (Distance(enemyTower.parent.Find("Tower_Far").position, transform.position) < NeutralTowerControl.CanOccuptDistance &&
            enemyTower.parent.Find("Tower_Far").GetComponent<NeutralTowerControl>().GetTowerStatus() != tag.Substring(0, 1) &&
            enemyTower.parent.Find("Tower_Far").GetComponent<NeutralTowerControl>().RealTimeStatus != TowerColor.White)
        {
            ani.Play("stand");
            SetRotation(enemyTower.parent.Find("Tower_Far"));
        }
        else if (Distance(enemyTower.parent.Find("Tower_Near").position, transform.position) < NeutralTowerControl.CanOccuptDistance &&
            enemyTower.parent.Find("Tower_Near").GetComponent<NeutralTowerControl>().GetTowerStatus() != tag.Substring(0, 1) &&
            enemyTower.parent.Find("Tower_Near").GetComponent<NeutralTowerControl>().RealTimeStatus != TowerColor.White)
        {
            ani.Play("stand");
            SetRotation(enemyTower.parent.Find("Tower_Near"));
            // Debug.Log(tag.Substring(0, 4));
        }
        else if (!IsCanAttack(Distance(transform.position, enemyTower.transform.position)))
        {
            SetRotation();//看着基地塔
            transform.position = Vector3.MoveTowards(transform.position, enemyTower.position, MoveSpeed * Time.deltaTime);//朝基地塔移动
            ani.Play("move");
        }
        else
        {
            //攻击基地塔
            ani.Play("attack");
            
            Attack_Time_Now -= Time.deltaTime;
            if (Attack_Time_Now > 0)
                return;
            WeaponEffect.Stop();
            WeaponEffect.Play();
            Attack_Time_Now = Attack_Time;
            bool CanBeAttack = enemyTower.GetComponent<APSystem>().BeAttack(Attack);//攻击
            if (!CanBeAttack)
            {
                if (MainGame.isPlay)
                {
                    //游戏结束
                    MainGame.isNeedReset = true;
                }

            }
        }
    }








    //基地塔的逻辑还没有写
    //子弹效果还没有





    /// <summary>
    /// 设置士兵血条
    /// </summary>
    public void SetHPSlider()
    {

        HPCanvas = Instantiate((GameObject)Resources.Load("Prefabs/HP_Soldier"));
        HPCanvas.transform.SetParent(gameObject.transform);


        HP = HPCanvas.transform.Find("Slider").GetComponent<Slider>();

        HPCanvas.transform.localPosition = new Vector3(0, 1.58f, 0);
        HP.transform.rotation = Quaternion.Euler(-136.8f, 180, 180);
        if (tag == "Left_Soldier") HP.transform.Find("Fill Area/Fill").GetComponent<Image>().color = new Color(0f, 210f / 255f, 1f, 118 / 255f);

        AttributePanel = HPCanvas.transform.Find("Slider/Text").GetComponent<Text>();
        AttributePanel.text = "最大HP： " + HP_MAX + "\n当前HP： " + HP_NOW + "\n攻击力： " + Attack;
    }

    void UpdateHPSlider()
    {
        HP.transform.rotation = Quaternion.Euler(-136.8f, 180, 180);
        if (HP.value == HP_NOW)
            return;
        HP.value = HP_NOW;
        AttributePanel.text = "最大HP： " + HP_MAX + "\n当前HP： " + HP_NOW + "\n攻击力： " + Attack;
    }



    void SetEnemyTower()
    {
        string TowerUrl = "Ground/Tower/Tower_";
        if (tag == "Left_Soldier")
        {
            enemyTower = GameObject.Find(TowerUrl + "Right").transform;
        }
        else
        {
            enemyTower = GameObject.Find(TowerUrl + "Left").transform;
        }
        // Debug.Log("enemyTower:" + enemyTower.name);
    }




    bool IsCanSee(float Distance)
    {
        return CanSeeDistance > Distance;
    }
    bool IsCanAttack(float Distance)
    {
        return AttackDistance > Distance;
    }
    float Distance(Vector3 v1, Vector3 v2)
    {
        return Vector3.Distance(v1, v2);
    }
}
