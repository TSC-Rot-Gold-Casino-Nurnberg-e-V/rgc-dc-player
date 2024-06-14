﻿using System.Windows;

namespace TournamentDJ.Essentials
{
    /// <summary>
    /// Helper Class that can be bound on a DataContext (like a ViewModel) to easily access it again without having to rely on FindAncestor Bindings or simular.
    /// </summary>
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

        public object Data
        {
            get { return GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }
    }
}
