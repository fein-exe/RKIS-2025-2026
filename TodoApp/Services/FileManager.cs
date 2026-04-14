using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TodoApp.Exceptions;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class FileManager : IDataStorage
    {
        private readonly string _dataDir;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;

        public FileManager(string dataDir, byte[] aesKey, byte[] aesIV)
        {
            _dataDir = dataDir;
            _aesKey = aesKey;
            _aesIV = aesIV;
            EnsureDataDirectory();
        }

        private void EnsureDataDirectory()
        {
            if (!Directory.Exists(_dataDir))
            {
                Directory.CreateDirectory(_dataDir);
            }
        }

        private string GetProfilesFilePath()
        {
            return Path.Combine(_dataDir, "profiles.dat");
        }

        private string GetTodosFilePath(Guid userId)
        {
            return Path.Combine(_dataDir, $"todos_{userId}.dat");
        }

        private void EncryptAndWrite(string filePath, string content)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (var bufferedStream = new BufferedStream(fileStream, 8192))
            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIV;
                
                using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var streamWriter = new StreamWriter(cryptoStream, Encoding.UTF8))
                {
                    streamWriter.Write(content);
                }
            }
        }

        private string DecryptAndRead(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var bufferedStream = new BufferedStream(fileStream, 8192))
                using (var aes = Aes.Create())
                {
                    aes.Key = _aesKey;
                    aes.IV = _aesIV;
                    
                    using (var cryptoStream = new CryptoStream(bufferedStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new InvalidOperationException($"Ошибка расшифровки файла {filePath}: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new InvalidOperationException($"Ошибка доступа к файлу {filePath}: {ex.Message}", ex);
            }
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            try
            {
                var lines = new List<string>();
                foreach (var profile in profiles)
                {
                    string line = $"{profile.Id}|{profile.Login}|{profile.Password}|{profile.FirstName}|{profile.LastName}|{profile.BirthYear}";
                    lines.Add(line);
                }
                
                string content = string.Join("\n", lines);
                string filePath = GetProfilesFilePath();
                EncryptAndWrite(filePath, content);
            }
            catch (Exception ex) when (ex is InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Не удалось сохранить профили: {ex.Message}", ex);
            }
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            var profiles = new List<Profile>();
            string filePath = GetProfilesFilePath();
            
            string content = DecryptAndRead(filePath);
            if (string.IsNullOrEmpty(content))
            {
                return profiles;
            }
            
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 6)
                {
                    try
                    {
                        var profile = new Profile
                        {
                            Id = Guid.Parse(parts[0]),
                            Login = parts[1],
                            Password = parts[2],
                            FirstName = parts[3],
                            LastName = parts[4],
                            BirthYear = int.Parse(parts[5])
                        };
                        profiles.Add(profile);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Ошибка парсинга профиля: {ex.Message}", ex);
                    }
                }
            }
            
            return profiles;
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            try
            {
                var lines = new List<string>();
                foreach (var item in todos)
                {
                    string escapedText = EscapeText(item.Text);
                    string line = $"{escapedText}|{item.Status}|{item.LastUpdate:yyyy-MM-ddTHH:mm:ss}";
                    lines.Add(line);
                }
                
                string content = string.Join("\n", lines);
                string filePath = GetTodosFilePath(userId);
                EncryptAndWrite(filePath, content);
            }
            catch (Exception ex) when (ex is InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Не удалось сохранить задачи: {ex.Message}", ex);
            }
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            var todos = new List<TodoItem>();
            string filePath = GetTodosFilePath(userId);
            
            string content = DecryptAndRead(filePath);
            if (string.IsNullOrEmpty(content))
            {
                return todos;
            }
            
            var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 3)
                {
                    try
                    {
                        string text = UnescapeText(parts[0]);
                        var status = Enum.Parse<TodoStatus>(parts[1]);
                        DateTime lastUpdate = DateTime.Parse(parts[2]);
                        
                        var item = new TodoItem(text)
                        {
                            Status = status,
                            LastUpdate = lastUpdate
                        };
                        todos.Add(item);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Ошибка парсинга задачи: {ex.Message}", ex);
                    }
                }
            }
            
            return todos;
        }

        private string EscapeText(string text)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        }

        private string UnescapeText(string escaped)
        {
            return Encoding.UTF8.GetString(Convert.FromBase64String(escaped));
        }
    }
}