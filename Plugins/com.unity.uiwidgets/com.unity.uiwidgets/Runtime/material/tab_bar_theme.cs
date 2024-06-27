using System;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

namespace Unity.UIWidgets.material {
    public class TabBarTheme : Diagnosticable, IEquatable<TabBarTheme> {
        public TabBarTheme(
            Decoration indicator = null,
            TabBarIndicatorSize? indicatorSize = null,
            Color labelColor = null,
            EdgeInsetsGeometry labelPadding = null,
            TextStyle labelStyle = null,
            Color unselectedLabelColor = null,
            TextStyle unselectedLabelStyle = null) {
            this.indicator = indicator;
            this.indicatorSize = indicatorSize;
            this.labelColor = labelColor;
            this.labelPadding = labelPadding;
            this.labelStyle = labelStyle;
            this.unselectedLabelColor = unselectedLabelColor;
            this.unselectedLabelStyle = unselectedLabelStyle;
        }

        public readonly Decoration indicator;

        public readonly TabBarIndicatorSize? indicatorSize;

        public readonly Color labelColor;

        public readonly EdgeInsetsGeometry labelPadding;

        public readonly TextStyle labelStyle;

        public readonly Color unselectedLabelColor;

        public readonly TextStyle unselectedLabelStyle;

        public TabBarTheme copyWith(
            Decoration indicator = null,
            TabBarIndicatorSize? indicatorSize = null,
            Color labelColor = null,
            EdgeInsetsGeometry labelPadding = null,
            TextStyle labelStyle = null,
            Color unselectedLabelColor = null,
            TextStyle unselectedLabelStyle = null
        ) {
            return new TabBarTheme(
                indicator: indicator ?? this.indicator,
                indicatorSize: indicatorSize ?? this.indicatorSize,
                labelColor: labelColor ?? this.labelColor,
                labelPadding: labelPadding ?? this.labelPadding,
                labelStyle: labelStyle ?? this.labelStyle,
                unselectedLabelColor: unselectedLabelColor ?? this.unselectedLabelColor,
                unselectedLabelStyle: unselectedLabelStyle ?? this.unselectedLabelStyle);
        }

        public static TabBarTheme of(BuildContext context) {
            return Theme.of(context).tabBarTheme;
        }

        public static TabBarTheme lerp(TabBarTheme a, TabBarTheme b, float t) {
            D.assert(a != null);
            D.assert(b != null);
            return new TabBarTheme(
                indicator: Decoration.lerp(a.indicator, b.indicator, t),
                indicatorSize: t < 0.5 ? a.indicatorSize : b.indicatorSize,
                labelColor: Color.lerp(a.labelColor, b.labelColor, t),
                labelPadding: EdgeInsetsGeometry.lerp(a.labelPadding, b.labelPadding, t),
                labelStyle: TextStyle.lerp(a.labelStyle, b.labelStyle, t),
                unselectedLabelColor: Color.lerp(a.unselectedLabelColor, b.unselectedLabelColor, t),
                unselectedLabelStyle: TextStyle.lerp(a.unselectedLabelStyle, b.unselectedLabelStyle, t)
            );
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = indicator != null ? indicator.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (indicatorSize != null ? indicatorSize.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (labelColor != null ? labelColor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (labelPadding != null ? labelPadding.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (labelStyle != null ? labelStyle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (unselectedLabelColor != null ? unselectedLabelColor.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^
                           (unselectedLabelStyle != null ? unselectedLabelStyle.GetHashCode() : 0);
                return hashCode;
            }
        }


        public bool Equals(TabBarTheme other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return other.indicator == indicator &&
                   other.indicatorSize == indicatorSize &&
                   other.labelColor == labelColor &&
                   other.labelPadding == labelPadding &&
                   other.unselectedLabelColor == unselectedLabelColor;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

            return Equals((TabBarTheme) obj);
        }

        public static bool operator ==(TabBarTheme left, TabBarTheme right) {
            return Equals(left, right);
        }

        public static bool operator !=(TabBarTheme left, TabBarTheme right) {
            return !Equals(left, right);
        }
    }
}