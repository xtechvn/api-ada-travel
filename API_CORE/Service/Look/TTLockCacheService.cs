using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Caching.RedisWorker;

using Entities.ViewModels.Lock;
using Utilities.Contants;

public class TTLockCacheService
{
    private readonly IConfiguration _configuration;
    private readonly RedisConn _redis;
    private const int DB_INDEX = 0;

    public TTLockCacheService(IConfiguration configuration, RedisConn redis)
    {
        _configuration = configuration;
        _redis = redis;
    }

    public async Task<TTLockSimpleResponse> ChangeAdminKeyboardPwdAsync(long lockId, string password, int changeType = 2)
    {
        using var httpClient = new HttpClient();

        // ✅ date = unix ms (TTLock thường yêu cầu ms)
        var dateMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

        var clientId = _configuration["DataBaseConfig:TTLock:ClientId"];
        var accessToken = _configuration["DataBaseConfig:TTLock:AccessToken"];

        var form = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string,string>("clientId", clientId),
            new KeyValuePair<string,string>("accessToken", accessToken),
            new KeyValuePair<string,string>("lockId", lockId.ToString()),
            new KeyValuePair<string,string>("password", password),
            new KeyValuePair<string,string>("changeType", changeType.ToString()),
            new KeyValuePair<string,string>("date", dateMs),
        });

        var url = "https://euapi.ttlock.com/v3/lock/changeAdminKeyboardPwd";
        var res = await httpClient.PostAsync(url, form);

        var body = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode)
        {
            return new TTLockSimpleResponse
            {
                errcode = -1,
                errmsg = $"HTTP {(int)res.StatusCode}",
                description = body
            };
        }

        return JsonConvert.DeserializeObject<TTLockSimpleResponse>(body) ?? new TTLockSimpleResponse
        {
            errcode = -1,
            errmsg = "Deserialize failed",
            description = body
        };
    }

    
}
