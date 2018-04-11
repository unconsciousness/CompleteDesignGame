using UnityEngine;
using System.Collections.Generic;
using System;

public class CharacterSystem
{
    private static List<SoldierAI> m_LeftTeam = new List<SoldierAI>();
    private static List<SoldierAI> m_RightTeam = new List<SoldierAI>();
    private int left_Soldier_Number=0;
    private int right_Soldier_Number = 0;

    public int Left_Soldier_Number
    {
        get
        {
            return left_Soldier_Number;
        }
        private set
        {
            left_Soldier_Number = value;
        }
    }

    public int Right_Soldier_Number
    {
        get
        {
            return right_Soldier_Number;
        }
        private set
        {
            right_Soldier_Number = value;
        }
    }

    public void AddLeftSoldier(SoldierAI soldier)
    {
        m_LeftTeam.Add(soldier);
        left_Soldier_Number++;
    }
    public void AddRightSoldier(SoldierAI soldier)
    {
        m_RightTeam.Add(soldier);
        Right_Soldier_Number++;
    }
   
     
    public List<SoldierAI> GetLeftList()
    {
        return m_LeftTeam;
    }
    public List<SoldierAI> GetRightList()
    {
        return m_RightTeam;
    }


    public bool DeleteLeftList(SoldierAI soldier)
    {
        bool b = m_LeftTeam.Remove(soldier);
        return b;
    }
    public bool DeleteRightList(SoldierAI soldier)
    {
        bool b = m_RightTeam.Remove(soldier);
        return b;
    }

    public bool HaveLeftList(SoldierAI soldier)
    {
        return m_LeftTeam.Contains(soldier);
    }

    public void ClearAllSoldier()
    {
        m_LeftTeam.Clear();
        m_RightTeam.Clear();
        left_Soldier_Number = 0;
        right_Soldier_Number = 0;
    }
}

