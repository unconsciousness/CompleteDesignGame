using UnityEngine;
using UnityEngine.UI;

public class APSystem : MonoBehaviour
{



    //public Text Tower_HP_Text = null;
    public Slider Tower_HP_Slider = null;
    static float MAXTowerHP = 500f;//Tower_HP
    float NowTowerHP;
    void Start()
    {
        Tower_HP_Slider.maxValue = MAXTowerHP;
        Tower_HP_Slider.value = MAXTowerHP;
        NowTowerHP = MAXTowerHP;
    }





    //
    public Slider MoneySlider = null;
    public Text Money = null;
    const float MAX_AP = 100f;
    const float AP_CD = 0.2f;
    const float Add_AP = 1f;
    float Now_AP = 20;
    float Now_AP_CD = AP_CD;
    //


    public void Update()
    {

        if (!MainGame.isPlay)
            return;
        if (MainGame.isPause)
            return;
        Now_AP_CD -= Time.deltaTime;
            if (Now_AP_CD > 0)
                return;
            Now_AP_CD = AP_CD;
            Now_AP += Add_AP;
            Now_AP = Mathf.Min(Now_AP, MAX_AP);
            MoneySlider.value = Now_AP;
            Money.text = Now_AP.ToString("#0");
        

    }

    public bool CostAP(float AP)
    {
        if (AP > Now_AP)
            return false;
        Now_AP -= AP;
        return true;
    }

    public bool BeAttack(float AttackPower)
    {
        if (AttackPower > NowTowerHP)
            return false;
        NowTowerHP -= AttackPower;
        UpdateHPSlider();
        return true;
    }

    void UpdateHPSlider()
    {
        Tower_HP_Slider.value = NowTowerHP;
    }

    public void ResetGame()
    {
        Tower_HP_Slider.maxValue = MAXTowerHP;
        Tower_HP_Slider.value = MAXTowerHP;
        NowTowerHP = MAXTowerHP;
        Now_AP = 100;
    }
}