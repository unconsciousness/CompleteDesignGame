//using UnityEngine;

//public enum ENUM_Soldier
//{
//    Null = 0,
//    Rookie = 1, // 新兵
//    Sergeant = 2,   // 中士
//    Captain = 3,    // 上尉    
//    Max,
//}
//public abstract class ISoldier
//{
//    protected string m_Name = "";               // 名稱

//    protected GameObject m_GameObject = null;   // 顯示的Uniyt模型
//    protected UnityEngine.AI.NavMeshAgent m_NavAgent = null;    // 控制角色移動使用
//    protected AudioSource m_Audio = null;

//    protected string m_IconSpriteName = ""; // 顯示Ico
//    protected string m_AssetName = "";      // 模型名稱
//    protected int m_AttrID = 0;         // 使用的角色屬性編號

//    protected bool m_bKilled = false;           // 是否陣亡
//    protected bool m_bCheckKilled = false;      // 是否確認過陣亡事件
//    protected float m_RemoveTimer = 1.5f;       // 陣亡後多久移除
//    protected bool m_bCanRemove = false;        // 是否可以移除

//    private IWeapon m_Weapon = null;            // 使用的武器
//    protected ICharacterAttr m_Attribute = null;// 數值
//    protected ICharacterAI m_AI = null;			// AI

//    protected ENUM_Soldier m_emSoldier = ENUM_Soldier.Null;
//    protected int m_MedalCount = 0;                 // 勳章數
//    protected const int MAX_MEDAL = 3;                  // 最多勳章數 
//    protected const int MEDAL_VALUE_ID = 20;				// 勳章數值起始值

//    public ISoldier() { }


//    // 設定Unity模型
//    public void SetGameObject(GameObject theGameObject)
//    {
//        m_GameObject = theGameObject;
//        m_NavAgent = m_GameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
//        m_Audio = m_GameObject.GetComponent<AudioSource>();
//    }

//    // 取得Unity模型
//    public GameObject GetGameObject()
//    {
//        return m_GameObject;
//    }

//    // 釋放
//    public void Release()
//    {
//        if (m_GameObject != null)
//            GameObject.Destroy(m_GameObject);
//    }

//    // 名稱
//    public string GetName()
//    {
//        return m_Name;
//    }

//    // 取得Asset名稱
//    public string GetAssetName()
//    {
//        return m_AssetName;
//    }

//    // 取得Icon名稱
//    public string GetIconSpriteName()
//    {
//        return m_IconSpriteName;
//    }

//    // 取得屬性ID
//    public int GetAttrID()
//    {
//        return m_AttrID;
//    }

//    // 取得目前的值
//    public ICharacterAttr GetCharacterAttr()
//    {
//        return m_Attribute;
//    }

//    // 取得角色名稱
//    public string GetCharacterName()
//    {
//        return m_Name;
//    }

//    // 更新
//    public void Update()
//    {
//        if (m_bKilled)
//        {
//            m_RemoveTimer -= Time.deltaTime;
//            if (m_RemoveTimer <= 0)
//                m_bCanRemove = true;
//        }

//        m_Weapon.Update();
//    }

//    #region 移動及位置	
//    // 移動到目標
//    public void MoveTo(Vector3 Position)
//    {
//        m_NavAgent.isStopped = false;
//        m_NavAgent.SetDestination(Position);
//    }

//    // 停止移動
//    public void StopMove()
//    {
//        m_NavAgent.isStopped = true;
//    }

//    //  取得位置
//    public Vector3 GetPosition()
//    {
//        return m_GameObject.transform.position;
//    }
//    #endregion

//    #region AI
//    // 設定AI
//    public void SetAI(ISoldierAI CharacterAI)
//    {
//        m_AI = CharacterAI;
//    }

//    // 更新AI
//    public void UpdateAI(List<ISoldier> Targets)
//    {
//        m_AI.Update(Targets);
//    }

//    // 通知AI有角色被移除
//    public void RemoveAITarget(ISoldier Targets)
//    {
//        m_AI.RemoveAITarget(Targets);
//    }
//    #endregion

