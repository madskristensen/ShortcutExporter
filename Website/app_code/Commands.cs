using System.Collections.Generic;
using System.Text;
using System.Web.Hosting;
using System.Xml.Linq;

public class Commands
{
    public static int ShortcutCount = 0;

    public static Dictionary<string, List<Binding>> GenerateList()
    {
        ShortcutCount = 0;

        Dictionary<string, List<Binding>> dic = new Dictionary<string, List<Binding>>();
        XDocument doc = XDocument.Load(HostingEnvironment.MapPath("~/app_data/commands.xml"));
        Binding prev = new Binding();

        foreach (XElement node in doc.Descendants("command"))
        {
            string name = node.Attribute("name").Value;
            string shortcut = node.Attribute("shortcut").Value;

            Binding binding = prev.FullName == name ? prev : CreateBinding(dic, name, shortcut);

            if (!binding.Shortcuts.Contains(shortcut))
                prev.Shortcuts.Add(shortcut);

            prev = binding;
            ShortcutCount += 1;
        }

        return dic;
    }

    private static Binding CreateBinding(Dictionary<string, List<Binding>> dic, string name, string shortcut)
    {
        int index = name.IndexOf('.');
        string displayName = index > 0 ? name.Substring(name.LastIndexOf('.') + 1) : name;
        string prefix = index > 0 ? CleanName(name.Substring(0, index)) : "Misc";
        Binding binding = new Binding(name, CleanName(displayName), shortcut);

        if (!dic.ContainsKey(prefix))
            dic[prefix] = new List<Binding>();

        dic[prefix].Add(binding);

        return binding;
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