using UnityEngine;
using UnityEngine.UI;
/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
public class FixedContainerLayoutGroup : LayoutGroup
{
    [Range(0f, 360f)]
    public float spacing;
    public float randRotation;
    int randSeed = (int)System.DateTime.Now.Ticks;

    protected override void OnEnable() { base.OnEnable(); CalculateLayout(); }
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
        CalculateLayout();
    }
#endif
    void CalculateLayout()
    {
        System.Random rand = new System.Random(randSeed);
        m_Tracker.Clear();
        if (transform.childCount == 0)
            return;

        
        float totalsize = 0;
        float flexSpacing = spacing;
        for (int i = 0; i < transform.childCount; i++)
        {
            totalsize += transform.GetChild(i).GetComponent<RectTransform>().rect.width;
        }
        float availableSpace = rectTransform.rect.width - padding.horizontal;
        float surplusSpace = availableSpace - totalsize;
        flexSpacing = surplusSpace / (transform.childCount - 1);
        flexSpacing = Mathf.Min(flexSpacing, spacing);


        float position = GetStartOffset(0, totalsize + flexSpacing * (transform.childCount - 1));

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);
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
                float verticalPosition = (rectTransform.rect.height - childRect.rect.height) * -(GetAlignmentOnAxis(1)-0.5f)  ;
                Vector3 pos = new Vector3(position, verticalPosition, 0);
                pos.x += childRect.rect.width / 2;
                child.anchoredPosition = pos;
                float offset = childRect.rect.width + flexSpacing;
                position += offset;
                if (randRotation > 0)
                {
                    child.localRotation = Quaternion.Euler(0, 0, randRotation * ((float)rand.NextDouble() - 0.5f));
                }
                
            }
        }

    }
}