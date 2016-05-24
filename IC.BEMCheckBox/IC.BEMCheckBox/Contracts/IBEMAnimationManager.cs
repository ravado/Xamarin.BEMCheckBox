using CoreAnimation;
using UIKit;

namespace IC.BEMCheckBox.Contracts
{
    /// <summary>
    /// Animation object used by BEMCheckBox to generate animations.
    /// </summary>
    internal interface IBEMAnimationManager
    {
        /// <summary>
        /// The duration of the animation created by the BEMAnimationManager object.
        /// </summary>
        float AnimationDuration { get; set; }

        /// <summary>
        /// Returns a CABasicAnimation which the stroke.
        /// </summary>
        /// <param name="reverse">The direction of the animation.
        /// Set to YES if the animation should go from opacity 0 to 1, or NO for the opposite.</param>
        /// <returns>the CABasicAnimation object.</returns>
        CABasicAnimation StrokeAnimationReverse(bool reverse);

        /// <summary>
        /// Returns a CABasicAnimation which animates the opacity.
        /// </summary>         
        /// <param name="reverse">The direction of the animation.
        /// Set to YES if the animation should go from opacity 0 to 1, or NO for the opposite.</param>
        /// <returns>The CABasicAnimation object.</returns>
        CABasicAnimation OpacityAnimationReverse(bool reverse);

        /// <summary>
        /// Returns a CABasicAnimation which animates between two paths.
        /// </summary>
        /// <param name="fromPath">The path to transform (morph) from.</param>
        /// <param name="toPath">The path to transform (morph) to.</param>
        /// <returns>The CABasicAnimation object.</returns>
        CABasicAnimation MorphAnimationFromPath(UIBezierPath fromPath, UIBezierPath toPath);

        /// <summary>
        /// Animation engine to create a fill animation.
        /// </summary>
        /// <param name="bounces">The number of bounces for the animation.</param>
        /// <param name="amplitude">How far does the animation bounce.</param>
        /// <param name="reverse">Flag to track if the animation should fill or empty the layer.</param>
        /// <returns>Returns the CAKeyframeAnimation object.</returns>
        CAKeyFrameAnimation FillAnimationWithBounces(int bounces, float amplitude, bool reverse);
    }
}
