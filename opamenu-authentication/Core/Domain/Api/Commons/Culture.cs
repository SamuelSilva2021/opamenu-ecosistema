using System.Globalization;

namespace Authenticator.API.Core.Domain.Api.Commons
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
