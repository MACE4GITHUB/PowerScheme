using System;

namespace PowerSchemeServiceAPI.Model
{
    public interface IPowerScheme
    {
        /// <summary>
        /// Gets or sets PowerScheme Name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets PowerScheme Description
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Gets true if PowerScheme is native (High, Balance, Low), otherwise false
        /// </summary>
        bool IsNative { get; }

        /// <summary>
        /// Gets true if PowerScheme is active, otherwise false
        /// </summary>
        bool IsActive { get; set; }

        /// <summary>
        /// Gets PowerScheme Guid 
        /// </summary>
        Guid Guid { get; }

        /// <summary>
        /// Gets or sets PowerScheme Image
        /// </summary>
        string Image { get; set; }
    }
}