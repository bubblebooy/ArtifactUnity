using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesManager : MonoBehaviour
{
    public GameObject abilities;
    private GameObject Canvas;
    private GridLayoutGroup abilitiesGridLayoutGroup;

    public float expandedWidth = 75.0f;
    private Vector2 collapsedSizeDelta;
    private Vector2 collapsedAnchoredPosition;

    private int sibIndex;

    public bool expanded = false;

    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        abilities = transform.Find("Color/Abilities").gameObject;
        abilitiesGridLayoutGroup = abilities.GetComponent<GridLayoutGroup>();

        collapsedSizeDelta = abilities.GetComponent<RectTransform>().sizeDelta;
        collapsedAnchoredPosition = abilities.GetComponent<RectTransform>().anchoredPosition;
    }

    public void CardUpdate()
    {
        Collapse();
        foreach (Ability ability in abilities.GetComponentsInChildren<Ability>())
        {
            ability.CardUpdate();
        }
    }

    public void OnPlay()
    {
        foreach (Ability ability in abilities.GetComponentsInChildren<Ability>())
        {
            ability.OnPlay();
        }
    }

    public void PlacedOnTopOf(Unit unit)
    {
        foreach (Ability ability in abilities.GetComponentsInChildren<Ability>())
        {
            ability.PlacedOnTopOf(unit);
        }
    }

    public void RoundStart()
    {
        foreach (Ability ability in abilities.GetComponentsInChildren<Ability>())
        {
            ability.RoundStart();
        }
    }

    public void OnActivate(int abilityIndex)
    {
        abilities.transform.GetChild(abilityIndex).GetComponent<ActiveAbility>().OnActivate();
    }

    public void OnClick()
    {
        
        if (expanded)
        {
            Collapse();
        }
        else if (GetComponentInParent<LaneManager>() != null && !GetComponent<Card>().staged)  // transform.IsChildOf(Canvas.transform.find("Board")
        {
            Expand();
        }
        
    }

    private void Expand()
    {
        expanded = true;
        sibIndex = abilities.transform.GetSiblingIndex();
        //abilities.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.8f);
        abilities.GetComponent<RectTransform>().sizeDelta = new Vector2(expandedWidth, GetComponent<RectTransform>().sizeDelta.y);  // 75,95 -> 75,120
        abilities.GetComponent<RectTransform>().anchoredPosition = new Vector2(-1 * expandedWidth, 0.0f);
        abilities.transform.SetParent(Canvas.transform, true);

        abilitiesGridLayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
        abilitiesGridLayoutGroup.childAlignment = TextAnchor.UpperLeft;
        abilitiesGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Vertical;
        abilitiesGridLayoutGroup.cellSize = new Vector2(expandedWidth - 2*abilitiesGridLayoutGroup.spacing.x, 15);
        foreach (Ability ability in abilities.GetComponentsInChildren<Ability>())
        {
            ability.Expand();
        }
    }

    public void Collapse()
    {
        expanded = false;
        abilities.transform.SetParent(transform.Find("Color"),false);
        abilities.transform.SetAsLastSibling();//.SetSiblingIndex(sibIndex);
        //abilities.GetComponent<Image>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
        abilities.GetComponent<RectTransform>().sizeDelta = collapsedSizeDelta;
        abilities.GetComponent<RectTransform>().anchoredPosition = collapsedAnchoredPosition;

        abilitiesGridLayoutGroup.startCorner = GridLayoutGroup.Corner.LowerLeft;
        abilitiesGridLayoutGroup.childAlignment = TextAnchor.LowerLeft;
        abilitiesGridLayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
        abilitiesGridLayoutGroup.cellSize = new Vector2(15, 15);

        foreach (Ability ability in abilities.GetComponentsInChildren<Ability>())
        {
            ability.Collapse();
        }
    }
}
