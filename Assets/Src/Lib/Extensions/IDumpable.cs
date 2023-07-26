using System.Collections.Generic;

public interface IDumpable
{
    /// <summary>
    /// В dumpLines должны быть добавлены DumpLine с описанием объекта
    /// </summary>
    /// <param name="dumpLines">Ненулевой список, куда должны быть добавлены сведения об объекте</param>
    /// <param name="key">Заголовок</param>
    /// <param name="depth">Отступ</param>
    /// <param name="isVerbose">Детальность описания</param>
    void ToDumpLines(List<DumpLine> dumpLines, string key, int depth, bool isVerbose);
}
