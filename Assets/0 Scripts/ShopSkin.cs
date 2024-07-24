using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class ShopSkin : MonoBehaviour {
    [Header("_____________________________ShopSkin_________________________")]
    public const string UNEQUIP = "Unequip";
    public const int COST_SKIN = 500;
    public const int COST_SET_SKIN = 2500;
    public int idTabSkinCur = -1, idSkinCur;
    public int idChooseTabSkin, idChooseSkin;
    public Sprite spriteSelectBtn, spriteUnequipBtn;
    public Text txt_attributeSet;
    public Text txt_hairCost, txt_pantCost, txt_shieldCost, txt_setCost;
    public Text txt_hairUse, txt_pantUse, txt_shieldUse, txt_setUse;
    public GameObject[] array_objHair, array_objShield, array_objSet;
    public GameObject[] array_tabSkin;
    public GameObject[] array_objAdsBuyBtn;
    public Image[] array_imgChoosePant, array_imgChooseHair, array_imgChooseShield, array_imgChooseSet;
    public Image[] array_imgChooseTabSkin;
    public Image[] array_imgUseSkinBtn;
    public RectTransform[] array_recttransUseSkinBtn;

    void OnEnable() {
        if (idSkinCur < 0)
            Btn_SwitchTabSkin(0);
        else
            Btn_SwitchTabSkin(idTabSkinCur);
    }

    #region action btn
    public async void Btn_ActivePopupShopSkin() {
        if (!HomeSceneManager.instance.isActiveBtn) {
            HomeSceneManager.instance.isActiveBtn = true;
            HomeSceneManager.instance.animationCam.Play();
            HomeSceneManager.instance.animationHome.Play();
            GamePlaySceneManager.instance.player.UpdateAnimation(StateAnimationCharacter.Dance);
            await Task.Delay(250);
            HomeSceneManager.instance.shopSkin.gameObject.SetActive(true);
        }
    }

    public void Btn_DeactivePopupShopSkin() {
        HomeSceneManager.instance.isActiveBtn = false;
        UseSkinCur();
        GamePlaySceneManager.instance.player.UpdateAnimation(StateAnimationCharacter.Idle);
        HomeSceneManager.instance.shopSkin.gameObject.SetActive(false);
        HomeSceneManager.instance.animationCam.Play(HomeSceneManager.instance.array_animationClip[0].name);
        HomeSceneManager.instance.animationHome.Play(HomeSceneManager.instance.array_animationClip[1].name);
    }

    public void Btn_SwitchTabSkin(int idTab) {
        idChooseTabSkin = idTab;//luu dem id tabskin
        //hien thi tabskin dang chon
        foreach (GameObject tab in array_tabSkin)
            tab.SetActive(false);
        array_tabSkin[idChooseTabSkin].SetActive(true);
        //hien thi noi bat icon tabskin dang chon
        foreach (Image imgIcon in array_imgChooseTabSkin)
            imgIcon.enabled = true;
        array_imgChooseTabSkin[idChooseTabSkin].enabled = false;
        //dung skin dang chon hoac mac dinh
        UseSkinOrigin();
        if (idChooseTabSkin == idTabSkinCur && idSkinCur >= 0)
            UseSkinWhenSwitchTabSkin(idSkinCur);
        else
            UseSkinWhenSwitchTabSkin(0);
    }

    public void Btn_SwitchSkin(int idSkin) {
        idChooseSkin = idSkin;//luu dem id skin
        TypeBuySkin typeBuySkin;
        switch (idChooseTabSkin) {
            case (int)TabSkin.Hair:
                //hien thi khung xanh quanh skin dang chon
                foreach (Image imgHair in array_imgChooseHair)
                    imgHair.enabled = false;
                array_imgChooseHair[idChooseSkin].enabled = true;
                typeBuySkin = (TypeBuySkin)Data.instance.dataPlayer.list_hairBought[idChooseSkin];//lay id type buy skin dang chon
                //an btn mua mot lan theo type buy skin dang chon
                if (typeBuySkin != TypeBuySkin.DontBuy)
                    DeactiveBtnBuySkinOnceTime(true);
                else
                    DeactiveBtnBuySkinOnceTime(false);
                //an text cost theo type buy skin dang chon
                if (typeBuySkin == TypeBuySkin.DontBuy || typeBuySkin == TypeBuySkin.UseBuyOnceTime)
                    DeactiveTextCostSkin(txt_hairUse, txt_hairCost);
                else
                    DeactiveTextCostSkin(txt_hairCost, txt_hairUse);
                //thay doi noi dung txt_use theo type buy skin dang chon
                if (idChooseTabSkin == idTabSkinCur && idChooseSkin == idSkinCur) {
                    txt_hairUse.text = UNEQUIP;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
                } else {
                    txt_hairUse.text = Constant.SELECT;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
                }
                break;
            case (int)TabSkin.Pant:
                //hien thi khung xanh quanh skin dang chon
                foreach (Image imgPant in array_imgChoosePant)
                    imgPant.enabled = false;
                array_imgChoosePant[idChooseSkin].enabled = true;
                typeBuySkin = (TypeBuySkin)Data.instance.dataPlayer.list_pantBought[idChooseSkin];//lay id type buy skin dang chon
                //an btn mua mot lan theo type buy skin dang chon
                if (typeBuySkin != TypeBuySkin.DontBuy)
                    DeactiveBtnBuySkinOnceTime(true);
                else
                    DeactiveBtnBuySkinOnceTime(false);
                //an text cost theo type buy skin dang chon
                if (typeBuySkin == TypeBuySkin.DontBuy || typeBuySkin == TypeBuySkin.UseBuyOnceTime)
                    DeactiveTextCostSkin(txt_pantUse, txt_pantCost);
                else
                    DeactiveTextCostSkin(txt_pantCost, txt_pantUse);
                //thay doi noi dung txt_use theo type buy skin dang chon
                if (idChooseTabSkin == idTabSkinCur && idChooseSkin == idSkinCur) {
                    txt_pantUse.text = UNEQUIP;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
                } else {
                    txt_pantUse.text = Constant.SELECT;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
                }
                break;
            case (int)TabSkin.Shield:
                //hien thi khung xanh quanh skin dang chon
                foreach (Image imgShield in array_imgChooseShield)
                    imgShield.enabled = false;
                array_imgChooseShield[idChooseSkin].enabled = true;
                typeBuySkin = (TypeBuySkin)Data.instance.dataPlayer.list_shieldBought[idChooseSkin];//lay id type buy skin dang chon
                //an btn mua mot lan theo type buy skin dang chon
                if (typeBuySkin != TypeBuySkin.DontBuy)
                    DeactiveBtnBuySkinOnceTime(true);
                else
                    DeactiveBtnBuySkinOnceTime(false);
                //an text cost theo type buy skin dang chon
                if (typeBuySkin == TypeBuySkin.DontBuy || typeBuySkin == TypeBuySkin.UseBuyOnceTime)
                    DeactiveTextCostSkin(txt_shieldUse, txt_shieldCost);
                else
                    DeactiveTextCostSkin(txt_shieldCost, txt_shieldUse);
                //thay doi noi dung txt_use theo type buy skin dang chon
                if (idChooseTabSkin == idTabSkinCur && idChooseSkin == idSkinCur) {
                    txt_shieldUse.text = UNEQUIP;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
                } else {
                    txt_shieldUse.text = Constant.SELECT;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
                }
                break;
            case (int)TabSkin.Set:
                //hien thi khung xanh quanh skin dang chon
                foreach (Image imgSet in array_imgChooseSet)
                    imgSet.enabled = false;
                array_imgChooseSet[idChooseSkin].enabled = true;
                //chan cac skinset hien chua lam
                if (idChooseSkin < Data.instance.array_setData.Length) {
                    txt_attributeSet.text = GameManager.instance.dataController.FindSetDataById(idChooseSkin).numAttributeBuff + "% "
                        + GameManager.instance.dataController.FindSetDataById(idChooseSkin).attribute;//hien thi attribute cua skinset dang chon
                    typeBuySkin = (TypeBuySkin)Data.instance.dataPlayer.list_setBought[idChooseSkin];//lay id type buy skin dang chon
                    //an text cost theo type buy skin dang chon
                    if (typeBuySkin == TypeBuySkin.DontBuy)
                        DeactiveTextCostSkin(txt_setUse, txt_setCost);
                    else
                        DeactiveTextCostSkin(txt_setCost, txt_setUse);
                }
                //thay doi noi dung txt_use theo type buy skin dang chon
                if (idChooseTabSkin == idTabSkinCur && idChooseSkin == idSkinCur) {
                    txt_setUse.text = UNEQUIP;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
                } else {
                    txt_setUse.text = Constant.SELECT;
                    array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
                }
                break;
        }
    }

    public void Btn_HairSkin(bool isBuyByGold) {
        if (isBuyByGold) {//mua vinh vien
            int goldTmp = Data.instance.dataPlayer.AmountGold;
            if (COST_SKIN <= goldTmp && txt_hairCost.gameObject.activeSelf) {
                goldTmp -= COST_SKIN;
                HomeSceneManager.instance.txt_amountGoldHome.text = goldTmp.ToString();
                Data.instance.dataPlayer.AmountGold = goldTmp;
                Data.instance.dataPlayer.list_hairBought[idChooseSkin] = (int)TypeBuySkin.Buy;
                DeactiveTextCostSkin(txt_hairCost, txt_hairUse);
                DeactiveBtnBuySkinOnceTime(true);
            }
        } else {//mua mot lan
            Data.instance.dataPlayer.list_hairBought[idChooseSkin] = (int)TypeBuySkin.BuyOnceTime;
            DeactiveTextCostSkin(txt_hairCost, txt_hairUse);
            DeactiveBtnBuySkinOnceTime(true);
        }
        //dung hairskin
        if (!txt_hairCost.gameObject.activeSelf) {
            if (idSkinCur != idChooseSkin || idChooseTabSkin != idTabSkinCur) {
                Data.instance.dataPlayer.idSkinCur = idSkinCur = idChooseSkin;
                Data.instance.dataPlayer.idTabSkinCur = idTabSkinCur = idChooseTabSkin;
                Btn_TryHair(idSkinCur);
                txt_hairUse.text = UNEQUIP;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
            } else {
                Btn_TryHair(-1);
                Data.instance.dataPlayer.idSkinCur = idSkinCur = -1;
                txt_hairUse.text = Constant.SELECT;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
            }
        }
        GameManager.instance.dataController.SaveGame();
    }

    public void Btn_PantSkin(bool isBuyByGold) {
        if (isBuyByGold) {//mua vinh vien
            int goldTmp = Data.instance.dataPlayer.AmountGold;
            if (COST_SKIN <= goldTmp && txt_pantCost.gameObject.activeSelf) {
                goldTmp -= COST_SKIN;
                HomeSceneManager.instance.txt_amountGoldHome.text = goldTmp.ToString();
                Data.instance.dataPlayer.AmountGold = goldTmp;
                Data.instance.dataPlayer.list_pantBought[idChooseSkin] = (int)TypeBuySkin.Buy;
                DeactiveTextCostSkin(txt_pantCost, txt_pantUse);
                DeactiveBtnBuySkinOnceTime(true);
            }
        } else {//mua mot lan
            Data.instance.dataPlayer.list_pantBought[idChooseSkin] = (int)TypeBuySkin.BuyOnceTime;
            DeactiveTextCostSkin(txt_pantCost, txt_pantUse);
            DeactiveBtnBuySkinOnceTime(true);
        }
        //dung pantskin
        if (!txt_pantCost.gameObject.activeSelf) {
            if (idSkinCur != idChooseSkin || idChooseTabSkin != idTabSkinCur) {
                Data.instance.dataPlayer.idSkinCur = idSkinCur = idChooseSkin;
                Data.instance.dataPlayer.idTabSkinCur = idTabSkinCur = idChooseTabSkin;
                Btn_TryPant(idSkinCur);
                txt_pantUse.text = UNEQUIP;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
            } else {
                Btn_TryPant(-1);
                Data.instance.dataPlayer.idSkinCur = idSkinCur = -1;
                txt_pantUse.text = Constant.SELECT;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
            }
        }
        GameManager.instance.dataController.SaveGame();
    }

    public void Btn_ShieldSkin(bool isBuyByGold) {
        if (isBuyByGold) {//mua vinh vien
            int goldTmp = Data.instance.dataPlayer.AmountGold;
            if (COST_SKIN <= goldTmp && txt_shieldCost.gameObject.activeSelf) {
                goldTmp -= COST_SKIN;
                HomeSceneManager.instance.txt_amountGoldHome.text = goldTmp.ToString();
                Data.instance.dataPlayer.AmountGold = goldTmp;
                Data.instance.dataPlayer.list_shieldBought[idChooseSkin] = (int)TypeBuySkin.Buy;
                DeactiveTextCostSkin(txt_shieldCost, txt_shieldUse);
                DeactiveBtnBuySkinOnceTime(true);
            }
        } else {//mua mot lan
            Data.instance.dataPlayer.list_shieldBought[idChooseSkin] = (int)TypeBuySkin.BuyOnceTime;
            DeactiveTextCostSkin(txt_shieldCost, txt_shieldUse);
            DeactiveBtnBuySkinOnceTime(true);
        }
        //dung shieldskin
        if (!txt_shieldCost.gameObject.activeSelf) {
            if (idSkinCur != idChooseSkin || idChooseTabSkin != idTabSkinCur) {
                Data.instance.dataPlayer.idSkinCur = idSkinCur = idChooseSkin;
                Data.instance.dataPlayer.idTabSkinCur = idTabSkinCur = idChooseTabSkin;
                Btn_TryShield(idSkinCur);
                txt_shieldUse.text = UNEQUIP;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
            } else {
                Btn_TryShield(-1);
                Data.instance.dataPlayer.idSkinCur = idSkinCur = -1;
                txt_shieldUse.text = Constant.SELECT;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
            }
        }
        GameManager.instance.dataController.SaveGame();
    }

    public void Btn_SetSkin() {
        //mua vinh vien
        int goldTmp = Data.instance.dataPlayer.AmountGold;
        if (COST_SET_SKIN <= goldTmp && txt_setCost.gameObject.activeSelf) {
            goldTmp -= COST_SET_SKIN;
            HomeSceneManager.instance.txt_amountGoldHome.text = goldTmp.ToString();
            Data.instance.dataPlayer.AmountGold = goldTmp;
            Data.instance.dataPlayer.list_setBought[idChooseSkin] = (int)TypeBuySkin.Buy;
            DeactiveTextCostSkin(txt_setCost, txt_setUse);
        }
        //dung setskin
        if (!txt_setCost.gameObject.activeSelf)
            if (idSkinCur != idChooseSkin || idChooseTabSkin != idTabSkinCur) {
                Data.instance.dataPlayer.idSkinCur = idSkinCur = idChooseSkin;
                Data.instance.dataPlayer.idTabSkinCur = idTabSkinCur = idChooseTabSkin;
                Btn_TrySet(idSkinCur);
                txt_setUse.text = UNEQUIP;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteUnequipBtn;
            } else {
                UseSkinOrigin();
                Data.instance.dataPlayer.idSkinCur = idSkinCur = -1;
                txt_setUse.text = Constant.SELECT;
                array_imgUseSkinBtn[idChooseTabSkin].sprite = spriteSelectBtn;
            }
        GameManager.instance.dataController.SaveGame();
    }

    public void Btn_TryHair(int idSkin) {
        if (idSkin < 0)
            foreach (GameObject hair in array_objHair)
                hair.SetActive(false);
        else if (idSkin < array_objHair.Length) {
            foreach (GameObject hair in array_objHair)
                hair.SetActive(false);
            array_objHair[idSkin].SetActive(true);
        }
    }

    public void Btn_TryPant(int idSkin) {
        if (idSkin < 0)
            GamePlaySceneManager.instance.player.skinMeshRen_pant.material = GameManager.instance.mat_pantOrigin;
        else if (idSkin < GameManager.instance.array_matPant.Length)
            GamePlaySceneManager.instance.player.skinMeshRen_pant.material = GameManager.instance.array_matPant[idSkin];
    }

    public void Btn_TryShield(int idSkin) {
        if (idSkin < 0)
            foreach (GameObject shield in array_objShield)
                shield.SetActive(false);
        else if (idSkin < array_objShield.Length) {
            foreach (GameObject shield in array_objShield)
                shield.SetActive(false);
            array_objShield[idSkin].SetActive(true);
        }
    }

    public void Btn_TrySet(int idSkin) {
        if (idSkin == 0) {
            UseSkinOrigin();
            GamePlaySceneManager.instance.player.skinMeshRen_body.material.color = GameManager.instance.array_color[0];
            GamePlaySceneManager.instance.player.skinMeshRen_pant.material.color = GameManager.instance.array_color[0];
            array_objHair[3].SetActive(true);
            array_objSet[0].SetActive(true);
            array_objSet[1].SetActive(true);
        } else if (idSkin == 1) {
            UseSkinOrigin();
            GamePlaySceneManager.instance.player.skinMeshRen_body.material.color = GameManager.instance.array_color[2];
            GamePlaySceneManager.instance.player.skinMeshRen_pant.material.color = GameManager.instance.array_color[2];
            array_objHair[4].SetActive(true);
            array_objSet[2].SetActive(true);
            array_objSet[3].SetActive(true);
        }
    }
    #endregion

    void UseSkinWhenSwitchTabSkin(int idSkin) {
        Btn_SwitchSkin(idSkin);
        switch (idChooseTabSkin) {
            case (int)TabSkin.Hair:
                array_objHair[idSkin].SetActive(true);
                break;
            case (int)TabSkin.Pant:
                GamePlaySceneManager.instance.player.skinMeshRen_pant.material = GameManager.instance.array_matPant[idSkin];
                break;
            case (int)TabSkin.Shield:
                array_objShield[idSkin].SetActive(true);
                break;
            case (int)TabSkin.Set:
                Btn_TrySet(idSkin);
                break;
        }
    }

    void DeactiveBtnBuySkinOnceTime(bool isDisplay) {
        if (isDisplay) {
            array_recttransUseSkinBtn[idChooseTabSkin].anchoredPosition = new Vector2(0, -520);
            array_objAdsBuyBtn[idChooseTabSkin].SetActive(false);
        } else {
            array_recttransUseSkinBtn[idChooseTabSkin].anchoredPosition = new Vector2(-300, -520);
            array_objAdsBuyBtn[idChooseTabSkin].SetActive(true);
        }
    }

    void DeactiveTextCostSkin(Text txtOff, Text txtOn) {
        txtOff.gameObject.SetActive(false);
        txtOn.gameObject.SetActive(true);
    }

    void UseSkinOrigin() {
        GamePlaySceneManager.instance.player.skinMeshRen_body.material = GameManager.instance.mat_bodyOrigin;//reset color body
        GamePlaySceneManager.instance.player.skinMeshRen_pant.material = GameManager.instance.mat_pantOrigin;//reset color pant
        foreach (GameObject hair in array_objHair)//thao mu
            hair.SetActive(false);
        foreach (GameObject shield in array_objShield)//thao khien
            shield.SetActive(false);
        foreach (GameObject set in array_objSet)//theo set
            set.SetActive(false);
    }

    public void UseSkinCur() {
        UseSkinOrigin();//thao tat ca skin cu
        //neu idSkinCur = -1 khong mac skin nao
        if (idSkinCur < 0)
            return;
        //use skin theo idSkinCur
        switch (idTabSkinCur) {
            case (int)TabSkin.Hair:
                array_objHair[idSkinCur].SetActive(true);
                break;
            case (int)TabSkin.Pant:
                GamePlaySceneManager.instance.player.skinMeshRen_pant.material = GameManager.instance.array_matPant[idSkinCur];
                break;
            case (int)TabSkin.Shield:
                array_objShield[idSkinCur].SetActive(true);
                break;
            case (int)TabSkin.Set:
                Btn_TrySet(idSkinCur);
                break;
        }
    }

    public void InitDataShopSkin() {
        idSkinCur = Data.instance.dataPlayer.idSkinCur;
        idTabSkinCur = Data.instance.dataPlayer.idTabSkinCur;
    }
}
