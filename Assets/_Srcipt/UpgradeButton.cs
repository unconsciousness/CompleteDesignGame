using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpgradeButton : MonoBehaviour {


    void Start() {
        GetComponent<Button>().onClick.AddListener(delegate{Camera.main.GetComponent<SoldierFactory>().UpgradeWeapon((EnumSoldier)int.Parse( name.Substring(name.Length - 1, 1))); });
	}
}
