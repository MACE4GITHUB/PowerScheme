using Common;
using System;

namespace PowerSchemeServiceAPI.Model
{
    public interface IPowerScheme
    {
        /// <summary>
        /// Gets or sets PowerScheme Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets or sets PowerScheme Description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets true if PowerScheme is native (High, Balance, Low), otherwise false
        /// </summary>
        bool IsNative { get; }

        /// <summary>
        /// Gets true if PowerScheme is visible, otherwise false
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Gets true if PowerScheme is Ultimate/Extreme, otherwise false
        /// </summary>
        bool IsMaxPerformance { get; }

        /// <summary>
        /// Gets true if PowerScheme is active, otherwise false
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Gets PowerScheme Guid 
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Gets or sets PowerScheme Picture
        /// </summary>
        ImageItem Picture { get; set; }
    }
}