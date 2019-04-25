using System.Windows;
using IconFontWpf;

namespace IconFontDemo.IconFontWpfs
{
    public class IconFontWpf : IconFontControl<IconFontKind>
    {
        static IconFontWpf()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconFontWpf),
                new FrameworkPropertyMetadata(typeof(IconFontWpf)));
        }

        public IconFontWpf() : base(IconFontDataFactory.Create)
        {
        }
    }
}