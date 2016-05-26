using CoreGraphics;
using UIKit;

namespace IC.BEMCheckBox.Contracts
{
    public interface IBEMCheckBox
    {
        /// <summary>
        /// This property allows you to retrieve and set (without animation) a value determining 
        /// whether the BEMCheckBox object is On or Off. Default to false.
        /// </summary>
        bool On { get; set; }

        /// <summary>
        /// The width of the lines of the check mark and the box. Default to 2.0.
        /// </summary>
        float LineWidth { get; set; }

        /// <summary>
        /// The duration in seconds of the animation when the check box switches from on and off. Default to 0.5.
        /// </summary>
        float AnimationDuration { get; set; }

        /// <summary>
        /// Determines if the box should be hidden or not. Defaults to false.
        /// </summary>
        bool HideBox { get; set; }

        /// <summary>
        /// The color of the line around the box when it is On.
        /// </summary>
        UIColor OnTintColor { get; set; }

        /// <summary>
        /// The color of the inside of the box when it is On.
        /// </summary>
        UIColor OnFillColor { get; set; }

        /// <summary>
        /// The color of the check mark when it is On.
        /// </summary>
        UIColor OnCheckColor { get; set; }

        /// <summary>
        /// The color of the box when the checkbox is Off.
        /// </summary>
        UIColor TintColor { get; set; }

        /// <summary>
        /// The type of box.
        /// <see cref="BEMBoxType"/>
        /// </summary>
        BEMBoxType BoxType { get; set; }

        /// <summary>
        /// The animation type when the check mark gets set to On.
        /// Some animations might not look as intended if the different colors of the control are not appropriatly configured.
        /// <see cref="BEMAnimationType"/>
        /// </summary>
        BEMAnimationType OnAnimationType { get; set; }

        /// <summary>
        /// The animation type when the check mark gets set to Off.
        /// Some animations might not look as intended if the different colors of the control are not appropriatly configured.
        /// <see cref="BEMAnimationType"/>
        /// </summary>
        BEMAnimationType OffAnimationType { get; set; }

        /// <summary>
        /// If the checkbox width or height is smaller than this value, the touch area will be increased. Allows for visually small checkboxes to still be easily tapped. Default: (44, 44)
        /// </summary>
        CGSize MinimumTouchSize { get; set; }

        /// <summary>
        /// Set the state of the check box to On or Off, optionally animating the transition.
        /// </summary>
        void SetOn(bool on, bool animated);

        /// <summary>
        /// Forces a redraw of the entire check box.
        /// The current value of On is kept.
        /// </summary>
        void Reload();
    }
}
