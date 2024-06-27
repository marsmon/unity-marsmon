using uiwidgets;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Unity.UIWidgets.material {
    public class Card : StatelessWidget {
        public Card(
            Key key = null,
            Color color = null,
            Color shadowColor = null,
            float? elevation = null,
            ShapeBorder shape = null,
            bool borderOnForeground = true,
            EdgeInsetsGeometry margin = null,
            Clip? clipBehavior = null,
            Widget child = null) : base(key: key) {
            D.assert(elevation == null || elevation >= 0.0f);
            this.color = color;
            this.shadowColor = shadowColor;
            this.elevation = elevation;
            this.shape = shape;
            this.borderOnForeground = borderOnForeground;
            this.margin = margin;
            this.clipBehavior = clipBehavior;
            this.child = child;
        }

        public readonly Color color;

        public readonly Color shadowColor;
        
        public readonly float? elevation;

        public readonly ShapeBorder shape;

        public readonly bool borderOnForeground;

        public readonly Clip? clipBehavior;

        public readonly EdgeInsetsGeometry margin;

        public readonly Widget child;
        
        const float _defaultElevation = 1.0f;
        const Clip _defaultClipBehavior = Clip.none;

        public override Widget build(BuildContext context) {
            CardTheme cardTheme = CardTheme.of(context);

            return new Container(
                margin: margin ?? cardTheme.margin ?? EdgeInsets.all(4.0f),
                child: new Material(
                    type: MaterialType.card,
                    color: color ?? cardTheme.color ?? Theme.of(context).cardColor,
                    shadowColor: shadowColor ?? cardTheme.shadowColor ?? Colors.black,
                    elevation: elevation ?? cardTheme.elevation ?? _defaultElevation,
                    shape: shape ?? cardTheme.shape ?? new RoundedRectangleBorder(
                               borderRadius: BorderRadius.all(Radius.circular(4.0f))
                           ),
                    borderOnForeground: borderOnForeground,
                    clipBehavior: clipBehavior ?? cardTheme.clipBehavior ?? Clip.none,
                    child: child)
            );
        }
    }
}