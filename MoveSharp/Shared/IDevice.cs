using System;

namespace MoveSharp
{
    /// <summary>
    /// Interface for the device
    /// </summary>
    public interface IDevice
    {
        /// <summary>
        /// Gets the width of the screen.
        /// </summary>
        /// <value>The width of the screen.</value>
        int ScreenWidth { get; }

        /// <summary>
        /// Gets the screen width raw pixels
        /// </summary>
        /// <value>The screen width in raw pixels</value>
//        int ScreenWidthRaw { get; }

        /// <summary>
        /// Gets the height of the screen.
        /// </summary>
        /// <value>The height of the screen.</value>
        int ScreenHeight { get; }

        /// <summary>
        /// Gets the screen height raw pixels
        /// </summary>
        /// <value>The screen height in raw pixels</value>
//        int ScreenHeightRaw { get; }

    }
}
    