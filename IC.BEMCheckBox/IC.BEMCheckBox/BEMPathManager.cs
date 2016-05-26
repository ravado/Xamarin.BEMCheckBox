using System;
using CoreGraphics;
using IC.BEMCheckBox.Contracts;
using UIKit;

namespace IC.BEMCheckBox
{
    public class BEMPathManager : IBEMPathManager
    {
        public BEMBoxType BoxType { get; set; }
        public float LineWidth { get; set; }
        public float Size { get; set; }

        public BEMPathManager() {}

        public BEMPathManager(BEMBoxType boxType, float lineWidth, float size)
        {
            BoxType = boxType;
            LineWidth = lineWidth;
            Size = size;
        }

        public UIBezierPath PathForBox()
        {
            UIBezierPath path = null;
            var pi = Math.PI;
            switch (BoxType)
            {
                case BEMBoxType.Square:
                    path = UIBezierPath.FromRoundedRect(new CGRect(0, 0, Size, Size), 3.0f);
                    path.ApplyTransform(CGAffineTransform.Rotate(CGAffineTransform.MakeIdentity(), new nfloat(pi*2.5)));
                    path.ApplyTransform(CGAffineTransform.MakeTranslation(Size, 0));
                    break;
                default:
                    var radius = Size/2;
                    var startAngle = new nfloat(-pi/4);
                    var endAngle = new nfloat(2*pi - pi/4);
                    path = UIBezierPath.FromArc(new CGPoint(Size/2, Size/2), radius, startAngle, endAngle, true);
                    break;
            }

            return path;
        }
        
        public UIBezierPath PathForCheckMark()
        {
            UIBezierPath checkMarkPath = UIBezierPath.Create();
            checkMarkPath.MoveTo(new CGPoint(Size/3.1578, Size/2));
            checkMarkPath.AddLineTo(new CGPoint(Size/2.0618, Size/1.57894));
            checkMarkPath.AddLineTo(new CGPoint(Size/1.3953, Size/2.7272));

            if (BoxType == BEMBoxType.Square)
            {
                // If we use a square box, the check mark should be a little bit bigger
                checkMarkPath.ApplyTransform(CGAffineTransform.MakeScale(1.5f, 1.5f));
                checkMarkPath.ApplyTransform(CGAffineTransform.MakeTranslation(-Size/4, -Size/4));
            }

            return checkMarkPath;
        }

        public UIBezierPath PathForFlatCheckMark()
        {
            UIBezierPath checkMarkPath = UIBezierPath.Create();
            checkMarkPath.MoveTo(new CGPoint(Size / 3.1578, Size / 2));
            checkMarkPath.AddLineTo(new CGPoint(Size / 2.0618, Size / 1.57894));

            if (BoxType == BEMBoxType.Square)
            {
                // If we use a square box, the check mark should be a little bit bigger
                checkMarkPath.AddLineTo(new CGPoint(Size / 1.2053, Size / 4.5272));
                checkMarkPath.ApplyTransform(CGAffineTransform.MakeScale(1.5f, 1.5f));
                checkMarkPath.ApplyTransform(CGAffineTransform.MakeTranslation(-Size / 4, -Size / 4));
            }
            else
            {
                checkMarkPath.AddLineTo(new CGPoint(Size / 1.1553, Size / 5.9272));
            }

            return checkMarkPath;
        }
        
        public UIBezierPath PathForLongCheckMark()
        {
            UIBezierPath flatCheckMarkPath = UIBezierPath.Create();
            flatCheckMarkPath.MoveTo(new CGPoint(Size/4, Size/2));
            flatCheckMarkPath.AddLineTo(new CGPoint(Size/2, Size/2));
            flatCheckMarkPath.AddLineTo(new CGPoint(Size/1.2, Size/2));

            return flatCheckMarkPath;
        }
    }
}