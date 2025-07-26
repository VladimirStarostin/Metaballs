using System.Windows;
using System.Windows.Media;

public class VisualHost : FrameworkElement
{
    private readonly Visual _visual;

    public VisualHost(Visual visual)
    {
        _visual = visual;
        AddVisualChild(_visual);
    }

    protected override int VisualChildrenCount => 1;

    protected override Visual GetVisualChild(int _) => _visual;
}