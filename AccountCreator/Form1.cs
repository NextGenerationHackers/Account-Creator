using PuppeteerSharp;
using PuppeteerSharp.Input;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;

namespace AccountCreator
{
    public partial class Form1 : Form
    {
        /* 
            https://csharp.hotexamples.com/es/examples/WindowsInput/KeyboardSimulator/-/php-keyboardsimulator-class-examples.html
            https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.sendkeys.send?view=net-5.0

            http://www.puppeteersharp.com/api/index.html
            https://www.kiltandcode.com/puppeteer-sharp-crawl-the-web-using-csharp-and-headless-chrome/
        */

        InputSimulator inputSimulator = new InputSimulator();
        public int delay = 100;
        public int Count;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
            labelResolution.Text = ("Resolution: " + screenWidth + "x" + screenHeight);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string url = "https://accounts.google.com/signup/v2/webcreateaccount?service=mail&continue=https%3A%2F%2Fmail.google.com"
            + "%2Fmail&hl=es&dsh=S1912622997%3A1611341957592098&gmb=exp&biz=false&flowName=GlifWebSignIn&flowEntry=SignUp";

            richTextBox1.Text = string.Empty;
            labelInformation.Text = string.Empty;
            button1.Enabled = false;
            timer2.Start();
            button1.Text = "Downloading the chromium version this may take a while...";
            await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);
            button1.Text = "Running chromium instance...";
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = false,
                DefaultViewport = null
            });

            button1.Text = "Working...";
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            await page.BringToFrontAsync();

            // Ignore this line: var six = await page.EvaluateFunctionAsync<int>("async () => await Promise.resolve(6)");

            // This "loop" is executed until the number of accounts is completed
            while (hScrollBar1.Value > Count)
            {
                await CreateAccount(page, delay);
                if (hScrollBar1.Value > Count)
                {
                    await page.ReloadAsync(); // Reload the page if there are more accounts to create
                }
            }

            timer2.Stop(); // Stops the timer of "animation"

            await browser.CloseAsync(); // Close chromium (browser) and all the pages in it
            button1.Text = "Automation finished.";
            await Task.Delay(1000);
            button1.Text = "Automation finished..";
            await Task.Delay(1000);
            button1.Text = "Automation finished...";
            await Task.Delay(1000);

            // Copy the text from the richtextbox to a notepad to save it on your computer
            StreamWriter writer = new StreamWriter("C:\\Users\\" + Environment.UserName + "\\Desktop\\Accounts from scripting.txt");
            foreach (object line in richTextBox1.Lines)
            {
                writer.WriteLine(line);
            }
            writer.Close();

            labelInformation.Text = "The account information has been saved to the desktop.";
            Count = 0;
            button1.Text = "Start bot";
            button1.Enabled = true;
        }

        private async Task CreateAccount(Page page, int delay)
        {
            // http://www.asciitable.com/
            Random random = new Random();

            var firstNameSelector = "#firstName";
            await page.WaitForSelectorAsync(firstNameSelector);
            int Number1 = random.Next(65, 91);
            var Letter1 = Convert.ToChar(Number1).ToString();
            int Number2 = random.Next(65, 91);
            var Letter2 = Convert.ToChar(Number2).ToString();
            int Number3 = random.Next(65, 91);
            var Letter3 = Convert.ToChar(Number3).ToString();
            int Number4 = random.Next(65, 91);
            var Letter4 = Convert.ToChar(Number4).ToString();
            int Number5 = random.Next(65, 91);
            var Letter5 = Convert.ToChar(Number5).ToString();
            // Name
            string Name = Letter1 + Letter2 + Letter3 + Letter4 + Letter5;
            await TypeFieldValue(page, firstNameSelector, Name, delay); // Run this method with the parameters

            var lastNameSelector = "#lastName";
            //await page.Keyboard.PressAsync("Tab");
            int Number6 = random.Next(65, 91);
            var Letter6 = Convert.ToChar(Number6).ToString();
            int Number7 = random.Next(65, 91);
            var Letter7 = Convert.ToChar(Number7).ToString();
            int Number8 = random.Next(65, 91);
            var Letter8 = Convert.ToChar(Number8).ToString();
            int Number9 = random.Next(65, 91);
            var Letter9 = Convert.ToChar(Number9).ToString();
            int Number10 = random.Next(65, 91);
            var Letter10 = Convert.ToChar(Number10).ToString();
            // Last name
            string LastName = Letter6 + Letter7 + Letter8 + Letter9 + Letter10;
            await TypeFieldValue(page, lastNameSelector, LastName, delay);

            var UserName = "#username";
            // The mail might not be available sometimes, i need to fix that
            string Mail = (Letter2 + Letter4 + Letter5 + Letter6 + Letter8 + Letter9 + Letter10).ToLower();
            await TypeFieldValue(page, UserName, Mail, delay);
            await Task.Delay(delay);
            SendKeys.Send("{TAB}");
            await Task.Delay(delay);
            // Password
            string password = Letter1 + Letter3 + Letter5 + Letter7 + Letter9 + "123";
            inputSimulator.Keyboard.TextEntry(password);
            await Task.Delay(delay);
            SendKeys.Send("{TAB}");
            await Task.Delay(delay);
            inputSimulator.Keyboard.TextEntry(password);
            await Task.Delay(delay);
            await page.ClickAsync(".VfPpkd-RLmnJb");    // Press the button
            await Task.Delay(delay * 30);
            Count++;
            richTextBox1.Text += "Gmail Account ~ " + Count + Environment.NewLine;
            richTextBox1.Text += "Name: " + Name + "    " + "LastName: " + LastName + Environment.NewLine;
            richTextBox1.Text += "Mail: " + Mail + "@gmail.com" + "    " + "Password: " + password + Environment.NewLine;
            richTextBox1.Text += Environment.NewLine;
            await Task.Delay(delay);
            // I need to configure the phone number looool
        }

        // It gives the impression that a real person is writing
        private static async Task TypeFieldValue(Page page, string fieldSelector, string value, int delay = 0)
        {
            await page.FocusAsync(fieldSelector);
            await page.TypeAsync(fieldSelector, value, new TypeOptions { Delay = delay });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // In my case i can't get the true resolution of the monitor, i dont know why
            labelPosition.Text = "Mouse position: (" + Cursor.Position.X + ", " + Cursor.Position.Y + ")";
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // This "animates" the label, lmfao (I dont know how to say that)
            if (button1.Text == "Downloading the chromium version this may take a while...")
            {
                button1.Text = "Downloading the chromium version this may take a while.";
            }
            else if (button1.Text == "Downloading the chromium version this may take a while.")
            {
                button1.Text = "Downloading the chromium version this may take a while..";
            }
            else if (button1.Text == "Downloading the chromium version this may take a while..")
            {
                button1.Text = "Downloading the chromium version this may take a while...";
            }
            else if (button1.Text == "Running chromium instance...")
            {
                button1.Text = "Running chromium instance.";
            }
            else if (button1.Text == "Running chromium instance.")
            {
                button1.Text = "Running chromium instance..";
            }
            else if (button1.Text == "Running chromium instance..")
            {
                button1.Text = "Running chromium instance...";
            }
            else if (button1.Text == "Working...")
            {
                button1.Text = "Working.";
            }
            else if (button1.Text == "Working.")
            {
                button1.Text = "Working..";
            }
            else if (button1.Text == "Working..")
            {
                button1.Text = "Working...";
            }
        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            labelNumber.Text = "Number of accounts: " + hScrollBar1.Value;
        }
    }
}