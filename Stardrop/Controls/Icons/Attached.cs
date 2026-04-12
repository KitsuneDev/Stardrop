using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;
using System.Collections.Generic;

namespace Stardrop.Controls.Icons
{
    public class Attached : AvaloniaObject
    {
        private static readonly AttachedProperty<object?> OriginalContentProperty =
            AvaloniaProperty.RegisterAttached<Attached, ContentControl, object?>("OriginalContent");

        public static readonly AttachedProperty<string?> IconProperty =
            AvaloniaProperty.RegisterAttached<Attached, ContentControl, string?>("Icon");

        static Attached()
        {
            IconProperty.Changed.AddClassHandler<ContentControl>(OnIconChanged);
        }

        public static string? GetIcon(ContentControl control) => control.GetValue(IconProperty);

        public static void SetIcon(ContentControl control, string? value) => control.SetValue(IconProperty, value);

        private static void OnIconChanged(ContentControl control, AvaloniaPropertyChangedEventArgs e)
        {
            if (control.GetValue(OriginalContentProperty) is null)
            {
                control.SetValue(OriginalContentProperty, control.Content);
            }

            var originalContent = control.GetValue(OriginalContentProperty);
            var iconName = e.NewValue as string;
            if (String.IsNullOrWhiteSpace(iconName))
            {
                control.Content = originalContent;
                return;
            }

            var icon = IconFactory.Create(iconName);
            if (originalContent is null || originalContent is string text && String.IsNullOrWhiteSpace(text))
            {
                control.Content = icon;
                return;
            }

            control.Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 6,
                VerticalAlignment = VerticalAlignment.Center,
                Children =
                {
                    icon,
                    originalContent as Control ?? new TextBlock
                    {
                        Text = originalContent.ToString(),
                        VerticalAlignment = VerticalAlignment.Center
                    }
                }
            };
        }
    }

    public class MenuItem : AvaloniaObject
    {
        public static readonly AttachedProperty<string?> IconProperty =
            AvaloniaProperty.RegisterAttached<MenuItem, Avalonia.Controls.MenuItem, string?>("Icon");

        static MenuItem()
        {
            IconProperty.Changed.AddClassHandler<Avalonia.Controls.MenuItem>(OnIconChanged);
        }

        public static string? GetIcon(Avalonia.Controls.MenuItem control) => control.GetValue(IconProperty);

        public static void SetIcon(Avalonia.Controls.MenuItem control, string? value) => control.SetValue(IconProperty, value);

        private static void OnIconChanged(Avalonia.Controls.MenuItem control, AvaloniaPropertyChangedEventArgs e)
        {
            var iconName = e.NewValue as string;
            control.Icon = String.IsNullOrWhiteSpace(iconName) ? null : IconFactory.Create(iconName);
        }
    }

    internal static class IconFactory
    {
        private static readonly IReadOnlyDictionary<string, string> Glyphs = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["mdi-cancel"] = "✕",
            ["mdi-check"] = "✓",
            ["mdi-check-outline"] = "✓",
            ["mdi-close"] = "✕",
            ["mdi-content-copy"] = "⧉",
            ["mdi-folder"] = "📁",
            ["mdi-minus"] = "−",
            ["mdi-pencil"] = "✎",
            ["mdi-playlist-edit"] = "☰",
            ["mdi-plus"] = "+",
            ["mdi-thumb-up"] = "👍",
            ["mdi-thumb-up-outline"] = "👍",
            ["mdi-window-maximize"] = "□",
            ["mdi-window-minimize"] = "−"
        };

        public static Control Create(string iconName)
        {
            return new TextBlock
            {
                Text = Glyphs.TryGetValue(iconName, out var glyph) ? glyph : "?",
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
        }
    }
}
