using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace TournamentDJ.Essentials
{
    public class MouseWheelSliderBehavior : Behavior<Slider>
    {
        public double Amount
        {
            get => (double)GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        public static readonly DependencyProperty AmountProperty
            = DependencyProperty.RegisterAttached(
                nameof(Amount),
                typeof(double),
                typeof(MouseWheelSliderBehavior),
                new UIPropertyMetadata(0.0));

        private void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Amount == 0.0) return;
            Slider slider = (Slider)sender;
            if (e.Delta > 0)
            {
                double newValue = slider.Value + Amount;
                if(newValue > slider.Maximum)
                {
                    newValue = slider.Maximum;
                }
                slider.Value = newValue;
            }
            else
            {
                double newValue = slider.Value - Amount;
                if (newValue < slider.Minimum)
                {
                    newValue = slider.Minimum;
                }
                slider.Value = newValue;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.PreviewMouseWheel += this.AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewMouseWheel -= this.AssociatedObject_PreviewMouseWheel;
        }
    }
}
