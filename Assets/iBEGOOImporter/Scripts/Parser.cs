using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class Parser
{

    public static List<string> GetUrls(string input)
    {
        List<string> result = new List<string>();
        MatchCollection matchList = Regex.Matches(input, "(?i)\\b((?:https?://|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))");
        result = matchList.Cast<Match>().Select(match => match.Value).ToList();
        return result;
    }

    public static List<string> GetUrlsContaining(List<string> urls, string keyword)
    {
        List<string> result = new List<string>();
        foreach (string url in urls)
        {
            if (url.Contains(keyword))
            {
                result.Add(url);
            }
        }
        return result;
    }

    public static bool StringContainsListItem(string str, List<string> list)
    {
        foreach(string item in list)
        {
            if (str.Contains(item))
            {
                return true;
            }
        }
        return false;
    }

}
