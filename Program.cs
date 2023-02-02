using System.Diagnostics;


var process = Process.GetCurrentProcess();
var stopwatch = Stopwatch.StartNew();


var txt = GetNonAsync();
stopwatch.Stop();
Console.WriteLine($"Non-async time: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Non-async memory: {process.PrivateMemorySize64 / 1024 / 1024} MB");

// Async request
stopwatch = Stopwatch.StartNew();
var txt2 = await GetAsync();
stopwatch.Stop();
Console.WriteLine($"Async time: {stopwatch.ElapsedMilliseconds} ms");
Console.WriteLine($"Async memory: {process.PrivateMemorySize64 / 1024 / 1024} MB");

// Console.ReadLine();


async Task<string> GetAsync()
{
    using (var client = new HttpClient())
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync("https://jsonplaceholder.typicode.com/posts");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return (responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", e.Message);
            return ("Error");
        }
    }
}

string GetNonAsync()
{
    using (var client = new HttpClient())
    {
        var responseTask = client.GetAsync("https://jsonplaceholder.typicode.com/posts");
        responseTask.Wait();

        var response = responseTask.Result;

        if (response.IsSuccessStatusCode)
        {
            var responseBodyTask = response.Content.ReadAsStringAsync();
            responseBodyTask.Wait();
            // Console.WriteLine(responseBodyTask.Result);
            return (responseBodyTask.Result);
        }
        else
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine("Message :{0} ", response.ReasonPhrase);
            return ("error");
        }
    }
}
