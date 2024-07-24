using UnityEngine;
using UnityEngine.UI;

public class ShopWeapon : MonoBehaviour {
    [Header("_____________________________ShopWeapon_______________________")]
    public const string EQUIPPED = "Equipped";
    public int idWeaponOpen;
    public int idChooseWeapon, idChooseSkinWeapon;
    public int idWeaponCur, idSkinWeaponCur;
    public Sprite imgWeaponCost, imgWeaponUse;
    public Image[] array_imgColorSelectBtn;
    public Text[] array_txtWeaponCost, array_txtWeaponUse;
    public RectTransform[] array_recttransUseWeaponBtn;
    public GameObject[] array_objTabWeapon;

    [Header("_____________________________WeaponSkin_______________________")]
    public GameObject obj_weaponSkin;
    public RectTransform[] array_recttransWeaponSkinChooseBtn;
    public Image[] array_imgWeaponSkinChooseBtn;
    public int idChooseColorWeaponCustom;
    public GameObject obj_colorWeaponCustom;
    public Image[] array_imgIdChooseMaterialWeaponCustom;
    public GameObject[] array_objIdMaterialWeaponCustom;
    public MeshRenderer[] array_meshrenDisplayWeapon;
    public MeshRenderer[] array_meshrenWeaponHold = new MeshRenderer[12];
    public MeshRenderer[] array_meshrenSkinWeapon;

    void OnEnable() {
        if (idWeaponOpen < 11 && Data.instance.dataPlayer.AmountGold >= GameManager.instance.dataController.FindWeaponDataById(idWeaponOpen + 1).cost) {
            idChooseWeapon = idWeaponOpen + 1;
            SwitchChooseTabWeapon(0);
        } else
            idChooseWeapon = idWeaponCur;
        SwitchChooseTabWeapon(0);
    }
    void OnDisable() {
        array_objTabWeapon[idChooseWeapon].SetActive(false);
        idChooseWeapon = idChooseWeapon = 0;
    }

