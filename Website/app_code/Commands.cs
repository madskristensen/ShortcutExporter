using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Xml;

/// <summary>
/// Summary description for Commands
/// </summary>
public class Commands
{
    public static int ShortcutCount = 0;

    public static Dictionary<string, List<Binding>> GenerateList()
    {
        Dictionary<string, List<Binding>> dic = new Dictionary<string, List<Binding>>();
        ShortcutCount = 0;

        XmlDocument doc = new XmlDocument();
        doc.Load(HostingEnvironment.MapPath("~/app_data/commands.xml"));
        string currentPrefix = string.Empty;
        Binding lastBinding = new Binding();

        foreach (XmlNode node in doc.SelectNodes("//command"))
        {
            string name = node.Attributes["name"].InnerText;
            string shortcut = node.Attributes["shortcut"].InnerText;
            int index = name.IndexOf('.');
            string prefix = index > 0 ? CleanName(name.Substring(0, index))  : "Misc";
            string displayName = index > 0 ? name.Substring(name.LastIndexOf('.') + 1) : name;

            if (currentPrefix != prefix)
            {
                dic[prefix] = new List<Binding>();
                currentPrefix = prefix;
            }

            if (lastBinding.FullName == name)
            {
                lastBinding.Shortcuts.Add(shortcut);
            }
            else
            {
                Binding binding = new Binding(name, CleanName(displayName), shortcut);

                dic[prefix].Add(binding);
                lastBinding = binding;
            }
            
            ShortcutCount += 1;
        }
        
        return dic;
    }

    public static string CleanName(string name)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(name[0]);

        for (int i = 1; i < name.Length; i++)
        {
            char c = name[i];

            if (char.IsUpper(c) && !char.IsUpper(name[i - 1]))
            {
                sb.Append(" ");
            }

            sb.Append(c);
        }

        return sb.ToString();
    }
}

public class Binding
{
    public Binding(string name, string displayName, string shortcut)
    {
        FullName = name;
        DisplayName = displayName;
        Shortcuts.Add(shortcut);
    }

    public Binding()
    {
        FullName = string.Empty;
    }

    public string FullName;
    public string DisplayName;
    public List<string> Shortcuts = new List<string>();
}