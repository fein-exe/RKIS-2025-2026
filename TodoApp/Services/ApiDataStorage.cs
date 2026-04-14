using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Services
{
    public class ApiDataStorage : IDataStorage
    {
        private readonly string _baseUrl;
        private readonly byte[] _aesKey;
        private readonly byte[] _aesIV;
        private readonly HttpClient _httpClient;

        public ApiDataStorage(string baseUrl, byte[] aesKey, byte[] aesIV)
        {
            _baseUrl = baseUrl;
            _aesKey = aesKey;
            _aesIV = aesIV;
            _httpClient = new HttpClient();
        }

        private byte[] EncryptData(string data)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIV;
                
                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(data);
                    writer.Flush();
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }

        private string DecryptData(byte[] encryptedData)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _aesKey;
                aes.IV = _aesIV;
                
                using (var memoryStream = new MemoryStream(encryptedData))
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var reader = new StreamReader(cryptoStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private async Task<bool> IsServerAvailable()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/profiles");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void SaveProfiles(IEnumerable<Profile> profiles)
        {
            var task = SaveProfilesAsync(profiles);
            task.Wait();
        }

        private async Task SaveProfilesAsync(IEnumerable<Profile> profiles)
        {
            if (!await IsServerAvailable())
            {
                throw new Exception("Сервер недоступен");
            }

            var json = JsonSerializer.Serialize(profiles);
            var encryptedData = EncryptData(json);
            
            var content = new ByteArrayContent(encryptedData);
            var response = await _httpClient.PostAsync($"{_baseUrl}/profiles", content);
            response.EnsureSuccessStatusCode();
        }

        public IEnumerable<Profile> LoadProfiles()
        {
            var task = LoadProfilesAsync();
            task.Wait();
            return task.Result;
        }

        private async Task<IEnumerable<Profile>> LoadProfilesAsync()
        {
            if (!await IsServerAvailable())
            {
                throw new Exception("Сервер недоступен");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/profiles");
            response.EnsureSuccessStatusCode();
            
            var encryptedData = await response.Content.ReadAsByteArrayAsync();
            if (encryptedData.Length == 0)
            {
                return new List<Profile>();
            }
            
            var json = DecryptData(encryptedData);
            return JsonSerializer.Deserialize<List<Profile>>(json) ?? new List<Profile>();
        }

        public void SaveTodos(Guid userId, IEnumerable<TodoItem> todos)
        {
            var task = SaveTodosAsync(userId, todos);
            task.Wait();
        }

        private async Task SaveTodosAsync(Guid userId, IEnumerable<TodoItem> todos)
        {
            if (!await IsServerAvailable())
            {
                throw new Exception("Сервер недоступен");
            }

            var json = JsonSerializer.Serialize(todos);
            var encryptedData = EncryptData(json);
            
            var content = new ByteArrayContent(encryptedData);
            var response = await _httpClient.PostAsync($"{_baseUrl}/todos/{userId}", content);
            response.EnsureSuccessStatusCode();
        }

        public IEnumerable<TodoItem> LoadTodos(Guid userId)
        {
            var task = LoadTodosAsync(userId);
            task.Wait();
            return task.Result;
        }

        private async Task<IEnumerable<TodoItem>> LoadTodosAsync(Guid userId)
        {
            if (!await IsServerAvailable())
            {
                throw new Exception("Сервер недоступен");
            }

            var response = await _httpClient.GetAsync($"{_baseUrl}/todos/{userId}");
            response.EnsureSuccessStatusCode();
            
            var encryptedData = await response.Content.ReadAsByteArrayAsync();
            if (encryptedData.Length == 0)
            {
                return new List<TodoItem>();
            }
            
            var json = DecryptData(encryptedData);
            return JsonSerializer.Deserialize<List<TodoItem>>(json) ?? new List<TodoItem>();
        }
    }
}