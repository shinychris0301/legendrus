using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>Bounded node pool for a 500-round chapter. Planned slots never imply playable content.</summary>
    [DisallowMultipleComponent]
    public sealed class ProductionChapterMapScroller : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        public const int PlannedLevels=500, SectionSize=50;
        public const float Step=112, WindowHeight=1040, BottomNodeY=896;
        public const float MaximumOffset=(PlannedLevels-9)*Step;
        static readonly float[] PathX={395,550,385,555,400,540};
        public ProductionChapterMapView View;
        public Button CurrentButton;
        public Text CurrentLabel, RangeLabel;
        public RectTransform[] Scenery;
        public ChapterMapRouteGraphic Route;
        [SerializeField] float offset;
        float speed;Vector2 lastPoint;bool dragging,entered;int first=-1, pointerId;
        float windowHeight=WindowHeight;
        float Spacing=>Step+(windowHeight-WindowHeight)/8f;
        float BottomY=>windowHeight-(WindowHeight-BottomNodeY);
        public float MaximumScrollOffset=>(PlannedLevels-9)*Spacing;
        public float Offset=>offset;
        public void Reflow(float height)
        {
            height=Mathf.Max(WindowHeight,height);
            if(Mathf.Abs(height-windowHeight)<.01f)return;
            float levelOffset=offset/Spacing;
            windowHeight=height;speed=0;
            if(Route)Route.rectTransform.sizeDelta=new Vector2(941,height);
            SetOffset(levelOffset*Spacing);
        }
        public bool IsDragging=>dragging;
        public bool HasBindings=>View && Route && CurrentButton && CurrentLabel && RangeLabel && Scenery!=null && Scenery.Length==3;
        void OnEnable(){speed=0;dragging=false;if(HasBindings){if(Application.isPlaying&&!entered){entered=true;JumpToCurrent();}else Refresh();}}
        void OnDisable(){speed=0;dragging=false;}
        void Update()
        {
            if(!HasBindings||dragging||Mathf.Abs(speed)<1)return;
            float dt=Mathf.Min(Time.unscaledDeltaTime,.05f);
            SetOffset(offset+speed*dt);speed*=Mathf.Exp(-8*dt);
            if(offset<=0||offset>=MaximumScrollOffset)speed=0;
        }
        public void SetOffset(float value)
        {
            offset=Mathf.Clamp(value,0,MaximumScrollOffset);Refresh();
        }
        public void JumpToLevel(int level)
        {
            speed=0;SetOffset((Mathf.Clamp(level,1,PlannedLevels)-5)*Spacing);
        }
        public void JumpToCurrent()=>JumpToLevel(GameState.Instance!=null?GameState.Instance.Save.selectedLevel:1);
        public void Refresh()
        {
            if(!HasBindings)return;
            int start=Mathf.Max(1,Mathf.FloorToInt(offset/Spacing));
            bool changed=start!=first;first=start;
            // Three overlapping source-scenery tiles are reused; geometry and nodes move together.
            const float span=1507;
            int tile=Mathf.FloorToInt(offset/span);
            for(int i=0;i<Scenery.Length;i++)
                Scenery[i].anchoredPosition=new Vector2(0,-(offset-(tile+i-1)*span-400));
            for(int i=0;i<View.Nodes.Length;i++)
            {
                var node=View.Nodes[i];int level=start+i;node.LevelId=level;
                float y=BottomY-(level-1)*Spacing+offset;
                node.Root.anchoredPosition=new Vector2(PathX[(level-1)%PathX.Length]-63,-y);
                node.Root.gameObject.SetActive(level<=PlannedLevels && y>-45 && y<windowHeight-45);
            }
            if(changed||!Application.isPlaying)View.RefreshState();
            RefreshLabels();
        }
        public void RefreshLabels()
        {
            if(!HasBindings)return;
            bool en=DemoFeatureState.LanguageCode=="en";
            int low=Mathf.Clamp(Mathf.FloorToInt(offset/Spacing)+1,1,PlannedLevels-8),high=Mathf.Min(PlannedLevels,low+8);
            int section=(low-1)/SectionSize+1;
            RangeLabel.text=(en?"SECTION ":"УЧАСТОК ")+section+" / 10    ·    "+low+"–"+high+" / 500";
            if(Route)Route.SetVerticesDirty();
            CurrentLabel.text=en?"CURRENT":"К УРОВНЮ";
        }
        bool Point(PointerEventData e,out Vector2 p)=>RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform,e.position,e.pressEventCamera,out p);
        public void OnBeginDrag(PointerEventData e){if(dragging||e.button!=PointerEventData.InputButton.Left)return;pointerId=e.pointerId;dragging=true;speed=0;Point(e,out lastPoint);}
        public void OnDrag(PointerEventData e)
        {
            if(!dragging||e.pointerId!=pointerId||!Point(e,out var p))return;
            float delta=lastPoint.y-p.y;lastPoint=p;speed=delta/Mathf.Max(.01f,Time.unscaledDeltaTime);SetOffset(offset+delta);e.eligibleForClick=false;
        }
        public void OnEndDrag(PointerEventData e){if(e.pointerId==pointerId)dragging=false;}
        public void OnScroll(PointerEventData e){speed=0;SetOffset(offset+e.scrollDelta.y*Spacing*.65f);}

        public static void Install(ProductionChapterMapView view)
        {
            var area=view.MapArea;
            var hit=area.GetComponent<Image>()??area.gameObject.AddComponent<Image>();hit.color=Color.clear;hit.raycastTarget=true;
            if(!area.GetComponent<RectMask2D>())area.gameObject.AddComponent<RectMask2D>();
            area.GetComponent<RectMask2D>().softness=new Vector2Int(0,24);
            var sc=area.GetComponent<ProductionChapterMapScroller>()??area.gameObject.AddComponent<ProductionChapterMapScroller>();
            sc.View=view;view.Scroller=sc;
            sc.Scenery=new RectTransform[3];
            for(int i=0;i<3;i++)
            {
                var t=area.Find("ScrollingScenery"+i) as RectTransform??ProductionChapterMapRuntime.Rect(area,"ScrollingScenery"+i,0,0,941,1647);
                t.sizeDelta=new Vector2(941,1647);
                var art=t.GetComponent<ChapterMapSceneryGraphic>()??t.gameObject.AddComponent<ChapterMapSceneryGraphic>();
                art.texture=view.Environment.sprite.texture;art.uvRect=new Rect(0,0,1,1);art.raycastTarget=false;
                t.SetAsFirstSibling();sc.Scenery[i]=t;
            }
            var route=area.Find("LevelRoute") as RectTransform??ProductionChapterMapRuntime.Rect(area,"LevelRoute",0,0,941,1040);
            sc.Route=route.GetComponent<ChapterMapRouteGraphic>()??route.gameObject.AddComponent<ChapterMapRouteGraphic>();
            sc.Route.Scroller=sc;sc.Route.raycastTarget=false;route.SetSiblingIndex(3);
            var root=view.Content.Find("CurrentLevelControl") as RectTransform??ProductionChapterMapRuntime.Rect(view.Content,"CurrentLevelControl",741,1508,180,100);
            var image=root.GetComponent<Image>()??root.gameObject.AddComponent<Image>();image.sprite=Resources.Load<Sprite>(ProductionChapterMapSourceControls.ArtRoot+"play_blank");image.preserveAspect=true;
            sc.CurrentButton=root.GetComponent<Button>()??root.gameObject.AddComponent<Button>();sc.CurrentButton.targetGraphic=image;sc.CurrentButton.transition=Selectable.Transition.None;
            sc.CurrentLabel=Label(root,"Caption",12,17,156,42,17);
            sc.RangeLabel=Label(view.Content,"VisibleLevelRange",210,1425,520,28,20);
            sc.first=-1;sc.Refresh();
        }
        static Text Label(Transform parent,string name,float x,float y,float w,float h,int size)
        {
            var rect=parent.Find(name) as RectTransform??ProductionChapterMapRuntime.Rect(parent,name,x,y,w*2,h*2);rect.localScale=Vector3.one*.5f;rect.anchoredPosition=new Vector2(x,-y);rect.sizeDelta=new Vector2(w*2,h*2);
            var text=rect.GetComponent<Text>()??rect.gameObject.AddComponent<Text>();text.font=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/PlayerNames");text.fontSize=size*2;text.color=new Color32(245,214,153,255);text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
            return text;
        }
    }
}
