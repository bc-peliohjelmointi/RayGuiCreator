using System.Numerics;
using System.Security;
using ZeroElectric.Vinculum;

namespace RayGuiCreator
{
	public class MenuCreator
	{
		int startY;
		int drawX;
		int drawY;
		int menuWidth;
		int rowHeight;
		int betweenRows;
		int textSize;
		Font font;

		Stack<MultipleChoiceEntry> dropDowns;

		/// <summary>
		/// Creates a MenuCreator object that can be used to
		/// create menu elements and layout them automatically
		/// </summary>
		/// <param name="x">Top left x coordinate of the menu</param>
		/// <param name="y">Top left y coordinate of the menu</param>
		/// <param name="rowHeight">Minimum height of one element. Text is made to always fit vertically inside a row or element</param>
		/// <param name="width">Minimum width of elements. Text is always made fit horizontally and be fully visible</param>
		/// <param name="betweenItems">Amount of pixels between items vertically, default 1 px</param>
		/// <param name="textHeightAdjust">Text height is rowHeight + this value. Default value 0. Negative values make the text smaller</param>
		public MenuCreator(int x, int y, int rowHeight, int width, int betweenItems = 1, int textHeightAdjust = 0)
		{
			drawX = x; 
			drawY = y;
			startY = y;
			this.rowHeight = rowHeight;
			betweenRows = betweenItems;
			menuWidth = width;
			textSize = rowHeight + textHeightAdjust;
			RayGui.GuiSetStyle((int)GuiControl.DEFAULT, (int)GuiDefaultProperty.TEXT_SIZE, textSize);
			font = RayGui.GuiGetFont();
			dropDowns = new Stack<MultipleChoiceEntry>();
		}
		
		/// <summary>
		/// Creates a text label.
		/// </summary>
		/// <param name="text">Text to be shown</param>
		public void Label(string text)
		{
			Vector2 textWH= GetTextArea(GuiControl.TEXTBOX, text);
			RayGui.GuiLabel(new Rectangle(drawX, drawY, textWH.X, textWH.Y), text);

			drawY += (int)textWH.Y + betweenRows;
		}

		/// <summary>
		/// Creates a clickable button with text and background rectangle
		/// </summary>
		/// <param name="text">Text on the button</param>
		/// <returns>True if clicked</returns>
		public bool Button(string text)
		{
			Vector2 textWH= GetTextArea(GuiControl.TEXTBOX, text);
			bool click = (RayGui.GuiButton(new Rectangle(drawX, drawY, textWH.X, textWH.Y), text) == 1);
			drawY += (int)textWH.Y + betweenRows;
			return click;
		}

		/// <summary>
		/// Creates a button without the background rectangle
		/// </summary>
		/// <param name="text">Text on the button</param>
		/// <returns>True if clicked</returns>
		public bool LabelButton(string text)
		{
			Vector2 textWH = GetTextArea(GuiControl.BUTTON, text);
			bool clicked = (RayGui.GuiLabelButton(new Rectangle(drawX, drawY, textWH.X, textWH.Y), text) == 1);
			drawY += (int)textWH.Y + betweenRows;
			return clicked;
		}

		/// <summary>
		/// Creates a checkbox that can be toggled on and off
		/// </summary>
		/// <param name="text">Text on the left of the box</param>
		/// <param name="value">Is the value toggled. This value changes if the box is clicked</param>
		public void Checkbox(string text, ref bool value)
		{
			Vector2 textWH = GetTextArea(GuiControl.TEXTBOX, text);

			RayGui.GuiCheckBox(new Rectangle(drawX, drawY, textWH.Y, textWH.Y), text, ref value);

			drawY += (int)textWH.Y + betweenRows;
		}

		/// <summary>
		/// Creates a textbox where user can write.
		/// The width of the textbox is determined by the TextBoxEntry's Length
		/// </summary>
		/// <param name="data">Contents of the text area</param>
		public void TextBox(TextBoxEntry data)
		{
			// Make an estimate of the width
			// Width of W: usually the widest letter
			Vector2 rayWH = Raylib.MeasureTextEx(font, "W", textSize, 0.0f);
			// Multiply with text box character count
			int allChars = (int)rayWH.X * data.GetLength();
			int padding = RayGui.GuiGetStyle((int)GuiControl.TEXTBOX, (int)GuiControlProperty.TEXT_PADDING);
			allChars += padding * 2;
			rayWH.Y += padding * 2;
			// Compare to menu width
			int w = Math.Max(allChars, menuWidth);
			int h = Math.Max((int)rayWH.Y, rowHeight);

			unsafe
			{
				fixed (sbyte* textPointer = data.bytes)
				{
					if (RayGui.GuiTextBox(new Rectangle(drawX, drawY, w, h), textPointer, data.GetLength(), data.IsActive) == 1)
					{
						data.IsActive = !data.IsActive;
					}
				}
			}

			drawY += h + betweenRows;
		}

