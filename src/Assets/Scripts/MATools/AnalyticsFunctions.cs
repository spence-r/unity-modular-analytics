/// <summary>
/// Shared functions for Modular Analytics tools
/// </summary>
public class AnalyticsFunctions
{
    /// <summary>
    /// Special values or escape sequences which should be stripped when validating user-specified values or parameters
    /// </summary>
    private static readonly string[] StripStrs = { "\n", " ", "\'", "\"", "\0", "\a", "\b", "\f", "\r", "\t", "\v" };

    /// <summary>
    /// Strip the return type from a MethodInfo string
    /// </summary>
    /// <param name="s">the string</param>
    /// <returns>a string[] with the method name as index 1</returns>
    public static string[] StripReturnType(string s)
    {
        string[] splitStr = s.Split(' '); // TODO: there is no error checking or redundancy here if this fails to split
        return splitStr; 
    }

    /// <summary>
    /// Strip the namespace from a MemberInfo string
    /// </summary>
    /// <param name="s">the string</param>
    /// <returns>a string[] with the memberinfo as last index</returns>
    public static string[] StripNamespace(string s)
    {
        string[] splitStr = s.Split('.'); // TODO: there is no error checking or redundancy here if this fails to split
        return splitStr;                  // some Types also cause problems; e.g. logging an instance of a generic collection where format is System.Collections.Generic.List`1[T]
    }

    /// <summary>
    /// Strip the Method parameters (and parentheses) from the string representation of MethodInfos
    /// </summary>
    /// <param name="s">the string</param>
    /// <returns>a string[] with the method name as index 0</returns>
    public static string[] StripMethodParameters(string s)
    {
        string[] splitStr = s.Split('(');
        return splitStr;
    }

    /// <summary>
    /// Get the short (non-qualified) assembly name by stripping other Assembly-qualified name information (discarding, i.e., Version=x.x.x.x, Culture=x, PublicKeyToken=x)
    /// </summary>
    /// <param name="s">the Assembly information as string</param>
    /// <returns>the non-qualified Assembly name</returns>
    public static string GetAssemblyShortName(string s)
    {
        string[] splitStr = s.Split(',');
        return splitStr[0];
    }

    /// <summary>
    /// Sanitize a user-specified property string, removing line breaks, escaped nulls, etc...
    /// </summary>
    /// <param name="propertyString">the string to sanitize</param>
    /// <returns>the propertyString with invalid characters or sequences removed</returns>
    public static string CleanPropertyString(string propertyString)
    {
        foreach (string str in StripStrs)
        {
            if (propertyString != null)
            {
                if (propertyString.Contains(str))
                {
                    propertyString = propertyString.Replace(str, string.Empty);
                }
            }
        }

        return propertyString;
    }
}