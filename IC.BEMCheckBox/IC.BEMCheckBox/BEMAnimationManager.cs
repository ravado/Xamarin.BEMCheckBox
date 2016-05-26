using System;
using System.Collections.Generic;
using CoreAnimation;
using Foundation;
using IC.BEMCheckBox.Contracts;
using UIKit;

namespace IC.BEMCheckBox
{
    public class BEMAnimationManager : IBEMAnimationManager
    {
        public float AnimationDuration { get; set; }

        public BEMAnimationManager(float animationDuration)
        {
            AnimationDuration = animationDuration;
        }

        public CABasicAnimation StrokeAnimationReverse(bool reverse)
        {
            var animation = CABasicAnimation.FromKeyPath("strokeEnd");
            if (reverse)
            {
                animation.From = NSNumber.FromFloat(1.0f);
                animation.To = NSNumber.FromFloat(0.0f);
            }
            else
            {
                animation.From = NSNumber.FromFloat(0.0f);
                animation.To = NSNumber.FromFloat(1.0f);
            }
            animation.Duration = AnimationDuration;
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

            return animation;
        }

        public CABasicAnimation OpacityAnimationReverse(bool reverse)
        {
            var animation = CABasicAnimation.FromKeyPath("opacity");
            if (reverse)
            {
                animation.From = NSNumber.FromFloat(1.0f);
                animation.To = NSNumber.FromFloat(0.0f);
            }
            else
            {
                animation.From = NSNumber.FromFloat(0.0f);
                animation.To = NSNumber.FromFloat(1.0f);
            }

            animation.Duration = AnimationDuration;
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

            return animation;
        }

        public CABasicAnimation MorphAnimationFromPath(UIBezierPath fromPath, UIBezierPath toPath)
        {
            var animation = CABasicAnimation.FromKeyPath("path");
            animation.Duration = AnimationDuration;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

            animation.SetFrom(fromPath.CGPath);
            animation.SetTo(toPath.CGPath);

            return animation;
        }

        public CAKeyFrameAnimation FillAnimationWithBounces(int bounces, float amplitude, bool reverse)
        {
            var values = new List<NSObject>();
            var keyTimes = new List<NSNumber>();

            if (reverse)
            {
                values.Add(NSValue.FromCATransform3D(CATransform3D.MakeScale(1, 1, 1)));
            }
            else
            {
                values.Add(NSValue.FromCATransform3D(CATransform3D.MakeScale(0, 0, 0)));
            }

            keyTimes.Add(NSNumber.FromFloat(0.0f));

            for (var i = 1; i <= bounces; i++)
            {
                var scale = (i%2) > 0 ? (1 + amplitude/i) : (1 - amplitude/i);
                var time = (float) (i*(1.0/(bounces + 1)));

                values.Add(NSValue.FromCATransform3D(CATransform3D.MakeScale(scale, scale, scale)));
                keyTimes.Add(NSNumber.FromFloat(time));
            }

            if (reverse)
            {
                values.Add(NSValue.FromCATransform3D(CATransform3D.MakeScale(0.0001f, 0.0001f, 0.0001f)));
            }
            else
            {
                values.Add(NSValue.FromCATransform3D(CATransform3D.MakeScale(1, 1, 1)));
            }

            keyTimes.Add(NSNumber.FromFloat(1.0f));

            var animation = CAKeyFrameAnimation.FromKeyPath("transform");
            animation.Values = values.ToArray();
            animation.KeyTimes = keyTimes.ToArray();
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.Duration = AnimationDuration;
            animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);

            return animation;
        }
    }
}