    public void SwitchChooseTabWeapon(int incre) {
        if ((incre == -1 && idChooseWeapon == (int)OderWeaponInShop.Hammer) || (incre == 1 && idChooseWeapon == (int)OderWeaponInShop.Uzi))
            return;

        array_objTabWeapon[idChooseWeapon].SetActive(false);
        if ((incre == -1 && idChooseWeapon == (int)OderWeaponInShop.Z) || (incre == 1 && idChooseWeapon == (int)OderWeaponInShop.Arrow)) {
            for (int i = 0; i < array_recttransWeaponSkinChooseBtn.Length; i++)
                array_recttransWeaponSkinChooseBtn[i].anchoredPosition = new Vector2(-300 + 150 * i, array_recttransWeaponSkinChooseBtn[i].anchoredPosition.y);

            array_recttransWeaponSkinChooseBtn[3].gameObject.SetActive(true);
            array_recttransWeaponSkinChooseBtn[4].gameObject.SetActive(true);
        }
        idChooseWeapon = idChooseWeapon + incre;
        if (idChooseWeapon == (int)OderWeaponInShop.Z || idChooseWeapon == (int)OderWeaponInShop.Arrow) {
            array_recttransWeaponSkinChooseBtn[3].gameObject.SetActive(false);
            array_recttransWeaponSkinChooseBtn[4].gameObject.SetActive(false);
            for (int i = 0; i < array_recttransWeaponSkinChooseBtn.Length - 2; i++)
                array_recttransWeaponSkinChooseBtn[i].anchoredPosition = new Vector2(-150 + 150 * i, array_recttransWeaponSkinChooseBtn[i].anchoredPosition.y);
        }
        array_objTabWeapon[idChooseWeapon].SetActive(true);

        if (idChooseWeapon <= idWeaponOpen) {
            array_imgColorSelectBtn[idChooseWeapon].sprite = imgWeaponUse;
            array_imgColorSelectBtn[idChooseWeapon].color = Color.white;
            obj_weaponSkin.SetActive(true);
            for (int i = 0; i < 5; i++) {
                array_meshrenSkinWeapon[idChooseWeapon * 5 + i].gameObject.SetActive(true);
            }
        } else {
            array_imgColorSelectBtn[idChooseWeapon].sprite = imgWeaponCost;
            array_imgColorSelectBtn[idChooseWeapon].color = Color.green;
            obj_weaponSkin.SetActive(false);
            for (int i = 0; i < 5; i++) {
                array_meshrenSkinWeapon[idChooseWeapon * 5 + i].gameObject.SetActive(false);
            }
        }

        if (idChooseWeapon != idWeaponCur) {
            Btn_ChooseSkinWeaponCustom(false);
            SwitchChooseSkinWeapon(2);
        } else {
            SwitchChooseSkinWeapon(idSkinWeaponCur);
            if (1 <= idSkinWeaponCur && idSkinWeaponCur < 5) {
                Btn_ChooseSkinWeaponCustom(false);
            } else if (idSkinWeaponCur == 0) {
                Btn_ChooseSkinWeaponCustom(true);
            }
        }
    }
    public void SwitchChooseSkinWeapon(int index) {
        idChooseSkinWeapon = index;
        for (int i = 0; i < array_recttransWeaponSkinChooseBtn.Length; i++) {
            array_recttransWeaponSkinChooseBtn[i].sizeDelta = Vector2.one * 100;
            array_imgWeaponSkinChooseBtn[i].color = Color.white;
        }

        array_recttransWeaponSkinChooseBtn[index].sizeDelta += Vector2.one * 20;
        array_imgWeaponSkinChooseBtn[index].color = new Color(0.6f, 0.6f, 0.6f);

        if (idChooseWeapon <= idWeaponOpen) {
            if (idWeaponCur == idChooseWeapon && idChooseSkinWeapon == idSkinWeaponCur) {
                array_txtWeaponUse[idChooseWeapon].text = EQUIPPED;
            } else {
                array_txtWeaponUse[idChooseWeapon].text = Constant.SELECT;
            }

            array_txtWeaponUse[idChooseWeapon].gameObject.SetActive(true);
            if (idChooseWeapon - 1 >= 0) {
                array_txtWeaponCost[idChooseWeapon - 1].gameObject.SetActive(false);
            }
        } else {
            array_txtWeaponCost[idChooseWeapon - 1].gameObject.SetActive(true);
            array_txtWeaponUse[idChooseWeapon].gameObject.SetActive(false);
        }

        if (index == 0) {
            Btn_ChooseSkinWeaponCustom(true);
            idChooseColorWeaponCustom = 0;
        } else {
            Btn_ChooseSkinWeaponCustom(false);
        }

        array_meshrenDisplayWeapon[idChooseWeapon].materials = GameManager.instance.array_meshrenSkinWeapon[idChooseSkinWeapon + idChooseWeapon * 5].sharedMaterials;
    }

    public void Btn_DeactivePopupShopWeapon() {
        HomeSceneManager.instance.isActiveBtn = false;
        HomeSceneManager.instance.shopWeapon.UseWeaponCur();
        GamePlaySceneManager.instance.player.transformAvatar.gameObject.SetActive(true);
        gameObject.SetActive(false);
        HomeSceneManager.instance.animationCam.Play(HomeSceneManager.instance.array_animationClip[0].name);
        HomeSceneManager.instance.animationHome.Play(HomeSceneManager.instance.array_animationClip[1].name);
        for (int i = 0; i < array_recttransWeaponSkinChooseBtn.Length; i++)
            array_recttransWeaponSkinChooseBtn[i].anchoredPosition = new Vector2(-300 + 150 * i, array_recttransWeaponSkinChooseBtn[i].anchoredPosition.y);
        array_recttransWeaponSkinChooseBtn[3].gameObject.SetActive(true);
        array_recttransWeaponSkinChooseBtn[4].gameObject.SetActive(true);
    }

