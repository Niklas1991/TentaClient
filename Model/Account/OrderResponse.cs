using System;
using System.Collections.Generic;
using System.Text;

namespace TentaClient.Model.Account
{
	public class OrderResponse
	{
		public string CustomerId { get; set; }
		public int EmployeeId { get; set; }
		public string ShipCountry { get; set; }
	}
}
