using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayGuiCreator
{
	public class TextBoxEntry
	{
		public sbyte[] bytes;
		private int characterAmount;
		public bool IsActive { get; set; }
		public TextBoxEntry(int characterAmount)
		{
			this.characterAmount = characterAmount;
			bytes = new sbyte[characterAmount];
		}

		public int GetLength()
		{
			return characterAmount;
		}


		public string ToString()
		{
			StringBuilder builder = new StringBuilder(bytes.Length);
			ASCIIEncoding ascii = new ASCIIEncoding();
			byte[] byteArray = new byte[characterAmount];
			for (int i = 0; i < characterAmount; i++)
			{
				byteArray[i] = Convert.ToByte(bytes[i]);
			}
			return ascii.GetString(byteArray);
		}
	}
}
