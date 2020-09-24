using System;
using System.Collections.Generic;
using System.Text;

namespace TentaClient.Model.Account
{
	public class UpdateResponse
	{
		public string UserName { get; set; }
		public string Email { get; set; }	
		public DateTime? Updated { get; set; }
	}
}
