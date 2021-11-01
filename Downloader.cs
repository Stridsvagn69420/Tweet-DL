using System;
using System.Net.Http;
using System.Collections.Generic;

namespace Tweet_DL
{
	public class Downloader
	{
		private HttpClient _client;
		public Downloader(HttpClient client)
		{
			_client = client;
		}
		public void DownloadURLs(List<string> URLs)
        {

        }
	}
}
