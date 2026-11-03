using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FB_Ultra
{
    internal class AiService
    {
        // Khởi tạo HttpClient tĩnh duy nhất dùng chung toàn ứng dụng (best practice để tránh cạn kiệt socket)
        private static readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMinutes(2)
        };

        // Giới hạn tối đa 2 luồng gọi AI đồng thời để bảo vệ phần cứng
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(2, 2);

        public async Task<string> CallAiRewriteAsync(string apiUrl, string systemPrompt, string userPrompt, string modelName = "qwen2.5:3b")
        {
            await _semaphore.WaitAsync();
            try
            {
                if (string.IsNullOrWhiteSpace(apiUrl))
                {
                    apiUrl = "http://localhost:11434/v1/chat/completions";
                }

                var payload = new
                {
                    model = modelName,
                    messages = new[]
                    {
                        new { role = "system", content = systemPrompt ?? "" },
                        new { role = "user", content = userPrompt ?? "" }
                    },
                    stream = false
                };

                string jsonPayload = JsonConvert.SerializeObject(payload);
                using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Gửi request qua HttpClient tĩnh
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    // Sửa lỗi ngắt dòng trong chuỗi nội suy
                    return $"[Lỗi API]: {(int)response.StatusCode} - {response.ReasonPhrase}";
                }

                string responseString = await response.Content.ReadAsStringAsync();
                JObject jsonResponse = JObject.Parse(responseString);
                string aiReply = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();

                if (string.IsNullOrWhiteSpace(aiReply))
                    return "[Không nhận được nội dung từ AI]";

                aiReply = aiReply.Replace("\r\n", "\n").Replace("\n", Environment.NewLine);
                return aiReply.Trim();
            }
            catch (Exception ex)
            {
                return $"[Lỗi kết nối AI]: {ex.Message}";
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}