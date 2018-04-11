using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct SoldierAttribute
{
    float MaxHP;
    float AttackPower;
    float AttackRadius;
}





public class MainGame : MonoBehaviour
{

    public static CharacterSystem CharacterSystem = new CharacterSystem();

    public SoldierFactory SF;
    public static Text Log;
    public NeutralTowerControl Tower_Near;
    public NeutralTowerControl Tower_Far;
    public APSystem Tower_Left;
    public APSystem Tower_Right;

    GameObject SelectModel = null;
    EnumSoldier EnumSelectSoldier = EnumSoldier.Null;
    EnumWeapon EnumSelectWeapon = EnumWeapon.Null;
    string SelectSoldierName = "";
    GameObject soldier_pick_up = null;
    public Transform Left_MagicArray = null;
    public Transform Right_MagicArray = null;

    public static bool isPlay = false;
    public static bool isNeedReset = false;
    public static bool isPause = false;

    //Mouse
    bool MouseDown = false;
    //Mouse

    //BirthPosition
    float BirthLocation_Mid = 2;//允许生成士兵的点距离中心线的距离

    void Start()
    {
        Log = GameObject.Find("Table/ThingOnTable/GameName/Log").GetComponent<Text>();
        PickUpSetEnable(false);
    }




    void Update()
    {

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            isPlay = true;
        }


        if (isPlay)
        {
            InputProcess();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            reset();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPause = !isPause;
        }





    }




    private void InputProcess()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length == 0) return;

        if (!isPlay)
            return;

        if (Input.GetMouseButtonDown(0))
        {

            // 走訪每一個被Hit到的GameObject
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.gameObject.name == "StartGame")
                {
                    //开始游戏
                    isPlay = true;
                    //UI close
                    return;
                }
                if (hit.transform.gameObject.name == "PauseGame")
                {
                    //暂停游戏
                    isPause = !isPause;
                    //UI open
                    return;
                }
                if (hit.transform.gameObject.name == "ResetGame")
                {
                    //重开游戏
                    reset();
                    return;
                }

                if (isPause)
                    return;
                if (hit.transform.parent != null)
                {
                    if (hit.transform.parent.gameObject.tag == "Left_Soldier_Model")
                    {
                        SelectModel = hit.transform.parent.gameObject;

                        Vector3 Model_Pos = SelectModel.transform.position;
                        Left_MagicArray.position = new Vector3(Model_Pos.x, Left_MagicArray.position.y, Model_Pos.z);//魔法阵位置切到点击的model
                        SelectSoldierName = SelectModel.name + "_Pick_Up";
                        soldier_pick_up = GameObject.Find("Table/ThingOnTable/Left_Soldier/" + SelectSoldierName);
                        MouseDown = true;
                    }
                    if (hit.transform.parent.gameObject.tag == "Right_Soldier_Model")
                    {
                        SelectModel = hit.transform.parent.gameObject;
                        Vector3 pos = SelectModel.transform.position;
                        Right_MagicArray.position = new Vector3(pos.x, Right_MagicArray.position.y, pos.z);
                        SelectSoldierName = SelectModel.name + "_Pick_Up";
                        soldier_pick_up = GameObject.Find("Table/ThingOnTable/Right_Soldier/" + SelectSoldierName);
                        MouseDown = true;

                    }
                }

            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            MouseDown = false;
            if (soldier_pick_up != null && SelectModel != null)
            {
                //z:8~22
                float _z = soldier_pick_up.transform.position.z;
                if (_z > 8.0f && _z < 22.0f)
                {
                    SF.TrainSoldier((EnumSoldier)Enum.Parse(typeof(EnumSoldier), SelectModel.name), soldier_pick_up.transform.position);
                }
                foreach (var i in soldier_pick_up.GetComponentsInChildren<SkinnedMeshRenderer>())
                    i.enabled = true;

                soldier_pick_up.transform.position = SelectModel.transform.position;
            }


        }


        if (Input.GetMouseButton(0))
        {

            if (hits.Length == 0)
                return;
            if (MouseDown)
            {

                foreach (var hit in hits)
                {

                    //中间线x=15;
                    if (hit.transform.gameObject.name == "Wall" || hit.transform.gameObject.name == "Ground")
                    {
                        Vector3 _Pos = hit.point - 0.5943662f * new Vector3(ray.direction.x, 0, ray.direction.z);
                        foreach (var i in soldier_pick_up.GetComponentsInChildren<SkinnedMeshRenderer>())
                            i.enabled = true;


                        if (SelectModel.tag == "Left_Soldier_Model")
                        {
                            _Pos = new Vector3(Mathf.Max(_Pos.x, 15 + BirthLocation_Mid), _Pos.y, _Pos.z);
                        }
                        else
                            _Pos = new Vector3(Mathf.Min(_Pos.x, 15 - BirthLocation_Mid), _Pos.y, _Pos.z);
                        soldier_pick_up.transform.position = _Pos;
                        Log.text = _Pos.ToString();
                        break;
                    }

                }
            }




        }





    }

    public void PickUpSetEnable(bool enable)
    {
        string Url1 = "Table/ThingOnTable/Left_Soldier";
        string Url2 = "Table/ThingOnTable/Right_Soldier";
        List<Transform> GOs = new List<Transform>();
        GameObject left = GameObject.Find(Url1);
        GameObject right = GameObject.Find(Url2);

        foreach (Transform i in left.transform)//父物体的表层子物体        
            if (i.name.Contains("Up"))
                GOs.Add(i);
        foreach (Transform i in right.transform)//父物体的表层子物体        
            if (i.name.Contains("Up"))
                GOs.Add(i);

        foreach (var i in GOs)
            foreach (var j in i.GetComponentsInChildren<SkinnedMeshRenderer>())
                j.enabled = enable;


    }

    public void reset()
    {
        isPlay = false;
        SF.reset();
        Tower_Near.reset();
        Tower_Far.reset();
        Tower_Left.ResetGame();
        Tower_Right.ResetGame();

        var LeftList = CharacterSystem.GetLeftList();
        var RightList = CharacterSystem.GetRightList();
        while (LeftList.Count > 0)
        {
            var j = LeftList[0];
            LeftList.Remove(LeftList[0]);
            Destroy(j.gameObject);
        }

        while (RightList.Count > 0)
        {
            var j = RightList[0];
            RightList.Remove(RightList[0]);
            Destroy(j.gameObject);
        }



        CharacterSystem.ClearAllSoldier();
    }
}
