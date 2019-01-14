using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ezfx.DataAnnotations.EmailOrPhoneAttribute
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class EmailOrPhoneAttribute : DataTypeAttribute
    {

        // This attribute provides server-side email validation equivalent to jquery validate,
        // and therefore shares the same regular expression.  See unit tests for examples.
        private static Regex _emailRegex = CreateEmailRegEx();
        private static Regex _phoneRegex = CreatePhoneRegEx();
        private const string _additionalPhoneNumberCharacters = "-.()";

        public EmailOrPhoneAttribute()
            : base(DataType.Custom)
        {

            // DevDiv 468241: set DefaultErrorMessage not ErrorMessage, allowing user to set
            // ErrorMessageResourceType and ErrorMessageResourceName to use localized messages.
            //DefaultErrorMessage = DataAnnotationsResources.EmailAddressAttribute_Invalid;
            base.ErrorMessage = "The Email/Phone field is not a valid e-mail address.";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            string valueAsString = value as string;
            // Use RegEx implementation if it has been created, otherwise use a non RegEx version.
            if (_emailRegex != null)
            {
                if (valueAsString != null && _emailRegex.Match(valueAsString).Length > 0) { return true; }
            }

            if (_phoneRegex != null)
            {
                if (valueAsString != null && _phoneRegex.Match(valueAsString).Length > 0) { return true; }
            }

            //
            if (IsEmail(valueAsString)) { return true; }

            if (IsPhone(valueAsString)) { return true; }

            return false;
        }

        private bool IsEmail(string valueAsString)
        {
            int atCount = 0;

            foreach (char c in valueAsString)
            {
                if (c == '@')
                {
                    atCount++;
                }
            }

            return (valueAsString != null
            && atCount == 1
            && valueAsString[0] != '@'
            && valueAsString[valueAsString.Length - 1] != '@');
        }

        private bool IsPhone(string valueAsString)
        {
            if (valueAsString == null)
            {
                return false;
            }

            valueAsString = valueAsString.Replace("+", string.Empty).TrimEnd();
            valueAsString = RemovePhoneExtension(valueAsString);

            bool digitFound = false;
            foreach (char c in valueAsString)
            {
                if (Char.IsDigit(c))
                {
                    digitFound = true;
                    break;
                }
            }

            if (!digitFound)
            {
                return false;
            }

            foreach (char c in valueAsString)
            {
                if (!(Char.IsDigit(c)
                    || Char.IsWhiteSpace(c)
                    || _additionalPhoneNumberCharacters.IndexOf(c) != -1))
                {
                    return false;
                }
            }
            return true;
        }

        private static string RemovePhoneExtension(string potentialPhoneNumber)
        {
            int lastIndexOfExtension = potentialPhoneNumber
                .LastIndexOf("ext.", StringComparison.InvariantCultureIgnoreCase);
            if (lastIndexOfExtension >= 0)
            {
                string extension = potentialPhoneNumber.Substring(lastIndexOfExtension + 4);
                if (MatchesPhoneExtension(extension))
                {
                    return potentialPhoneNumber.Substring(0, lastIndexOfExtension);
                }
            }

            lastIndexOfExtension = potentialPhoneNumber
                .LastIndexOf("ext", StringComparison.InvariantCultureIgnoreCase);
            if (lastIndexOfExtension >= 0)
            {
                string extension = potentialPhoneNumber.Substring(lastIndexOfExtension + 3);
                if (MatchesPhoneExtension(extension))
                {
                    return potentialPhoneNumber.Substring(0, lastIndexOfExtension);
                }
            }


            lastIndexOfExtension = potentialPhoneNumber
                .LastIndexOf("x", StringComparison.InvariantCultureIgnoreCase);
            if (lastIndexOfExtension >= 0)
            {
                string extension = potentialPhoneNumber.Substring(lastIndexOfExtension + 1);
                if (MatchesPhoneExtension(extension))
                {
                    return potentialPhoneNumber.Substring(0, lastIndexOfExtension);
                }
            }

            return potentialPhoneNumber;
        }

        private static bool MatchesPhoneExtension(string potentialExtension)
        {
            potentialExtension = potentialExtension.TrimStart();
            if (potentialExtension.Length == 0)
            {
                return false;
            }

            foreach (char c in potentialExtension)
            {
                if (!Char.IsDigit(c))
                {
                    return false;
                }
            }

            return true;
        }
        
        private static Regex CreateEmailRegEx()
        {
            // We only need to create the RegEx if this switch is enabled.
            if (AppSettings.DisableRegEx)
            {
                return null;
            }

            const string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

            // Set explicit regex match timeout, sufficient enough for email parsing
            // Unless the global REGEX_DEFAULT_MATCH_TIMEOUT is already set
            TimeSpan matchTimeout = TimeSpan.FromSeconds(2);

            try
            {
                if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null)
                {
                    return new Regex(pattern, options, matchTimeout);
                }
            }
            catch
            {
                // Fallback on error
            }

            // Legacy fallback (without explicit match timeout)
            return new Regex(pattern, options);
        }

        private static Regex CreatePhoneRegEx()
        {
            // We only need to create the RegEx if this switch is enabled.
            //if (AppSettings.DisableRegEx)
            //{
            //    return null;
            //}


            const string pattern = @"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$";
            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;

            // Set explicit regex match timeout, sufficient enough for phone parsing
            // Unless the global REGEX_DEFAULT_MATCH_TIMEOUT is already set
            TimeSpan matchTimeout = TimeSpan.FromSeconds(2);

            try
            {
                if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null)
                {
                    return new Regex(pattern, options, matchTimeout);
                }
            }
            catch
            {
                // Fallback on error
            }

            // Legacy fallback (without explicit match timeout)
            return new Regex(pattern, options);
        }
    }
}