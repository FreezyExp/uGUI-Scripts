using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Problem: dynamic amount of content in a scrollable rect
///
/// uGUI with multiple resolutions is handled poorly due to the fact that we can only set fixed sizes inside.
/// When we scale the canvas to a certain resolution, we can take advantage of the additional screen space.
/// This is done by using layout elements and parameters like minimum, desired and flexible sizes.
/// 
/// But to create a scrollable rect that can handle dynamic parameters, 
/// all these would need to be fixed to get the correct layout.
/// 
/// 
/// Solution: (for horizontal or vertical layout)
/// Define a fixed size for items in the scroll direction.
/// Allow the other dimension to match the parent rect.
/// This allows for some adaptation when dealing with a scaling canvas
/// 
/// 
/// Functionality:
/// 
/// It sets the minimum size of a scrollrect (identical to parent rect)
/// Children added below the script will match the parent and item size.
/// It can stretch items if there are not enough, or it creates an invisible filler item to maintain a constant size.
/// 
/// I'm not too happy with flipping the direction of the scrollrect layout by using a filler rect.
/// There is probably another way, but for now this solves the problem.
/// 
/// Pros:
/// - Handles spacing, padding of Horizontal & Vertical Layout Group
/// - Supports canvas scaling
/// 
/// Todo:
/// - Add support for grid layout (solve empty space problems)
/// 
/// Usage: 
/// 
/// Create a Panel: 
/// Add a mask
/// Add a scrollbar (align with desired direction)
/// Add a child panel
/// 
/// Child Panel:
/// Add vertical or horizontal group
/// Set both Force expand width and height to enabled
/// 
/// Add UISizePerChild and set the desired size per child
/// The other axis will be auto scaled based on parent size
/// 
/// Your scroll rect is now ready for a dynamic amount of items
/// It will grow shrink and all that good stuff.
/// 
/// 
/// Created by Freezy 
/// http://www.ElicitIce.nl
/// http://forum.unity3d.com/members/freezy.118499/ 
/// 
/// Source available on: 
/// https://github.com/FreezyExp/uGUI-SizePerChild
/// If you make any usefull changes please share them with the community.
/// 
/// </summary>

[AddComponentMenu( "Layout/Size Per Child", 141 )]
[ExecuteInEditMode]
[RequireComponent( typeof( RectTransform ) )]
public class UISizePerChild : UIBehaviour, ILayoutController, ILayoutSelfController {

    [SerializeField]
    protected RectTransform.Axis direction;

    [SerializeField]
    protected float m_SizePerChild;

    [SerializeField]
    protected bool m_StretchItemsToParent = true;

    [Header( "Debug output" )]
    [SerializeField]
    protected Vector2 m_MinimumSize;

    private RectTransform m_Rect;
    private RectTransform m_RectParent;

    //Not saved
    private LayoutElement m_Filler;

    protected override void Start() {
        OnValidate();
    }

    protected override void OnEnable() {
        base.OnEnable();
        OnValidate();
    }

    protected override void OnRectTransformDimensionsChange() {
        base.OnRectTransformDimensionsChange();
        OnValidate();
    }

    protected override void OnValidate() {
        //Call the base validation
        base.OnValidate();
        if( enabled ) {
            Setup();
        }
    }

    void Setup() {
        //It's easier if we just do this once in a while
        //I found that on some edge cases OnValidate is called before Start
        m_Rect = (RectTransform)transform;
        m_RectParent = (RectTransform)transform.parent;

        //Force anchors and pivot to top left
        m_Rect.anchorMin = Vector2.zero;
        m_Rect.anchorMax = Vector2.zero;
        m_Rect.pivot = Vector2.zero;

        //usefull for debugging
        if( m_StretchItemsToParent )
            m_MinimumSize = m_RectParent.sizeDelta;
        else {
            if( direction == RectTransform.Axis.Horizontal ) {
                m_MinimumSize.x = 0f;
                m_MinimumSize.y = m_RectParent.sizeDelta.y;
            } else {
                m_MinimumSize.x = m_RectParent.sizeDelta.x;
                m_MinimumSize.y = 0f;
            }
        }

        HandleFiller();

        SetDirty();
    }

    protected void SetDirty() {
        Vector2 setsize = m_RectParent.sizeDelta;
        //Vector2 position = Vector3.zero;

        int cnt = m_Rect.childCount;
        if( m_Filler != null )
            cnt--;

        float size = m_SizePerChild * cnt;

        //this makes the usecase easier for external scripts that read the children
        //now you just need to check the last child for validity
        if( m_Filler )
            m_Filler.transform.SetAsLastSibling();

        if( m_StretchItemsToParent ) {
            //in case you toggle on runtime
        } else {
            //hide filler in case we have enough items or none at all
            if( cnt == 0 || size >= (direction == RectTransform.Axis.Horizontal ? setsize.x : setsize.y) ) {
                ShowFiller( false );
            } else {
                ShowFiller( true );

                //set the fill to the correct axis
                if( direction == RectTransform.Axis.Horizontal ) {
                    m_Filler.preferredHeight = float.NaN;
                    m_Filler.preferredWidth = m_RectParent.sizeDelta.x - size - m_SizePerChild;
                } else {
                    m_Filler.preferredHeight = m_RectParent.sizeDelta.y - size - m_SizePerChild;
                    m_Filler.preferredWidth = float.NaN;
                }
            }
        }

        if( direction == RectTransform.Axis.Horizontal ) {
            setsize.x = Mathf.Max( setsize.x, size );
        } else {
            setsize.y = Mathf.Max( setsize.y, size );
        }

        //already handles scaling
        m_Rect.SetSizeWithCurrentAnchors( RectTransform.Axis.Horizontal, setsize.x );
        m_Rect.SetSizeWithCurrentAnchors( RectTransform.Axis.Vertical, setsize.y );
    }

    public void SetLayoutHorizontal() {
        if( enabled ) {
            HandleFiller();
            SetDirty();
        }
    }

    public void SetLayoutVertical() {
        SetLayoutHorizontal();
    }

    private void HandleFiller() {
        if( m_StretchItemsToParent ) {
            ShowFiller( false );
        } else if( m_Filler == null && m_Rect.childCount > 0 ) {
            m_Filler = new GameObject( "empty filler" ).AddComponent<LayoutElement>();
            m_Filler.transform.SetParent( transform );
            m_Filler.transform.localScale = Vector3.one;
            m_Filler.gameObject.hideFlags = HideFlags.HideAndDontSave;
        }
    }

    private void ShowFiller( bool set ) {
        if( m_Filler != null && set != m_Filler.IsActive() )
            m_Filler.gameObject.SetActive( set );
    }
}
