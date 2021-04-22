using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragDrop : NetworkBehaviour
{
    public GameObject Canvas;
    //public PlayerManager PlayerManager;
    public GameManager GameManager;

    private bool isDragging = false;
    private List<GameObject> dropZone = new List<GameObject>();
    private Card card;
    private Vector2 startPosition;
    private GameObject startParent;


    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        card = gameObject.GetComponent<Card>();
        //NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        //PlayerManager = networkIdentity.GetComponent<PlayerManager>();

        if (!hasAuthority)
        {
            card.isDraggable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);  
        }
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dropZone.Add(collision.gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        dropZone.Remove(collision.gameObject);
    }

    public void StartDrag()
    {
        if (!card.isDraggable) return;
        startPosition = transform.position;
        startParent = transform.parent.gameObject;
        transform.SetParent(Canvas.transform, false);
        isDragging = true;
    }

    public void EndDrag()
    {
        if (!card.isDraggable) return;
        isDragging = false;
        if (dropZone.Count > 0 )
        {
            for (int i = dropZone.Count - 1; i >= 0; i--)
            {
                if(gameObject.GetComponent<Card>().IsVaildPlay(dropZone[i]))
                {
                    if (dropZone[i].GetComponent<CardSlot>() != null) { dropZone[i].GetComponent<CardSlot>().UnStage(); }
                    transform.SetParent(dropZone[i].transform, false);
                    transform.position = dropZone[i].transform.position + new Vector3(10,-10,0);
                    gameObject.GetComponent<Card>().Stage();
                    return;
                }

            }

        }

        gameObject.GetComponent<Card>().unStage();

    }
    public void unStage()
    {
        if (gameObject.GetComponent<Hero>() != null)
        {
            transform.SetParent(GameObject.Find(hasAuthority ? "PlayerFountain" : "EnemyFountain").transform, false);
        }
        else
        {
            transform.SetParent(GameObject.Find(hasAuthority ? "PlayerArea" : "EnemyArea").transform, false);
        }
        //transform.SetParent(startParent.transform, false);
        //transform.position = startPosition;
    }


}
