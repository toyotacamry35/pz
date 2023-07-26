using System;
using System.Text;

public class DumpLine
{
    public const char IndentChar = ' ';
    public const int IndentCharRepeat = 2;

    public string Key;
    public string Val;
    public string ValVerboseInfo;
    public int Indent;


    //=== Ctors ===========================================================

    public DumpLine(int indent, string key, string valVerboseInfo, string val)
    {
        Indent = indent;
        Key = key;
        ValVerboseInfo = valVerboseInfo;
        Val = val;
    }

    public DumpLine(int indent, string key, string val) :
        this(indent, key, null, val)
    {
    }


    //=== Public ==========================================================

    public string ToDump(bool isVerbose)
    {
        var sb = new StringBuilder();
        if (Indent > 0)
        {
            sb.Append(new String(IndentChar, IndentCharRepeat * Indent));
        }

        if (Key != null)
        {
            sb.Append(Key + " = ");
        }

        if (isVerbose && !string.IsNullOrEmpty(ValVerboseInfo))
        {
            sb.Append(ValVerboseInfo + " ");
        }

        sb.Append(Val);

        return sb.ToString();
    }
}
