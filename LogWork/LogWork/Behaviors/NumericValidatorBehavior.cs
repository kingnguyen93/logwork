namespace Xamarin.Forms.Behaviors
{
    public class NumericValidatorBehavior : Behavior<Entry>
    {
        protected override void OnAttachedTo(Entry bindable)
        {
            bindable.TextChanged += OnEntryTextChanged;
            bindable.Unfocused += OnEntryUnfocused;

            base.OnAttachedTo(bindable);
        }

        protected override void OnDetachingFrom(Entry bindable)
        {
            bindable.TextChanged -= OnEntryTextChanged;
            bindable.Unfocused -= OnEntryUnfocused;

            base.OnDetachingFrom(bindable);
        }

        private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (string.IsNullOrWhiteSpace(entry.Text))
                {
                    entry.Text = "0";
                }
                else if (entry.Text.Length >= 2 && entry.Text.StartsWith("0"))
                {
                    entry.Text = entry.Text.Remove(0, 1);
                }
                else if (entry.Text.StartsWith("00"))
                {
                    entry.Text = "0";
                }
            }
        }

        private void OnEntryUnfocused(object sender, FocusEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (string.IsNullOrEmpty(entry.Text))
                    return;

                if (entry.Text.EndsWith("."))
                {
                    entry.Text = entry.Text.Remove(entry.Text.Length - 1);
                }
            }
        }
    }
}