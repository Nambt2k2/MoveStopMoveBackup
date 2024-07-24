using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZCSkillBuffController : MonoBehaviour {
    public const int COST_BUFF_SHIELD = 1000, COST_BUFF_SPEED = 250, COST_BUFF_RANGE = 250, COST_BUFF_MAXBULLET = 500;
    public const int LEVEL_MAX_BUFF_SHIELD = 5, LEVEL_MAX_BUFF_SPEED = 10, LEVEL_MAX_BUFF_RANGE = 10, LEVEL_MAX_BUFF_MAXBULLET = 8;
    public const string MAX_LEVEL = "MAX LEVEL";

    public GameObject obj_skillBuff;
    public GameObject obj_remainGameplay;
    public Button btn_Pause;
    public Text txtAmoutGold;
    public Image imgSkill;
    public Text txtNameSkill;
    public Sprite[] array_imgAllSkill;
    public List<int> list_idSkillBuffAbility;
    public Text[] array_txtBuff;
    public Image[] array_imgBuyBuffBtn;
    public RawImage[] array_imgIconGoldInBuyBuffBtn;
    public Sprite spriteCanBuyBuffBtn, spriteNotCanBuyBuffBtn;
    public Image imgSkillChoosed;
    public Text txtNameSkillChoosed;
    public Text txtContentSkillChoosed;
    public string[] array_skillZombieCity;

    public void Init() {
        obj_skillBuff.SetActive(true);
        obj_remainGameplay.SetActive(true);
        btn_Pause.gameObject.SetActive(false);
        txtAmoutGold.text = Data.instance.dataPlayer.AmountGold.ToString();
        list_idSkillBuffAbility = Data.instance.dataPlayer.list_idSkillBuffAbility;
        BtnChangeSkillBuff();

        // load level buff cur
        int tmp = Data.instance.dataPlayer.levelBuffShield;
        array_txtBuff[0].text = tmp.ToString() + " Time";
        array_txtBuff[1].text = (Mathf.Pow(2, tmp) * COST_BUFF_SHIELD).ToString();
        tmp = Data.instance.dataPlayer.levelBuffSpeed;
        array_txtBuff[2].text = "+" + (tmp * 10).ToString() + "%" + " Speed";
        array_txtBuff[3].text = ((2 * tmp + 2) * COST_BUFF_SPEED).ToString();
        tmp = Data.instance.dataPlayer.levelBuffRange;
        array_txtBuff[4].text = "+" + (tmp * 10).ToString() + "%" + " Range";
        array_txtBuff[5].text = ((2 * tmp + 2) * COST_BUFF_RANGE).ToString();
        tmp = Data.instance.dataPlayer.levelBuffMaxBullet;
        array_txtBuff[6].text = "Max: " + (tmp + 2).ToString();
        array_txtBuff[7].text = (Mathf.Pow(2, tmp) * COST_BUFF_MAXBULLET).ToString();
        UpdateCanBuyBuffBtn(-1);
    }

    public void BtnPlayGame(int id) {
        obj_skillBuff.SetActive(false);
        btn_Pause.gameObject.SetActive(true);
        GameManager.instance.UseSkinOnceTime();
        ZCSceneManager.instance.enabled = true;
        enabled = false;

        //change data in pause UI
        if (id == 0) {
            ZCSceneManager.instance.obj_notChooseSkill.SetActive(true);
            ZCSceneManager.instance.obj_chooseSkill.SetActive(false);
            ZCSceneManager.instance.player.idSkillChoosed = -1;
            return;
        }
        ZCSceneManager.instance.obj_notChooseSkill.SetActive(false);
        ZCSceneManager.instance.obj_chooseSkill.SetActive(true);
        imgSkillChoosed.sprite = imgSkill.sprite;
        txtNameSkillChoosed.text = txtNameSkill.text;
        txtContentSkillChoosed.text = array_skillZombieCity[ZCSceneManager.instance.player.idSkillChoosed];
    }
    public void BtnChangeSkillBuff() {
        if (imgSkill.sprite == array_imgAllSkill[list_idSkillBuffAbility[0]]) {
            imgSkill.sprite = array_imgAllSkill[list_idSkillBuffAbility[1]];
            txtNameSkill.text = ((ZCSkillBuffAbility)list_idSkillBuffAbility[1]).ToString().Replace("_", " ");
            ZCSceneManager.instance.player.idSkillChoosed = list_idSkillBuffAbility[1];
            return;
        }
        imgSkill.sprite = array_imgAllSkill[list_idSkillBuffAbility[0]];
        txtNameSkill.text = ((ZCSkillBuffAbility)list_idSkillBuffAbility[0]).ToString().Replace("_", " ");
        ZCSceneManager.instance.player.idSkillChoosed = list_idSkillBuffAbility[0];
    }
    public void BtnLevelUpBuff(int idBuff) {
        int tmp;
        switch (idBuff) {
            case (int)ZCTypeBuff.Shield:
                tmp = Data.instance.dataPlayer.levelBuffShield;
                if (tmp == LEVEL_MAX_BUFF_SHIELD)
                    return;

                if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, tmp) * COST_BUFF_SHIELD) {
                    Data.instance.dataPlayer.AmountGold -= (int)(Mathf.Pow(2, tmp) * COST_BUFF_SHIELD);
                    Data.instance.dataPlayer.levelBuffShield++;
                    txtAmoutGold.text = Data.instance.dataPlayer.AmountGold.ToString();
                    array_txtBuff[idBuff * 2].text = (tmp + 1).ToString() + " Time";
                    array_txtBuff[idBuff * 2 + 1].text = (Mathf.Pow(2, tmp + 1) * COST_BUFF_SHIELD).ToString();
                    ZCSceneManager.instance.player.array_iconShield[tmp].gameObject.SetActive(true);
                    UpdateCanBuyBuffBtn(0);
                    GameManager.instance.dataController.SaveGame();
                }
                break;
            case (int)ZCTypeBuff.Speed:
                tmp = Data.instance.dataPlayer.levelBuffSpeed;
                if (tmp == LEVEL_MAX_BUFF_SPEED)
                    return;

                if (Data.instance.dataPlayer.AmountGold >= (2 * tmp + 2) * COST_BUFF_SPEED) {
                    Data.instance.dataPlayer.AmountGold -= (2 * tmp + 2) * COST_BUFF_SPEED;
                    Data.instance.dataPlayer.levelBuffSpeed++;
                    txtAmoutGold.text = Data.instance.dataPlayer.AmountGold.ToString();
                    array_txtBuff[idBuff * 2].text = "+" + ((tmp + 1) * 10).ToString() + "%" + " Speed"; // level up once time incre 10%
                    array_txtBuff[idBuff * 2 + 1].text = ((2 * tmp + 4) * COST_BUFF_SPEED).ToString();
                    UpdateCanBuyBuffBtn(1);
                    GameManager.instance.dataController.SaveGame();
                }
                break;
            case (int)ZCTypeBuff.Range:
                tmp = Data.instance.dataPlayer.levelBuffRange;
                if (tmp == LEVEL_MAX_BUFF_RANGE)
                    return;

                if (Data.instance.dataPlayer.AmountGold >= (2 * tmp + 2) * COST_BUFF_RANGE) {
                    Data.instance.dataPlayer.AmountGold -= (2 * tmp + 2) * COST_BUFF_RANGE;
                    Data.instance.dataPlayer.levelBuffRange++;
                    txtAmoutGold.text = Data.instance.dataPlayer.AmountGold.ToString();
                    array_txtBuff[idBuff * 2].text = "+" + ((tmp + 1) * 10).ToString() + "%" + " Range"; // level up once time incre 10%
                    array_txtBuff[idBuff * 2 + 1].text = ((2 * tmp + 4) * COST_BUFF_RANGE).ToString();
                    UpdateCanBuyBuffBtn(2);
                    GameManager.instance.dataController.SaveGame();
                    ZCSceneManager.instance.player.obj_circleRangeAtk.transform.localScale *= 1.05f;
                    ZCSceneManager.instance.camController.camMain.fieldOfView *= 1.04f;
                }
                break;
            case (int)ZCTypeBuff.MaxBullet:
                tmp = Data.instance.dataPlayer.levelBuffMaxBullet;
                if (tmp == LEVEL_MAX_BUFF_MAXBULLET)
                    return;

                if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, tmp) * COST_BUFF_MAXBULLET) {
                    Data.instance.dataPlayer.AmountGold -= (int)Mathf.Pow(2, tmp) * COST_BUFF_MAXBULLET;
                    Data.instance.dataPlayer.levelBuffMaxBullet++;
                    txtAmoutGold.text = Data.instance.dataPlayer.AmountGold.ToString();
                    array_txtBuff[idBuff * 2].text = "Max: " + (tmp + 1 + 2).ToString(); // level 0 have 2 bullet
                    array_txtBuff[idBuff * 2 + 1].text = (Mathf.Pow(2, tmp + 1) * COST_BUFF_MAXBULLET).ToString();
                    UpdateCanBuyBuffBtn(3);
                    GameManager.instance.dataController.SaveGame();
                }
                break;
        }
    }
    void UpdateCanBuyBuffBtn(int id) {
        switch (id) {
            case -1:
                break;
            case (int)ZCTypeBuff.Shield:
                if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, Data.instance.dataPlayer.levelBuffShield) * COST_BUFF_SHIELD) {
                    array_imgBuyBuffBtn[0].sprite = spriteCanBuyBuffBtn;
                } else {
                    array_imgBuyBuffBtn[0].sprite = spriteNotCanBuyBuffBtn;
                }
                break;
            case (int)ZCTypeBuff.Speed:
                if (Data.instance.dataPlayer.AmountGold >= (2 * Data.instance.dataPlayer.levelBuffSpeed + 2) * COST_BUFF_SPEED) {
                    array_imgBuyBuffBtn[1].sprite = spriteCanBuyBuffBtn;
                } else {
                    array_imgBuyBuffBtn[1].sprite = spriteNotCanBuyBuffBtn;
                }
                break;
            case (int)ZCTypeBuff.Range:
                if (Data.instance.dataPlayer.AmountGold >= (2 * Data.instance.dataPlayer.levelBuffRange + 2) * COST_BUFF_RANGE) {
                    array_imgBuyBuffBtn[2].sprite = spriteCanBuyBuffBtn;
                } else {
                    array_imgBuyBuffBtn[2].sprite = spriteNotCanBuyBuffBtn;
                }
                break;
            case (int)ZCTypeBuff.MaxBullet:
                if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, Data.instance.dataPlayer.levelBuffMaxBullet) * COST_BUFF_MAXBULLET) {
                    array_imgBuyBuffBtn[3].sprite = spriteCanBuyBuffBtn;
                } else {
                    array_imgBuyBuffBtn[3].sprite = spriteNotCanBuyBuffBtn;
                }
                break;
        }

        if (Data.instance.dataPlayer.levelBuffShield == LEVEL_MAX_BUFF_SHIELD) {
            array_imgBuyBuffBtn[0].sprite = spriteNotCanBuyBuffBtn;
            array_imgIconGoldInBuyBuffBtn[0].enabled = false;
            array_txtBuff[1].text = MAX_LEVEL;
            array_txtBuff[1].rectTransform.anchoredPosition = Vector2.up * 5;
        } else
            if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, Data.instance.dataPlayer.levelBuffShield) * COST_BUFF_SHIELD)
            array_imgBuyBuffBtn[0].sprite = spriteCanBuyBuffBtn;
        else
            array_imgBuyBuffBtn[0].sprite = spriteNotCanBuyBuffBtn;

        if (Data.instance.dataPlayer.levelBuffSpeed == LEVEL_MAX_BUFF_SPEED) {
            array_imgBuyBuffBtn[1].sprite = spriteNotCanBuyBuffBtn;
            array_imgIconGoldInBuyBuffBtn[1].enabled = false;
            array_txtBuff[3].text = MAX_LEVEL;
            array_txtBuff[3].rectTransform.anchoredPosition = Vector2.up * 5;
        } else
            if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, Data.instance.dataPlayer.levelBuffSpeed) * COST_BUFF_SPEED)
            array_imgBuyBuffBtn[1].sprite = spriteCanBuyBuffBtn;
        else
            array_imgBuyBuffBtn[1].sprite = spriteNotCanBuyBuffBtn;

        if (Data.instance.dataPlayer.levelBuffRange == LEVEL_MAX_BUFF_RANGE) {
            array_imgBuyBuffBtn[2].sprite = spriteNotCanBuyBuffBtn;
            array_imgIconGoldInBuyBuffBtn[2].enabled = false;
            array_txtBuff[5].text = MAX_LEVEL;
            array_txtBuff[5].rectTransform.anchoredPosition = Vector2.up * 5;
        } else
            if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, Data.instance.dataPlayer.levelBuffRange) * COST_BUFF_RANGE)
            array_imgBuyBuffBtn[2].sprite = spriteCanBuyBuffBtn;
        else
            array_imgBuyBuffBtn[2].sprite = spriteNotCanBuyBuffBtn;

        if (Data.instance.dataPlayer.levelBuffMaxBullet == LEVEL_MAX_BUFF_MAXBULLET) {
            array_imgBuyBuffBtn[3].sprite = spriteNotCanBuyBuffBtn;
            array_imgIconGoldInBuyBuffBtn[3].enabled = false;
            array_txtBuff[7].text = MAX_LEVEL;
            array_txtBuff[7].rectTransform.anchoredPosition = Vector2.up * 5;
        } else
            if (Data.instance.dataPlayer.AmountGold >= Mathf.Pow(2, Data.instance.dataPlayer.levelBuffMaxBullet) * COST_BUFF_MAXBULLET)
            array_imgBuyBuffBtn[3].sprite = spriteCanBuyBuffBtn;
        else
            array_imgBuyBuffBtn[3].sprite = spriteNotCanBuyBuffBtn;
    }

}

enum ZCTypeBuff { Shield, Speed, Range, MaxBullet }
