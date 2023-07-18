using System.ComponentModel.DataAnnotations;

namespace ModelValidation.Extensions
{
    public static class StringExtensions
    {
        private static readonly EmailAddressAttribute _emailValidator = new();
        private static readonly PhoneAttribute _phoneValidator = new();
        private static readonly UrlAttribute _urlValidator = new();

        /// <summary>
        /// Returns a boolean value indicating whether the string is a valid email address.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsEmailAddress(this string value)
        {
            return _emailValidator.IsValid(value);
        }

        /// <summary>
        /// Returns a boolean value indicating whether the string is a valid phone number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string value)
        {
            return _phoneValidator.IsValid(value);
        }

        /// <summary>
        /// Returns a boolean value indicating whether the string is a valid URL.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsUrl(this string value)
        {
            return _urlValidator.IsValid(value);
        }
    }
}