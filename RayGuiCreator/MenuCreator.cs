using System.Numerics;
using System.Security;
using ZeroElectric.Vinculum;

namespace RayGuiCreator
{
	public class MenuCreator
	{
		int drawX;
		int drawY;
		int menuWidth;
		int textSize;
		Font font;

		Stack<MultipleChoiceEntry> dropDowns;

		public MenuCreator(int x, int y, int width)
		{
			drawX = x; drawY = y; 
			menuWidth = width; 
			textSize = RayGui.GuiGetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_SIZE);
			font = RayGui.GuiGetFont();
			dropDowns = new Stack<MultipleChoiceEntry>();
		}
		
		public void Label(string text)
		{
			Vector2 textWH= GetTextArea(GuiControl.TEXTBOX, text);
			RayGui.GuiLabel(new Rectangle(drawX, drawY, menuWidth, textWH.Y), text);

			drawY += (int)textWH.Y;
		}

		public bool LabelButton(string text)
		{
			Vector2 textWH = GetTextArea(GuiControl.BUTTON, text);
			bool clicked = (RayGui.GuiLabelButton(new Rectangle(drawX, drawY, menuWidth, textWH.Y), text) == 1);
			drawY += (int)textWH.Y;
			return clicked;
		}

		public void Checkbox(string text, ref bool value)
		{
			Vector2 textWH = GetTextArea(GuiControl.TEXTBOX, text);

			RayGui.GuiCheckBox(new Rectangle(drawX, drawY, textWH.Y, textWH.Y), text, ref value);

			drawY += (int)textWH.Y;
		}

		public void TextBox(TextBoxEntry data)
		{
			Vector2 textWH = GetTextArea(GuiControl.TEXTBOX, "A");
			unsafe
			{
				fixed (sbyte* textPointer = data.bytes)
				{
					if (RayGui.GuiTextBox(new Rectangle(drawX, drawY, menuWidth, textWH.Y), textPointer, data.GetLength(), data.IsActive) == 1)
					{
						data.IsActive = !data.IsActive;
					}
				}
			}

			drawY += (int)textWH.Y;
		}

		public bool Spinner(string text, ref int currentValue, int minValue, int maxValue, ref bool isActive)
		{
			int oldValue = currentValue;
			Vector2 textWH = GetTextArea(GuiControl.SPINNER, text);
			unsafe {
				isActive = (RayGui.GuiSpinner(new Rectangle(drawX, drawY, menuWidth, textWH.Y), text, ref currentValue, minValue, maxValue, isActive) == 1);
			}

			drawY += (int)textWH.Y;
			return (oldValue != currentValue);
		}

		public bool ToggleGroup(MultipleChoiceEntry data)
		{
			// Find the widest text?
			// What if menu is too narrow?
			Vector2 textWH = GetTextArea(GuiControl.TOGGLE, data.GetSelected());
			// padding between choices is about 1 px
			int between = 1;
			int oldIndex = data.GetIndex();
			RayGui.GuiToggleGroup(
				new Rectangle(drawX, drawY, menuWidth, textWH.Y),
				data.GetConcatOptions(), ref data.GetIndex());

			drawY += (int)(data.GetChoiceAmount() * (textWH.Y + between));
			return (data.GetIndex() != oldIndex);
		}

		public void Slider(string minText, string maxText, ref float value, float min, float max)
		{
			Vector2 textWH = GetTextArea(GuiControl.SLIDER, minText+maxText);
			RayGui.GuiSlider(new Rectangle(drawX, drawY, menuWidth, textWH.Y), minText, maxText, ref value, min, max);
			drawY += (int)textWH.Y;
		}

		public void DropDown(MultipleChoiceEntry data)
		{
			data.DrawY = drawY;
			if (data.IsOpen)
			{
				RayGui.GuiDisable();
				dropDowns.Push(data);
			}
			else
			{
				// Draw here 
				DrawDropdown(data);
			}
			drawY += (int)GetTextArea(GuiControl.DROPDOWNBOX, "A").Y;
		}

		public void EndMenu()
		{
			// Draw all open dropdowns in reverse order
			// so that the are drawn over other elements
			 if (dropDowns.Count > 0)
			{
				RayGui.GuiEnable();
				for (int i = 0; i < dropDowns.Count; i++)
				{
					DrawDropdown(dropDowns.Pop());
				}
			}
		}

		private void DrawDropdown(MultipleChoiceEntry data)
		{
			int tempActive = data.GetIndex();
			Vector2 textWH = GetTextArea(GuiControl.DROPDOWNBOX, data.GetEntryAt(tempActive));
			unsafe
			{
				if (RayGui.GuiDropdownBox(new Rectangle(drawX, data.DrawY, menuWidth, textWH.Y), data.GetConcatOptions(), ref tempActive, data.IsOpen) == 1)
				{
					data.IsOpen = !data.IsOpen;
				}
			}
			data.SetIndex(tempActive);
		}

		private Vector2 GetTextArea(GuiControl control, string text)
		{
			int padding = RayGui.GuiGetStyle((int)control, (int)GuiControlProperty.TEXT_PADDING);
			Vector2 textWH = Raylib.MeasureTextEx(font, text, textSize, 0.0f);
			textWH.X += padding * 2;
			textWH.Y += padding * 2;
			return textWH;
		}
	}
}
