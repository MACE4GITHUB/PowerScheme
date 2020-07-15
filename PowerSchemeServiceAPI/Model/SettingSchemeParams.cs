using System;
using Common;

namespace PowerSchemeServiceAPI.Model
{
    internal class SettingSchemeParams
    {
        public SettingSchemeParams(Guid guid, bool isNative, ImageItem image, bool isVisible = true)
        {
            Image = image;
            IsVisible = isVisible;
            IsNative = isNative;
            Guid = guid;
        }

        public Guid Guid { get; }

        public ImageItem Image { get; }

        public bool IsNative { get; }
        
        public bool IsVisible { get; }
    }
}