//    #region 攻擊
//    // 攻擊目標
//    public void Attack(ISoldier Target)
//    {
//        // 設定武器額外攻擊加乘
//        SetWeaponAtkPlusValue(m_Attribute.GetAtkPlusValue());

//        // 攻擊
//        WeaponAttackTarget(Target);

//        // 攻擊動作
//        m_GameObject.GetComponent<CharacterMovement>().PlayAttackAnim();

//        // 面向目標
//        m_GameObject.transform.forward = Target.GetPosition() - GetPosition();
//    }

//    // 被其他角色攻擊
//    public abstract void UnderAttack(ISoldier Attacker);
//    #endregion

//    #region 武器
//    // 設定使用的武器
//    public void SetWeapon(IWeapon Weapon)
//    {
//        if (m_Weapon != null)
//            m_Weapon.Release();
//        m_Weapon = Weapon;

//        // 設定武器擁有者
//        m_Weapon.SetOwner(this);

//        // 設定Unity GameObject的層級
//        UnityTool.AttachToRefPos(m_GameObject, m_Weapon.GetGameObject(), "weapon-point", Vector3.zero);
//    }

//    // 取得武器
//    public IWeapon GetWeapon()
//    {
//        return m_Weapon;
//    }

//    // 設定額外攻擊力
//    protected void SetWeaponAtkPlusValue(int Value)
//    {
//        m_Weapon.SetAtkPlusValue(Value);
//    }

//    // 武器攻擊目標
//    protected void WeaponAttackTarget(ISoldier Target)
//    {
//        m_Weapon.Fire(Target);
//    }

//    // 計算攻擊力
//    public int GetAtkValue()
//    {
//        // 武器攻擊力 + 角色數值的加乘
//        return m_Weapon.GetAtkValue();
//    }

//    // 取得攻擊距離
//    public float GetAttackRange()
//    {
//        return m_Weapon.GetAtkRange();
//    }
//    #endregion

//    #region 陣亡及移除
//    // 陣亡
//    public void Killed()
//    {
//        if (m_bKilled == true)
//            return;
//        m_bKilled = true;
//        m_bCheckKilled = false;
//    }

//    // 是否陣亡
//    public bool IsKilled()
//    {
//        return m_bKilled;
//    }

//    // 是否確認陣亡過
//    public bool CheckKilledEvent()
//    {
//        if (m_bCheckKilled)
//            return true;
//        m_bCheckKilled = true;
//        return false;
//    }

//    //  是否可以移除
//    public bool CanRemove()
//    {
//        return m_bCanRemove;
//    }
//    #endregion

//    #region 角色數值
//    // 設定角色數值
//    public virtual void SetCharacterAttr(ISoldierAttr CharacterAttr)
//    {
//        // 設定
//        m_Attribute = CharacterAttr;
//        m_Attribute.InitAttr();

//        // 設定移動速度
//        m_NavAgent.speed = m_Attribute.GetMoveSpeed();
//        //Debug.Log ("設定移動速度:"+m_NavAgent.speed);

//        // 名稱
//        m_Name = m_Attribute.GetAttrName();
//    }
//    #endregion

//    #region 音效特效

//    // 播放音效
//    public void PlaySound(string ClipName)
//    {
//        //  取得音效
//        IAssetFactory Factory = PBDFactory.GetAssetFactory();
//        AudioClip theClip = Factory.LoadAudioClip(ClipName);
//        if (theClip == null)
//            return;
//        m_Audio.clip = theClip;
//        m_Audio.Play();
//    }

//    // 播放特效
//    public void PlayEffect(string EffectName)
//    {
//        //  取得特效
//        IAssetFactory Factory = PBDFactory.GetAssetFactory();
//        GameObject EffectObj = Factory.LoadEffect(EffectName);
//        if (EffectObj == null)
//            return;

//        // 加入
//        UnityTool.Attach(m_GameObject, EffectObj, Vector3.zero);
//    }
//    #endregion

//    // 執行Visitor
//    public virtual void RunVisitor(ISoldierVisitor Visitor)
//    {
//        Visitor.VisitCharacter(this);
//    }
//}