using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public TileBoard board;
    public CanvasGroup gameOver;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    
    private int score;
    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        highScoreText.text = LoadHiScore().ToString();        

        gameOver.alpha = 0f;
        gameOver.interactable = false;

        board.ClearBoard();
        board.CreateTile();
        board.CreateTile();
        board.enabled = true;
    }

    public void GameOver()
    {
        board.enabled = false;
        gameOver.interactable = false;

        board.enabled = false;
        StartCoroutine(Fade(gameOver, 1f, 1f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float to, float delay)
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0f;
        float duration = 0.5f;
        float from = canvasGroup.alpha;

        while(elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = to;
    }
    public void IncreaseScore(int points)
    {
        SetScore(score + points);
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = score.ToString();
        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int hiScore = LoadHiScore();
        if(score > hiScore)
        {
            PlayerPrefs.SetInt("hiScore", score);
        }
    }

    private int LoadHiScore()
    {
        return PlayerPrefs.GetInt("hiScore", 0);
    }


    /*public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Press Poistion " + eventData.pressPosition);
        Debug.Log("End Poistion " + eventData.position);
        Vector3 dragVectionDirection = (eventData.position - eventData.pressPosition).normalized;
        Debug.Log("Normalized Direction " + dragVectionDirection);
        GetDragDirection(dragVectionDirection);
    }
    public void OnDrag(PointerEventData eventData)
    {

    }
    private enum DragDirection
    {
        up,
        down,
        left,
        right
    }

    private DragDirection GetDragDirection(Vector3 dragVector)
    {

        float positiveX = Mathf.Abs(dragVector.x);
        float positiveY = Mathf.Abs(dragVector.y);
        DragDirection dragDir;


        if (positiveX > positiveY)
        {
            dragDir = (dragVector.x > 0) ? DragDirection.right : DragDirection.left;
        }
        else
        {
            dragDir = (dragVector.y > 0) ? DragDirection.up : DragDirection.down;
        }

        return dragDir;
    }*/
}
