using System;
using System.Windows.Forms;

namespace lab1Protection
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            toolStripComboBox1.SelectedIndex = 0;
            textBox1.Focus();
            uTF16ToolStripMenuItem.Checked = true;
            }


        private int sizeOfBlock; //в DES размер блока 64 бит, но поскольку в unicode символ в два раза длинее, то увеличим блок тоже в два раза
        private int sizeOfChar; //размер одного символа (in Unicode 16 bit)

        private int shiftKey = 2; //сдвиг ключа 

        private int quantityOfRounds = 16; //количество раундов

        string[] Blocks; //сами блоки в двоичном формате

        byte comboBoxIndex;

        private void button1_Click(object sender, EventArgs e)
        {
            textBox5.Clear();
            try
            {
                if (textBox3.Text.Length == 0)
                {
                    MessageBox.Show("Введите сообщение", "Ошибка");
                    return;
                }
                if (uTF8ToolStripMenuItem.Checked == true)
                {
                    sizeOfBlock = 64;
                    sizeOfChar = 8;

                    textBox3.MaxLength = 64;
                    textBox3.Text = textBox3.Text;
                }
                else
                {
                    sizeOfBlock = 128;
                    sizeOfChar = 16;
                }

                if (textBox1.Text.Length > 0)
                {
                    string s = textBox3.Text;

                    string key = textBox1.Text;

                    s = StringToRightLength(s);

                    CutStringIntoBlocks(s);


                    if (comboBoxIndex == 0)
                    {
                        key = CorrectKeyWord(key, (sizeOfBlock - sizeOfChar) / sizeOfChar);
                        textBox1.Text = key;
                        key = StringToBinaryFormat(key);
                    }
                    else if (comboBoxIndex == 1)
                    {
                        key = CorrectKeyWord(key, (sizeOfBlock - sizeOfChar));
                        textBox1.Text = key;
                    }
                    else
                    {
                        key = CorrectKeyWord(key, (sizeOfBlock - sizeOfChar) / 4);
                        textBox1.Text = key;
                        key = StringFromHexadecimalToBinaryFormat(key);
                    }

                    for (int j = 0; j < quantityOfRounds; j++)
                    {
                        for (int i = 0; i < Blocks.Length; i++)
                            Blocks[i] = EncodeDES_One_Round(Blocks[i], key);

                        key = KeyToNextRound(key);
                    }

                    key = KeyToPrevRound(key);

                    if (comboBoxIndex == 0)
                    {
                        textBox2.Text = StringFromBinaryToNormalFormat(key);
                    }
                    else if (comboBoxIndex == 1)
                    {
                        textBox2.Text = key;
                    }
                    else
                    {
                        textBox2.Text = StringFromBinaryToHexadecimalFormat(key);
                    }

                    string result = "";

                    for (int i = 0; i < Blocks.Length; i++)
                    {
                        result += Blocks[i];
                    }

                    textBox4.Text = StringFromBinaryToNormalFormat(result);
                }
                else
                {
                    MessageBox.Show("Введите ключ!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                uTF16ToolStripMenuItem.Checked = true;
                uTF8ToolStripMenuItem.Checked = false;

                button1_Click(sender, e);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0)
            {
                string s = textBox4.Text;
                string key = "";
                if (comboBoxIndex == 0)
                {
                    key = StringToBinaryFormat(textBox2.Text);
                }
                else if (comboBoxIndex == 1)
                {
                    key = textBox2.Text;
                }
                else
                {
                    key = StringFromHexadecimalToBinaryFormat(textBox2.Text);
                }

                s = StringToBinaryFormat(s);

                CutBinaryStringIntoBlocks(s);

                for (int j = 0; j < quantityOfRounds; j++)
                {
                    for (int i = 0; i < Blocks.Length; i++)
                        Blocks[i] = DecodeDES_One_Round(Blocks[i], key);

                    key = KeyToPrevRound(key);
                }

                key = KeyToNextRound(key);

                if (comboBoxIndex == 0)
                {
                    textBox1.Text = StringFromBinaryToNormalFormat(key);
                }
                else if (comboBoxIndex == 1)
                {
                    textBox1.Text = key;
                }
                else
                {
                    textBox1.Text = StringFromBinaryToHexadecimalFormat(key);
                }

                string result = "";

                for (int i = 0; i < Blocks.Length; i++)
                    result += Blocks[i];

                textBox5.Text = StringFromBinaryToNormalFormat(result).TrimEnd('#');

            }
            else
                MessageBox.Show("Введите ключевое слово!");
        }

        private string StringToRightLength(string input)
        {
            while (((input.Length * sizeOfChar) % sizeOfBlock) != 0)
            {
                input += "#";
            }

            return input;
        }

        private void CutStringIntoBlocks(string input)
        {
            Blocks = new string[(input.Length * sizeOfChar) / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
            {
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
                Blocks[i] = StringToBinaryFormat(Blocks[i]);
            }
        }

        private void CutBinaryStringIntoBlocks(string input)
        {
            Blocks = new string[input.Length / sizeOfBlock];

            int lengthOfBlock = input.Length / Blocks.Length;

            for (int i = 0; i < Blocks.Length; i++)
                Blocks[i] = input.Substring(i * lengthOfBlock, lengthOfBlock);
        }

        private string StringToBinaryFormat(string input)
        {
            string output = "";

            for (int i = 0; i < input.Length; i++)
            {
                string char_binary = Convert.ToString(input[i], 2);

                while (char_binary.Length < sizeOfChar)
                    char_binary = "0" + char_binary;

                output += char_binary;
            }

            return output;
        }

        private string CorrectKeyWord(string input, int lengthKey)
        {
            if (input.Length > lengthKey)
                input = input.Substring(0, lengthKey);
            else
                while (input.Length < lengthKey)
                    input = "0" + input;

            return input;
        }

        private string EncodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (R + XOR(L, f(R, key)));
        }

        private string DecodeDES_One_Round(string input, string key)
        {
            string L = input.Substring(0, input.Length / 2);
            string R = input.Substring(input.Length / 2, input.Length / 2);

            return (XOR(f(L, key), R) + L);
        }

        private string XOR(string s1, string s2)
        {
            string result = "";

            for (int i = 0; i < s1.Length; i++)
            {
                bool a = Convert.ToBoolean(Convert.ToInt32(s1[i].ToString()));
                bool b = Convert.ToBoolean(Convert.ToInt32(s2[i].ToString()));

                if (a ^ b)
                    result += "1";
                else
                    result += "0";
            }
            return result;
        }

        private string f(string s1, string s2)
        {
            return XOR(s1, s2);
        }

        private string KeyToNextRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key[key.Length - 1] + key;
                key = key.Remove(key.Length - 1);
            }

            return key;
        }

        private string KeyToPrevRound(string key)
        {
            for (int i = 0; i < shiftKey; i++)
            {
                key = key + key[0];
                key = key.Remove(0, 1);
            }

            return key;
        }

        private string StringFromBinaryToNormalFormat(string input)
        {
            string output = "";

            while (input.Length > 0)
            {
                string char_binary = input.Substring(0, sizeOfChar);
                input = input.Remove(0, sizeOfChar);

                int a = 0;
                int degree = char_binary.Length - 1;

                foreach (char c in char_binary)
                    a += Convert.ToInt32(c.ToString()) * (int)Math.Pow(2, degree--);

                output += ((char)a).ToString();
            }

            return output;
        }

        private string StringFromHexadecimalToBinaryFormat(string str)
        {
            string res = "";
            for (int i = 0; i < str.Length; i++)
            {
                switch (str[i])
                {
                    case '0': res += "0000"; break;
                    case '1': res += "0001"; break;
                    case '2': res += "0010"; break;
                    case '3': res += "0011"; break;
                    case '4': res += "0100"; break;
                    case '5': res += "0101"; break;
                    case '6': res += "0110"; break;
                    case '7': res += "0111"; break;
                    case '8': res += "1000"; break;
                    case '9': res += "1001"; break;
                    case 'A':
                    case 'a': res += "1010"; break;
                    case 'B':
                    case 'b': res += "1011"; break;
                    case 'C':
                    case 'c': res += "1100"; break;
                    case 'D':
                    case 'd': res += "1101"; break;
                    case 'E':
                    case 'e': res += "1110"; break;
                    case 'F':
                    case 'f': res += "1111"; break;
                }
            }

            return res;
        }

        private string StringFromBinaryToHexadecimalFormat(string str)
        {
            string res = "";
            string[] strmas = new string[str.Length / 4];

            for (int i = 0, j = 0; i < str.Length; i += 4, j++)
            {
                string eb = str[i].ToString();
                eb += str[i + 1].ToString();
                eb += str[i + 2].ToString();
                eb += str[i + 3].ToString();
                strmas[j] = eb;
            }

            for (int i = 0; i < str.Length / 4; i++)
            {
                if (strmas[i] == "0000")
                {
                    res += "0";
                }
                else if (strmas[i] == "0001")
                {
                    res += "1";
                }
                else if (strmas[i] == "0010")
                {
                    res += "2";
                }
                else if (strmas[i] == "0011")
                {
                    res += "3";
                }
                else if (strmas[i] == "0100")
                {
                    res += "4";
                }
                else if (strmas[i] == "0101")
                {
                    res += "5";
                }
                else if (strmas[i] == "0110")
                {
                    res += "6";
                }
                else if (strmas[i] == "0111")
                {
                    res += "7";
                }
                else if (strmas[i] == "1000")
                {
                    res += "8";
                }
                else if (strmas[i] == "1001")
                {
                    res += "9";
                }
                else if (strmas[i] == "1010")
                {
                    res += "A";
                }
                else if (strmas[i] == "1011")
                {
                    res += "B";
                }
                else if (strmas[i] == "1100")
                {
                    res += "C";
                }
                else if (strmas[i] == "1101")
                {
                    res += "D";
                }
                else if (strmas[i] == "1110")
                {
                    res += "E";
                }
                else if (strmas[i] == "1111")
                {
                    res += "F";
                }
            }

            return res;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (comboBoxIndex == 0)
            {
                textBox1.MaxLength = 7;
            }
            else if (comboBoxIndex == 1)
            {
                if (uTF8ToolStripMenuItem.Checked == true)
                {
                    textBox1.MaxLength = 56;
                }
                else
                {
                    textBox1.MaxLength = 112;
                }

                if ((e.KeyChar <= 47 || e.KeyChar >= 50) && e.KeyChar != 8)
                {
                    e.Handled = true;
                }
            }
            else if (comboBoxIndex == 2)
            {
                if (uTF8ToolStripMenuItem.Checked == true)
                {
                    textBox1.MaxLength = 14;
                }
                else
                {
                    textBox1.MaxLength = 28;
                }

                if ((e.KeyChar <= 47 || e.KeyChar >= 58) && (e.KeyChar <= 64 || e.KeyChar >= 71) && (e.KeyChar <= 96 || e.KeyChar >= 103) && e.KeyChar != 8)
                {
                    e.Handled = true;
                }
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBoxIndex = ((byte)toolStripComboBox1.SelectedIndex);
            textBox1.Clear();
            textBox1.Focus();
        }

        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uTF16ToolStripMenuItem.Checked = false;
        }

        private void uTF16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            uTF8ToolStripMenuItem.Checked = false;
        }

        private void очиститьПоляToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Text = textBox2.Text = textBox3.Text = textBox4.Text = textBox5.Text = "";
        }
    }
}