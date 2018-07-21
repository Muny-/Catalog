using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using SimpleHttp;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;
using System.Net.Http;
using Catalog.DigiKey;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

namespace Catalog
{
    public class MainWindow : Window
    {


        // https://catalog.muny.us/barcode-scan/oauth/return?code=hTffmSiu4o7IakoTJ-VLIkCGA-AFRy9bV1aT9AAD

        private DigiClient DKClient = new DigiClient()
        {
            ClientID = "31801af8-5c22-4b34-b2f6-cc3a38275b58",
            ClientSecret = "F7dI6bP6yQ5jS0nF0fT7oW2dN4bF1xA5oP1bT2xE3dN7fJ5qM0",
            AccessToken = "wl3CWdZlt0agFI3HLHxcnl6PTouD",
            RefreshToken = "c02VcPPoGVNZVyBzx1W2mYkHCfeA31MstcZsBStscA",
            RedirectUri = "https://catalog.muny.us/barcode-scan/oauth/return"
        };

        public MainWindow()
        {
            InitializeComponent();

            Route.Add("/barcode-scan/add-item", (HttpAction)HandleBarcodeScanAddItem, "POST");

            Route.Add("/barcode-scan/oauth/return{retarded}", (HttpAction)HandleOAuthCallback);

            int port = 8074;
            Console.WriteLine($"HTTP server listening on port {port}");

            CancellationTokenSource cts = new CancellationTokenSource();

            Task ts = HttpServer.ListenAsync(port, cts.Token, Route.OnHttpRequestAsync, useHttps: false);

            Console.WriteLine("Visit https://sso.digikey.com/as/authorization.oauth2?response_type=code&client_id=31801af8-5c22-4b34-b2f6-cc3a38275b58&redirect_uri=https://catalog.muny.us/barcode-scan/oauth/return to get OAuth tokens");
        }

        private async void HandleBarcodeScanAddItem(HttpListenerRequest req, HttpListenerResponse res, Dictionary<string, string> args)
        {
            var p = req.ParseBody(args);
            string barcodeData = args["data"];

            FileStream fs = new FileStream("test.jpeg", FileMode.OpenOrCreate);
            fs.Write(new byte[] { 0xFF, 0xD8 });
            p["barcode_bmp"].Value.CopyTo(fs);
            fs.Close();


            using (var ms = new MemoryStream())
            {
                ms.Write(new byte[] { 0xFF, 0xD8 });
                p["barcode_bmp"].Value.CopyTo(ms);
                ms.Seek(0, SeekOrigin.Begin);

                Dispatcher.UIThread.Post(() =>
                {
                    this.Get<Image>("barcodeImage").Source = new Bitmap(ms);
                });
            }

            //var barcodeResp = await DKClient.GetBarcodeData(barcodeData);

            res.AsText("Thanks!");
        }

        private async void HandleOAuthCallback(HttpListenerRequest req, HttpListenerResponse res, Dictionary<string, string> args)
        {
            string code = req.QueryString["code"];

            var tokensResp = await DKClient.GetTokens(code);

            Console.WriteLine(tokensResp);

            res.AsText($"Your tokens: {tokensResp}");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Hello");
        }
    }
}