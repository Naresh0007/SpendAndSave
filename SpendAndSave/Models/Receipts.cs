using System;
namespace SpendAndSave.Models
{
	public class Receipts
	{

		public int Id { get; set; }

		public string UserName { get; set; }

		public byte[] ReceiptPicture { get; set; } // Store image as byte array


	}
}

