using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using IC.BEMCheckBox.Contracts;
using UIKit;

namespace IC.BEMCheckBox
{
    public class BEMCheckBox : UIView, IBEMCheckBox
    {
        #region Fields

        private bool _on;
        private float _animationDuration;
        private BEMBoxType _boxType;
        private float _lineWidth;
        private BEMAnimationType _offAnimationType;
        private BEMAnimationType _onAnimationType;
        private UIColor _tintColor;
        private UIColor _onTintColor;
        private UIColor _onFillColor;
        private UIColor _onCheckColor;

        /// <summary>
        /// The layer where the box is drawn when the check box is set to On.
        /// </summary>
        private CAShapeLayer _onBoxLayer;

        /// <summary>
        /// The layer where the box is drawn when the check box is set to Off.
        /// </summary>
        private CAShapeLayer _offBoxLayer;

        /// <summary>
        /// The layer where the check mark is drawn when the check box is set to On.
        /// </summary>
        private CAShapeLayer _checkMarkLayer;

        /// <summary>
        /// The BEMAnimationManager object used to generate animations.
        /// </summary>
        private BEMAnimationManager _animationManager;

        /// <summary>
        /// The BEMPathManager object used to generate paths.
        /// </summary>
        private BEMPathManager _pathManager;

        #endregion

        public bool On
        {
            get { return _on; }
            set { SetOn(value); }
        }

        public float LineWidth
        {
            get { return _lineWidth; }
            set
            {
                _lineWidth = value;
                _pathManager.LineWidth = _lineWidth;
            }
        }

        public float AnimationDuration
        {
            get { return _animationDuration; }
            set
            {
                _animationDuration = value;
                _animationManager.AnimationDuration = _animationDuration;
            }
        }

        public bool HideBox { get; set; }

        public UIColor OnTintColor
        {
            get { return _onTintColor; }
            set
            {
                _onTintColor = value;
                Reload();
            }
        }

        public UIColor OnFillColor
        {
            get { return _onFillColor; }
            set
            {
                _onFillColor = value;
                Reload();
            }
        }

        public UIColor OnCheckColor
        {
            get { return _onCheckColor; }
            set
            {
                _onCheckColor = value;
                Reload();
            }
        }

        public override UIColor TintColor
        {
            get { return _tintColor; }
            set
            {
                _tintColor = value;
                DrawOffBox();
            }
        }

        public BEMBoxType BoxType
        {
            get { return _boxType; }
            set
            {
                _boxType = value;
                _pathManager.BoxType = _boxType;
            }
        }

        public BEMAnimationType OnAnimationType
        {
            get { return _onAnimationType; }
            set { _onAnimationType = value; }
        }

        public BEMAnimationType OffAnimationType
        {
            get { return _offAnimationType; }
            set { _offAnimationType = value; }
        }

        public CGSize MinimumTouchSize { get; set; }

        public BEMCheckBox(CGRect frame) : base(frame)
        {
            CommonInit();
        }

        public BEMCheckBox(NSCoder coder) : base(coder)
        {
            CommonInit();
        }

        public void SetOn(bool on, bool animated = false)
        {
            _on = on;
            DrawEntireCheckBox();

            if (on)
            {
                if (animated)
                {
                    AddOnAnimation();
                }
            }
            else
            {
                if (animated)
                {
                    AddOffAnimation();
                }
                else
                {
                    _onBoxLayer.RemoveFromSuperLayer();
                    _checkMarkLayer.RemoveFromSuperLayer();
                }
            }
        }

        public void Reload()
        {
            _offBoxLayer.RemoveFromSuperLayer();
            _offBoxLayer = null;

            _onBoxLayer.RemoveFromSuperLayer();
            _onBoxLayer = null;

            _checkMarkLayer.RemoveFromSuperLayer();
            _checkMarkLayer = null;

            SetNeedsDisplay();
            LayoutIfNeeded();
        }

        public override void LayoutSubviews()
        {
            _pathManager.Size = (float) Frame.Size.Height;
            base.LayoutSubviews();
        }

        public override void DrawRect(CGRect area, UIViewPrintFormatter formatter)
        {
            SetOn(On, false);
        }

