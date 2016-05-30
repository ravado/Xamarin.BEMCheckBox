using System;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using IC.BEMCheckBox.Contracts;
using UIKit;

namespace IC.BEMCheckBox
{
    public class BEMCheckBox : UIView, IBEMCheckBox
    {
        const int NsecPerSec = 1000000000;

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

        private CAAnimationDelegate _animationDelegate;

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
            _animationDelegate = new BEMCheckBoxAnimationDelegate(this);

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

        private void AddOnAnimation()
        {
            if (_animationDuration <= 0)
            {
                return;
            }

            switch (OnAnimationType)
            {
                case BEMAnimationType.Stroke:
                {
                    var animation = _animationManager.StrokeAnimationReverse(false);
                    _onBoxLayer.AddAnimation(animation, "strokeEnd");
                    animation.Delegate = _animationDelegate;
                    _checkMarkLayer.AddAnimation(animation, "strokeEnd");
                    break;
                }
                case BEMAnimationType.Fill:
                {
                    var wiggle = _animationManager.FillAnimationWithBounces(1, 0.18f, false);
                    var opacityAnimation = _animationManager.OpacityAnimationReverse(false);
                    opacityAnimation.Delegate = _animationDelegate;
                    _onBoxLayer.AddAnimation(wiggle, "transform");
                    _checkMarkLayer.AddAnimation(opacityAnimation, "opacity");
                    break;
                }
                case BEMAnimationType.Bounce:
                {
                    var amplitude = (float) (BoxType == BEMBoxType.Square ? 0.20 : 0.35);
                    var wiggle = _animationManager.FillAnimationWithBounces(1, amplitude, false);
                    wiggle.Delegate = _animationDelegate;
                    var opacity = _animationManager.OpacityAnimationReverse(false);
                    opacity.Duration = AnimationDuration/1.4;
                    _onBoxLayer.AddAnimation(opacity, "opacity");
                    _checkMarkLayer.AddAnimation(wiggle, "transform");
                    break;
                }
                case BEMAnimationType.Flat:
                {
                    var morphAnimation = _animationManager.MorphAnimationFromPath(_pathManager.PathForFlatCheckMark(),
                        _pathManager.PathForCheckMark());
                    morphAnimation.Delegate = _animationDelegate;
                    var opacity = _animationManager.OpacityAnimationReverse(false);
                    opacity.Duration = AnimationDuration/5;
                    _onBoxLayer.AddAnimation(opacity, "opacity");
                    _checkMarkLayer.AddAnimation(morphAnimation, "path");
                    _checkMarkLayer.AddAnimation(opacity, "opacity");
                    break;
                }
                case BEMAnimationType.OneStroke:
                {
                    // Temporary set the path of the checkmarl to the long checkmarl
                    _checkMarkLayer.Path = _pathManager.PathForLongCheckMark().BezierPathByReversingPath().CGPath;

                    var boxStrokeAnimation = _animationManager.StrokeAnimationReverse(false);
                    boxStrokeAnimation.Duration = boxStrokeAnimation.Duration/2;
                    _onBoxLayer.AddAnimation(boxStrokeAnimation, "strokeEnd");

                    var checkStrokeAnimation = _animationManager.StrokeAnimationReverse(false);
                    checkStrokeAnimation.Duration = checkStrokeAnimation.Duration/3;
                    checkStrokeAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
                    checkStrokeAnimation.FillMode = CAFillMode.Backwards;
                    checkStrokeAnimation.BeginTime = CAAnimation.CurrentMediaTime() + boxStrokeAnimation.Duration;
                    _checkMarkLayer.AddAnimation(checkStrokeAnimation, "strokeEnd");

                    var checkMorphAnimation =
                        _animationManager.MorphAnimationFromPath(_pathManager.PathForLongCheckMark(),
                            _pathManager.PathForCheckMark());
                    checkMorphAnimation.Duration = checkMorphAnimation.Duration/6;
                    checkMorphAnimation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
                    checkMorphAnimation.BeginTime = CAAnimation.CurrentMediaTime() + boxStrokeAnimation.Duration +
                                                    checkStrokeAnimation.Duration;
                    checkMorphAnimation.RemovedOnCompletion = false;
                    checkMorphAnimation.FillMode = CAFillMode.Forwards;
                    checkMorphAnimation.Delegate = _animationDelegate;
                    _checkMarkLayer.AddAnimation(checkMorphAnimation, "path");
                    break;
                }
                default:
                {
                    var animation = _animationManager.OpacityAnimationReverse(false);
                    _onBoxLayer.AddAnimation(animation, "opacity");
                    animation.Delegate = _animationDelegate;
                    _checkMarkLayer.AddAnimation(animation, "opacity");
                    break;
                }
            }
        }

