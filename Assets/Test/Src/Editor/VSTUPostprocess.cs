#if ENABLE_VSTU
using System.Text;
using UnityEditor;

using SyntaxTree.VisualStudio.Unity.Bridge;
using System.Xml;

[InitializeOnLoad]
public class ProjectFileHook
{
    private static readonly string MSBuildNamespaceUri = "http://schemas.microsoft.com/developer/msbuild/2003";

    static ProjectFileHook()
    {
        ProjectFilesGenerator.ProjectFileGeneration += Generate;
    }

    private static string Generate(string name, string content)
    {
        var doc = new XmlDocument();
        doc.LoadXml(content);
        var manager = new XmlNamespaceManager(doc.NameTable);
        manager.AddNamespace("msb", MSBuildNamespaceUri);

        var rootNode = doc.SelectSingleNode("/msb:Project", manager);

        var grp = doc.CreateElement("ItemGroup", MSBuildNamespaceUri);
        rootNode.AppendChild(grp);

        var newNode = doc.CreateElement("Reference", MSBuildNamespaceUri);
        newNode.SetAttribute("Include", "System.Net.Http");
        grp.AppendChild(newNode);

        return Beautify(doc);
    }

    private static string Beautify(XmlDocument doc)
    {
        StringBuilder sb = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\r\n",
            NewLineHandling = NewLineHandling.Replace
        };
        using (XmlWriter writer = XmlWriter.Create(sb, settings))
        {
            doc.Save(writer);
        }
        return sb.ToString();
    }
}
#endif
