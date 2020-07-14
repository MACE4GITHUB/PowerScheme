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
        /// <param name="image"></param>
        public PowerScheme(Guid guid, bool isNative, string image)
        {
            Guid = guid;
            IsNative = isNative;
            Image = image;
        }

        /// <summary>
        /// Gets or sets PowerScheme Name
        /// </summary>
        public string Name
        {
            get => PowerManager.GetPlanName(Guid);
            set
            {
                if (IsNative) return;
                //
            }
        }

        /// <summary>
        /// Gets or sets PowerScheme Description
        /// </summary>
        public string Description
        {
            get => PowerManager.GetPlanDescription(Guid);
            set
            {
                if (IsNative) return;
                //
            }
        }

        /// <summary>
        /// Gets true if PowerScheme is native (High, Balance, Low), otherwise false
        /// </summary>
        public bool IsNative { get; }

        /// <summary>
        /// Gets true if PowerScheme is active, otherwise false
        /// </summary>
        public bool IsActive
        {
            get => PowerManager.GetActivePlan() == Guid;
            set
            {
                //
            }
        }

        /// <summary>
        /// Gets PowerScheme Guid 
        /// </summary>
        public Guid Guid { get; }

        /// <summary>
        /// Gets or sets PowerScheme Image
        /// </summary>
        public string Image { get; set; }
    }
}