        /// <summary>
        /// Increase touch area
        /// </summary>
        public override bool PointInside(CGPoint point, UIEvent uievent)
        {
            var found = base.PointInside(point, uievent);

            var minimumSize = MinimumTouchSize;
            var width = Bounds.Size.Width;
            var height = Bounds.Size.Height;

            if (found == false && (width < minimumSize.Width || height < minimumSize.Height))
            {
                var increaseWidth = minimumSize.Width - width;
                var increaseHeight = minimumSize.Height - height;

                var rect = CGRect.Inflate(Bounds, (-increaseWidth/2), (-increaseHeight/2));
                found = new CGRect(rect.X, rect.Y, rect.Width, rect.Height).Contains(point);
            }

            return found;
        }

        #region Private Methods

        private void CommonInit()
        {
            // Default values
            On = false;
            HideBox = false;
            OnTintColor = UIColor.FromRGBA(0, 122, 255, 1);
            OnFillColor = UIColor.Clear;
            OnCheckColor = UIColor.FromRGBA(0, 122, 255, 1);
            TintColor = UIColor.LightGray;
            LineWidth = 2.0f;
            AnimationDuration = 0.5f;
            MinimumTouchSize = new CGSize(44, 44);
            OnAnimationType = BEMAnimationType.Stroke;
            OffAnimationType = BEMAnimationType.Stroke;
            BackgroundColor = UIColor.Clear;

            InitPathManager();
            InitAnimationManager();

            AddGestureRecognizer(new UITapGestureRecognizer(HandleTapCheckBox));
        }

        private void InitPathManager()
        {
            _pathManager = new BEMPathManager();
            _pathManager.LineWidth = LineWidth;
            _pathManager.BoxType = BoxType;
        }

        private void InitAnimationManager()
        {
            _animationManager = new BEMAnimationManager(AnimationDuration);
        }

        private void HandleTapCheckBox()
        {

        }

        #region Drawings

        /// <summary>
        /// Draws the entire checkbox, depending on the current state of the on property.
        /// </summary>
        private void DrawEntireCheckBox()
        {
            if (!HideBox)
            {
                if (_offBoxLayer == null || _offBoxLayer.Path.BoundingBox.Size.Height == 0.0f)
                {
                    DrawOffBox();
                }
                if (On)
                {
                    DrawOnBox();
                }
            }
            if (On)
            {
                DrawCheckMark();
            }
        }

        /// <summary>
        /// Draws the box used when the checkbox is set to Off.
        /// </summary>
        private void DrawOffBox()
        {
            _offBoxLayer.RemoveFromSuperLayer();
            _offBoxLayer = new CAShapeLayer()
            {
                Frame = Bounds,
                Path = _pathManager.PathForBox().CGPath,
                FillColor = UIColor.Clear.CGColor,
                StrokeColor = TintColor.CGColor,
                LineWidth = LineWidth,
                RasterizationScale = 2*UIScreen.MainScreen.Scale,
                ShouldRasterize = true
            };
            Layer.AddSublayer(_offBoxLayer);
        }

        /// <summary>
        /// Draws the box when the checkbox is set to On.
        /// </summary>
        private void DrawOnBox()
        {
            _onBoxLayer.RemoveFromSuperLayer();
            _onBoxLayer = new CAShapeLayer()
            {
                Frame = Bounds,
                Path = _pathManager.PathForBox().CGPath,
                LineWidth = LineWidth,
                FillColor = OnFillColor.CGColor,
                StrokeColor = OnTintColor.CGColor,
                RasterizationScale = 2*UIScreen.MainScreen.Scale,
                ShouldRasterize = true
            };
            Layer.AddSublayer(_onBoxLayer);
        }

        /// <summary>
        /// Draws the check mark when the checkbox is set to On.
        /// </summary>
        private void DrawCheckMark()
        {
            _checkMarkLayer.RemoveFromSuperLayer();
            _checkMarkLayer = new CAShapeLayer()
            {
                Frame = Bounds,
                Path = _pathManager.PathForCheckMark().CGPath,
                StrokeColor = OnCheckColor.CGColor,
                LineWidth = LineWidth,
                FillColor = UIColor.Clear.CGColor,
                LineCap = CAShapeLayer.CapRound,
                LineJoin = CAShapeLayer.JoinRound,

                RasterizationScale = 2*UIScreen.MainScreen.Scale,
                ShouldRasterize = true
            };
            Layer.AddSublayer(_checkMarkLayer);
        }

        #endregion

        #region Animations

        #endregion

        #endregion
    }
}