    void Btn_ChooseSkinWeaponCustom(bool b) {
        obj_colorWeaponCustom.SetActive(b);
        array_objIdMaterialWeaponCustom[idChooseWeapon].SetActive(b);
        if (b) {
            array_recttransUseWeaponBtn[idChooseWeapon].anchoredPosition = Vector2.down * 675;
        } else {
            array_recttransUseWeaponBtn[idChooseWeapon].anchoredPosition = Vector2.down * 415;
        }
    }
    public void Btn_ChooseColorPicker(int id) {
        Data.instance.dataPlayer.list_weaponColorCustom[idChooseWeapon * 3 + idChooseColorWeaponCustom] = id;
        array_imgIdChooseMaterialWeaponCustom[idChooseWeapon * 3 + idChooseColorWeaponCustom].color = GameManager.instance.array_color[id];
        GameManager.instance.array_meshrenSkinWeapon[idChooseWeapon * 5].sharedMaterials[idChooseColorWeaponCustom].color = GameManager.instance.array_color[id];
    }
    public void Btn_IdChooseMaterialWeaponCustom(int id) {
        idChooseColorWeaponCustom = id;
    }
    public void Btn_Weapon(int id) {
        if (array_txtWeaponUse[id].gameObject.activeSelf && (idWeaponCur != id || idSkinWeaponCur != idChooseSkinWeapon)) {
            Data.instance.dataPlayer.idWeaponCur = idWeaponCur = id;
            Data.instance.dataPlayer.idSkinWeaponCur = idSkinWeaponCur = idChooseSkinWeapon;
            GamePlaySceneManager.instance.player.obj_weaponHold.SetActive(false);
        }
        if (array_txtWeaponUse[id].gameObject.activeSelf)
            Btn_DeactivePopupShopWeapon();

        id--;
        if (id < 0) {
            GameManager.instance.dataController.SaveGame();
            return;
        }
        int cost = GameManager.instance.dataController.FindWeaponDataById(id + 1).cost;
        int goldTmp = Data.instance.dataPlayer.AmountGold;

        if (idWeaponOpen == id && cost <= goldTmp) {
            goldTmp -= cost;
            HomeSceneManager.instance.txt_amountGoldHome.text = goldTmp.ToString();
            Data.instance.dataPlayer.AmountGold = goldTmp;
            array_txtWeaponCost[idWeaponOpen].gameObject.SetActive(false);
            idWeaponOpen++;
            array_txtWeaponUse[idWeaponOpen].gameObject.SetActive(true);
            Data.instance.dataPlayer.idWeaponOpen = idWeaponOpen;
            Data.instance.dataPlayer.idWeaponCur = idWeaponCur = idWeaponOpen;
            Data.instance.dataPlayer.idSkinWeaponCur = idSkinWeaponCur = idChooseSkinWeapon;
            GamePlaySceneManager.instance.player.obj_weaponHold.SetActive(false);
            SwitchChooseTabWeapon(0);
        }
        GameManager.instance.dataController.SaveGame();
    }

    public void UseWeaponCur() {
        if (array_meshrenWeaponHold[idWeaponCur] == null) {
            array_meshrenWeaponHold[idWeaponCur] = Instantiate(GameManager.instance.array_meshrenWeaponHold[idWeaponCur], GamePlaySceneManager.instance.player.transformWeaponHold);
            GamePlaySceneManager.instance.player.meshrenWeaponHold = array_meshrenWeaponHold[idWeaponCur];
            GamePlaySceneManager.instance.player.obj_weaponHold = array_meshrenWeaponHold[idWeaponCur].gameObject;
        }
        array_meshrenWeaponHold[idWeaponCur].materials = GameManager.instance.array_meshrenSkinWeapon[idSkinWeaponCur + idWeaponCur * 5].sharedMaterials;
        GamePlaySceneManager.instance.player.meshrenWeaponHold.materials = array_meshrenWeaponHold[idWeaponCur].materials;
        GamePlaySceneManager.instance.player.obj_weaponHold = array_meshrenWeaponHold[idWeaponCur].gameObject;
        GamePlaySceneManager.instance.player.obj_weaponHold.SetActive(true);
    }
    public void InitDataShopWeapon() {
        idWeaponCur = Data.instance.dataPlayer.idWeaponCur;
        idSkinWeaponCur = Data.instance.dataPlayer.idSkinWeaponCur;
        idWeaponOpen = Data.instance.dataPlayer.idWeaponOpen;
        for (int i = 0; i <= idWeaponOpen; i++) {
            for (int j = 0; j < GameManager.instance.array_meshrenSkinWeapon[i * 5].sharedMaterials.Length; j++) {
                array_imgIdChooseMaterialWeaponCustom[i * 3 + j].color = GameManager.instance.array_color[Data.instance.dataPlayer.list_weaponColorCustom[i * 3 + j]];
                GameManager.instance.array_meshrenSkinWeapon[i * 5].sharedMaterials[j].color = array_imgIdChooseMaterialWeaponCustom[i * 3 + j].color;
            }
        }
    }
}

public enum OderWeaponInShop { Hammer, Lollipop, Knife, Candycane, Boomerang, Swirlypop, Axe, Icecreamcone, Battleaxe, Z, Arrow, Uzi }