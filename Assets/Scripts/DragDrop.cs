using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class DragDrop : NetworkBehaviour
{
    public GameObject Canvas;
    //public PlayerManager PlayerManager;
    public GameManager GameManager;

    public bool isDragging = false;
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

        Unit caster = null;
        CardPlayed_e cardPlayed_e = new CardPlayed_e();

        if (dropZone.Count > 0 )
        {
            for (int i = dropZone.Count - 1; i >= 0; i--)
            {
                if ((card as Spell)?.vaildPlay.targetCaster == true)
                {
                    caster = dropZone[i].GetComponent<Unit>();
                }
                if (card is Hero) { caster = card as Unit; }
                if (caster is null)
                {
                    LaneManager lane = dropZone[i].GetComponentInParent<LaneManager>();
                    CardSlot slot = dropZone[i].GetComponentInParent<CardSlot>();
                    if (slot == null)
                    {
                        slot = lane.GetComponentInChildren<CardSlot>();
                    }
                    Unit[] casters = lane.transform.Find("PlayerSide").GetComponentsInChildren<Unit>();
                    casters = casters.Where(x => x.caster).ToArray();
                    // COLOR CHECK HERE
                    casters = casters.Where(x => !x.silenced && !x.stun).ToArray();
                    System.Array.Sort(casters, delegate(Unit m, Unit n)
                    {
                        int SortValue(Unit u)
                        {
                            int v = slot.transform.GetSiblingIndex() - u.GetCardSlot().transform.GetSiblingIndex();
                            v = Mathf.Abs(v);
                            return u.color == card.color ? v : v + 1000 ;
                        }
                        return SortValue(m) - SortValue(n);
                    });
                    if(casters.Length == 0)
                    {
                        // If there are no casters then not valid play.
                        break;
                    }
                    else
                    {
                        caster = casters[0];
                    }
                }

                if (card.IsVaildPlay(dropZone[i]))
                {
                    if (dropZone[i].GetComponent<CardSlot>() != null) { dropZone[i].GetComponent<CardSlot>().UnStage(); }
                    transform.SetParent(dropZone[i].transform, false);
                    transform.position = dropZone[i].transform.position + new Vector3(10,-10,0);
                    cardPlayed_e.card = card.gameObject;
                    cardPlayed_e.caster = caster.gameObject;
                    cardPlayed_e.lane = caster.GetLane().gameObject;
                    card.Stage(cardPlayed_e);
                    return;
                }

            }

        }
        gameObject.GetComponent<Card>().UnStage();
    }
    public void UnStage()
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
