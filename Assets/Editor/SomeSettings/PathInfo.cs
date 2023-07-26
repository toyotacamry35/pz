using System.IO;
using UnityEngine;

/// <summary>
/// По переданному полному/относительному пути достраивает остальные параметры объекта: полный/относительный пути, имя файла, расширение,
/// а также определяет существование объекта
/// </summary>
public class PathInfo
{
    public const string RelativePathStartPart = "Assets";

    /// <summary>
    /// Если можно показать RelativePath - возвращает его, иначе - FullPath. Удобно для отображения пути в GUI
    /// </summary>
    public string DisplayablePath
    {
        get { return string.IsNullOrEmpty(RelativePath) ? FullPath : RelativePath; }
    }

    /// <summary>
    /// Расширение файла/папки, начинается с точки
    /// </summary>
    public string Extension { get; private set; }

    /// <summary>
    /// Имя файла/папки без расширения
    /// </summary>
    public string Filename { get; private set; }

    /// <summary>
    /// Имя файла/папки с расширением
    /// </summary>
    public string FilenameWithExtension { get; private set; }

    /// <summary>
    /// Полный путь
    /// </summary>
    public string FullPath { get; private set; }

    /// <summary>
    /// Существует ли?
    /// </summary>
    public bool IsExists { get; private set; }

    /// <summary>
    /// Файл или папка
    /// </summary>
    public bool IsFile { get; private set; }

    public string Guid { get; private set; }

    /// <summary>
    /// Путь относительно корня проекта - папки, которая на уровень выше Assets. ВНИМАНИЕ! НЕ относительно Application.dataPath.
    /// Может отсутствовать, если выбран объект вне проекта
    /// </summary>
    public string RelativePath { get; private set; }

    /// <summary>
    /// Путь вида "c:/.../ThisProjectFolder" до "Assets", не включая "Assets"
    /// </summary>
    private static string _rootFullPath;


    //=== Ctor ================================================================

    public PathInfo(string path, bool isFile = false, bool isRelativePath = false, string guid = null)
    {
        IsFile = isFile;
        Guid = guid;
        if (_rootFullPath == null)
        {
            _rootFullPath = Application.dataPath.Replace("\\", "/");
            _rootFullPath = _rootFullPath.Substring(0, _rootFullPath.Length - "Assets/".Length);
        }
        ChangePath(isRelativePath ? _rootFullPath + "/" + path : path);
    }


    //=== Public ==============================================================

    public void ChangePath(string newFullPath)
    {
        FullPath = newFullPath.Contains("\\")
            ? newFullPath.Replace("\\", "/")
            : newFullPath;

        FullPath = FullPath.TrimEnd('/');
        //--- Filename, Extension
        FilenameWithExtension = Filename = Extension = "";
        var lastCharPos = FullPath.LastIndexOf('/');
        if (lastCharPos > -1)
        {
            //То, что не последняя, мы уже знаем из-за TrimEnd
            FilenameWithExtension = FullPath.Substring(lastCharPos + 1);
            lastCharPos = FilenameWithExtension.LastIndexOf('.');
            if (lastCharPos > -1)
            {
                Filename = lastCharPos > 0 ? FilenameWithExtension.Substring(0, lastCharPos) : "";
                Extension = lastCharPos < FilenameWithExtension.Length - 1 ? FilenameWithExtension.Substring(lastCharPos) : "";
            }
            else
            {
                Filename = FilenameWithExtension;
            }
        }

        //--- RelativePath
        if (FullPath.Contains(_rootFullPath))
        {
            RelativePath = FullPath.Replace(_rootFullPath, "").TrimStart('/');
        }
        else
        {
            RelativePath = null;
        }

        IsExists = IsFile ? File.Exists(FullPath) : Directory.Exists(FullPath);
    }

    public override string ToString()
    {
        return string.Format("PathInfo[ IsFile{0} IsExists{1} full='{2}', displ.='{3}']",
            IsFile.AsSign(), IsExists.AsSign(), FullPath, DisplayablePath);
    }
}
