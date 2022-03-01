using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int Width;
    public int Height;
    public PlayerController Player;
    public Sprite BoundarySprite;
    public TextMeshProUGUI ScoreLabel;
    public Image Fader;
    public GameObject GameOverText;
    public GameObject TryAgainText;
    public GameObject PressAnyKeyText;

    private int points = 0;
    private GameObject currentPoint;
    private List<Vector3> grid = new List<Vector3>();
    private List<Vector3> openCells = new List<Vector3>();

    //start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Height - 1; i++)
        {
            for (int j = 0; j < Width - 1; j++)
            {
                grid.Add(new Vector3(j - (Width / 2) + 1, i - (Height / 2) + 1, 0));
            }
        }
        InitBoundary();
        SpawnNewPickup();
    }

    //called whenever the player executes a move
    public void PlayerMove()
    {
        int playerX = Mathf.RoundToInt(Player.transform.position.x);
        int playerY = Mathf.RoundToInt(Player.transform.position.y);

        //check if the player overlaps with a boundary
        if (playerX >= (Width / 2) || playerX <= (-Width / 2) || playerY >= (Height / 2) || playerY <= (-Height / 2))
        {
            Player.StopAllCoroutines();
            StartCoroutine(StartGameOver());
            return;
        }

        //check if the player overlaps with their own body
        List<Vector3> segments = Player.GetSegmentPositions();
        foreach (Vector3 segment in segments)
        {
            if (playerX == Mathf.RoundToInt(segment.x) && playerY == Mathf.RoundToInt(segment.y))
            {
                Player.StopAllCoroutines();
                StartCoroutine(StartGameOver());
                return;
            }
        }

        var result = grid.Except(segments);
        openCells = result.ToList();

        //check if the player's head overlaps with the current point
        bool xEquals = playerX == Mathf.RoundToInt(currentPoint.transform.position.x);
        bool yEquals = playerY == Mathf.RoundToInt(currentPoint.transform.position.y);
        if (xEquals && yEquals)
        {
            points++;
            ScoreLabel.text = points.ToString();
            if (points % 5 == 0)
            {
                Player.IncreaseSpeed();
            }
            Player.Grow();
            SpawnNewPickup();
        }
    }

    private void SpawnNewPickup()
    {
        if (currentPoint == null)
        {
            currentPoint = new GameObject("POINT");
            currentPoint.AddComponent<SpriteRenderer>().sprite = BoundarySprite;
            currentPoint.GetComponent<SpriteRenderer>().color = Color.red;
            currentPoint.transform.position = new Vector3(0, Height / 4);
        }
        else
        {
            currentPoint.transform.position = openCells[Random.Range(0, openCells.Count - 1)];
        }
    }

    private IEnumerator StartGameOver()
    {
        yield return new WaitForSeconds(2);

        Fader.color = new Color(0, 0, 0, .25f);
        yield return new WaitForSeconds(.5f);

        Fader.color = new Color(0, 0, 0, .5f);
        yield return new WaitForSeconds(.5f);

        Fader.color = new Color(0, 0, 0, .75f);
        yield return new WaitForSeconds(.5f);

        Fader.color = Color.black;
        yield return new WaitForSeconds(2);

        GameOverText.SetActive(true);
        yield return new WaitForSeconds(1);

        GameOverText.SetActive(true);
        TryAgainText.SetActive(true);
        PressAnyKeyText.SetActive(true);

        while (!Input.anyKeyDown)
        {
            yield return null;
        }

        Player.ResetPlayer();
        points = 0;
        ScoreLabel.text = points.ToString();
        currentPoint.transform.position = new Vector3(0, Height / 4);
        Fader.color = Color.clear;
        GameOverText.SetActive(false);
        TryAgainText.SetActive(false);
        PressAnyKeyText.SetActive(false);
    }

    private void InitBoundary()
    {
        GameObject square;

        //Top boundary
        for (int i = 0; i <= Width; i++)
        {
            square = new GameObject("Boundary Square Top");
            square.AddComponent<SpriteRenderer>().sprite = BoundarySprite;
            square.transform.position = new Vector3((-Width / 2) + i, (Height / 2));
        }

        //Bottom boundary
        for (int i = 0; i <= Width; i++)
        {
            square = new GameObject("Boundary Square Bottom");
            square.AddComponent<SpriteRenderer>().sprite = BoundarySprite;
            square.transform.position = new Vector3((-Width / 2) + i, (-Height / 2));
        }

        //Left boundary
        for (int i = 1; i < Height; i++)
        {
            square = new GameObject("Boundary Square Left");
            square.AddComponent<SpriteRenderer>().sprite = BoundarySprite;
            square.transform.position = new Vector3((-Width / 2), (-Height / 2) + i);
        }

        //Right boundary
        for (int i = 1; i < Height; i++)
        {
            square = new GameObject("Boundary Square Right");
            square.AddComponent<SpriteRenderer>().sprite = BoundarySprite;
            square.transform.position = new Vector3((Width / 2), (-Height / 2) + i);
        }
    }
}
