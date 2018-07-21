using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Catalog.DigiKey.Responses;
using System.Threading.Tasks;

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


        private void AddAuthHeaders()
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("content-type", "application/json");
            client.DefaultRequestHeaders.Add("authorization", AccessToken);
            client.DefaultRequestHeaders.Add("x-ibm-client-id", ClientID);
            client.DefaultRequestHeaders.Add("x-digikey-locale-shiptocountry", "us");
        }

        public async Task<TokensResponse> GetTokens(string code)
        {
            var tokensResp = (await (await client.PostAsync(OAUTH_CODE_REDEEM, Utils.FormUrlEncodeData(new
            {
                code = code,
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

            return JsonConvert.DeserializeObject<BarcodeResponse>(bcResp);
        }
    }

}