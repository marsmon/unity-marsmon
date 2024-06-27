using System;
using uiwidgets;
using Unity.UIWidgets.animation;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.widgets;

namespace Unity.UIWidgets.material {
    static class DrawerHeaderUtils {
        public const float _kDrawerHeaderHeight = 160.0f + 1.0f;
    }


    public class DrawerHeader : StatelessWidget {
        public DrawerHeader(
            Key key = null,
            Decoration decoration = null,
            EdgeInsetsGeometry margin = null,
            EdgeInsetsGeometry padding = null,
            TimeSpan? duration = null,
            Curve curve = null,
            Widget child = null
        ) : base(key: key) {
            D.assert(child != null);
            this.decoration = decoration;
            this.margin = margin ?? EdgeInsets.only(bottom: 8.0f);
            this.padding = padding ?? EdgeInsets.fromLTRB(16.0f, 16.0f, 16.0f, 8.0f);
            this.duration = duration ?? new TimeSpan(0, 0, 0, 0, 250);
            this.curve = curve ?? Curves.fastOutSlowIn;
            this.child = child;
        }


        public readonly Decoration decoration;

        public readonly EdgeInsetsGeometry padding;

        public readonly EdgeInsetsGeometry margin;

        public readonly TimeSpan duration;

        public readonly Curve curve;

        public readonly Widget child;


        public override Widget build(BuildContext context) {
            D.assert(material_.debugCheckHasMaterial(context));
            ThemeData theme = Theme.of(context);
            float statusBarHeight = MediaQuery.of(context).padding.top;
            return new Container(
                height: statusBarHeight + DrawerHeaderUtils._kDrawerHeaderHeight,
                margin: margin,
                decoration: new BoxDecoration(
                    border: new Border(
                        bottom: Divider.createBorderSide(context)
                    )
                ),
                child: new AnimatedContainer(
                    padding: padding.add(EdgeInsets.only(top: statusBarHeight)),
                    decoration: decoration,
                    duration: duration,
                    curve: curve,
                    child: child == null
                        ? null
                        : new DefaultTextStyle(
                            style: theme.textTheme.bodyText1,
                            child: MediaQuery.removePadding(
                                context: context,
                                removeTop: true,
                                child: child)
                        )
                )
            );
        }
    }
}