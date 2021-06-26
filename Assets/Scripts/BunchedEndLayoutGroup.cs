using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BunchedEndLayoutGroup : LayoutGroup
{
    public float minSpacing;
    public float maxSpacing;
    [Range(0f, 1f)]
    public float scrunch;
    [Range(-1f, 1f)]
    public float verticalFan;
    public float randRotation;
    public float scrollOffest;
    int randSeed = (int)System.DateTime.Now.Ticks;
    System.Random rand;

    protected override void Start()
    {
        base.Start();
        rand = new System.Random(randSeed);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        CalculateLayout();
    }
    public override void SetLayoutHorizontal()
    {
    }
    public override void SetLayoutVertical()
    {
    }
    public override void CalculateLayoutInputVertical()
    {
        //CalculateLayout();
    }
    public override void CalculateLayoutInputHorizontal()
    {
        CalculateLayout();
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        rand = new System.Random(randSeed);
        CalculateLayout();
    }
#endif

    void CalculateLayout()
    {
        m_Tracker.Clear();
        Transform[] children = transform.Cast<Transform>()
            .Where(transform => transform.GetComponent<LayoutElement>()?.ignoreLayout != true &&
            transform.gameObject.activeInHierarchy).ToArray();
        if (children.Length == 0)
            return;

        float totalsize = 0;
        float flexSpacing = minSpacing;
        for (int i = 0; i < children.Length; i++)
        {
            totalsize += children[i].GetComponent<RectTransform>().rect.width;
        }
        float availableSpace = rectTransform.rect.width;
        float surplusSpace = availableSpace - totalsize -.01f;
        flexSpacing = surplusSpace / (children.Length - 1);
        flexSpacing = Mathf.Max(flexSpacing, minSpacing);
        flexSpacing = Mathf.Min(flexSpacing, maxSpacing);


        float position = GetStartOffset(0, totalsize + flexSpacing * (children.Length - 1)) + .001f;
        if((totalsize + flexSpacing * (children.Length - 1)) > availableSpace)
        {
            scrollOffest = Mathf.Max(scrollOffest, position - 1f);
            scrollOffest = Mathf.Min(scrollOffest, -position + 1f);
            position += scrollOffest;
        }


        for (int i = 0; i < children.Length; i++)
        {
            RectTransform child = (RectTransform)children[i];
            if (child != null)
            {
                //Adding the elements to the tracker stops the user from modifiying their positions via the editor.
                m_Tracker.Add(this, child,
                    DrivenTransformProperties.Anchors |
                    DrivenTransformProperties.AnchoredPosition |
                    DrivenTransformProperties.Pivot);
                RectTransform childRect = child.GetComponent<RectTransform>();
                child.anchorMin = child.anchorMax = Vector2.up / 2;
                child.pivot = new Vector2(0.5f, 0.5f);
                float verticalPosition = (rectTransform.rect.height - childRect.rect.height) * -(GetAlignmentOnAxis(1) - 0.5f);
                float bunchedPosition = position;
                if(position < padding.left)
                {
                    bunchedPosition = padding.left;
                    bunchedPosition += (position - bunchedPosition) * scrunch;
                    verticalPosition += (position - bunchedPosition) * verticalFan;
                }
                if (position > availableSpace - childRect.rect.width + padding.right)
                {
                    bunchedPosition = availableSpace - childRect.rect.width + padding.right;
                    bunchedPosition += (position - bunchedPosition) * scrunch;
                    verticalPosition -= (position - bunchedPosition) * verticalFan;
                }
                
                if (randRotation > 0 && (position <= padding.left || position >= availableSpace - childRect.rect.width + padding.right))
                {
                    child.localRotation = Quaternion.Euler(0, 0, randRotation * ((float)rand.NextDouble() - 0.5f));
                }
                else
                {
                    child.localRotation = Quaternion.Euler(0, 0, 0);
                }
                Vector3 pos = new Vector3(bunchedPosition, verticalPosition, 0);
                pos.x += childRect.rect.width / 2;
                child.anchoredPosition = pos;
                float offset = childRect.rect.width + flexSpacing;
                position += offset;
                

            }
        }

    }
}