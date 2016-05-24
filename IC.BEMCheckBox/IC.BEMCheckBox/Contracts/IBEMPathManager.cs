using UIKit;

namespace IC.BEMCheckBox.Contracts
{
    /// <summary>
    /// Path object used by BEMCheckBox to generate paths.
    /// </summary>
    public interface IBEMPathManager
    {
        /// <summary>
        /// The paths are assumed to be created in squares. 
        /// This is the size of width, or height, of the paths that will be created.
        /// </summary>
        float Size { get; set; }

        /// <summary>
        /// The width of the lines on the created paths.
        /// </summary>
        float LineWidth { get; set; }

        /// <summary>
        /// The type of box.
        /// Depending on the box type, paths may be created differently
        /// </summary>
        BEMBoxType BoxType { get; set; }

        /// <summary>
        /// Returns a UIBezierPath object for the box of the checkbox
        /// </summary>
        /// <returns> The path of the box.</returns>
        UIBezierPath PathForBox();

        /// <summary>
        /// Returns a UIBezierPath object for the checkmark of the checkbox 
        /// </summary>
        /// <returns> The path of the checkmark.</returns>
        UIBezierPath PathForCheckMark();

        /// <summary>
        /// Returns a UIBezierPath object for an extra long checkmark which is in contact with the box. 
        /// </summary>
        /// <returns> The path of the checkmark.</returns>
        UIBezierPath PathForLongCheckMark();

        /// <summary>
        /// Returns a UIBezierPath object for the flat checkmark of the checkbox
        /// </summary>
        /// <returns> The path of the flat checkmark.</returns>
        UIBezierPath PathForFlatCheckMark();
    }
}