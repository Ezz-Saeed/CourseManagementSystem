using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

public class FakeResponseCookies : IResponseCookies
{
    public Dictionary<string, (string Value, CookieOptions Options)> Cookies { get; } = new();

    public void Append(string key, string value, CookieOptions options)
    {
        Cookies[key] = (value, options);
    }

    public void Append(string key, string value)
    {
        Cookies[key] = (value, new CookieOptions());
    }

    public void Delete(string key, CookieOptions options)
    {
        Cookies.Remove(key);
    }

    public void Delete(string key)
    {
        Cookies.Remove(key);
    }
}
