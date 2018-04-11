using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
public enum EnumWeapon
{
    Null = 0,
    Gun = 1,
    Rifle = 2,
    Rocket = 3
}
public enum EnumSoldier
{
    Null = 0,
    Sergeant = 1,
    Rookie = 2,
    Captain = 3,
    Elf = 4,
    Troll = 5,
    Ogre = 6

}






public class SoldierFactory : MonoBehaviour
{
    //Left&Right_Tower_APSystem
    public APSystem Left_APS, Right_APS;


    //Left Soldier
    public GameObject Sergeant, Rookie, Captain;
    //Right Soldier
    public GameObject Elf, Troll, Ogre;
    //Weapon
    public GameObject Gun, Rifle, Rocket;
    //WeaponEffect
    public GameObject Effect;
    //List Of Weapon
    public Dictionary<EnumSoldier, EnumWeapon> enumWeapons = new Dictionary<EnumSoldier, EnumWeapon>();
    public Dictionary<EnumSoldier, int> Cost = new Dictionary<EnumSoldier, int>();
    //HP Array
    static int[] _HPs = { 0, 80, 90, 100, 80, 90, 100 };
    public int[] HPs = { 0, 80, 90, 100, 80, 90, 100 };
    //AttackPower
    static int[] _WeaponPowers = { 0, 10, 12, 15 };
    public int[] WeaponPowers = { 0, 10, 12, 15 };
    //Cost Text
    public Text[] CostTexts = new Text[6];


    //Upgrade Cost
    float UpCost = 50;
    float UpAddCost = 5;

    //Weapon Instance Config
    List<GameObject> WeaponsModel = null;
    string WeaponLocation = "Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Spine1/Bip001 Spine2/Bip001 Neck/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/weapon-point";
    Vector3 WeaponLocalPosition = new Vector3(1.045f, -0.227f, -0.539f);
    Quaternion WeaponLocalRotation = Quaternion.Euler(90, 0, 0);
    //Weapon Upgrade
    string WeaponUpgradeButton = "Table/ThingOnTable/UpgradeWeaponButton/Button";

    private void Start()
    {
        Initialized();
    }