		/// <summary>
		/// Creates a spinner that can be increased and decreased
		/// </summary>
		/// <param name="text">Name of the spinner, shown on the left side</param>
		/// <param name="currentValue">Current value. This is changed when value is decreased or increased.</param>
		/// <param name="minValue">Smallest possible value</param>
		/// <param name="maxValue">Largets possible value</param>
		/// <param name="isActive">Is this control active, is changed automatically</param>
		/// <param name="horizontalPush">How much is pushed to the right, default value 0</param>
		/// <returns></returns>
		public bool Spinner(string text, ref int currentValue, int minValue, int maxValue, ref bool isActive)
		{
			int oldValue = currentValue;
			Vector2 textWH = GetTextArea(GuiControl.SPINNER, text);
			Vector2 aWH = Raylib.MeasureTextEx(font, "A", textSize, 0.0f);
			int textW = (int)(Raylib.MeasureTextEx(font, text, textSize, 0.0f).X + aWH.X);
			unsafe {
				isActive = (RayGui.GuiSpinner(new Rectangle(drawX + textW, drawY, textWH.X-textW, textWH.Y), text, ref currentValue, minValue, maxValue, isActive) == 1);
			}

			drawY += (int)textWH.Y + betweenRows;
			return (oldValue != currentValue);
		}

		/// <summary>
		/// Creates a vertical group of buttons. Only one of the buttons
		/// can be selected at one time.
		/// </summary>
		/// <param name="data">Available choices</param>
		/// <returns>True if the selected button changes</returns>
		public bool ToggleGroup(MultipleChoiceEntry data)
		{
			// Find the widest text?
			// What if menu is too narrow?
			Vector2 textWH = GetTextArea(GuiControl.TOGGLE, data.GetSelected());
			// padding between choices is about 1 px
			int oldIndex = data.GetIndex();
			RayGui.GuiToggleGroup(
				new Rectangle(drawX, drawY, menuWidth, textWH.Y),
				data.GetConcatOptions(), ref data.GetIndex());

			drawY += (int)(data.GetChoiceAmount() * (textWH.Y + betweenRows));
			return (data.GetIndex() != oldIndex);
		}

		/// <summary>
		/// Creates a slider that can be moved horizontally with the mouse.
		/// </summary>
		/// <param name="minText">Text on the left side of slider: minimum value</param>
		/// <param name="maxText">Text on the right side of slider: max value</param>
		/// <param name="value">Current value of slider. Is changed when value changes.</param>
		/// <param name="min">Smallest possible value</param>
		/// <param name="max">Largest possible value</param>
		public void Slider(string minText, string maxText, ref float value, float min, float max)
		{
			Vector2 areaWH = GetTextArea(GuiControl.SLIDER, maxText);
			Vector2 minWH = Raylib.MeasureTextEx(font, minText, textSize, 0.0f);
			Vector2 maxWH = Raylib.MeasureTextEx(font, maxText, textSize, 0.0f);
			Vector2 aWH = Raylib.MeasureTextEx(font, "A", textSize, 0.0f);
			int textW = (int)(minWH.X + maxWH.X + aWH.X);
			int x = drawX + (int)(minWH.X + aWH.X/2);
			RayGui.GuiSlider(new Rectangle(x, drawY, menuWidth-textW, areaWH.Y), minText, maxText, ref value, min, max);
			drawY += (int)areaWH.Y + betweenRows;
		}

		/// <summary>
		/// Creates a dropdown menu where user can select one of the choices.
		/// The choices are drawn only when the dropdown is clicked first.
		/// </summary>
		/// <param name="data">Available choices</param>
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
			drawY += (int)GetTextArea(GuiControl.DROPDOWNBOX, "A").Y + 1;
		}

		/// <summary>
		/// Call this after drawing all items.
		/// This will draw the opened dropdowns correctly.
		/// </summary>
		/// <returns>Height of the menu in pixels</returns>
		public int EndMenu()
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
			return drawY - startY;
		}

		public static Color GetBackgroundColor()
		{
			return Raylib.GetColor(((uint)RayGui.GuiGetStyle(((int)GuiControl.DEFAULT), ((int)GuiDefaultProperty.BACKGROUND_COLOR))));
		}
		public static Color GetLineColor()
		{
			return Raylib.GetColor(((uint)RayGui.GuiGetStyle(((int)GuiControl.DEFAULT), ((int)GuiDefaultProperty.LINE_COLOR))));
		}

		/// <summary>
		/// Draws an open dropdown over other menu elements
		/// </summary>
		/// <param name="data">The choices of the dropdown</param>
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

		/// <summary>
		/// Calculates the dimensions that a string of text will need to be visible.
		/// </summary>
		/// <param name="control">The type of menu element or control</param>
		/// <param name="text">Text to be measured</param>
		/// <param name="addPadding">Should the style's padding be added to the results. Default value is true</param>
		/// <returns>Width and height of the area in pixels.</returns>
		private Vector2 GetTextArea(GuiControl control, string text, bool addPadding = true)
		{
			int padding = RayGui.GuiGetStyle((int)control, (int)GuiControlProperty.TEXT_PADDING);
			Vector2 textWH = Raylib.MeasureTextEx(font, text, textSize, 0.0f);
			if (addPadding)
			{
				textWH.X += padding * 2;
				textWH.Y += padding * 2;
			}
			int w = Math.Max((int)textWH.X, menuWidth);
			int h = Math.Max((int)textWH.Y, rowHeight);
			return new Vector2(w,h);
		}
	}
}
