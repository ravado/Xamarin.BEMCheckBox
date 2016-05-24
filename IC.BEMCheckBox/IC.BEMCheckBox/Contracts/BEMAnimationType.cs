using System;
using System.Collections.Generic;
using System.Text;

namespace IC.BEMCheckBox.Contracts
{
    /// <summary>
    /// The different type of animations available.
    /// </summary>
    public enum BEMAnimationType
    {
        /// <summary>
        /// Animates the box and the check as if they were drawn.
        /// Should be used with a clear colored onFillColor property.
        /// </summary>
        Stroke = 0,

        /// <summary>
        /// When tapped, the checkbox is filled from its center.
        /// Should be used with a colored onFillColor property.
        /// </summary>
        Fill = 1,

        /// <summary>
        /// Animates the check mark with a bouncy effect.
        /// </summary>
        Bounce = 2,

        /// <summary>
        /// Morphs the checkmark from a line.
        /// Should be used with a colored onFillColor property.
        /// </summary>
        Flat = 3,

        /// <summary>
        /// Animates the box and check as if they were drawn in one continuous line.
        /// Should be used with a clear colored onFillColor property.
        /// </summary>
        OneStroke = 4,

        /// <summary>
        /// When tapped, the checkbox is fading in or out (opacity).
        /// </summary>
        Fade = 5
    }
}
