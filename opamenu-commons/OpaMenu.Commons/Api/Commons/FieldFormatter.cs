using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Commons
{
    public static class FieldFormatter
    {
        private const char MASK = '*';

        public static string MaskPhoneNumber(string phone) => MaskDigits(phone, 2, 2);

        public static string MaskCardNumber(string cardNumber) => MaskDigits(cardNumber, 6, 4);

        public static string MaskCpfNumber(string cpf) => MaskDigits(cpf, 3, 2);

        public static string MaskCnpjNumber(string cnpj) => MaskDigits(cnpj, 3, 3);

        public static string MaskDigits(string str, int preserveStartDigits, int preserveEndDigits)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var sb = new StringBuilder(str);

            var startIndex = 0;
            var startCount = 0;
            var endCount = 0;

            // Find the start index to preserve and not be masked
            while (startIndex < sb.Length && startCount < preserveStartDigits)
            {
                if (char.IsDigit(sb[startIndex]))
                {
                    startCount++;
                }

                startIndex++;
            }

            // Loop backwards preserving the end number of digits and masking the rest
            for (var i = sb.Length - 1; i >= startIndex; i--)
            {
                if (char.IsDigit(sb[i]) is false) continue;

                if (endCount < preserveEndDigits)
                    endCount++;
                else
                    sb[i] = MASK;
            }

            return sb.ToString();
        }

        public static string MaskEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return email;

            const char at = '@';
            const int preserveStartCount = 3;
            const int preserveEndCount = 1;

            var hiddenMask = new string(MASK, 10);

            var split = email.Split(at);

            var lengthBeforeAt = split[0].Length;

            // More than 6 chars, keep visible first 3 and last 1
            if (lengthBeforeAt > 6)
            {
                // Preserves the first 3
                var preserveStart = split[0][..preserveStartCount];
                // preserves the last 1
                var preserveEnd = split[0][(lengthBeforeAt - preserveEndCount)..];
                return $"{preserveStart}{hiddenMask}{preserveEnd}{at}{split[1]}";
            }

            // Between 0 and 6 chars, preserves the first 3 and mask the rest 
            var lengthVisible = Math.Min(3, lengthBeforeAt);
            var visibleChars = split[0][..lengthVisible];
            return $"{visibleChars}{hiddenMask}{at}{split[1]}";
        }

        public static string MaskString(string str, int preserveStartChars = 1, int preserveEndChars = 1)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var sb = new StringBuilder(str);

            var startIndex = Math.Max(preserveStartChars < sb.Length ? preserveStartChars : sb.Length, 0);
            var endCount = 0;

            // Loop backwards preserving the end number of chars and masking the rest
            for (var i = sb.Length - 1; i >= startIndex; i--)
            {
                if (endCount < preserveEndChars)
                    endCount++;
                else
                    sb[i] = MASK;
            }

            return sb.ToString();
        }
    }
}
