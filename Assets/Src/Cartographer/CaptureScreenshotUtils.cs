using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections;
using System;
using Core.Environment.Logging.Extension;
using NLog;

namespace Assets.Src.Cartographer
{
    public class CaptureScreenshotUtils
    {
        // logger ---------------------------------------------------------------------------------
        public static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // public methods -------------------------------------------------------------------------
        public static string GetScreenshotFolder()
        {
            var folder = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures), "Colony Shots");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        public static string GetUniqueScreenshotPath(bool useGUID, string extension)
        {
            var targetFolder = GetScreenshotFolder();
            var filePrefix = "Shot";
            if (useGUID)
            {
                var date = DateTime.Now.ToString("dd.MM.yyyy");
                var guid = Guid.NewGuid().ToString("N");
                return Path.Combine(targetFolder, $"{filePrefix}.{date}.{guid}{extension}");
            }
            else
            {
                var files = Directory.EnumerateFiles(targetFolder, $"{filePrefix}_*{extension}", SearchOption.TopDirectoryOnly);
                int count = 0;
                foreach (var file in files) { count++; }
                var uniquePath = string.Empty;
                for (int index = count; index < (count + 100); ++index)
                {
                    uniquePath = Path.Combine(targetFolder, $"{filePrefix}_{index:0000}{extension}");
                    if (!File.Exists(uniquePath))
                    {
                        break;
                    }
                }
                return uniquePath;
            }
        }

        public static string CreateCameraInfo(UnityEngine.Camera renderCamera)
        {
            return $"{renderCamera.transform.position.x} {renderCamera.transform.position.y} {renderCamera.transform.position.z} {renderCamera.transform.rotation.eulerAngles.x} {renderCamera.transform.rotation.eulerAngles.y} {renderCamera.transform.rotation.eulerAngles.z}";
        }

        public static bool SaveScreenshot(string filePath, byte[] bytes, string cameraInfo)
        {
            if (bytes == null) { return false; }
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                var file = File.Open(filePath, FileMode.CreateNew);
                var writer = new BinaryWriter(file);
                if (!string.IsNullOrEmpty(cameraInfo))
                {
                    writer.Write(bytes[0]);
                    writer.Write(bytes[1]);
                    writer.Write((byte)0xFF);
                    writer.Write((byte)0xFE);
                    writer.Write((byte)((short)((cameraInfo.Length + 2) >> 8) & 0xFF));
                    writer.Write((byte)((short)(cameraInfo.Length + 2) & 0xFF));
                    writer.Write(cameraInfo.ToCharArray());
                    writer.Write(bytes, 2, bytes.Length - 2);
                }
                else
                {
                    writer.Write(bytes);
                }
                writer.Close();
                file.Close();
            }
            catch (Exception ex)
            {
                Logger.IfError()?.Message($"Error saving screensot: {filePath}, message: {ex.Message}").Write();
                return false;
            }
            return true;
        }

        public static void MakeGameScreenshot()
        {
            var renderCamera = UnityEngine.Camera.current;
            if (renderCamera == null) { return; }

            var cameraInfo = CreateCameraInfo(renderCamera);

            var texure = ScreenCapture.CaptureScreenshotAsTexture();
            var bytes = texure.EncodeToJPG(100);
            UnityEngine.Object.DestroyImmediate(texure);

            var filePath = GetUniqueScreenshotPath(true, ".jpg" );
            SaveScreenshot(filePath, bytes, cameraInfo);
        }

        public static IEnumerator MakeGameScreenshotCoroutine()
        {
            yield return new WaitForEndOfFrame();
            MakeGameScreenshot();
        }
    }
}