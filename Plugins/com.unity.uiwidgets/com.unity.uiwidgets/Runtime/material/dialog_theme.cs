using System;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

namespace Unity.UIWidgets.material {
    public class DialogTheme : Diagnosticable, IEquatable<DialogTheme> {
        public DialogTheme(
            Color backgroundColor = null,
            float? elevation = null,
            ShapeBorder shape = null,
            TextStyle titleTextStyle = null,
            TextStyle contentTextStyle = null
        ) {
            this.backgroundColor = backgroundColor;
            this.elevation = elevation;
            this.shape = shape;
            this.titleTextStyle = titleTextStyle;
            this.contentTextStyle = contentTextStyle;
        }

        public readonly Color backgroundColor;

        public readonly float? elevation;

        public readonly ShapeBorder shape;

        public readonly TextStyle titleTextStyle;

        public readonly TextStyle contentTextStyle;

        DialogTheme copyWith(
            Color backgroundColor = null,
            float? elevation = null,
            ShapeBorder shape = null,
            TextStyle titleTextStyle = null,
            TextStyle contentTextStyle = null
        ) {
            return new DialogTheme(
                backgroundColor: backgroundColor ?? this.backgroundColor,
                elevation: elevation ?? this.elevation,
                shape: shape ?? this.shape,
                titleTextStyle: titleTextStyle ?? this.titleTextStyle,
                contentTextStyle: contentTextStyle ?? this.contentTextStyle
            );
        }

        public static DialogTheme of(BuildContext context) {
            return Theme.of(context).dialogTheme;
        }

        public static DialogTheme lerp(DialogTheme a, DialogTheme b, float t) {
            return new DialogTheme(
                backgroundColor: Color.lerp(a?.backgroundColor, b?.backgroundColor, t),
                elevation: MathUtils.lerpNullableFloat(a?.elevation, b?.elevation, t),
                shape: ShapeBorder.lerp(a?.shape, b?.shape, t),
                titleTextStyle: TextStyle.lerp(a?.titleTextStyle, b?.titleTextStyle, t),
                contentTextStyle: TextStyle.lerp(a?.contentTextStyle, b?.contentTextStyle, t)
            );
        }

        public bool Equals(DialogTheme other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Equals(backgroundColor, other.backgroundColor)
                   && Equals(elevation, other.elevation)
                   && Equals(shape, other.shape)
                   && Equals(titleTextStyle, other.titleTextStyle)
                   && Equals(contentTextStyle, other.contentTextStyle);
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

            return Equals((DialogTheme) obj);
        }

        public override int GetHashCode() {
            return (shape != null ? shape.GetHashCode() : 0);
        }

        public static bool operator ==(DialogTheme left, DialogTheme right) {
            return Equals(left, right);
        }

        public static bool operator !=(DialogTheme left, DialogTheme right) {
            return !Equals(left, right);
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.add(new ColorProperty("backgroundColor", backgroundColor));
            properties.add(new DiagnosticsProperty<ShapeBorder>("shape", shape));
            properties.add(new FloatProperty("elevation", elevation));
            properties.add(new DiagnosticsProperty<TextStyle>("titleTextStyle", titleTextStyle));
            properties.add(new DiagnosticsProperty<TextStyle>("contentTextStyle", contentTextStyle));
        }
    }
}