using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Modules;

namespace DotNetNuke.Modules.UserDefinedTable.Controllers
{
    public class FriendlyUrlHelpers
    {
        /// <summary>
        /// Get friendly URL
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static string GetFriendlyUrl(int moduleId)
        {
            var tabId = moduleId > 0 ? GetTabId(moduleId) : 0;
            return Common.Globals.NavigateURL(tabId);
        }

        /// <summary>
        /// Get Tab ID by module ID
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public static int GetTabId(int moduleId)
        {
            var mc = new ModuleController();
            var module = mc.GetModule(moduleId);
            return module != null ? module.TabID : 0;
        }

        public static string MakeUrlSafe(string urlName, string punctuationReplacement, int maxLength)
        {
            if (urlName == null) urlName = "";
            urlName = urlName.Normalize(NormalizationForm.FormD);
            const string illegalCharacters = "#%&*{}\\:<>?/+'.";
            const string unwantedCharacters = ";,\"+!'{}[]()^$*";
            var outUrl = new StringBuilder(urlName.Length);
            var i = 0;
            foreach (var c in urlName)
            {
                if (!illegalCharacters.Contains(c.ToString(CultureInfo.InvariantCulture)))
                {
                    //can't have leading .. or trailing .
                    if (!((i <= 0 || i == urlName.Length) && c == '.'))
                    {
                        if (c == ' ' || unwantedCharacters.Contains(c.ToString(CultureInfo.InvariantCulture)))
                            //replace spaces, commas and semicolons
                            outUrl.Append(punctuationReplacement);
                        else if (CharUnicodeInfo.GetUnicodeCategory(c)
                                 != UnicodeCategory.NonSpacingMark)
                        {
                            outUrl.Append(c);
                        }
                    }
                }
                i++;
                if (i >= maxLength)
                    break;
            }
            var result = outUrl.ToString();
            //replace double replacements
            var doubleReplacement = punctuationReplacement + punctuationReplacement;
            if (result.Contains(doubleReplacement))
            {
                result = result.Replace(doubleReplacement, punctuationReplacement);
                //once more for triples
                result = result.Replace(doubleReplacement, punctuationReplacement);
            }
            return result;
        }

        /// <summary>
        /// parse url format to get id
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="idPattern">office-(\d+)- or employee-(\d+)-</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ReturnItemIdFromPath(string urlPath, string idPattern, out int value)
        {
            value = 0; //initialise to 0 to show if failed
            var success = false;
            var idRegex = new Regex(idPattern, RegexOptions.IgnoreCase);
            var idmatch = idRegex.Match(urlPath);
            if (!idmatch.Success) return false;

            var rawValue = idmatch.Groups[1].Value;
            if (int.TryParse(rawValue, out value))
                success = true;
            return success;
        }
    }
}