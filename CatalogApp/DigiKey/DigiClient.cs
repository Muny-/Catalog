using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Catalog.DigiKey.Responses;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;

namespace Catalog.DigiKey
{
    public class DigiClient
    {
        public string ClientID;
        public string ClientSecret;

        public string AccessToken;
        public string RefreshToken;

        public string RedirectUri;

        private static readonly HttpClient client = new HttpClient();

        const string OAUTH_CODE_REDEEM = "https://sso.digikey.com/as/token.oauth2";

        const string API_BASE = "https://api.digikey.com/services/";

        const string API_BARCODE_BASE = API_BASE + "barcode/v1/";
        const string API_BARCODE_PRODUCTQRCODE = API_BARCODE_BASE + "productqrcode";

        const string API_PARTSEARCH_BASE = API_BASE + "partsearch/v2/";
        const string API_PARTSEARCH_PARTDETAILS = API_PARTSEARCH_BASE + "partdetails";

        static SolidColorBrush green;
        static SolidColorBrush red;

        public DigiClient()
        {
            // interesting that I need this...
            Dispatcher.UIThread.Post(() =>
            {
                green = new SolidColorBrush(Color.FromRgb(54, 201, 198));
                red = new SolidColorBrush(Color.FromRgb(224, 66, 81));
            });
        }


        private void AddAuthHeaders()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("authorization", AccessToken);
            client.DefaultRequestHeaders.Add("x-ibm-client-id", ClientID);
            client.DefaultRequestHeaders.Add("x-digikey-locale-shiptocountry", "us");
        }

        public async Task<TokensResponse> GetTokens(string code)
        {
            var tokensResp = (await (await client.PostAsync(OAUTH_CODE_REDEEM, Utils.FormUrlEncodeData(new
            {
                code,
                client_id = ClientID,
                client_secret = ClientSecret,
                redirect_uri = RedirectUri,
                grant_type = "authorization_code"
            }))).Content.ReadAsStringAsync());

            return JsonConvert.DeserializeObject<TokensResponse>(tokensResp);
        }

        public async Task<BarcodeResponse> GetBarcodeData(string data)
        {
            AddAuthHeaders();

            var bcResp = (await (await client.PostAsync(API_BARCODE_PRODUCTQRCODE, Utils.JsonEncodeData(new
            {
                QRCode = data
            }))).Content.ReadAsStringAsync());

            var resp = JsonConvert.DeserializeObject<BarcodeResponse>(bcResp);

            if (resp.Status == "Success")
                resp.StatusColor = green;
            else
                resp.StatusColor = red;

            return resp;
        }

        public async Task<PartDetailsResponse> GetPartDetails(string partNo)
        {
            var partResp = (await (await client.PostAsync(API_PARTSEARCH_PARTDETAILS, Utils.JsonEncodeData(new
            {
                Part = partNo
            }))).Content.ReadAsStringAsync());

            Console.WriteLine(partResp);

            //return JsonConvert.DeserializeObject<PartDetailsResponse>(partResp);
            return new PartDetailsResponse();
        }
    }

}