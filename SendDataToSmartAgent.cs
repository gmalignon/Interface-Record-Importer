using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceRecordImporter
{
    public class SendDataToSmartAgent
    {
        private static HttpClient SmartAgentClient;
        private static string APIKey = "a6bae5be-6610-47ad-84bd-d598defea3db";

        public SendDataToSmartAgent()
        {
            SmartAgentClient = new HttpClient();
            SmartAgentClient.BaseAddress = new Uri(GlobalSettings.MyAppSettings.ServerIP);
            SmartAgentClient.DefaultRequestHeaders.Accept.Clear();
            SmartAgentClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
           // SmartAgentClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            
            SmartAgentClient.DefaultRequestHeaders.Add("APIKey", APIKey);
            
        }

        public void Dispose()
        {
            SmartAgentClient.Dispose();
        }

        public  async Task<String> SendFileToSmartAgent(string Data, string FileName)
        {
            // String Query = "api/Interface/PostStoreInterfaceRecord?InterfaceRecord=" + System.Web.HttpUtility.UrlEncode(StringCipher.Encrypt(Data, new NetworkCredential("", GlobalSettings.PassPhrase).Password))  + "&FileName=" + FileName;
            String Query = "api/Interface/PostStoreInterfaceRecord?FileName=" + FileName;
            String MyResponse = null;
            //SmartAgentClient;
            InterfaceRecord MyInterfaceRecord = new InterfaceRecord();
            MyInterfaceRecord.FileName = FileName;
            //MyInterfaceRecord.Record = StringCipher.Encrypt(Data, new NetworkCredential("", GlobalSettings.PassPhrase).Password);
            MyInterfaceRecord.Record = Data;
            string json =  JsonConvert.SerializeObject(MyInterfaceRecord);
            HttpContent httpcontent = new StringContent(json, Encoding.UTF8, "application/json");
            
            //SmartAgentClient.
            HttpResponseMessage response = await SmartAgentClient.PostAsync(Query, httpcontent).ConfigureAwait(false); 
            if (response.IsSuccessStatusCode)
            {
                MyResponse = "OK";
            }
            else
            {  MyResponse = "ERROR"; }
            httpcontent.Dispose();
            return MyResponse;
         
        }
        //ByVal ErrorNumber As Integer,
        //ByVal ErrorDescription As String,
        //ByVal ErrorSource As String,
        //                  ByVal ProgramCode As String,
        //                  ByVal ProgramMode As String,
        //                  ByVal FunctionName As String)
        // Program Code 2 characters, Program Mode 2 characters
        public async Task<String> SendErrorToSmartAgent(int ErrorNumber, 
                                                        string ErrorDescription, 
                                                        string ErrorSource,
                                                        string ProgramCode,
                                                        string ProgramMode,
                                                        string FunctionName)
        {
            String Query = "api/Interface/GetStoreError?ErrorNumber=" + ErrorNumber +"&ErrorDescription="+ErrorDescription+"&ErrorSource="+ErrorSource+"&ProgramCode="+ProgramCode+"&ProgramMode="+ProgramMode+"&FunctionName="+FunctionName;
            String MyResponse = null;
            HttpResponseMessage response = await SmartAgentClient.GetAsync(Query).ConfigureAwait(continueOnCapturedContext: false);
            if (response.IsSuccessStatusCode)
            {   //MyResponse = await response.Content.ReadAsAsync<string>();
                MyResponse = "OK";
            }
            else
            { MyResponse = "ERROR"; }
            return MyResponse;
        }


        static async Task<String> GetStoreInterfaceRecordAsync(string path)
        {
            String MyResponse = null;
            //HttpResponseMessage response = await SmartAgentClient.GetAsync(path);
            //if (response.IsSuccessStatusCode)
           // {
                //MyResponse = await response.Content.ReadAsAsync<string>();
               // MyResponse = "OK";
            //}
            return MyResponse;
        }

        public class InterfaceRecord
        {
            public string FileName { get; set; }

            public string Record { get; set; }

        }

    }
}
