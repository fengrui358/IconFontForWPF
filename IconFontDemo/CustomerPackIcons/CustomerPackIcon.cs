﻿using System.Windows;
using MahApps.Metro.IconPacks;

namespace IconFontDemo.CustomerPackIcons
{
    public class CustomerPackIcon : PackIconControl<CustomerPackIconKind>
    {
        static CustomerPackIcon()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomerPackIcon), new FrameworkPropertyMetadata(typeof(CustomerPackIcon)));
        }

        public CustomerPackIcon()
            : base(CustomerPackIconDataFactory.Create)
        {
        }
    }
}
