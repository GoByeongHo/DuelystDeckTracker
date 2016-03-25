using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuelystDeckTracker
{
    public partial class DuelstDeckTrecker : Form
    {
        Dictionary<string, int> currentDeck = new Dictionary<string, int>();
        Dictionary<string, int> viewDeck;

        List<Button> buttonList = new List<Button>();
        
        public DuelstDeckTrecker()
        {
            InitializeComponent();
            CardListLoad();
        }

        public void CardListLoad()
        {
            using(System.IO.StreamReader sr = 
                new System.IO.StreamReader("CardList.txt"))
            { 
                while(true)
                {
                    string cardName = sr.ReadLine();
                    if (cardName == null)
                        return;
                    SearchBox.AutoCompleteCustomSource.Add(cardName);
                }
            }
        }

        private void textEnter(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AddDeck(SearchBox.Text);
            }
        }

        private void AddDeck(string _cardName)
        {
            if (currentDeck.ContainsKey(_cardName))
            {
                if (currentDeck[_cardName] == 3)
                    return;
                currentDeck[_cardName] += 1;
                this.Controls.Find(_cardName, false)[0].Text = _cardName + "\nx" + currentDeck[_cardName];
            }
            else
            {
                currentDeck.Add(_cardName, 1);

                ResetDeck();
            }
        }

        private void CreateCardButton(string _cardName, int _count)
        {
            Button cardBtn = new Button();

            cardBtn.Name = _cardName;
            cardBtn.Text = "        "+_cardName + "\nx" + currentDeck[_cardName];
            cardBtn.TextAlign = ContentAlignment.MiddleRight;
            cardBtn.Location = new Point(5, 25 + 42 * _count);
            cardBtn.Height = 44;
            cardBtn.Width = 175;

            if(System.IO.File.Exists("Resource\\" + _cardName.Trim().Replace(" ", "") + ".png"))
                cardBtn.Image = new Bitmap("Resource\\" + _cardName.Trim().Replace(" ", "") + ".png");
            cardBtn.ImageAlign = ContentAlignment.MiddleLeft;

            cardBtn.MouseClick += new MouseEventHandler(CardBtn_Click);
            this.Controls.Add(cardBtn);

            buttonList.Add(cardBtn);

            Button cardDeleteBtn = new Button();
            cardDeleteBtn.Name = "Minus"+_cardName;
            cardDeleteBtn.Text = "-";
            cardDeleteBtn.Location = new Point(180, 25 + 42 * _count);
            cardDeleteBtn.Height = 44;
            cardDeleteBtn.Width = 25;
            cardDeleteBtn.MouseClick += new MouseEventHandler(CardDeleteBtn_Click);
            this.Controls.Add(cardDeleteBtn);

            buttonList.Add(cardDeleteBtn);
        }

        private void RenewalCardButton()
        {
            int count = 0;
            int total = 0;

            foreach(KeyValuePair<string,int> card in currentDeck)
            {
                CreateCardButton(card.Key, ++count);

                total += card.Value;
            }

            Total.Text = "Total : " + total;
            viewDeck = new Dictionary<string,int>(currentDeck);
        }

        private void ResetDeck()
        {
            foreach(Button btn in buttonList)
            {
                btn.Dispose();
            }

            RenewalCardButton();
        }

        private void CardBtn_Click(object sender, EventArgs e)
        {
            Button sendButton = sender as Button;

            if (viewDeck[sendButton.Name] == 0)
                return;

            viewDeck[sendButton.Name] -= 1;
            this.Controls.Find(sendButton.Name, false)[0].Text = sendButton.Name + "\nx" + viewDeck[sendButton.Name];
            if (viewDeck[sendButton.Name] == 0)
                this.Controls.Find(sendButton.Name, false)[0].BackColor = Color.Gray;
        }

        private void CardDeleteBtn_Click(object sender, EventArgs e)
        {
            Button sendButton = sender as Button;

            currentDeck.Remove(sendButton.Name.Remove(0, 5));

            ResetDeck();
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            using(SaveFileDialog saveFileDialog = new SaveFileDialog())
            { 
                saveFileDialog.Filter = "Deck|*.deck";
                saveFileDialog.Title = "Save a Deck";
                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "")
                {
                    using(System.IO.FileStream fs =
                        (System.IO.FileStream)saveFileDialog.OpenFile())
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter format =
                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        format.Serialize(fs, currentDeck);
                    }
                }
            }
        }

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Deck|*.deck";
                openFileDialog.Title = "Load a Deck";
                openFileDialog.ShowDialog();


                if (openFileDialog.FileName != "")
                {
                    using(System.IO.FileStream fs =
                        (System.IO.FileStream)openFileDialog.OpenFile())
                    {
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter format =
                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        object read = format.Deserialize(fs);
                        currentDeck = read as Dictionary<string, int>;
                    }

                    ResetDeck();
                }
            }
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            ResetDeck();
        }

        private void TopMostBtn_Click(object sender, EventArgs e)
        {
            if (TopMost == true)
            {
                TopMost = false;
                TopMostBtn.Text = "D";
                TopMostBtn.BackColor = SystemColors.Control;
            }
            else if (TopMost == false)
            {
                TopMost = true;
                TopMostBtn.Text = "T";
                TopMostBtn.BackColor = Color.Gray;
            }
        }
    }
}
