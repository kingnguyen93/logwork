using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.Behaviors
{
    /// <summary>
    /// Enumerator of event types our behavior could execute on raising.
    /// </summary>
    public enum EventTypeEnumerator
    {
        /// <summary>
        /// View received a tap.
        /// </summary>
        Tapped,

        /// <summary>
        /// View received focus.
        /// </summary>
        Focused,

        /// <summary>
        /// View lost focus.
        /// </summary>
        Unfocused,

        /// <summary>
        /// View add a child.
        /// </summary>
        ChildAdded,

        /// <summary>
        /// View remove a child.
        /// </summary>
        ChildRemoved,

        /// <summary>
        /// Behavior is attached to view.
        /// </summary>
        Attached,

        /// <summary>
        /// Behavior is dettaching from view.
        /// </summary>
        Detaching
    }

    /// <summary>
    /// Enumerator with easing methods.
    /// </summary>
    public enum EasingMethodEnumerator
    {
        // <summary>
        /// Jumps towards, and then bounces as it settles at the final value.
        /// </summary>
        BounceIn,

        /// <summary>
        /// Leaps to final values, bounces 3 times, and settles.
        /// </summary>
        BounceOut,

        /// <summary>
        /// Starts slowly and accelerates.
        /// </summary>
        CubicIn,

        /// <summary>
        /// Starts quickly and the decelerates.
        /// </summary>
        CubicOut,

        /// <summary>
        /// Accelerates and decelerates. Often a natural-looking choice.
        /// </summary>
        CubicInOut,

        /// <summary>
        /// Linear transformation.
        /// </summary>
        Linear,

        /// <summary>
        /// Smoothly accelerates.
        /// </summary>
        SinIn,

        /// <summary>
        /// Smoothly decelerates.
        /// </summary>
        SinOut,

        /// <summary>
        /// Accelerates in and out.
        /// </summary>
        SinInOut,

        /// <summary>
        /// Moves away and then leaps toward the final value.
        /// </summary>
        SpringIn,

        /// <summary>
        /// Overshoots and then returns.
        /// </summary>
        SpringOut
    }

    /// <summary>
    /// Enumerator of rotation types.
    /// </summary>
    public enum RotationAxisEnumerator
    {
        // <summary>
        /// Rotate the element over the X axis.
        /// </summary>
        X,

        /// <summary>
        /// Rotate the element over the Y axis.
        /// </summary>
        Y,

        /// <summary>
        /// Rotate the element over the Z axis.
        /// </summary>
        Z
    }

    public class AnimationBaseBehavior : Behavior<View>
    {
        public static readonly BindableProperty DurationProperty = BindableProperty.Create(nameof(Duration), typeof(int), typeof(AnimationBaseBehavior), 250);
        public static readonly BindableProperty OnEventProperty = BindableProperty.Create(nameof(OnEvent), typeof(EventTypeEnumerator), typeof(AnimationBaseBehavior), EventTypeEnumerator.Attached);
        public static readonly BindableProperty EasingMethodProperty = BindableProperty.Create(nameof(EasingMethod), typeof(EasingMethodEnumerator), typeof(AnimationBaseBehavior), EasingMethodEnumerator.Linear);

        /// <summary>
        /// Animation duration in milliseconds, default: 250ms
        /// </summary>
        public int Duration
        {
            get { return (int)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        /// <summary>
        /// Launching event, default: Attached
        /// </summary>
        public EventTypeEnumerator OnEvent
        {
            get { return (EventTypeEnumerator)GetValue(OnEventProperty); }
            set { SetValue(OnEventProperty, value); }
        }

        /// <summary>
        /// Easing function to apply to animation, default: Linear
        /// </summary>
        public EasingMethodEnumerator EasingMethod
        {
            get { return (EasingMethodEnumerator)GetValue(EasingMethodProperty); }
            set { SetValue(EasingMethodProperty, value); }
        }

        private TapGestureRecognizer tapGestureRecognizer;

        protected override void OnAttachedTo(View element)
        {
            base.OnAttachedTo(element);

            switch (OnEvent)
            {
                case EventTypeEnumerator.Tapped:
                    tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += ElementEvent;
                    element.GestureRecognizers.Add(tapGestureRecognizer);
                    break;

                case EventTypeEnumerator.Focused:
                    element.Focused += ElementEvent;
                    break;

                case EventTypeEnumerator.Unfocused:
                    element.Unfocused += ElementEvent;
                    break;

                case EventTypeEnumerator.ChildAdded:
                    element.ChildAdded += ElementEvent;
                    break;

                case EventTypeEnumerator.ChildRemoved:
                    element.ChildRemoved += ElementEvent;
                    break;

                case EventTypeEnumerator.Attached:
                    DoAnimation(element);
                    break;

                case EventTypeEnumerator.Detaching:
                    break;

                default:
                    break;
            }
        }

        protected override void OnDetachingFrom(View element)
        {
            base.OnDetachingFrom(element);

            switch (OnEvent)
            {
                case EventTypeEnumerator.Tapped:
                    if (tapGestureRecognizer != null)
                        element.GestureRecognizers.Remove(tapGestureRecognizer);
                    break;

                case EventTypeEnumerator.Focused:
                    element.Focused -= ElementEvent;
                    break;

                case EventTypeEnumerator.Unfocused:
                    element.Unfocused -= ElementEvent;
                    break;

                case EventTypeEnumerator.ChildAdded:
                    element.ChildAdded -= ElementEvent;
                    break;

                case EventTypeEnumerator.ChildRemoved:
                    element.ChildRemoved -= ElementEvent;
                    break;

                case EventTypeEnumerator.Attached:
                    break;

                case EventTypeEnumerator.Detaching:
                    DoAnimation(element);
                    break;

                default:
                    break;
            }
        }

        protected virtual Task DoAnimation(View element)
        {
            return null;
        }

        protected Easing GetEasingMethodFromEnumerator()
        {
            switch (EasingMethod)
            {
                case EasingMethodEnumerator.BounceIn:
                    return Easing.BounceIn;

                case EasingMethodEnumerator.BounceOut:
                    return Easing.BounceOut;

                case EasingMethodEnumerator.CubicIn:
                    return Easing.CubicIn;

                case EasingMethodEnumerator.CubicOut:
                    return Easing.CubicOut;

                case EasingMethodEnumerator.CubicInOut:
                    return Easing.CubicInOut;

                case EasingMethodEnumerator.Linear:
                    return Easing.Linear;

                case EasingMethodEnumerator.SinIn:
                    return Easing.SinIn;

                case EasingMethodEnumerator.SinOut:
                    return Easing.SinOut;

                case EasingMethodEnumerator.SinInOut:
                    return Easing.SinInOut;

                case EasingMethodEnumerator.SpringIn:
                    return Easing.SpringIn;

                case EasingMethodEnumerator.SpringOut:
                    return Easing.SpringOut;

                default:
                    return Easing.Linear;
            }
        }

        private void ElementEvent(object sender, EventArgs e)
        {
            DoAnimation((sender as View));
        }
    }
}