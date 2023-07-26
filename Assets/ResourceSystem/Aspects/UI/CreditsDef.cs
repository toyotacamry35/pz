using System.Collections.Generic;
using Assets.ColonyShared.SharedCode.ResourcesSystem.Base;
using Assets.ColonyShared.SharedCode.Utils;
using Assets.Src.ResourcesSystem.Base;
using EnumerableExtensions;
using GeneratedCode.Custom.Config;
using UnityEngine;

public class CreditsDef : BaseResource
{
    public ResourceRef<CreditsBlock>[] Credits { get; set; }
    public float ShowTimeInSeconds { get; set; }
}

public abstract class CreditsBlock : BaseResource
{
    public string Title { get; set; }
    public FormatMod TitleFormatMod { get; set; }

    public override string ToString()
    {
        return $"{GetType().NiceName()}: {nameof(Title)}='{Title}'{(TitleFormatMod.IsEmpty ? "" : $", {nameof(TitleFormatMod)}: {TitleFormatMod}")}";
    }
}

public class ImageBlock : CreditsBlock
{
    public UnityRef<Sprite> Image { get; set; }
    public Vec2 Size { get; set; }

    public override string ToString()
    {
        return $"({base.ToString()}, {nameof(Size)}={Size.X}x{Size.Y}, {nameof(Image)}={Image})";
    }
}

public class LabelBlock : CreditsBlock
{
    public string[] Names { get; set; }

    public override string ToString()
    {
        return $"({base.ToString()}, {nameof(Names)}: {Names.ItemsToString()})";
    }
}

public class DivisionBlock : CreditsBlock
{
    public CreditsHeadedLine[] HeadedLines { get; set; }
    public CreditsLine[] Lines { get; set; }

    public override string ToString()
    {
        return $"({base.ToString()}, {nameof(HeadedLines)}: '{HeadedLines.ItemsToString()}', {nameof(Lines)}: {Lines.ItemsToString()})";
    }
}

public struct CreditsLine
{
    public string Description { get; set; }
    public string Name { get; set; }
    public FormatMod FormatMod { get; set; }

    public override string ToString()
    {
        return $"({nameof(CreditsLine)}: {nameof(Description)}='{Description}', {nameof(Name)}='{Name}'{(FormatMod.IsEmpty ? "" : $", {FormatMod}")})";
    }
}

public struct CreditsHeadedLine
{
    public string Head { get; set; }
    public string[] Names { get; set; }

    public override string ToString()
    {
        return $"({nameof(CreditsLine)}: {nameof(Head)}='{Head}', {nameof(Names)}='{Names.ItemsToString()}')";
    }
}

public struct FormatMod
{
    public Vec2 VerticalPadding { get; set; }
    public float FontSizeDelta { get; set; }
    public bool HasFrame { get; set; }

    public bool IsEmpty => VerticalPadding.IsZero && !HasFrame && SharedHelpers.Approximately(FontSizeDelta, 0);

    public override string ToString()
    {
        var details = new List<string>();
        if (IsEmpty)
        {
            details.Add("empty");
        }
        else
        {
            if (!VerticalPadding.IsZero)
                details.Add($"{nameof(VerticalPadding)} top={VerticalPadding.X}, bottom={VerticalPadding.Y}");

            if (!HasFrame)
                details.Add("has frame");

            if (!SharedHelpers.Approximately(FontSizeDelta, 0))
                details.Add($"{nameof(FontSizeDelta)}={FontSizeDelta}");
        }

        return $"({details.ItemsToStringSimple()})";
    }
}