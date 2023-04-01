using System.IO;
using UnityEngine;

namespace Base
{
    public static class FileC
    {
        public static void TryCreateDirectroryOfFile(string filePath)
        {
            Debug.Log($"CreateDirectrory {filePath}[folder_path],");
            if (!string.IsNullOrEmpty(filePath))
            {
                var dir_name = filePath; //Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir_name))
                {
                    Debug.Log($"No Exists {dir_name}[dir_name],");
                    Directory.CreateDirectory(dir_name);
                }
                else
                {
                    Debug.Log($"Exists {dir_name}[dir_name],");
                }
            }
        }
    }
}