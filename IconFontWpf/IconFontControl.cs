using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace IconFontWpf
{
    /// <summary>
    /// Class IconFontControl which is the custom base class for any PackIcon control.
    /// </summary>
    /// <typeparam name="TKind">The type of the enum kind.</typeparam>
    /// <seealso cref="T:ControlzEx.PackIconBase`1" />
    public abstract class IconFontControl<TKind> : IconFontBase<TKind> where TKind : Enum
    {
        /// <summary>Identifies the Flip dependency property.</summary>
        public static readonly DependencyProperty FlipProperty = DependencyProperty.Register(nameof(Flip),
            typeof(IconFontFlipOrientation), typeof(IconFontControl<TKind>),
            new PropertyMetadata((object) IconFontFlipOrientation.Normal));

        /// <summary>Identifies the Rotation dependency property.</summary>
        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(nameof(Rotation),
            typeof(double), typeof(IconFontControl<TKind>),
            new PropertyMetadata((object) 0.0, (PropertyChangedCallback) null,
                new CoerceValueCallback(IconFontControl<TKind>.RotationPropertyCoerceValueCallback)));

        /// <summary>Identifies the Spin dependency property.</summary>
        public static readonly DependencyProperty SpinProperty = DependencyProperty.Register(nameof(Spin), typeof(bool),
            typeof(IconFontControl<TKind>),
            new PropertyMetadata((object) false,
                new PropertyChangedCallback(IconFontControl<TKind>.SpinPropertyChangedCallback),
                new CoerceValueCallback(IconFontControl<TKind>.SpinPropertyCoerceValueCallback)));

        private static readonly string SpinnerStoryBoardName =
            $"{(object) typeof(IconFontControl<TKind>).Name}SpinnerStoryBoard";

        /// <summary>Identifies the SpinDuration dependency property.</summary>
        public static readonly DependencyProperty SpinDurationProperty = DependencyProperty.Register(
            nameof(SpinDuration), typeof(double), typeof(IconFontControl<TKind>),
            new PropertyMetadata((object) 1.0,
                new PropertyChangedCallback(IconFontControl<TKind>.SpinDurationPropertyChangedCallback),
                new CoerceValueCallback(IconFontControl<TKind>.SpinDurationCoerceValueCallback)));

        /// <summary>
        /// Identifies the SpinEasingFunction dependency property.
        /// </summary>
        public static readonly DependencyProperty SpinEasingFunctionProperty = DependencyProperty.Register(
            nameof(SpinEasingFunction), typeof(IEasingFunction), typeof(IconFontControl<TKind>),
            new PropertyMetadata((object) null,
                new PropertyChangedCallback(IconFontControl<TKind>.SpinEasingFunctionPropertyChangedCallback)));

        /// <summary>Identifies the SpinAutoReverse dependency property.</summary>
        public static readonly DependencyProperty SpinAutoReverseProperty = DependencyProperty.Register(
            nameof(SpinAutoReverse), typeof(bool), typeof(IconFontControl<TKind>),
            new PropertyMetadata((object) false,
                new PropertyChangedCallback(IconFontControl<TKind>.SpinAutoReversePropertyChangedCallback)));

        private FrameworkElement _innerGrid;

        static IconFontControl()
        {
            UIElement.OpacityProperty.OverrideMetadata(typeof(IconFontControl<TKind>),
                (PropertyMetadata) new UIPropertyMetadata((object) 1.0,
                    (PropertyChangedCallback) ((d, e) => d.CoerceValue(IconFontControl<TKind>.SpinProperty))));
            UIElement.VisibilityProperty.OverrideMetadata(typeof(IconFontControl<TKind>),
                (PropertyMetadata) new UIPropertyMetadata((object) Visibility.Visible,
                    (PropertyChangedCallback) ((d, e) => d.CoerceValue(IconFontControl<TKind>.SpinProperty))));
        }

        public IconFontControl(Func<IDictionary<TKind, string>> dataIndexFactory)
            : base(dataIndexFactory)
        {
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.CoerceValue(IconFontControl<TKind>.SpinProperty);
            if (!this.Spin)
                return;
            this.StopSpinAnimation();
            this.BeginSpinAnimation();
        }

        /// <summary>Gets or sets the flip orientation.</summary>
        public IconFontFlipOrientation Flip
        {
            get { return (IconFontFlipOrientation) this.GetValue(IconFontControl<TKind>.FlipProperty); }
            set { this.SetValue(IconFontControl<TKind>.FlipProperty, (object) value); }
        }

        private static object RotationPropertyCoerceValueCallback(
            DependencyObject dependencyObject,
            object value)
        {
            double num = (double) value;
            if (num < 0.0)
                return (object) 0.0;
            if (num <= 360.0)
                return value;
            return (object) 360.0;
        }

        /// <summary>Gets or sets the rotation (angle).</summary>
        /// <value>The rotation.</value>
        public double Rotation
        {
            get { return (double) this.GetValue(IconFontControl<TKind>.RotationProperty); }
            set { this.SetValue(IconFontControl<TKind>.RotationProperty, (object) value); }
        }

        private static object SpinPropertyCoerceValueCallback(
            DependencyObject dependencyObject,
            object value)
        {
            IconFontControl<TKind> IconFontControl = dependencyObject as IconFontControl<TKind>;
            if (IconFontControl != null && (!IconFontControl.IsVisible || IconFontControl.Opacity <= 0.0 ||
                                            IconFontControl.SpinDuration <= 0.0))
                return (object) false;
            return value;
        }

        private static void SpinPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            IconFontControl<TKind> IconFontControl = dependencyObject as IconFontControl<TKind>;
            if (IconFontControl == null || e.OldValue == e.NewValue || !(e.NewValue is bool))
                return;
            if ((bool) e.NewValue)
                IconFontControl.BeginSpinAnimation();
            else
                IconFontControl.StopSpinAnimation();
        }

        private FrameworkElement InnerGrid
        {
            get
            {
                return this._innerGrid ??
                       (this._innerGrid = this.GetTemplateChild("PART_InnerGrid") as FrameworkElement);
            }
        }

        private void BeginSpinAnimation()
        {
            FrameworkElement innerGrid = this.InnerGrid;
            if (innerGrid == null)
                return;
            TransformGroup transformGroup = innerGrid.RenderTransform as TransformGroup ?? new TransformGroup();
            RotateTransform rotateTransform =
                transformGroup.Children.OfType<RotateTransform>().LastOrDefault<RotateTransform>();
            if (rotateTransform != null)
            {
                rotateTransform.Angle = 0.0;
            }
            else
            {
                transformGroup.Children.Add((Transform) new RotateTransform());
                innerGrid.RenderTransform = (Transform) transformGroup;
            }

            Storyboard storyboard = new Storyboard();
            DoubleAnimation doubleAnimation1 = new DoubleAnimation();
            doubleAnimation1.From = new double?(0.0);
            doubleAnimation1.To = new double?(360.0);
            doubleAnimation1.AutoReverse = this.SpinAutoReverse;
            doubleAnimation1.EasingFunction = this.SpinEasingFunction;
            doubleAnimation1.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation1.Duration = new Duration(TimeSpan.FromSeconds(this.SpinDuration));
            DoubleAnimation doubleAnimation2 = doubleAnimation1;
            storyboard.Children.Add((Timeline) doubleAnimation2);
            Storyboard.SetTarget((DependencyObject) doubleAnimation2, (DependencyObject) innerGrid);
            Storyboard.SetTargetProperty((DependencyObject) doubleAnimation2, new PropertyPath("(0).(1)[2].(2)",
                new object[3]
                {
                    (object) UIElement.RenderTransformProperty,
                    (object) TransformGroup.ChildrenProperty,
                    (object) RotateTransform.AngleProperty
                }));
            innerGrid.Resources.Add((object) IconFontControl<TKind>.SpinnerStoryBoardName, (object) storyboard);
            storyboard.Begin();
        }

        private void StopSpinAnimation()
        {
            FrameworkElement innerGrid = this.InnerGrid;
            if (innerGrid == null)
                return;
            Storyboard resource =
                innerGrid.Resources[(object) IconFontControl<TKind>.SpinnerStoryBoardName] as Storyboard;
            if (resource == null)
                return;
            resource.Stop();
            innerGrid.Resources.Remove((object) IconFontControl<TKind>.SpinnerStoryBoardName);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the inner icon is spinning.
        /// </summary>
        /// <value><c>true</c> if spin; otherwise, <c>false</c>.</value>
        public bool Spin
        {
            get { return (bool) this.GetValue(IconFontControl<TKind>.SpinProperty); }
            set { this.SetValue(IconFontControl<TKind>.SpinProperty, (object) value); }
        }

        private static void SpinDurationPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            IconFontControl<TKind> IconFontControl = dependencyObject as IconFontControl<TKind>;
            if (IconFontControl == null || e.OldValue == e.NewValue ||
                (!IconFontControl.Spin || !(e.NewValue is double)))
                return;
            IconFontControl.StopSpinAnimation();
            IconFontControl.BeginSpinAnimation();
        }

        private static object SpinDurationCoerceValueCallback(
            DependencyObject dependencyObject,
            object value)
        {
            if ((double) value >= 0.0)
                return value;
            return (object) 0.0;
        }

        /// <summary>
        /// Gets or sets the duration of the spinning animation (in seconds). This will also restart the spin animation.
        /// </summary>
        /// <value>The duration of the spin in seconds.</value>
        public double SpinDuration
        {
            get { return (double) this.GetValue(IconFontControl<TKind>.SpinDurationProperty); }
            set { this.SetValue(IconFontControl<TKind>.SpinDurationProperty, (object) value); }
        }

        private static void SpinEasingFunctionPropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            IconFontControl<TKind> IconFontControl = dependencyObject as IconFontControl<TKind>;
            if (IconFontControl == null || e.OldValue == e.NewValue || !IconFontControl.Spin)
                return;
            IconFontControl.StopSpinAnimation();
            IconFontControl.BeginSpinAnimation();
        }

        /// <summary>
        /// Gets or sets the EasingFunction of the spinning animation. This will also restart the spin animation.
        /// </summary>
        /// <value>The spin easing function.</value>
        public IEasingFunction SpinEasingFunction
        {
            get { return (IEasingFunction) this.GetValue(IconFontControl<TKind>.SpinEasingFunctionProperty); }
            set { this.SetValue(IconFontControl<TKind>.SpinEasingFunctionProperty, (object) value); }
        }

        private static void SpinAutoReversePropertyChangedCallback(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            IconFontControl<TKind> IconFontControl = dependencyObject as IconFontControl<TKind>;
            if (IconFontControl == null || e.OldValue == e.NewValue || (!IconFontControl.Spin || !(e.NewValue is bool)))
                return;
            IconFontControl.StopSpinAnimation();
            IconFontControl.BeginSpinAnimation();
        }

        /// <summary>
        /// Gets or sets the AutoReverse of the spinning animation. This will also restart the spin animation.
        /// </summary>
        /// <value><c>true</c> if [spin automatic reverse]; otherwise, <c>false</c>.</value>
        public bool SpinAutoReverse
        {
            get { return (bool) this.GetValue(IconFontControl<TKind>.SpinAutoReverseProperty); }
            set { this.SetValue(IconFontControl<TKind>.SpinAutoReverseProperty, (object) value); }
        }
    }
}