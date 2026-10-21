using System.Net;
using System.Text;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;

using NetCord.Rest;

public class Program
{

    private const int ServerPort = 7777;
    private GatewayClient client;

    public static void Main(string[] args)
    {
        new Program();
    }

    public Program()
    {
        InitializeBot();
        InitializeHTTPServer();

        Console.WriteLine("Server started.");

        while (true)
        {
            Thread.Sleep(20);
        }
    }

    public async void InitializeHTTPServer()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://+:8080/");
        listener.Start();
        while (true)
        {
            var context = await listener.GetContextAsync();
            _ = Task.Run(() => HandleRequest(context));
        }
    }

    public async void InitializeBot()
    {
        client = new(new BotToken("token"), new GatewayClientConfiguration
        {
            Logger = new ConsoleLogger(),
        });

        SendToDiscord("**" + "Initialized" + "**", 1562265378938159125);

        await client.StartAsync();
        await Task.Delay(-1);
    }

    public async void SendToDiscord(string message, ulong channelID)
    {
        MessageProperties p = new MessageProperties();
        p.Content = message;
        await client.Rest.SendMessageAsync(channelID, p);
    }

    public void HandleRequest(HttpListenerContext context)
    {
        try
        {
            Console.WriteLine("Received request");
            string path = context.Request.Url.AbsolutePath;

            if (path == "/error")
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                SendToDiscord("**" + reader.ReadToEnd() + "**", 1562265436396068864);

                context.Response.StatusCode = 200;
                context.Response.Close();
            } else
            if (path == "/score")
            {
                using var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding);
                SendToDiscord("**" + reader.ReadToEnd() + "**", 1562265378938159125);

                context.Response.StatusCode = 200;
                context.Response.Close();
            }
            else
            {
                context.Response.StatusCode = 404;
                context.Response.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

}