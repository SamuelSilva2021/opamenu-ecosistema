using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Common.Localization
{
    public static class Culture
    {
        public static CultureInfo New(string culture)
        {
            if (string.IsNullOrWhiteSpace(culture))
                return CommonLocalization.DefaultCulture;

            try
            {
                return new CultureInfo(culture);
            }
            catch (Exception)
            {
                return CommonLocalization.DefaultCulture;
            }
        }

        public static string ToShortDateCulture(this DateTime date, CultureInfo? culture = null)
        {
            var cultureInfo = culture ?? CommonLocalization.CurrentCulture;
            var shortDateFormatString = cultureInfo.DateTimeFormat.ShortDatePattern;
            return date.ToString(shortDateFormatString);
        }
    }
}
