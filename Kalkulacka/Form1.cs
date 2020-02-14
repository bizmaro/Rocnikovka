using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kalkulacka
{
    public partial class Form1 : Form
    {
        private Rectangle outputLabelRect, button0Rect, button1Rect, button2Rect, button3Rect, button4Rect, button5Rect, button6Rect, button7Rect, button8Rect, button9Rect,
            button20Rect, buttonCommaRect, buttonEqualRect, buttonPlusRect, buttonMinusRect, buttonDivRect, buttonTimesRect, buttonBackRect, buttonCRect, buttonCERect, buttonCopyRect, buttonPasteRect, richTextBoxMainRect;

        int _previousLength = 0;
        bool doesntNeedTextChange = false;

        #region Operator Buttons
        private void buttonMinus_Click(object sender, EventArgs e)
        {
            InputString( "-");
            FocusInputText();
        }

        private void buttonTimes_Click(object sender, EventArgs e)
        {
            InputString( "*");
            FocusInputText();
        }

        private void buttonDiv_Click(object sender, EventArgs e)
        {
            InputString( "/");
            FocusInputText();
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            InputString("+");
            FocusInputText();
        }
        private void buttonComma_Click(object sender, EventArgs e)
        {
            InputString(",");
            FocusInputText();
        }
        #endregion
        #region Action Buttons
        private void buttonBack_Click(object sender, EventArgs e)
        {
            int selectionStart = richTextBoxMain.SelectionStart;
            doesntNeedTextChange = true;
            if(richTextBoxMain.SelectionStart > 0)
                richTextBoxMain.Text = richTextBoxMain.Text.Remove(richTextBoxMain.SelectionStart-1, 1);
            if (selectionStart != 0)
                richTextBoxMain.SelectionStart = selectionStart - 1;
            FocusInputText();
        }

        private void buttonC_Click(object sender, EventArgs e)
        {
            richTextBoxMain.Text = string.Empty;
            FocusInputText();
        }

        private void buttonPaste_Click(object sender, EventArgs e)
        {
            string input = Clipboard.GetText();
            char charr;
            for(int i = 0; i < input.Length; i++)
            {
                charr = input[i];
                if (!((charr > 46 && charr < 59) || (charr > 39 && charr < 44) || charr == 45 || charr == ' ' ))
                {
                    input = input.Remove(i, 1);
                    i--;
                }
            }   
            this.doesntNeedTextChange = true;

            InputString(input);
            if(input.Length > 1)
                richTextBoxMain.SelectionStart += input.Length - 1;
            FocusInputText();
        }

        private void OutputLabel_Click(object sender, EventArgs e)
        {

        }

        private void richTextBoxMain_Click(object sender, EventArgs e)
        {
            if (richTextBoxMain.Text == "Input...")
            {
                richTextBoxMain.Text = string.Empty;
            }
        }

        private void richTextBoxOutput_Click(object sender, EventArgs e)
        {
        }

        private void richTextBoxOutput_Leave(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(richTextBoxOutput.Text))
            {
                richTextBoxOutput.Text = "Output...";
            }
        }

        private void richTextBoxOutput_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void richTextBoxMain_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(richTextBoxMain.Text))
            {
                richTextBoxMain.Text = "Input...";
            }
        }

        /// <summary>
        /// Úprava vloženého textu. Nedovolí zadat nepovolené znaky.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void richTextBoxMain_TextChanged(object sender, EventArgs e)
        {
            int currentLength = richTextBoxMain.Text.Length;
            int selectionStart = richTextBoxMain.SelectionStart;
            int selectionLength = richTextBoxMain.SelectionLength;
            int remove = 0;
            char charr;
            if (Math.Abs(this._previousLength - currentLength) > 1 && !this.doesntNeedTextChange && richTextBoxMain.Text != "Input..." && !string.IsNullOrWhiteSpace(richTextBoxMain.Text))
            {
                for (int i = 0; i < richTextBoxMain.Text.Length; i++)
                {
                    charr = richTextBoxMain.Text[i];
                    if (!((charr > 46 && charr < 59) || (charr > 39 && charr < 44) || charr == 45 || charr == ' '))
                    {
                        richTextBoxMain.Text = richTextBoxMain.Text.Remove(i, 1);
                        i--;
                        remove++;
                    }

                }
                richTextBoxMain.SelectionStart = selectionStart + selectionLength - remove;
                richTextBoxMain.SelectionLength = 0;
                FocusInputText();
            }
            
            else if(currentLength - _previousLength == 1 && !doesntNeedTextChange)
            {
                charr = richTextBoxMain.Text[selectionStart - 1];
                if (!((charr > 46 && charr < 59) || (charr > 39 && charr < 44) || charr == 45 || charr == ' '))
                {
                    richTextBoxMain.Text = richTextBoxMain.Text.Remove(selectionStart - 1, 1);
                    richTextBoxMain.SelectionStart = selectionStart - 1;
                }               
            }

            //Zabrani zadani na zacatku jineho znaku nez cislice
            if (selectionStart == 1 && richTextBoxMain.Text.Length > 0)
            {
                charr = richTextBoxMain.Text[0];
                if(charr < 48 || charr > 57)
                {
                    richTextBoxMain.Text = richTextBoxMain.Text.Remove(0, 1);
                }
            }

            //Zabrani zadavani vice nul a operatoru po sobe
            if(selectionStart > 2 && selectionStart <= richTextBoxMain.Text.Length)
            {
                charr = richTextBoxMain.Text[selectionStart - 1];
                if (charr == ' ')
                {
                    if(richTextBoxMain.Text[selectionStart-2] == ' ')
                    {
                        richTextBoxMain.Text = richTextBoxMain.Text.Remove(selectionStart - 1, 1);
                        richTextBoxMain.SelectionStart = selectionStart - 1;
                    }
                }
                else if((charr < 48 || charr > 57) && (richTextBoxMain.Text[selectionStart - 2] < 48 || richTextBoxMain.Text[selectionStart - 2] > 57) && richTextBoxMain.Text[selectionStart - 2] != ' ')
                {
                    richTextBoxMain.Text = richTextBoxMain.Text.Remove(selectionStart - 1, 1);
                    richTextBoxMain.SelectionStart = selectionStart - 1;
                }
            }
            _previousLength = richTextBoxMain.Text.Length;
            
            doesntNeedTextChange = false;
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBoxOutput.Text);
        }
        /// <summary>
        /// Vyhodnocuje výraz pomocí třídy BigNumbers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonEquals_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(richTextBoxOutput.Text))
            {
                Stopwatch stopwatch = new Stopwatch();
                long t;
                BigNumbers arit = new BigNumbers();
                int digits = 0;
                arit.AnalyzeInput(richTextBoxMain.Text);
                stopwatch.Start();
                int vysledek1 = arit.VypocetPostfix(arit.InfixToPostfix(arit.ReadInput(richTextBoxMain.Text)));
                stopwatch.Stop();
                t = stopwatch.ElapsedMilliseconds;
                int remove = 0;
                int[] vysledek = arit.List[vysledek1];
                string stringVysledek = "";
                richTextBoxOutput.Text = string.Empty;
                for (int i = arit.NumberOfNines[vysledek1] - 1; i >= 0; i--)
                {
                    stringVysledek = vysledek[i].ToString();
                    if (stringVysledek.Length > 9)
                    {
                        while (stringVysledek[remove] != '0')
                        {
                            remove++;
                        }
                        stringVysledek = stringVysledek.Remove(0, remove);
                        remove = 0;
                    }
                    digits += stringVysledek.Length;
                    richTextBoxOutput.Text += stringVysledek;
                }
                if (digits > 1)
                {
                    label_digits.Text = "OUTPUT HAS " + digits.ToString() + " DIGITS";
                }
                else
                {
                    label_digits.Text = "OUTPUT HAS " + digits.ToString() + " DIGIT";
                }
                label_computation.Text = "COMPUTATION TOOK " + t.ToString() + " MILISECONDS";
            }
        }

        #endregion
        #region Number Buttons
        private void button9_Click(object sender, EventArgs e)
        {
            InputString("9");
            FocusInputText();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            InputString( "8");
            FocusInputText();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            InputString( "7");
            FocusInputText();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            InputString( "6");
            FocusInputText();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            InputString( "5");
            FocusInputText();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            InputString( "4");
            FocusInputText();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            InputString( "3");
            FocusInputText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InputString( "2");
            FocusInputText();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            InputString( "1");
            FocusInputText();
        }

        private void button0_Click(object sender, EventArgs e)
        {
            InputString( "0");
            FocusInputText();
        }

        #endregion

        private Size originalForm;

        

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            originalForm = this.Size;
            InitializeRectangle();
        }

        #region Resizing
        private void ResizeChildren()
        {
            ResizeControl(button0Rect, button0);
            ResizeControl(button1Rect, button1);
            ResizeControl(button2Rect, button2);
            ResizeControl(button3Rect, button3);
            ResizeControl(button4Rect, button4);
            ResizeControl(button5Rect, button5);
            ResizeControl(button6Rect, button6);
            ResizeControl(button7Rect, button7);
            ResizeControl(button8Rect, button8);
            ResizeControl(button9Rect, button9);
            ResizeControl(buttonEqualRect, buttonEqual);
            ResizeControl(buttonMinusRect, buttonMinus);
            ResizeControl(buttonPlusRect, buttonPlus);
            ResizeControl(buttonTimesRect, buttonTimes);
            ResizeControl(buttonDivRect, buttonDiv);
            ResizeControl(richTextBoxMainRect, richTextBoxMain);
            ResizeControl(buttonBackRect, buttonBack);
            ResizeControl(buttonCRect, buttonC);
            ResizeControl(buttonCopyRect, buttonCopy);

        }

        private void ResizeControl(Rectangle originalRect, Control control)
        {
            float xRatio = (float)(this.Size.Width) / (float)(originalForm.Width);
            float yRatio = (float)(this.Size.Height) / (float)(originalForm.Height);

            control.Location = new Point((int)((float)(originalRect.X) * xRatio), (int)((float)(originalRect.Y) * yRatio));
            control.Size = new Size((int)((float)(originalRect.Width) * xRatio), (int)((float)(originalRect.Height) * yRatio));
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (originalForm.Width != 0)
            {
                ResizeChildren();
            }
        }

        private void InitializeRectangle()
        {
            button0Rect = new Rectangle(button0.Location.X, button0.Location.Y, button0.Size.Width, button0.Size.Height);
            button1Rect = new Rectangle(button1.Location.X, button1.Location.Y, button1.Size.Width, button1.Size.Height);
            button2Rect = new Rectangle(button2.Location.X, button2.Location.Y, button2.Size.Width, button2.Size.Height);
            button3Rect = new Rectangle(button3.Location.X, button3.Location.Y, button3.Size.Width, button3.Size.Height);
            button4Rect = new Rectangle(button4.Location.X, button4.Location.Y, button4.Size.Width, button4.Size.Height);
            button5Rect = new Rectangle(button5.Location.X, button5.Location.Y, button5.Size.Width, button5.Size.Height);
            button6Rect = new Rectangle(button6.Location.X, button6.Location.Y, button6.Size.Width, button6.Size.Height);
            button7Rect = new Rectangle(button7.Location.X, button7.Location.Y, button7.Size.Width, button7.Size.Height);
            button8Rect = new Rectangle(button8.Location.X, button8.Location.Y, button8.Size.Width, button8.Size.Height);
            button9Rect = new Rectangle(button9.Location.X, button9.Location.Y, button9.Size.Width, button9.Size.Height);
            richTextBoxMainRect = new Rectangle(richTextBoxMain.Location.X, richTextBoxMain.Location.Y, richTextBoxMain.Size.Width, richTextBoxMain.Size.Height);
            buttonEqualRect = new Rectangle(buttonEqual.Location.X, buttonEqual.Location.Y, buttonEqual.Size.Width, buttonEqual.Size.Height);
            buttonPlusRect = new Rectangle(buttonPlus.Location.X, buttonPlus.Location.Y, buttonPlus.Size.Width, buttonPlus.Size.Height);
            buttonMinusRect = new Rectangle(buttonMinus.Location.X, buttonMinus.Location.Y, buttonMinus.Size.Width, buttonMinus.Size.Height);
            buttonTimesRect = new Rectangle(buttonTimes.Location.X, buttonTimes.Location.Y, buttonTimes.Size.Width, buttonTimes.Size.Height);
            buttonDivRect = new Rectangle(buttonDiv.Location.X, buttonDiv.Location.Y, buttonDiv.Size.Width, buttonDiv.Size.Height);
            buttonBackRect = new Rectangle(buttonBack.Location.X, buttonBack.Location.Y, buttonBack.Size.Width, buttonBack.Size.Height);
            buttonCRect = new Rectangle(buttonC.Location.X, buttonC.Location.Y, buttonC.Size.Width, buttonC.Size.Height);
            buttonCopyRect = new Rectangle(buttonCopy.Location.X, buttonCopy.Location.Y, buttonCopy.Size.Width, buttonCopy.Size.Height);
        }
        #endregion
        #region Helpers
        private void FocusInputText()
        {
            richTextBoxMain.Focus();
        }

        private void InputString(string value)
        {
            doesntNeedTextChange = true;
            if (richTextBoxMain.Text != "Input...")
            {
                int selectionStart = richTextBoxMain.SelectionStart;
                if (richTextBoxMain.SelectionLength > 0)
                {
                    richTextBoxMain.Text = richTextBoxMain.Text.Remove(selectionStart, richTextBoxMain.SelectionLength);
                    richTextBoxMain.SelectionStart = selectionStart;
                }
                richTextBoxMain.Text = richTextBoxMain.Text.Insert(selectionStart, value);
                richTextBoxMain.SelectionStart = selectionStart + richTextBoxMain.SelectionLength + 1;
                if(richTextBoxMain.SelectionStart > 2)
                {
                    int charr = richTextBoxMain.Text[richTextBoxMain.SelectionStart - 1];
                    if ((charr < 48 || charr > 57) && (richTextBoxMain.Text[richTextBoxMain.SelectionStart - 2] < 48 || richTextBoxMain.Text[richTextBoxMain.SelectionStart - 2] > 57) && richTextBoxMain.Text[richTextBoxMain.SelectionStart - 2] != ' ')
                    {
                        richTextBoxMain.Text = richTextBoxMain.Text.Remove(richTextBoxMain.SelectionStart - 1, 1);
                        richTextBoxMain.SelectionStart = selectionStart;
                    }
                }
                richTextBoxMain.SelectionLength = 0;
            }
            else
            {
                richTextBoxMain.Text = value;
                richTextBoxMain.SelectionStart = richTextBoxMain.SelectionLength + 1;
            }

        }
        #endregion
    }
}
