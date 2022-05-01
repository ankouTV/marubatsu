using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks
{
    int width = 3;
    int turn = 0;
    int myTurn;
    Button[] buttons;
    int[] board = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    [SerializeField] Transform parent;
    [SerializeField] Sprite[] marubatsu;
    [SerializeField] Text turnText;
    [SerializeField] GameObject retryButton;

    private void Start()
    {
        buttons = new Button[parent.childCount];
        for (var i = 0; i < buttons.Length; ++i)
        {
            buttons[i] = parent.GetChild(i).GetComponent<Button>();
            int index = i;
            buttons[index].onClick.AddListener(() => OnButtonClick(index));
        }

        myTurn = PhotonNetwork.IsMasterClient ? 0 : 1;
        Debug.Log(myTurn);
        SetButtonsActive(turn == myTurn);
    }

    void OnButtonClick(int index)
    {
        photonView.RPC(nameof(ChangeIcon), RpcTarget.All, index, myTurn);
    }

    [PunRPC]
    public void ChangeIcon(int index, int thisTurn)
    {
        if (check(index, thisTurn + 1))
        {
            Debug.Log(board[0] + " " + board[1] + " " + board[2]);
            Debug.Log(board[3] + " " + board[4] + " " + board[5]);
            Debug.Log(board[6] + " " + board[7] + " " + board[8]);
            if (winner() == thisTurn + 1)
            {
                buttons[index].GetComponent<Image>().sprite = marubatsu[turn];
                SetButtonsActive(false);
                retryButton.SetActive(true);
                if (thisTurn == myTurn)
                {
                    turnText.text = "あなたの勝ちです";
                }
                else
                {
                    turnText.text = "あなたの負けです";
                }
            }
            else
            {
                buttons[index].GetComponent<Image>().sprite = marubatsu[turn];
                turn = (turn + 1) % 2;
                SetButtonsActive(turn == myTurn);
            }
        }
    }

    void SetButtonsActive(bool active)
    {
        foreach (Button button in buttons)
        {
            button.interactable = active;
        }
        turnText.text = active ? "あなたの番です" : "相手の番です";
    }

    int winner()
    {
        int count1, count2;
        for (int i = 0; i < width; i++)
        {
            count1 = 0;
            count2 = 0;
            for (int j = 0; j < width; j++)
            {
                int index = width * i + j;
                if (board[index] == 1) count1++;
                if (board[index] == 2) count2++;
            }
            if (count1 == width) return 1;
            if (count2 == width) return 2;
        }
        for (int i = 0; i < width; i++)
        {
            count1 = 0;
            count2 = 0;
            for (int j = 0; j < width; j++)
            {
                int index = width * j + i;
                if (board[index] == 1) count1++;
                if (board[index] == 2) count2++;
            }
            if (count1 == width) return 1;
            if (count2 == width) return 2;
        }
        count1 = 0;
        count2 = 0;
        for (int i = 0; i < width; i++)
        {
            if (board[i + width * i] == 1) count1++;
            if (board[i + width * i] == 2) count2++;
        }
        if (count1 == width) return 1;
        if (count2 == width) return 2;
        count1 = 0;
        count2 = 0;
        for (int i = 0; i < width; i++)
        {
            if (board[width * (i + 1) - i - 1] == 1) count1++;
            if (board[width * (i + 1) - i - 1] == 2) count2++;
        }
        if (count1 == width) return 1;
        if (count2 == width) return 2;
        return 0;
    }

    bool check(int index, int val)
    {
        int i = index / 3;
        int j = index % 3;
        if (i < 0 || j >= width || j < 0 || j >= width) return false;
        if (board[index] != 0)
        {
            return false;
        }
        else
        {
            board[index] = val;
            return true;
        }
    }

    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene("GameScene");
    }
}

