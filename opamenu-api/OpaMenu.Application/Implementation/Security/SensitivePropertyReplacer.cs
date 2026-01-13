using OpaMenu.Application.Implementation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Application.Implementation.Security
{
    public static class SensitivePropertyReplacer
    {
        public const string DEFAULT_REPLACEMENT_MASK = "****";

        public static string ToMaskedString(string propertyName, string propertyValue)
        {

            if (SensitivePropertyValidator.IsCpfDocumentProperty(propertyName))
                return FieldFormatter.MaskCpfNumber(propertyValue);

            if (SensitivePropertyValidator.IsCnpjDocumentProperty(propertyName))
                return FieldFormatter.MaskCnpjNumber(propertyValue);

            if (SensitivePropertyValidator.IsCardNumberProperty(propertyName))
                return FieldFormatter.MaskCardNumber(propertyValue);

            if (SensitivePropertyValidator.IsSensitive(propertyName))
                return DEFAULT_REPLACEMENT_MASK;

            return propertyValue;
        }
    }
}