        private void AddOffAnimation()
        {
            if (AnimationDuration <= 0)
            {
                _onBoxLayer.RemoveFromSuperLayer();
                _checkMarkLayer.RemoveFromSuperLayer();
                return;
            }

            switch (_offAnimationType)
            {
                case BEMAnimationType.Stroke:
                {
                    var animation = _animationManager.StrokeAnimationReverse(true);
                    _onBoxLayer.AddAnimation(animation, "strokeEnd");
                    animation.Delegate = _animationDelegate;
                    _checkMarkLayer.AddAnimation(animation, "strokeEnd");
                    break;
                }
                case BEMAnimationType.Fill:
                {
                    var wiggle = _animationManager.FillAnimationWithBounces(1, 0.18f, true);
                    wiggle.Duration = AnimationDuration;
                    wiggle.Delegate = _animationDelegate;
                    _onBoxLayer.AddAnimation(wiggle, "transform");
                    _checkMarkLayer.AddAnimation(_animationManager.OpacityAnimationReverse(true), "opacity");
                    break;
                }
                case BEMAnimationType.Bounce:
                {
                    float amplitude = (float) ((BoxType == BEMBoxType.Square) ? 0.20 : 0.35);
                    var wiggle = _animationManager.FillAnimationWithBounces(1, amplitude, true);
                    wiggle.Duration = AnimationDuration/1.1;
                    var opacity = _animationManager.OpacityAnimationReverse(true);
                    opacity.Delegate = _animationDelegate;
                    _onBoxLayer.AddAnimation(opacity, "opacity");
                    _checkMarkLayer.AddAnimation(wiggle, "transform");
                    break;
                }
                case BEMAnimationType.Flat:
                {
                    var animation = _animationManager.MorphAnimationFromPath(_pathManager.PathForCheckMark(),
                        _pathManager.PathForFlatCheckMark());
                    animation.Delegate = _animationDelegate;
                    var opacity = _animationManager.OpacityAnimationReverse(true);
                    opacity.Duration = AnimationDuration;
                    _onBoxLayer.AddAnimation(opacity, "opacity");
                    _checkMarkLayer.AddAnimation(animation, "path");
                    _checkMarkLayer.AddAnimation(opacity, "opacity");
                    break;
                }
                case BEMAnimationType.OneStroke:
                {
                    _checkMarkLayer.Path = _pathManager.PathForLongCheckMark().BezierPathByReversingPath().CGPath;

                    var checkMorphAnimation =
                        _animationManager.MorphAnimationFromPath(_pathManager.PathForCheckMark(),
                            _pathManager.PathForLongCheckMark());
                    checkMorphAnimation.Delegate = null;
                    checkMorphAnimation.Duration = checkMorphAnimation.Duration/6;
                    _checkMarkLayer.AddAnimation(checkMorphAnimation, "path");

                    var checkStrokeAnimation = _animationManager.StrokeAnimationReverse(true);
                    checkStrokeAnimation.Delegate = null;
                    checkStrokeAnimation.BeginTime = CAAnimation.CurrentMediaTime() + checkMorphAnimation.Duration;
                    checkStrokeAnimation.Duration = checkStrokeAnimation.Duration/3;
                    _checkMarkLayer.AddAnimation(checkStrokeAnimation, "strokeEnd");

                    var fireAfter = new DispatchTime(DispatchTime.Now,
                        (int)
                            (CAAnimation.CurrentMediaTime() + checkMorphAnimation.Duration +
                             checkStrokeAnimation.Duration*NsecPerSec));

                        var self = this;
                    DispatchQueue.MainQueue.DispatchAfter(fireAfter, () =>
                    {
                        self._checkMarkLayer.LineCap = CAShapeLayer.CapButt;
                    });

                    var boxStrokeAnimation = _animationManager.StrokeAnimationReverse(true);
                    boxStrokeAnimation.BeginTime = CAAnimation.CurrentMediaTime() + checkMorphAnimation.Duration + checkStrokeAnimation.Duration;
                    boxStrokeAnimation.Duration = boxStrokeAnimation.Duration/2;
                    boxStrokeAnimation.Delegate = _animationDelegate;
                    _onBoxLayer.AddAnimation(boxStrokeAnimation, "strokeEnd");
                    break;
                }
                default:
                {
                    var animation = _animationManager.OpacityAnimationReverse(true);
                    _onBoxLayer.AddAnimation(animation, "opacity");
                    animation.Delegate = _animationDelegate;
                    _checkMarkLayer.AddAnimation(animation, "opacity");
                    break;
                }
            }
        }

        #endregion

        #endregion

        private class BEMCheckBoxAnimationDelegate:CAAnimationDelegate
        {
            private BEMCheckBox _checkBox;

            public BEMCheckBoxAnimationDelegate(BEMCheckBox checkBox)
            {
                _checkBox = checkBox;
            }

            public override void AnimationStopped(CAAnimation anim, bool finished)
            {
                if (finished)
                {
                    if (_checkBox.On)
                    {
                        _checkBox._onBoxLayer.RemoveFromSuperLayer();
                        _checkBox._checkMarkLayer.RemoveFromSuperLayer();
                    }
                }
            }
        }
    }
}