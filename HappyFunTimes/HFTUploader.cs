/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using HappyFunTimes;
using UnityEngine;
using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace HappyFunTimes
{
    public class HFTUploader {
        public HFTUploader(bool debug)
        {
            m_debug = debug;
        }

        private void Log(string msg)
        {
            if (m_debug)
            {
                Debug.Log(msg);
            }
        }

        public void UploadTree(string url, string gameId, string baseFolder)
        {
            UploadTree(url, baseFolder, "", gameId);
        }

        private string Combine(string parent, string child)
        {
            return System.String.IsNullOrEmpty(parent) ? child : Path.Combine(parent, child);
        }

        private string CombineHFT(string parent, string child)
        {
            if (System.String.IsNullOrEmpty(parent))
            {
                return child;
            }
            else
            {
                return parent + "/" + child;
            }
        }

        public bool UploadTree(string url, string path, string hftPath, string gameId)
        {
            bool success = true;
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                Match m = s_ignoreExtensionsRE.Match(file);
                if (m.Success)
                {
                    continue;
                }

                if (!Upload(url, Combine(path, file), CombineHFT(hftPath, file), gameId))
                {
                    Debug.LogError("could not upload: " + file);
                    success = false;
                }
            }
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                if (!UploadTree(url, Path.Combine(path, directory), CombineHFT(hftPath, directory), gameId))
                {
                    success = false;
                }
            }
            return success;
        }

        public bool Upload(string url, string filePath, string hftPath, string gameId)
        {
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("gameId", gameId);
            return Upload(url, filePath, hftPath, "file", "application/bin", nvc);
        }

        public bool Upload(string url, string filePath, string hftPath, string paramName, string contentType, NameValueCollection nvc)
        {
            bool result = false;
            if (m_debug)
            {
                Log(string.Format("Uploading {0} as {1} to {2}", filePath, hftPath, url));
            }
            string boundary = "---------------------------" + System.DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;
            wr.AllowWriteStreamBuffering = true;

            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, hftPath, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                if (m_debug)
                {
                    Log(string.Format("File uploaded, server response is: {0}", reader2.ReadToEnd()));
                }
                result = true;
            } catch(System.Exception ex) {
                Debug.LogError("Error uploading file" + ex);
                if(wresp != null) {
                    wresp.Close();
                    wresp = null;
                }
            } finally {
                wr = null;
            }

            return result;
        }

        delegate void Logger(string msg);

        public bool m_debug;
        private static Regex s_ignoreExtensionsRE = new Regex(@"\.(meta)");
    }

}  // namespace HappyFunTimes


