using System;
using System.Collections.Generic;
using System.Text;

namespace TentaClient.Model.Entities
{
	public class Account
	{
		public int ID { get; set; }
		public string UserName { get; set; }
		public string JwtToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
