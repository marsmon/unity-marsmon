using System;
using System.Collections.Generic;
using System.Text;
using Unity.UIWidgets.async;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace Unity.UIWidgets.material {
    public abstract class MaterialLocalizations {
        public abstract string openAppDrawerTooltip { get; }
        public abstract string backButtonTooltip { get; }
        public abstract string closeButtonTooltip { get; }
        public abstract string deleteButtonTooltip { get; }
        public abstract string moreButtonTooltip { get; }
        public abstract string nextMonthTooltip { get; }
        public abstract string previousMonthTooltip { get; }
        public abstract string nextPageTooltip { get; }
        public abstract string previousPageTooltip { get; }
        public abstract string showMenuTooltip { get; }
        public abstract string aboutListTileTitle(string applicationName);
        public abstract string licensesPageTitle { get; }
        public abstract string pageRowsInfoTitle(int firstRow, int lastRow, int rowCount, bool rowCountIsApproximate);
        public abstract string rowsPerPageTitle { get; }
        public abstract string tabLabel(int tabIndex, int tabCount);
        public abstract string selectedRowCountTitle(int selectedRowCount);
        public abstract string cancelButtonLabel { get; }
        public abstract string closeButtonLabel { get; }
        public abstract string continueButtonLabel { get; }
        public abstract string copyButtonLabel { get; }
        public abstract string cutButtonLabel { get; }
        public abstract string okButtonLabel { get; }
        public abstract string pasteButtonLabel { get; }
        public abstract string selectAllButtonLabel { get; }
        public abstract string viewLicensesButtonLabel { get; }

        public abstract string anteMeridiemAbbreviation { get; }

        public abstract string postMeridiemAbbreviation { get; }

        public abstract string dialogLabel { get; }

        public abstract string searchFieldLabel { get; }

        public abstract TimeOfDayFormat timeOfDayFormat(bool alwaysUse24HourFormat = false);

        public abstract ScriptCategory scriptCategory { get; }

        public abstract string formatDecimal(int number);

        public abstract string formatHour(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false);

        public abstract string formatMinute(TimeOfDay timeOfDay);

        public abstract string formatTimeOfDay(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false);

        public abstract string formatYear(DateTime date);

        public abstract string formatCompactDate(DateTime date);

        public abstract string formatShortDate(DateTime date);

        public abstract string formatShortMonthDay(DateTime date);

        public abstract DateTime? parseCompactDate(string inputString);

        public abstract string formatMediumDate(DateTime date);

        public abstract string formatFullDate(DateTime date);

        public abstract string formatMonthYear(DateTime date);

        public abstract List<string> narrowWeekdays { get; }

        public abstract int firstDayOfWeekIndex { get; }

        public abstract string modalBarrierDismissLabel { get; }

        public static MaterialLocalizations of(BuildContext context) {
            return Localizations.of<MaterialLocalizations>(context, typeof(MaterialLocalizations));
        }
    }

    class _MaterialLocalizationsDelegate : LocalizationsDelegate<MaterialLocalizations> {
        public _MaterialLocalizationsDelegate() {
        }

        public override bool isSupported(Locale locale) {
            return locale.languageCode == "en";
        }

        public override Future<WidgetsLocalizations> load(Locale locale) {
            return DefaultMaterialLocalizations.load(locale).to<WidgetsLocalizations>();
        }

        public override bool shouldReload(LocalizationsDelegate old) {
            return false;
        }

        public override string ToString() {
            return "DefaultMaterialLocalizations.delegate(en_US)";
        }
    }

    public class DefaultMaterialLocalizations : MaterialLocalizations {
        public DefaultMaterialLocalizations() {
        }

        static readonly List<string> _shortWeekdays = new List<string>() {
            "Mon",
            "Tue",
            "Wed",
            "Thu",
            "Fri",
            "Sat",
            "Sun",
        };

        static readonly List<string> _weekdays = new List<string>() {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday",
        };

        static readonly List<string> _narrowWeekdays = new List<string>() {
            "S",
            "M",
            "T",
            "W",
            "T",
            "F",
            "S",
        };

        static readonly List<string> _shortMonths = new List<string>() {
            "Jan",
            "Feb",
            "Mar",
            "Apr",
            "May",
            "Jun",
            "Jul",
            "Aug",
            "Sep",
            "Oct",
            "Nov",
            "Dec",
        };

        static readonly List<string> _months = new List<string>() {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
        };

        static int _getDaysInMonth(int year, int month) {
            if (month == 2) {
                bool isLeapYear = (year % 4 == 0) && (year % 100 != 0) ||
                                  (year % 400 == 0);
                if (isLeapYear) {
                    return 29;
                }

                return 28;
            }

            int[] daysInMonth = new int[] {31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31};
            return daysInMonth[month - 1];
        }

        public override string formatHour(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false) {
            TimeOfDayFormat format = timeOfDayFormat(alwaysUse24HourFormat: alwaysUse24HourFormat);
            switch (format) {
                case TimeOfDayFormat.h_colon_mm_space_a:
                    return formatDecimal(timeOfDay.hourOfPeriod == 0 ? 12 : timeOfDay.hourOfPeriod);
                case TimeOfDayFormat.HH_colon_mm:
                    return _formatTwoDigitZeroPad(timeOfDay.hour);
                default:
                    throw new AssertionError($"runtimeType does not support {format}.");
            }
        }

        string _formatTwoDigitZeroPad(int number) {
            D.assert(0 <= number && number < 100);

            if (number < 10) {
                return "0" + number;
            }

            return number.ToString();
        }

        public override string formatMinute(TimeOfDay timeOfDay) {
            int minute = timeOfDay.minute;
            return minute < 10 ? "0" + minute : minute.ToString();
        }

        public override string formatYear(DateTime date) {
            return date.Year.ToString();
        }

        public override string formatCompactDate(DateTime date) {
            string month = _formatTwoDigitZeroPad(date.Month);
            string day = _formatTwoDigitZeroPad(date.Day);
            string year = date.Year.ToString().PadLeft(4, '0');
            return $"{month}/{day}/{year}";
        }

        public override string formatShortDate(DateTime date) {
            string month = _shortMonths[date.Month - 1];
            return $"{month} {date.Day}, {date.Year}";
        }

        public override string formatMediumDate(DateTime date) {
            string day = _shortWeekdays[((int) date.DayOfWeek + 6) % 7];
            string month = _shortMonths[date.Month - 1];
            return $"{day}, {month} {date.Day}";
        }

        public override string formatFullDate(DateTime date) {
            string month = _months[date.Month - 1];
            return $"{_weekdays[((int) date.DayOfWeek + 6) % 7]}, {month} {date.Day}, {date.Year}";
        }

        public override string formatMonthYear(DateTime date) {
            string year = formatYear(date);
            string month = _months[date.Month - 1];
            return $"{month} {year}";
        }

        public override string formatShortMonthDay(DateTime date) {
            string month = _shortMonths[date.Month - 1];
            return $"{month} {date.Day}";
        }

        public override DateTime? parseCompactDate(string inputString) {
            string[] inputParts = inputString.Split('/');
            if (inputParts.Length != 3) {
                return null;
            }

            bool success = int.TryParse(inputParts[2], out int year);
            if (!success || year < 1) {
                return null;
            }

            success = int.TryParse(inputParts[0], out int month);
            if (!success || month < 1 || month > 12) {
                return null;
            }

            success = int.TryParse(inputParts[1], out int day);
            if (!success || day < 1 || day > _getDaysInMonth(year, month)) {
                return null;
            }

            return new DateTime(year, month, day);
        }

        public override List<string> narrowWeekdays {
            get { return _narrowWeekdays; }
        }

        public override int firstDayOfWeekIndex {
            get { return 0; }
        }

        string _formatDayPeriod(TimeOfDay timeOfDay) {
            switch (timeOfDay.period) {
                case DayPeriod.am:
                    return anteMeridiemAbbreviation;
                case DayPeriod.pm:
                    return postMeridiemAbbreviation;
            }

            return null;
        }

        public override string formatDecimal(int number) {
            if (number > -1000 && number < 1000) {
                return number.ToString();
            }

            string digits = number.abs().ToString();
            StringBuilder result = new StringBuilder(number < 0 ? "-" : "");
            int maxDigitIndex = digits.Length - 1;
            for (int i = 0; i <= maxDigitIndex; i += 1) {
                result.Append(digits[i]);
                if (i < maxDigitIndex && (maxDigitIndex - i) % 3 == 0) {
                    result.Append(',');
                }
            }

            return result.ToString();
        }

        public override string formatTimeOfDay(TimeOfDay timeOfDay, bool alwaysUse24HourFormat = false) {
            StringBuilder buffer = new StringBuilder();

            buffer.Append(formatHour(timeOfDay, alwaysUse24HourFormat: alwaysUse24HourFormat));
            buffer.Append(":");
            buffer.Append(formatMinute(timeOfDay));

            if (alwaysUse24HourFormat) {
                return buffer.ToString();
            }

            buffer.Append(" ");
            buffer.Append(_formatDayPeriod(timeOfDay));
            return buffer.ToString();
        }

        public override string openAppDrawerTooltip {
            get { return "Open navigation menu"; }
        }

        public override string backButtonTooltip {
            get { return "Back"; }
        }

        public override string closeButtonTooltip {
            get { return "Close"; }
        }

        public override string deleteButtonTooltip {
            get { return "Delete"; }
        }

        public override string moreButtonTooltip {
            get { return "More"; }
        }

        public override string nextMonthTooltip {
            get { return "Next month"; }
        }

        public override string previousMonthTooltip {
            get { return "Previous month"; }
        }

        public override string nextPageTooltip {
            get { return "Next page"; }
        }

        public override string previousPageTooltip {
            get { return "Previous page"; }
        }

        public override string showMenuTooltip {
            get { return "Show menu"; }
        }

        public override string dialogLabel {
            get {
                return "Dialog";
            }
        }

        public override string searchFieldLabel {
            get { return "Search"; }
        }

        public override string aboutListTileTitle(string applicationName) {
            return "About " + applicationName;
        }

        public override string licensesPageTitle {
            get { return "Licenses"; }
        }

        public override string pageRowsInfoTitle(int firstRow, int lastRow, int rowCount, bool rowCountIsApproximate) {
            return rowCountIsApproximate
                ? $"{firstRow}–{lastRow} of about {rowCount}"
                : $"{firstRow}–{lastRow} of {rowCount}";
        }

        public override string rowsPerPageTitle {
            get { return "Rows per page:"; }
        }

        public override string tabLabel(int tabIndex, int tabCount) {
            D.assert(tabIndex >= 1);
            D.assert(tabCount >= 1);
            return $"Tab {tabIndex} of {tabCount}";
        }

        public override string selectedRowCountTitle(int selectedRowCount) {
            switch (selectedRowCount) {
                case 0:
                    return "No items selected";
                case 1:
                    return "1 item selected";
                default:
                    return selectedRowCount + " items selected";
            }
        }

        public override string cancelButtonLabel {
            get { return "CANCEL"; }
        }

        public override string closeButtonLabel {
            get { return "CLOSE"; }
        }

        public override string continueButtonLabel {
            get { return "CONTINUE"; }
        }

        public override string copyButtonLabel {
            get { return "COPY"; }
        }

        public override string cutButtonLabel {
            get { return "CUT"; }
        }

        public override string okButtonLabel {
            get { return "OK"; }
        }

        public override string pasteButtonLabel {
            get { return "PASTE"; }
        }

        public override string selectAllButtonLabel {
            get { return "SELECT ALL"; }
        }

        public override string viewLicensesButtonLabel {
            get { return "VIEW LICENSES"; }
        }

        public override string anteMeridiemAbbreviation {
            get { return "AM"; }
        }

        public override string postMeridiemAbbreviation {
            get { return "PM"; }
        }

        public override string modalBarrierDismissLabel {
            get { return "Dismiss"; }
        }

        public override ScriptCategory scriptCategory {
            get { return ScriptCategory.englishLike; }
        }

        public override TimeOfDayFormat timeOfDayFormat(bool alwaysUse24HourFormat = false) {
            return alwaysUse24HourFormat
                ? TimeOfDayFormat.HH_colon_mm
                : TimeOfDayFormat.h_colon_mm_space_a;
        }

        public static Future<MaterialLocalizations> load(Locale locale) {
            return new SynchronousFuture<MaterialLocalizations>(new DefaultMaterialLocalizations());
        }

        public static readonly LocalizationsDelegate<MaterialLocalizations> del = new _MaterialLocalizationsDelegate();
    }
}