    //可以改成协程，士兵慢慢显性
    public GameObject TrainSoldier(EnumSoldier eSoldier, Vector3 Pos)
    {
        GameObject targetSoldier = null;
        switch (eSoldier)
        {
            case EnumSoldier.Null:
                Debug.Log("你的EnumSoldier没有赋值!");
                break;
            case EnumSoldier.Sergeant:
                targetSoldier = Sergeant;
                break;
            case EnumSoldier.Rookie:
                targetSoldier = Rookie;
                break;
            case EnumSoldier.Captain:
                targetSoldier = Captain;
                break;
            case EnumSoldier.Elf:
                targetSoldier = Elf;
                break;
            case EnumSoldier.Troll:
                targetSoldier = Troll;
                break;
            case EnumSoldier.Ogre:
                targetSoldier = Ogre;
                break;
            default:
                Debug.Log("传入TrainSoldier的EnumSoldier有问题！");
                return null;
        }
        GameObject targetWeapon = null;
        switch (enumWeapons[eSoldier])
        {
            case EnumWeapon.Null:
                Debug.Log("你的EnumWeapon没有赋值!");
                break;
            case EnumWeapon.Gun:
                targetWeapon = Gun;
                break;
            case EnumWeapon.Rifle:
                targetWeapon = Rifle;
                break;
            case EnumWeapon.Rocket:
                targetWeapon = Rocket;
                break;
            default:
                Debug.Log("传入TrainSoldier的EnumWeapon有问题！");
                return null;
        }


        //够钱，训练士兵
        if ((int)eSoldier <= 3)
        {
            if (Left_APS.CostAP(Cost[eSoldier]))
            {
                GameObject Soldier = Instantiate(targetSoldier, Pos, new Quaternion());
                GameObject Weapon = Instantiate(targetWeapon, new Vector3(), new Quaternion());             
                Weapon.transform.parent = Soldier.transform.Find(WeaponLocation);            
                Weapon.transform.localPosition = WeaponLocalPosition;
                Weapon.transform.localRotation = WeaponLocalRotation;

                GameObject WeaponEffect = Instantiate(Effect, new Vector3(), new Quaternion());
                WeaponEffect.transform.parent = Soldier.transform.Find(WeaponLocation);
                WeaponEffect.transform.localScale = new Vector3(0.2f, 0.2f,0.2f);
                WeaponEffect.transform.localPosition = new Vector3(0.002f, -0.991f, -0.147f);
                if(enumWeapons[eSoldier]== EnumWeapon.Rocket)
                    WeaponEffect.transform.localPosition = new Vector3(-0.014f, -1.321f, -0.147f);
                WeaponEffect.transform.localRotation =  Quaternion.Euler(92.79f,96.02f,96.94f);
                WeaponEffect.name = "Effect";
                //改Tag
                Soldier.tag = "Left_Soldier";
                //加入士兵脚本
                Soldier.AddComponent<NavMeshAgent>();
                //加入士兵脚本
                Soldier.AddComponent<SoldierAI>().SetHPAndAttack(HPs[(int)eSoldier], WeaponPowers[(int)enumWeapons[eSoldier]]);
                //改名字
                Soldier.name = eSoldier.ToString() + (MainGame.CharacterSystem.Left_Soldier_Number + 1).ToString();
                //把士兵加入管理器
                MainGame.CharacterSystem.AddLeftSoldier(Soldier.GetComponent<SoldierAI>());
                return Soldier;
            }
        }
        if ((int)eSoldier > 3)
        {
            if (Right_APS.CostAP(Cost[eSoldier]))
            {
                GameObject Soldier = Instantiate(targetSoldier, Pos, new Quaternion());
                GameObject Weapon = Instantiate(targetWeapon, Pos, new Quaternion());
                //装备武器
                Weapon.transform.parent = Soldier.transform.Find(WeaponLocation);
                Weapon.transform.localPosition = WeaponLocalPosition;
                Weapon.transform.localRotation = WeaponLocalRotation;

                GameObject WeaponEffect = Instantiate(Effect, new Vector3(), new Quaternion());
                WeaponEffect.transform.parent = Soldier.transform.Find(WeaponLocation);
                WeaponEffect.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                WeaponEffect.transform.localPosition = new Vector3(0.002f, -0.991f, -0.147f);
                WeaponEffect.transform.localRotation = Quaternion.Euler(92.79f, 96.02f, 96.94f);
                WeaponEffect.name = "Effect";
                //加入导航
                Soldier.AddComponent<NavMeshAgent>();
                //加入士兵脚本
                Soldier.AddComponent<SoldierAI>().SetHPAndAttack(HPs[(int)eSoldier], WeaponPowers[(int)enumWeapons[eSoldier]]);

                //改Tag
                Soldier.tag = "Right_Soldier";
                //改名字
                Soldier.name = eSoldier.ToString() + (MainGame.CharacterSystem.Right_Soldier_Number + 1).ToString();
                //把士兵加入管理器
                MainGame.CharacterSystem.AddRightSoldier(Soldier.GetComponent<SoldierAI>());
                return Soldier;
            }
        }
        //不够钱
        return null;


    }
    /// <summary>
    /// 升级武器
    /// </summary>
    public void UpgradeWeapon(EnumSoldier enumSoldier)
    {
        if (!MainGame.isPlay)
            return;
        if (MainGame.isPause)
            return;
        if (enumWeapons[enumSoldier] == EnumWeapon.Null)
        {
            // Debug.Log("UpgradeWeapon接受的参数有问题");
            return;
        }
        if ((int)enumWeapons[enumSoldier] < 3)
        {
            GameObject.Find("Camera").GetComponent<MainGame>().PickUpSetEnable(true);
            enumWeapons[enumSoldier] = (EnumWeapon)((int)enumWeapons[enumSoldier] + 1);
            if ((int)enumSoldier <= 3)
            {
                if (!Left_APS.CostAP(UpCost))
                {
                    GameObject.Find("Camera").GetComponent<MainGame>().PickUpSetEnable(false);
                    return;
                }
            }
            else
            {
                if (!Right_APS.CostAP(UpCost))
                {
                    GameObject.Find("Camera").GetComponent<MainGame>().PickUpSetEnable(false);
                    return;
                }
            }

            Cost[enumSoldier] += 5;
            CostTexts[(int)enumSoldier - 1].text = Cost[enumSoldier].ToString();


            foreach (var i in WeaponsModel)
            {
                if (i.name == enumWeapons[enumSoldier].ToString())
                {
                    GameObject WeaponModel = Instantiate(i, new Vector3(), new Quaternion());
                    GameObject WeaponPickUp = Instantiate(i, new Vector3(), new Quaternion());
                    WeaponModel.name = i.name;
                    WeaponPickUp.name = i.name;

                    GameObject SoldierModel = null;
                    GameObject SoldierPickUp = null;
                    if ((int)enumSoldier <= 3)
                    {
                        SoldierModel = GameObject.Find("Table/ThingOnTable/Left_Soldier/" + enumSoldier.ToString());
                        SoldierPickUp = GameObject.Find("Table/ThingOnTable/Left_Soldier/" + enumSoldier.ToString() + "_Pick_Up");
                    }
                    else
                    {
                        SoldierModel = GameObject.Find("Table/ThingOnTable/Right_Soldier/" + enumSoldier.ToString());
                        SoldierPickUp = GameObject.Find("Table/ThingOnTable/Right_Soldier/" + enumSoldier.ToString() + "_Pick_Up");
                    }

                    Destroy(SoldierModel.transform.Find(WeaponLocation + "/" + ((EnumWeapon)((int)enumWeapons[enumSoldier] - 1)).ToString()).gameObject);
                    Destroy(SoldierPickUp.transform.Find(WeaponLocation + "/" + ((EnumWeapon)((int)enumWeapons[enumSoldier] - 1)).ToString()).gameObject);
                    SoldierModel.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                    SoldierPickUp.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
                    WeaponModel.transform.parent = SoldierModel.transform.Find(WeaponLocation);
                    WeaponModel.transform.localPosition = WeaponLocalPosition;
                    WeaponModel.transform.localRotation = WeaponLocalRotation;
                    WeaponPickUp.transform.parent = SoldierPickUp.transform.Find(WeaponLocation);
                    WeaponPickUp.transform.localPosition = WeaponLocalPosition;
                    WeaponPickUp.transform.localRotation = WeaponLocalRotation;
                    SoldierModel.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                    SoldierPickUp.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);
                }
            }//升级武器时，把Model和PickUp上的武器更换
            GameObject.Find("Camera").GetComponent<MainGame>().PickUpSetEnable(false);
            UpgradeHPAndAttackPower(enumSoldier);
            ///SetHPAndAttackPower(enumSoldier);
            //Debug.Log("升级了" + enumSoldier.ToString() + "的武器，现在是" + enumWeapons[enumSoldier]);
            return;
        }
        // Debug.Log(enumSoldier.ToString() + "的武器已经满级了！");


    }





    /// <summary>
    /// 升级增加10HP和3攻击力
    /// </summary>
    void UpgradeHPAndAttackPower(EnumSoldier enumSoldier)
    {
        HPs[(int)enumSoldier] += 10;
    }
    public void reset()
    {
        WeaponPowers = (int[])_WeaponPowers.Clone();
        HPs = (int[])_HPs.Clone();
        Initialized();
    }
    private void Initialized()
    {
        enumWeapons.Clear();
        enumWeapons.Add(EnumSoldier.Sergeant, EnumWeapon.Gun);
        enumWeapons.Add(EnumSoldier.Rookie, EnumWeapon.Gun);
        enumWeapons.Add(EnumSoldier.Captain, EnumWeapon.Gun);
        enumWeapons.Add(EnumSoldier.Elf, EnumWeapon.Gun);
        enumWeapons.Add(EnumSoldier.Troll, EnumWeapon.Gun);
        enumWeapons.Add(EnumSoldier.Ogre, EnumWeapon.Gun);
        Cost.Clear();
        Cost.Add(EnumSoldier.Sergeant, 10);
        Cost.Add(EnumSoldier.Rookie, 12);
        Cost.Add(EnumSoldier.Captain, 15);
        Cost.Add(EnumSoldier.Elf, 10);
        Cost.Add(EnumSoldier.Troll, 12);
        Cost.Add(EnumSoldier.Ogre, 15);
        WeaponsModel = new List<GameObject> { Gun, Rifle, Rocket };
        for (int i = 0; i < CostTexts.Length; i++)
        {
            CostTexts[i].text = Cost[(EnumSoldier)(i + 1)].ToString();
        }
    }
}
