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
			// Need to add once since one place
			// is taken up by the terminating '\0'
			this.characterAmount = characterAmount + 1;
			bytes = new sbyte[this.characterAmount];
		}

		public int GetLength()
		{
			return characterAmount;
		}

		override public string ToString()
		{
			ASCIIEncoding ascii = new ASCIIEncoding();
			byte[] byteArray = new byte[characterAmount];
			for (int i = 0; i < characterAmount; i++)
			{
				byteArray[i] = Convert.ToByte(bytes[i]);
			}
			// Remove '\0' characters from the end
			string output = ascii.GetString(byteArray);
			return output.Trim('\0');		
		}
	}
}
