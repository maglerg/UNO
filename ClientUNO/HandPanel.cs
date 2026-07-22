using System;
using System.Windows;
using System.Windows.Controls;

namespace ClientUNO.Panels
{
    public class HandPanel : Panel
    {
        private const double CardWidth = 90;
        private const double CardHeight = 130;
        private const double MaxSpacing = 95;
        private const double MinSpacing = 25;

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (UIElement child in InternalChildren)
                child.Measure(new Size(CardWidth, CardHeight));

            return new Size(GetPanelWidth(), CardHeight);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double spacing = CalculateSpacing(finalSize.Width);

            for (int i = 0; i < InternalChildren.Count; i++)
            {
                double x = i * spacing;

                InternalChildren[i].Arrange(
                    new Rect(x, 0, CardWidth, CardHeight));
            }

            return new Size(GetPanelWidth(spacing), CardHeight);
        }

        private double CalculateSpacing(double availableWidth)
        {
            int count = InternalChildren.Count;

            if (count <= 1)
                return MaxSpacing;

            double spacing =
                (availableWidth - CardWidth) / (count - 1);

            spacing = Math.Min(MaxSpacing, spacing);
            spacing = Math.Max(MinSpacing, spacing);

            return spacing;
        }

        private double GetPanelWidth()
        {
            return GetPanelWidth(MaxSpacing);
        }

        private double GetPanelWidth(double spacing)
        {
            if (InternalChildren.Count == 0)
                return 0;

            return CardWidth +
                   (InternalChildren.Count - 1) * spacing;
        }
    }
}