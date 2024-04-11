using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TrxerConsole;

public class XsltExtensions
{
    public string RemoveAssemblyName(string asm) 
    {
        int idx = asm.IndexOf(',');
        if (idx == -1)
            return asm;
        return asm.Substring(0, idx);
    }
    public string RemoveNamespace(string asm) 
    {
        int coma = asm.IndexOf(',');
        return asm.Substring(coma + 2, asm.Length - coma - 2);
    }
    public string GetShortDateTime(string time)
    {
        if( string.IsNullOrEmpty(time) )
        {
            return string.Empty;
        }
      
        return DateTime.Parse(time).ToString(CultureInfo.InvariantCulture);
    }
    
    private string ToExtactTime(double ms)
    {
        if (ms < 1000)
            return $"{ms:0.00} ms";

        if (ms >= 1000 && ms < 60000)
            return $"{TimeSpan.FromMilliseconds(ms).TotalSeconds:0.00} seconds";

        if (ms >= 60000 && ms < 3600000)
            return $"{TimeSpan.FromMilliseconds(ms).TotalMinutes:0.00} minutes";

        return $"{TimeSpan.FromMilliseconds(ms).TotalHours:0.00} hours";
    }
    
    public string ToExactTimeDefinition(string duration)
    {
        if( string.IsNullOrEmpty(duration) )
        {
            return string.Empty;
        } 
    
        return  ToExtactTime(TimeSpan.Parse(duration).TotalMilliseconds);
    }
    
    public string ToExactTimeDefinition(string start,string finish)
    {
        TimeSpan datetime=DateTime.Parse(finish)-DateTime.Parse(start);
        return ToExtactTime(datetime.TotalMilliseconds);
    }
    
    public string CurrentDateTime()
    {
        return DateTime.Now.ToString(CultureInfo.InvariantCulture);
    }
    
    public string ExtractImageUrl(string text)
    {
        Match match = Regex.Match(text, "('|\")([^\\s]+(\\.(?i)(jpg|png|gif|bmp)))('|\")",
            RegexOptions.IgnoreCase);

	
        if (match.Success)
        {
            return match.Value.Replace("\'",string.Empty).Replace("\"",string.Empty).Replace("\\","\\\\");
        }
        return string.Empty;
    }
}