using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpaMenu.Commons.Api.Commons
{
    /// <summary>
    /// Prove metodos para substituir valores de propriedades sensíveis por mascaras.
    /// </summary>
    public static class SensitivePropertyReplacer
    {
        public const string DEFAULT_REPLACEMENT_MASK = "****";
        /// <summary>
        /// Retorna o valor mascarado de uma propriedade sensível.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
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
