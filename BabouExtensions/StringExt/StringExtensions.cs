﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BabouExtensions
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string into proper title case.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="lowerCaseWords">Words to lowercase</param>
        /// <returns></returns>
        public static string ToTitleCase(this string source, string[] lowerCaseWords = null)
        {
            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(source.ToLower());

            if (lowerCaseWords != null)
            {
                const RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
                var lowerWordsSplit = string.Join("|", lowerCaseWords);
                result = Regex.Replace(result, $@"({lowerWordsSplit})", m => m.Value.ToLower(), options);
            }

            return result;
        }

        /// <summary>
        /// Uppercases the first letter of a string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string UppercaseFirstLetter(this string source)
        {
            // Uppercase the first letter in the string.
            if (source.Length <= 0) return source;

            var array = source.ToCharArray();
            array[0] = char.ToUpper(array[0]);
            return new string(array);
        }

        /// <summary>
        /// Truncates your string and optionally adds an ellipses suffix
        /// </summary>
        /// <param name="source">Original string</param>
        /// <param name="length">How long should the string be?</param>
        /// <param name="showSuffix">Do you want to show ellipses?</param>
        /// <returns></returns>
        [Obsolete("This method is obsolete. Use WithMaxLength instead.")]
        public static string Truncate(this string source, int length, bool showSuffix)
        {
            var truncatedString = string.Empty;

            if (!string.IsNullOrEmpty(source) && source.Length >= length)
            {
                const string suffix = "...";

                truncatedString = source.Substring(0, length);

                if (showSuffix)
                {
                    truncatedString = $"{source}{suffix}";
                }
            }
            return truncatedString;
        }


        /// <summary>
        /// Truncates a string and optionally adds suffix
        /// </summary>
        /// <param name="source">The original strength</param>
        /// <param name="maxLength">The max length of the string</param>
        /// <param name="suffix">Suffix to add to the end of the string</param>
        /// <returns></returns>
        public static string WithMaxLength(this string source, int maxLength, string suffix = null)
        {
            if (source == null)
                return null;

            if (source.Length <= maxLength) 
                return source;

            var returnValue = source.Substring(0, Math.Min(source.Length, maxLength));

            if (suffix != null)
            {
                returnValue = $"{returnValue}{suffix}";
            }

            return returnValue;
        }

        /// <summary>
        /// Goes through the string and adds a space between capital letters, excluding the first character.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string AddSpacesToSentence(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
                return string.Empty;

            var newText = new StringBuilder(source.Length * 2);
            newText.Append(source[0]);

            for (var i = 1; i < source.Length; i++)
            {
                if (char.IsUpper(source[i]) && source[i - 1] != ' ')
                    newText.Append(' ');
                newText.Append(source[i]);
            }
            return newText.ToString();
        }

        /// <summary>
        /// Returns just the digits from a string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetTheDigits(this string source) => new Regex(@"[^\d]").Replace(source, string.Empty);

        /// <summary>
        /// Returns just alpha characters.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StripSpacesAndNonAlphaNumeric(this string source) =>
            new Regex(@"[^A-Za-z0-9]+").Replace(source, string.Empty);

        /// <summary>
        /// Removes HTML from a string.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="replacementString">What to replace the HTML with. Defaults to an empty space.</param>
        /// <returns></returns>
        public static string StripHtml(this string source, string replacementString = " ") => new Regex(@"<[^>]*>").Replace(source, replacementString);

        /// <summary>
        /// Removes line endings
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveLineEndings(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            var lineSeparator = ((char)0x2028).ToString();
            var paragraphSeparator = ((char)0x2029).ToString();

            return source.Replace("\r\n", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty)
                .Replace(lineSeparator, string.Empty).Replace(paragraphSeparator, string.Empty);
        }

        /// <summary>
        /// Removes tabs, line breaks, extra spaces, etc. Trims string as well.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="additionalReplacements"></param>
        /// <returns></returns>
        public static string CleanString(this string source, string[] additionalReplacements = null)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            source = Regex.Replace(source, @"\r\n?|\n|\t", string.Empty);
            source = Regex.Replace(source, @"(<br \/?>|<br\/?>|<\/? br>|<\/?br>)", string.Empty);
            source = Regex.Replace(source, @"(<p>)|(<\/p>)", string.Empty);
            source = Regex.Replace(source, @"[ ]{2,}", string.Empty);
            if (additionalReplacements != null)
                source = additionalReplacements.Aggregate(source, (current, word) => current.Replace(word, string.Empty));

            source = source.Trim();

            return source;
        }

        /// <summary>
        /// Removes Trailing Spaces
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveTrailingSpaces(this string source)
        {
            return source?.TrimStart().TrimEnd();
        }

        /// <summary>
        /// Gets words from the source
        /// </summary>
        /// <param name="source"></param>
        /// <param name="count"></param>
        /// <param name="wordDelimiter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] GetWords(this string source, int count = -1, string[] wordDelimiter = null, StringSplitOptions options = StringSplitOptions.None)
        {
            if (string.IsNullOrEmpty(source))
                return new string[] { };

            if (count < 0)
                return source.Split(wordDelimiter, options);

            var words = source.Split(wordDelimiter, count + 1, options);
            if (words.Length <= count)
                return words; // not so many words found

            // remove last "word" since that contains the rest of the string
            Array.Resize(ref words, words.Length - 1);

            return words;
        }

        /// <summary>
        /// Removes special characters that are often generated by word.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string CleanWordFormatting(this string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                return source.Replace('\u2013', '-')
                    .Replace('\u2013', '-')
                    .Replace('\u2014', '-')
                    .Replace('\u2015', '-')
                    .Replace('\u2017', '_')
                    .Replace('\u2018', '\'')
                    .Replace('\u2019', '\'')
                    .Replace('\u201a', ',')
                    .Replace('\u201b', '\'')
                    .Replace('\u201c', '\"')
                    .Replace('\u201d', '\"')
                    .Replace('\u201e', '\"')
                    .Replace("\u2026", "...")
                    .Replace('\u2032', '\'')
                    .Replace('\u2033', '\"');
            }
            return source;
        }

        /// <summary>
        /// Produces optional, URL-friendly version of a title, "like-this-one".
        /// </summary>
        /// <param name="source">String to make URL Friendly</param>
        /// <param name="maxLength">Maximum length of the url</param>
        public static string UrlFriendly(this string source, int maxLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            var len = source.Length;
            var prevdash = false;
            var sb = new StringBuilder(len);

            for (var i = 0; i < len; i++)
            {
                var c = source[i];
                if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
                {
                    sb.Append(c);
                    prevdash = false;
                }
                else if (c >= 'A' && c <= 'Z')
                {
                    // tricky way to convert to lowercase
                    sb.Append((char)(c | 32));
                    prevdash = false;
                }
                else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                         c == '\\' || c == '-' || c == '_' || c == '=')
                {
                    if (!prevdash && sb.Length > 0)
                    {
                        sb.Append('-');
                        prevdash = true;
                    }
                }
                else if (c >= 128)
                {
                    var prevlen = sb.Length;
                    sb.Append(RemapInternationalCharToAscii(c));
                    if (prevlen != sb.Length)
                        prevdash = false;
                }
                if (i == maxLength)
                    break;
            }

            return prevdash ? sb.ToString().Substring(0, sb.Length - 1) : sb.ToString();
        }

        /// <summary>
        /// Remaps International Char to Ascii
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string RemapInternationalCharToAscii(char c)
        {
            var s = c.ToString().ToLowerInvariant();
            if ("àåáâäãåą".Contains(s))
            {
                return "a";
            }
            if ("èéêëę".Contains(s))
            {
                return "e";
            }
            if ("ìíîïı".Contains(s))
            {
                return "i";
            }
            if ("òóôõöøőð".Contains(s))
            {
                return "o";
            }
            if ("ùúûüŭů".Contains(s))
            {
                return "u";
            }
            if ("çćčĉ".Contains(s))
            {
                return "c";
            }
            if ("żźž".Contains(s))
            {
                return "z";
            }
            if ("śşšŝ".Contains(s))
            {
                return "s";
            }
            if ("ñń".Contains(s))
            {
                return "n";
            }
            if ("ýÿ".Contains(s))
            {
                return "y";
            }
            if ("ğĝ".Contains(s))
            {
                return "g";
            }
            if (c == 'ř')
            {
                return "r";
            }
            if (c == 'ł')
            {
                return "l";
            }
            if (c == 'đ')
            {
                return "d";
            }
            if (c == 'ß')
            {
                return "ss";
            }
            if (c == 'Þ')
            {
                return "th";
            }
            if (c == 'ĥ')
            {
                return "h";
            }
            if (c == 'ĵ')
            {
                return "j";
            }
            return "";
        }

        /// <summary>
        /// Checks if source is a valid URL
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsValidUrl(this string source) => Uri.TryCreate(source, UriKind.Absolute, out var uri)
                                                             && (uri.Scheme == Uri.UriSchemeHttp
                                                                 || uri.Scheme == Uri.UriSchemeHttps
                                                                 || uri.Scheme == Uri.UriSchemeFtp
                                                                 || uri.Scheme == Uri.UriSchemeMailto);

        /// <summary>
        /// Generates a list from a string based on the delimiter.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter"></param>
        /// <param name="replaceLineBreaksAndTabs">Replaces line breaks and tabs with delimiter.</param>
        /// <returns></returns>
        public static List<string> GetList(this string source, char delimiter = ',', bool replaceLineBreaksAndTabs = true)
        {
            if (string.IsNullOrEmpty(source))
                return new List<string>();

            var charString = delimiter.ToString();

            var cleanString = source;

            if (replaceLineBreaksAndTabs)
            {
                cleanString = Regex.Replace(source, @"\r\n?|\n", charString);
                cleanString = cleanString.Replace("\t", charString);
            }

            var stringList = cleanString.Split(delimiter).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).Distinct().ToList();
            return stringList;
        }

        /// <summary>
        /// Generates a list from a string based on the delimiter. Replaces line breaks and tabs with delimiter.
        /// </summary>
        /// <param name="source">Source string</param>
        /// <param name="delimiter">Character to split on</param>
        /// <param name="sourceList">If source is empty, returns an empty list</param>
        /// <returns>Either an empty list if source is empty or a list of strings.</returns>
        public static bool TryGetList(this string source, char delimiter, out List<string> sourceList)
        {
            if (string.IsNullOrEmpty(source))
            {
                sourceList = new List<string>();
                return false;
            }

            var charString = delimiter.ToString();

            var cleanString = Regex.Replace(source, @"\r\n?|\n", charString);
            cleanString = cleanString.Replace("\t", charString);

            var stringList = cleanString.Split(delimiter).Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim()).Distinct().ToList();
            sourceList = stringList;
            return true;
        }

        /// <summary>
        /// Indicates whether the specified string is null or an empty string
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            return string.IsNullOrEmpty(source);
        }

        /// <summary>
        /// Indicates whether a specified string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string source)
        {
            return string.IsNullOrWhiteSpace(source);
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string source)
        {
            return string.IsNullOrEmpty(source) ? string.Empty : System.Net.WebUtility.HtmlDecode(source);
        }

        /// <summary>
        /// Converts a string into an HTML-encoded string.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string source)
        {
            return string.IsNullOrEmpty(source) ? string.Empty : System.Net.WebUtility.HtmlEncode(source);
        }

        /// <summary>
        /// Determines if a string is a valid date. Returns dateTime as a date or null.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static bool IsDate(this string source, out DateTime? dateTime)
        {
            if (!string.IsNullOrEmpty(source) && System.DateTime.TryParse(source, out var realDateTime))
            {
                dateTime = realDateTime;
                return true;
            }
            else
            {
                dateTime = null;
                return false;
            }
        }

        /// <summary>
        /// Encrypts a string using the supplied key. Encoding is done using RSA encryption.
        /// Only works on Windows as it requires the Windows Cryptographic API.
        /// </summary>
        /// <param name="source">String that must be encrypted.</param>
        /// <param name="key">Encryption Key.</param>
        /// <param name="useAes">If true, uses the new method of encryption.</param>
        /// <returns>A string representing a byte array separated by a minus sign.</returns>
        /// <exception cref="ArgumentNullException">Occurs when source or key is null or empty.</exception>
        [Obsolete("This method is obsolete as it relies on Windows. To keep the method name, set useAes to true or use EncryptUsingAes.")]
        public static string Encrypt(this string source, string key, bool useAes = false)
        {
            if (useAes)
                return EncryptUsingAes(source, key);

            if (source.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(source), "Source string cannot by empty or null.");

            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key), "Key cannot be empty or null.");

            var cspp = new CspParameters
            {
                KeyContainerName = key
            };

            var rsa = new RSACryptoServiceProvider(cspp)
            {
                PersistKeyInCsp = true
            };

            var bytes = rsa.Encrypt(Encoding.UTF8.GetBytes(source), true);

            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// Decrypts a string using the supplied key. Decoding is done using RSA encryption.
        /// Only works on Windows as it requires the Windows Cryptographic API.
        /// </summary>
        /// <param name="source">String that must be decrypted.</param>
        /// <param name="key">The Decryption Key.</param>
        /// <param name="useAes">If true, uses the new method of decryption</param>
        /// <returns>The decrypted string or null if decryption failed.</returns>
        /// <exception cref="ArgumentNullException">Occurs when source or key is null or empty.</exception>
        [Obsolete("This method is obsolete as it relies on Windows. To keep the method name, set useAes to true or use DecryptUsingAes.")]
        public static string Decrypt(this string source, string key, bool useAes = false)
        {
            if (useAes)
                return DecryptUsingAes(source, key);

            string result = null;

            if (source.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(source), "Source string cannot by empty or null.");

            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key), "Key cannot be empty or null.");

            var cspp = new CspParameters
            {
                KeyContainerName = key
            };

            var rsa = new RSACryptoServiceProvider(cspp);
            rsa.PersistKeyInCsp = true;

            var decryptArray = source.Split(new[] { "-" }, StringSplitOptions.None);
            var decryptByteArray = Array.ConvertAll(decryptArray, (s => Convert.ToByte(byte.Parse(s, NumberStyles.HexNumber))));

            var bytes = rsa.Decrypt(decryptByteArray, true);

            result = Encoding.UTF8.GetString(bytes);

            return result;
        }

        /// <summary>
        /// Encrypts a string using AES Encryption.
        /// </summary>
        /// <param name="source">The string you wish to encrypt.</param>
        /// <param name="key">The key used for encryption.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Occurs when source string or key is null or empty.</exception>
        /// <exception cref="ArgumentException">Occurs when unable to create a AES key.</exception>
        public static string EncryptUsingAes(this string source, string key)
        {
            if (source.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(source),"Source string cannot by empty or null.");

            if (key.IsNullOrEmpty())
                throw new ArgumentNullException(nameof(key), "Key cannot be empty or null.");

            var buffer = Encoding.UTF8.GetBytes(source);
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using var aes = Aes.Create();
            if (aes == null)
                throw new ArgumentException("Parameter must not be null.", nameof(aes));

            aes.Key = aesKey;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var resultStream = new MemoryStream();
            using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
            {
                using var plainStream = new MemoryStream(buffer);
                plainStream.CopyTo(aesStream);
            }

            var result = resultStream.ToArray();
            var combined = new byte[aes.IV.Length + result.Length];
            Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
            Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

            return Convert.ToBase64String(combined);
        }

        /// <summary>
        /// Decrypts a string using AES Encryption.
        /// </summary>
        /// <param name="source">The string you wish to decrypt.</param>
        /// <param name="key">The key used for encryption.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Occurs when source string or key is null or empty.</exception>
        /// <exception cref="ArgumentException">Occurs when unable to create an AES key.</exception>
        public static string DecryptUsingAes(this string source, string key)
        {
            if (source.IsNullOrEmpty())
                throw new ArgumentException("The source must have valid value.", nameof(source));

            if (key.IsNullOrEmpty())
                throw new ArgumentException("Key must have valid value.", nameof(key));

            var combined = Convert.FromBase64String(source);
            var buffer = new byte[combined.Length];
            var hash = new SHA512CryptoServiceProvider();
            var aesKey = new byte[24];
            Buffer.BlockCopy(hash.ComputeHash(Encoding.UTF8.GetBytes(key)), 0, aesKey, 0, 24);

            using var aes = Aes.Create();
            if (aes == null)
                throw new ArgumentException("Parameter must not be null.", nameof(aes));

            aes.Key = aesKey;

            var iv = new byte[aes.IV.Length];
            var cypherText = new byte[buffer.Length - iv.Length];

            Array.ConstrainedCopy(combined, 0, iv, 0, iv.Length);
            Array.ConstrainedCopy(combined, iv.Length, cypherText, 0, cypherText.Length);

            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using var resultStream = new MemoryStream();
            using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
            {
                using var plainStream = new MemoryStream(cypherText);
                plainStream.CopyTo(aesStream);
            }

            return Encoding.UTF8.GetString(resultStream.ToArray());
        }

        /// <summary>
        /// Tries to get a URL from a string. Also uses IsValidUrl extension.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool TryGetUrl(this string source, out string url)
        {
            const string urlRegex = @"(?:(?:https?):\/\/|www\.|ftp\.)(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[-A-Z0-9+&@#\/%=~_|$?!:,.])*(?:\([-A-Z0-9+&@#\/%=~_|$?!:,.]*\)|[A-Z0-9+&@#\/%=~_|$])";

            if (Regex.IsMatch(source, urlRegex, RegexOptions.IgnoreCase))
            {
                var tempUrl = Regex.Match(source, urlRegex, RegexOptions.IgnoreCase).Value;
                if (tempUrl.IsValidUrl())
                {
                    url = tempUrl;
                    return true;
                }
            }
            url = string.Empty;
            return false;
        }

        /// <summary>
        /// Returns null or date in the format provided. Default format is yyyy-MM-dd
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format">The format of the date time string.</param>
        /// <returns></returns>
        [Obsolete("This method is obsolete. Please use TryTo method instead.")]
        public static string TryGetDate(this string source, string format = "yyyy-MM-dd")
        {
            return source.IsDate(out var releaseDateTime) ? releaseDateTime?.ToString(format) : null;
        }

        /// <summary>
        /// Returns a double or null value
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        [Obsolete("This method is obsolete. Please use TryTo method instead.")]
        public static double? TryGetDouble(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            if (double.TryParse(source, out var rating))
                return rating;

            return null;
        }

        /// <summary>
        /// Easily converts a string to an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T To<T>(this string json) => JsonConvert.DeserializeObject<T>(json);
    }
}