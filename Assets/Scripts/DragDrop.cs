using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class DragDrop : NetworkBehaviour
{
    public Canvas Canvas;

    public GameManager GameManager;

    public bool isDragging = false;
    //private List<GameObject> dropZone = new List<GameObject>();
    private Card card;
    private Vector2 startPosition;
    private GameObject startParent;

    private GameObject targetingLine;


    private void Start()
    {
        Canvas = GameObject.Find("Main Canvas").GetComponent<Canvas>();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        card = gameObject.GetComponent<Card>();
        targetingLine = GameObject.Find("TargetingArrow");
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
            //transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            Vector3 start = transform.position;
            Vector3 end = Input.mousePosition;
            (targetingLine.transform as RectTransform).position = start;

            float length = Vector2.Distance(start, Input.mousePosition) / Canvas.scaleFactor;
            float angle = Vector2.SignedAngle(Vector2.up, end - start);
            (targetingLine.transform as RectTransform).sizeDelta = new Vector2(10, length);
            (targetingLine.transform as RectTransform).eulerAngles = new Vector3(0, 0, angle);
        }
        
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    dropZone.Add(collision.gameObject);
    //}

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    dropZone.Remove(collision.gameObject);
    //}

    public void StartDrag()
    {
        if (!card.isDraggable || !card.faceup) return;
        startPosition = transform.position;
        startParent = transform.parent.gameObject;
        transform.SetParent(Canvas.transform, false);
        transform.position = startPosition + 100f * Vector2.up;
        isDragging = true;
    }

    public void EndDrag()
    {
        if (!card.isDraggable || !card.faceup) return;
        isDragging = false;

        (targetingLine.transform as RectTransform).position = new Vector2(-30, 0);
        (targetingLine.transform as RectTransform).sizeDelta = new Vector2(10, 100);
        (targetingLine.transform as RectTransform).eulerAngles = new Vector3(0, 0, 0);

        Unit caster = null;
        CardPlayed_e cardPlayed_e = new CardPlayed_e();

        RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.zero, 0f);
        foreach (RaycastHit2D hit in hits)
        {
            GameObject target = hit.collider.gameObject;
            if (card.IsVaildPlay(target))
            {
                //Color check
                caster = GetCaster(target);
                if (caster is null) { break; }
                if (target.GetComponent<CardSlot>() != null) { target.GetComponent<CardSlot>().UnStage(); }
                transform.SetParent(target.transform, false);
                transform.position = target.transform.position + new Vector3(10, -10, 0);
                cardPlayed_e.card = card.gameObject;
                cardPlayed_e.caster = caster.gameObject;
                cardPlayed_e.lane = caster.GetLane().gameObject;
                card.Stage(cardPlayed_e);
                return;
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

    public Unit GetCaster(GameObject dropZone)
    {
        if (card is Hero) { return card as Unit; }

        LaneManager lane = dropZone.GetComponentInParent<LaneManager>();
        Unit[] casters = lane.transform.Find("PlayerSide").GetComponentsInChildren<Unit>();
        casters = casters.Where(x => x.caster).ToArray();
        if (!ColorCheck(casters))
        {
            return null;
        }

        if ((card as Spell)?.vaildPlay.targetCaster == true)
        {
            return dropZone.GetComponent<Unit>();
        }
        
        CardSlot slot = dropZone.GetComponentInParent<CardSlot>();
        if (slot == null)
        {
            slot = lane.GetComponentInChildren<CardSlot>();
        }

        casters = casters.Where(x => !x.silenced && !x.stun).ToArray();
        System.Array.Sort(casters, delegate (Unit m, Unit n)
        {
            int SortValue(Unit u)
            {
                int v = slot.transform.GetSiblingIndex() - u.GetCardSlot().transform.GetSiblingIndex();
                v = Mathf.Abs(v);
                return u.color == card.color ? v : v + 1000;
            }
            return SortValue(m) - SortValue(n);
        });
        if (casters.Length == 0)
        {
            // If there are no casters then not valid play.
            return null;
        }
        else
        {
            return casters[0];
        }  
    }

    public bool ColorCheck(Unit[] casters)
    {
        if( card.color == "red" ||
            card.color == "black" ||
            card.color == "blue" ||
            card.color == "green")
        {
            return casters.Any(x => card.color == x.color);
        }
        return true;
    }


}
