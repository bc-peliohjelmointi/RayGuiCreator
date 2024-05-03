using System.Text;

namespace RayGuiCreator
{
	public class MultipleChoiceEntry
	{
		private string[] choices;
		private int selectedIndex;
		private string concatChoises;

		public int DrawY { get; set; }
		public bool IsOpen { get; set; }

		public MultipleChoiceEntry(string[] choices)
		{
			this.choices = choices;
			selectedIndex = 0;

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < choices.Length; i++)
			{
				builder.Append(choices[i]);
				if (i != choices.Length - 1)
				{
					builder.Append("\n");
				}
			}
			concatChoises = builder.ToString();
		}

		public string GetConcatOptions()
		{
			return concatChoises;
		}

		public int GetChoiceAmount()
		{
			return choices.Length;
		}

		public void SetIndex(int index)
		{
			selectedIndex = Math.Clamp(index, 0, choices.Length-1);
		}

		public ref int GetIndex()
		{
			return ref selectedIndex;
		}

		public string GetSelected()
		{
			return choices[selectedIndex];
		}

		public string GetEntryAt(int index)
		{
			index = Math.Clamp(index, 0, choices.Length - 1);
			return choices[index];
		}

		public string ToString()
		{
			return choices[selectedIndex];
		}
	}
}
