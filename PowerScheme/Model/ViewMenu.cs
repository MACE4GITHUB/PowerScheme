namespace PowerScheme.Model
{
    using Common;

    internal class ViewMenu
    {
        public ViewMenu(string name, string description = null) 
            : this(name, ImageItem.Unknown, null, description)
        { }

        public ViewMenu(string name, ImageItem picture, object tag = null, string description = null)
        {
            Name = name;
            Picture = picture;
            Tag = tag;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public ImageItem Picture { get; }

        public object Tag { get; }
    }
}
