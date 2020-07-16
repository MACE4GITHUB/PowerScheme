using Common;
using PowerManagerAPI;
using System;

namespace PowerSchemeServiceAPI.Model
{
    /// <summary>
    /// Represents PowerScheme
    /// </summary>
    public class PowerScheme : IPowerScheme
    {
        /// <summary>
        /// Creates PowerScheme from Guid
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="isNative"></param>
        /// <param name="picture"></param>
        /// <param name="isVisible"></param>
        public PowerScheme(Guid guid, bool isNative, ImageItem picture, bool isVisible = true)
        {
            Guid = guid;
            IsNative = isNative;
            Picture = picture;
            IsVisible = isVisible;
        }

        /// <summary>
        /// Gets or sets PowerScheme Name
        /// </summary>
        public string Name => PowerManager.GetPlanName(Guid);

        /// <summary>
        /// Gets or sets PowerScheme Description
        /// </summary>
        public string Description => PowerManager.GetPlanDescription(Guid);

        /// <summary>
        /// Gets true if PowerScheme is native (High, Balance, Low), otherwise false
        /// </summary>
        public bool IsNative { get; }

        /// <summary>
        /// Gets true if PowerScheme is visible, otherwise false
        /// </summary>
        public bool IsVisible { get; }

        /// <summary>
        /// Gets true if PowerScheme is active, otherwise false
        /// </summary>
        public bool IsActive => PowerManager.GetActivePlan() == Guid;

        /// <summary>
        /// Gets PowerScheme Guid 
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets or sets PowerScheme Picture
        /// </summary>
        public ImageItem Picture { get; set; }
    }
}
