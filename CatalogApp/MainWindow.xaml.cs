using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using SimpleHttp;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections.Generic;
using Catalog.DigiKey;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Catalog.DigiKey.Responses;
using Newtonsoft.Json;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Input;

namespace Catalog
{
    public class MainWindow : Window
    {


        // https://catalog.muny.us/barcode-scan/oauth/return?code=hTffmSiu4o7IakoTJ-VLIkCGA-AFRy9bV1aT9AAD

        private MainWindowModel Model = new MainWindowModel();

        private DigiClient DKClient = new DigiClient()
        {
            ClientID = "31801af8-5c22-4b34-b2f6-cc3a38275b58",
            ClientSecret = "D1iB6qN0iV4lA2bT4vS1rP5mS2yT0jM0xW0mQ2nT7cO0jA0iL0",
            AccessToken = "ss6HfE6E1E4uZuaqJkYm0driCYml",
            RefreshToken = "Am9xaGy44VQLlLQECYmf0cFaRKYFgeW8ZgmgOmaGYq",
            RedirectUri = "https://catalog.muny.us/barcode-scan/oauth/return"
        };

        public MainWindow()
        {
            InitializeComponent();

            Console.WriteLine(Convert.ToByte("FF", 16));


            this.Get<NumericUpDown>("HowManyBox").KeyDown += HowMany_KeyDown;

            this.Get<Button>("HowManyButton").Click += HowMany_Click;

            DataContext = Model;

            Route.Add("/barcode-scan/add-item", (HttpAction)HandleBarcodeScanAddItem, "POST");

            Route.Add("/barcode-scan/oauth/return{unused}", (HttpAction)HandleOAuthCallback);

            int port = 8074;
            Console.WriteLine($"HTTP server listening on port {port}");

            CancellationTokenSource cts = new CancellationTokenSource();

            Task ts = HttpServer.ListenAsync(port, cts.Token, Route.OnHttpRequestAsync, useHttps: false);

            Console.WriteLine("Visit https://sso.digikey.com/as/authorization.oauth2?response_type=code&client_id=31801af8-5c22-4b34-b2f6-cc3a38275b58&redirect_uri=https://catalog.muny.us/barcode-scan/oauth/return to get OAuth tokens");
        }

        private async void HandleBarcodeScanAddItem(HttpListenerRequest req, HttpListenerResponse res, Dictionary<string, string> args)
        {
            var p = req.ParseBody(args);
            res.AsText("Thanks!");

            string barcodeData = args["data"];

            /*FileStream fs = new FileStream("test.jpeg", FileMode.OpenOrCreate);
            fs.Write(new byte[] { 0xFF, 0xD8 });
            p["barcode_bmp"].Value.CopyTo(fs);
            fs.Close();*/

            var ms = new MemoryStream();
            ms.Write(new byte[] { 0xFF, 0xD8 });
            p["barcode_bmp"].Value.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            Model.BarcodeBitmap = new Bitmap(ms);
            ms.Close();

            barcodeData = barcodeData.Replace('\u001e', '\u241E').Replace('\u001d', '\u241D');

            Model.BarcodeData = await DKClient.GetBarcodeData(barcodeData);

            var partDetails = await DKClient.GetPartDetails(Model.BarcodeData.DigiKeyPartNumber);

            
        }

        private async void HandleOAuthCallback(HttpListenerRequest req, HttpListenerResponse res, Dictionary<string, string> args)
        {
            string code = req.QueryString["code"];

            var tokensResp = await DKClient.GetTokens(code);

            string tokensJson = JsonConvert.SerializeObject(tokensResp);

            Console.WriteLine(tokensJson);

            res.AsText($"Your tokens: {tokensJson}");
        }

        private void HowMany_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == InputModifiers.None && e.Key == Key.Enter)
            {
                HowMany_Click(null, null);
            }
        }

        private void HowMany_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Click");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }

    public class MainWindowModel : ReactiveObject
    {
        private Bitmap _barcodeBitmap;
        private BarcodeResponse _barcodeData;

        public Bitmap BarcodeBitmap
        {
            get { return _barcodeBitmap; }
            set { this.RaiseAndSetIfChanged(ref _barcodeBitmap, value); }
        }

        public BarcodeResponse BarcodeData
        {
            get { return _barcodeData; }
            set { this.RaiseAndSetIfChanged(ref _barcodeData, value); }
        }

    }
}