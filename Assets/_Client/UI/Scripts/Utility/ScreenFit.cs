using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [ExecuteInEditMode]
    public class ScreenFit : UIBehaviour
    {
        public static ScreenFit Ins { get; private set; }

        public Canvas Canvas;

        [Tooltip("支持分辨率比例最高")]
        public Vector2 Tall = new Vector2(1125, 2436);

        [Tooltip("支持分辨率比例最宽")]
        public Vector2 Wide = new Vector2(3, 4);

        [Tooltip("是否需要底部留出安全区")]
        public bool BottomMargin = true;

        [Tooltip("游戏实际渲染区域")]
        public RectTransform GameArea;

        [Tooltip("安全区，可以为空")]
        public RectTransform SafeArea;

        public RectTransform LeftEdge;
        public RectTransform RightEdge;
        public RectTransform TopEdge;
        public RectTransform BottomEdge;

        private Vector2 lastSize;

        protected override void Awake()
        {
            Ins = this;
        }

        protected override void OnEnable()
        {
            Invoke(nameof(OnRectTransformDimensionsChange), 0);
        }

        protected override void OnDestroy()
        {
            if (Ins == this)
            {
                Ins = null;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                lastSize = Vector2.zero;
                OnRectTransformDimensionsChange();
            }
        }

        public static Vector2 GetMainGameViewSize()
        {
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var T = Type.GetType("UnityEditor.GameView,UnityEditor");
                var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static;
                var GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", flag);
                var Res = GetSizeOfMainGameView.Invoke(null, null);
                return (Vector2)Res;
            }
            else
            {
                return new Vector2(Screen.width, Screen.height);
            }
        }
#else
        public static Vector2 GetMainGameViewSize() {
            return new Vector2(Screen.width, Screen.height);
        }
#endif

        private bool Check()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return false;
            }
#elif UNITY_IOS
            //IOS第一帧获取到的安全区是错的
            if (Time.frameCount == 0) {
                return false;
            }
#endif
            if (Canvas == null || GameArea == null)
            {
                return false;
            }
            if (LeftEdge == null || RightEdge == null || TopEdge == null || BottomEdge == null)
            {
                return false;
            }
            if (Tall.y / Tall.x < Wide.y / Wide.x)
            {
                Debug.LogError("ScreenFit: Tall rate < Wide rate!");
                return false;
            }
            return true;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            if (!Check())
            {
                return;
            }

            var size = GetMainGameViewSize();

            if (lastSize == size)
            {
                return;
            }
            lastSize = size;
            Debug.Log($"ScreenFit: {size.x} x {size.y}");

            var radio = size.y / size.x;
            var design = Canvas.GetComponent<CanvasScaler>().referenceResolution;
            var r = design.y / design.x;

            LeftEdge.gameObject.SetActive(false);
            RightEdge.gameObject.SetActive(false);
            TopEdge.gameObject.SetActive(false);
            BottomEdge.gameObject.SetActive(false);

            float scale;
            if (radio > r)
            {
                //屏幕比较高，按宽度适配，高度扩展
                r = Tall.y / Tall.x;
                scale = design.x / size.x;
                size *= scale;
                if (radio >= r)
                {
                    //超过最高比例，上下切边
                    var h = (float)Math.Ceiling((size.y - size.x * r) / 2);
                    TopEdge.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
                    TopEdge.gameObject.SetActive(true);
                    BottomEdge.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
                    BottomEdge.gameObject.SetActive(true);
                    GameArea.sizeDelta = new Vector2(size.x, size.x * r);
                }
                else
                {
                    GameArea.sizeDelta = size;
                }
            }
            else
            {
                //屏幕比较宽，按高度适配，宽度扩展
                scale = design.y / size.y;
                size *= scale;
                r = Wide.y / Wide.x;
                if (radio < r)
                {
                    //超过最宽比例，左右切边
                    var w = (float)Math.Ceiling((size.x - size.y / r) / 2);
                    LeftEdge.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
                    LeftEdge.gameObject.SetActive(true);
                    RightEdge.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
                    RightEdge.gameObject.SetActive(true);
                    GameArea.sizeDelta = new Vector2(size.y / r, size.y);
                }
                else
                {
                    GameArea.sizeDelta = size;
                }
            }

            if (SafeArea)
            {
                var safeRect = GetSafeAreaRect(scale, GameArea.rect);
                SafeArea.anchoredPosition = safeRect.center;
                if (safeRect.width >= design.x && safeRect.height >= design.y)
                {
                    //双边长度都足够，扩展SafeArea
                    SafeArea.localScale = new Vector3(1, 1, 0);
                    SafeArea.sizeDelta = safeRect.size;
                }
                else
                {
                    //有一边长度不够，需要找出缩放比例最小的边
                    var scaleX = safeRect.width / design.x;
                    var scaleY = safeRect.height / design.y;
                    if (scaleX < scaleY)
                    {
                        SafeArea.localScale = new Vector3(scaleX, scaleX, 0);
                        SafeArea.sizeDelta = new Vector2(design.x, safeRect.height / scaleX);
                    }
                    else
                    {
                        SafeArea.localScale = new Vector3(scaleY, scaleY, 0);
                        SafeArea.sizeDelta = new Vector2(safeRect.width / scaleY, design.y);
                    }
                }
            }
        }

        protected Rect GetSafeAreaRect(float scale, Rect gameRect)
        {
            var screen = GetMainGameViewSize();

#if UNITY_EDITOR
            //数据取自iPhoneX
            var rect = new Rect(Vector2.zero, screen);
            const float w = 1125f;
            const float h = 2436f;
            if (Math.Abs(screen.y / screen.x - h / w) <= 0.01f)
            {
                //竖屏
                var header = 132f / h * screen.y;
                var footer = 102f / h * screen.y;
                rect = Rect.MinMaxRect(rect.xMin, rect.yMin + footer, rect.xMax, rect.yMax - header);
                Debug.Log($"Simulated SafeArea: {rect}");
            }
            else if (Math.Abs(screen.x / screen.y - h / w) <= 0.01f)
            {
                //横屏
                var header = 132f / h * screen.x;
                var footer = 63f / h * screen.x;
                rect = Rect.MinMaxRect(rect.xMin + header, rect.yMin + footer, rect.xMax - header, rect.yMax);
                Debug.Log($"Simulated SafeArea: {rect}");
            }
#else
            var rect = Screen.safeArea;
            Debug.Log($"SafeArea: {rect}");
#endif
            if (!BottomMargin && rect.yMin > 0)
            {
                rect = Rect.MinMaxRect(rect.xMin, 0, rect.xMax, rect.yMax);
            }

            var p = rect.position - screen / 2;
            rect = new Rect(p * scale, rect.size * scale);

            if (!gameRect.Overlaps(rect))
            {
                return Rect.zero;
            }

            var xMin = Math.Max(rect.xMin, gameRect.xMin);
            var yMin = Math.Max(rect.yMin, gameRect.yMin);
            var xMax = Math.Min(rect.xMax, gameRect.xMax);
            var yMax = Math.Min(rect.yMax, gameRect.yMax);
            rect = Rect.MinMaxRect(xMin, yMin, xMax, yMax);

            return rect;
        }
    }
}
