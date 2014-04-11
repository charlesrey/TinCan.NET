﻿/*
    Copyright 2014 Rustici Software

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace TinCan
{
    public class RemoteLRS : LRS
    {
        // TODO: add trailing slash
        public Uri endpoint { get; set; }
        public TCAPIVersion version { get; set; }
        // TODO: set auth with username/password
        public String auth { get; set; }
        public Dictionary<String, String> extended { get; set; }

        private HttpWebRequest GetRequest(String method, String resource, Dictionary<String, String> headers, Dictionary<String, String> query_params)
        {
            // TODO: handle full path resources
            // TODO: handle query string params passed in
            var url = endpoint.ToString() + resource;

            var req = (HttpWebRequest) WebRequest.Create(url);
            req.Method = method;
            req.Headers.Add("X-Experience-API-Version", version.ToString());
            if (auth != null)
            {
                req.Headers.Add("Authorization", auth);
            }
            // TODO: handle additional headers passed in

            return req;
        }
        private HttpWebRequest GetRequest(String method, String resource, Dictionary<String, String> headers)
        {
            return this.GetRequest(method, resource, headers, null);
        }
        private HttpWebRequest GetRequest(String method, String resource)
        {
            return this.GetRequest(method, resource, null, null);
        }

        protected void Done(WebResponse resp)
        {
            resp.Close();
        }

        public TinCan.LRSResponse.About About()
        {
            var r = new LRSResponse.About();

            try
            {
                var webReq = this.GetRequest("GET", "about");
                var webResp = (HttpWebResponse)webReq.GetResponse();

                r.httpResponse = webResp;

                if (webResp.StatusCode == HttpStatusCode.OK)
                {
                    r.success = true;

                    var responseStr = new StreamReader(webResp.GetResponseStream()).ReadToEnd();
                    r.content = new About(responseStr);
                }
                this.Done(webResp);
            }
            catch (WebException ex)
            {
                r.success = false;
                r.httpException = ex;
                r.httpResponse = (HttpWebResponse)ex.Response;
            }

            return r;
        }
    }
}
