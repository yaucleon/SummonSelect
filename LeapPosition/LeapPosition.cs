using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leap;
using GloveLibrary;
using System.IO;
using System.Diagnostics;


namespace LeapPosition
{
    public partial class LeapPosition : Form
    {

        bool testRun = false;
        Hand right_hand;
        Hand left_hand;
        Vector handPosition;
        Finger thumbfinger;
        Finger indexfinger;
        Finger midfinger;
        Point clickpos;
        const int LEAP_SENS = 3;
        const int LEAP_HEIGHT_OFFSET = 350; // {250 , 500} comfortable range, Higher is higher
        const int LEFT_THRESH = -50;
        const int RIGHT_THRESH = 50;
        const int TOP_THRESH = 50;
        const int BOT_THRESH = -50;
        const int TOP_TOP_THRESH = 100;
        const int BOT_BOT_THRESH = -100;
        const int SLIDER_MIN = 0;
        const int SLIDER_MAX = 100;
        bool sliderReached = false;
        bool testing = false;
        bool left_hand_closed = false;
        const int numButtonTrials = 24;
        const int numSliderTrials = 10;
        int scrollClicks = 0;
        int t_0 = 0;
        int t_1 = 0;
        int t_2 = 0;
        int t_2_ = 0;
        int t_3 = 0;
        int t_4 = 0;
        int numTabs = 0;
        bool first = true;
        Button[] button = new Button[6];
        Label[] buttonLabel = new Label[6];
        TrackBar[] scroll = new TrackBar[5];
        TextBox[] scrollValue = new TextBox[5];
        Label[] scrollLabel = new Label[5];
        TrackBar selectedScroll = new TrackBar();
        Button clickedButton = new Button();
        int clickedButtonIndex = 0;
        StreamWriter writer;
        int slider = 0;
        int sliderInit = 0;
        int currTab = 0;
        bool scrollSelected = false;
        bool initHand = false;
        Vector initHandPos = new Vector();
        bool clicked = false;
        bool validated = false;
        bool cursorExp = false;
        bool posExp = false;
        bool tabExp = false;
        bool buttonExp = false;
        bool sliderExp = false;
        bool buttonGesture = false;
        bool sliderGesture = false;
        bool tabbed = false;
        int clicks = 0;
        int[] trialTime = new int[20];
        int[] trialItem;
        int[] sliderTrialVal;
        int click = 0;
        int currTrial = 0;
        int xpos;
        int ypos;
        Point controlCenter;
        float midflexed;
        float thumbflexed;
        bool thumbExtended;
        Stopwatch sw = new Stopwatch();
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        public LeapPosition()
        {
            InitializeComponent();
            Controller controller = new Controller();
            controller.Connect += OnServiceConnect;
            controller.Device += OnConnect;
            controller.FrameReady += OnFrame;
            this.StartButton.Click += new System.EventHandler(this.Start_Click);
            //this.button1.Click += new System.EventHandler(this.button1_click);
            this.MouseClick += new MouseEventHandler(this.mouse_click);
            this.button1.Click += new System.EventHandler(this.button1_click);
            this.button2.Click += new System.EventHandler(this.button2_click);
            this.button3.Click += new System.EventHandler(this.button3_click);
            this.button4.Click += new System.EventHandler(this.button4_click);
            this.button5.Click += new System.EventHandler(this.button5_click);
            this.button6.Click += new System.EventHandler(this.button6_click);
            this.button1.MouseEnter += new System.EventHandler(this.buttonMouseOver);
            this.button2.MouseEnter += new System.EventHandler(this.buttonMouseOver);
            this.button3.MouseEnter += new System.EventHandler(this.buttonMouseOver);
            this.button4.MouseEnter += new System.EventHandler(this.buttonMouseOver);
            this.button5.MouseEnter += new System.EventHandler(this.buttonMouseOver);
            this.button6.MouseEnter += new System.EventHandler(this.buttonMouseOver);
            this.button1.MouseLeave += new System.EventHandler(this.buttonLeave);
            this.button2.MouseLeave += new System.EventHandler(this.buttonLeave);
            this.button3.MouseLeave += new System.EventHandler(this.buttonLeave);
            this.button4.MouseLeave += new System.EventHandler(this.buttonLeave);
            this.button5.MouseLeave += new System.EventHandler(this.buttonLeave);
            this.button6.MouseLeave += new System.EventHandler(this.buttonLeave);

            this.Height = 2160;
            this.Width = 3840;
            nullButton.Focus();
            nullButton.Select();
            
            button = new Button[]{ button1, button2, button3, button4, button5, button6};
            buttonLabel = new Label[] { button1Label, button2Label, button3Label, button4Label, button5Label, button6Label };
            scroll = new TrackBar[] { Brightness, Contrast, Hue, Saturation, Volume };
            scrollValue = new TextBox[] { BrightnessValue, ContrastValue, HueValue, SaturationValue, VolumeValue };
            scrollLabel = new Label[] { BrightnessLabel, ContrastLabel, HueLabel, SaturationLabel, VolumeLabel };
            this.scroll[0].MouseEnter += new System.EventHandler(this.sliderMouseOver);
            this.scroll[1].MouseEnter += new System.EventHandler(this.sliderMouseOver);
            this.scroll[2].MouseEnter += new System.EventHandler(this.sliderMouseOver);
            this.scroll[3].MouseEnter += new System.EventHandler(this.sliderMouseOver);
            this.scroll[4].MouseEnter += new System.EventHandler(this.sliderMouseOver);

        }
        public void sliderMouseOver(object sender, EventArgs e)
        {
            if (testing && sliderExp && first && cursorExp)
            {
                if (sender.Equals(scroll[trialItem[currTrial]]))
                {
                    t_2_ = (int)sw.ElapsedMilliseconds;
                    first = false;
                }
            }
        }
        public void buttonMouseOver(object sender, EventArgs e)
        {
            if (testing && buttonExp && first && cursorExp)
            {
                if (sender.Equals(button[trialItem[currTrial]]))
                {
                    t_2_ = (int)sw.ElapsedMilliseconds;
                    first = false;
                }
            }
            for (int i = 0; i < button.Length; i++)
            {
                if (button[i].Equals(sender))
                {
                    buttonFocus(i);
                }
            }
        }
        public void buttonLeave(object sender, EventArgs e)
        {
            buttonFocus(-1);
        }
        public float distance(Point p, Point q)
        {
            double a = p.X - q.X;
            double b = p.Y - q.Y;
            double distance = Math.Sqrt(a * a + b * b);
            return (float) distance;
        }
        public static void Randomize<T>(T[] items)
        {
            Random rand = new Random();
            for (int i = 0; i < items.Length - 1; i++)
            {
                int j = rand.Next(i, items.Length);
                T temp = items[i];
                items[i] = items[j];
                items[j] = temp;
            }
        }
        public void validate()
        {
            testing = false;
            validated = true;
            if (cursorExp)
            {
                Cursor.Position = new Point(this.Width/2, this.Height/2);
            }
            if (buttonExp)
            {
                t_3 = (int)sw.ElapsedMilliseconds;
                buttonFocus(-1);
                if (cursorExp)
                {
                    writer.Write(" " + clickedButtonIndex + " " + clicks + " " + t_1 + " " + t_2_ + " " + t_3);
                }
                else if (tabExp)
                {
                    writer.Write(" " + clickedButtonIndex + " " + numTabs + " " + t_1 + " " + t_2 + " " + t_3);
                }
                else
                {
                    writer.Write(" " + clickedButtonIndex + " "  + t_1 + " " + t_2 + " " + t_3);
                }
            }
            if (sliderExp)
            {
                int selectedScrollIndex = 0;
                t_4 = (int)sw.ElapsedMilliseconds;
                for (int i = 0; i < scroll.Length; i++)
                {
                    if (selectedScroll.Equals(scroll[i]))
                    {
                        selectedScrollIndex = i;
                    }
                }
                if (cursorExp)
                {
                    writer.Write(" " + selectedScrollIndex + " " + selectedScroll.Value + " " + scrollClicks + " " + t_1 + " " + t_2_ + " " + t_3 + " " + t_4);
                }
                else if (tabExp)
                {
                    writer.Write(" " + selectedScrollIndex + " " + selectedScroll.Value + " " + numTabs + " " + t_1 + " " + t_2 + " " + t_3 + " " + t_4);
                }
                else
                {
                    writer.Write(" " + selectedScrollIndex + " " + selectedScroll.Value + " " + t_1 + " " + t_2 + " " + t_3 + " " + t_4);
                }
            }
            

            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Done..."; }));
            clicks = 0;
            numTabs = 0;
            first = true;
        }
        public void mouse_click(object sender, MouseEventArgs e)
        {
            if (testing)
            {
                //controlCenter = new Point(buttonArray[currTrial].Location.X + buttonArray[currTrial].Width / 2, buttonArray[currTrial].Location.Y + buttonArray[currTrial].Height / 2);
                //writer.Write(" " + distance(controlCenter, e.Location));
                clicks++;
            }
            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(click++);
            //Console.WriteLine(e.Location);
        }
        private void button1_click(object sender, EventArgs e)
        {
           // Console.WriteLine("1 clicked");
            if (testing && buttonExp)
            {
                clickedButton = button1;
                clickedButtonIndex = 0;
                clicks++;
                validate();
                
                //sw.Stop();
                //writer.WriteLine(" " + "0 " + clicks + " " + sw.ElapsedMilliseconds);
                //nextTrial();
            }
            
            
            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(controlCenter);
            //Console.WriteLine(e.Location);
        }
        private void button2_click(object sender, EventArgs e)
        {
            //Console.WriteLine("2 clicked");
            if (testing && buttonExp)
            {
                clickedButton = button2;
                clickedButtonIndex = 1;
                clicks++;
                validate();
                //sw.Stop();
                
                //writer.WriteLine(" " + "0 " + clicks + " " + sw.ElapsedMilliseconds);
                //nextTrial();
            }
            
            
            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(controlCenter);
            //Console.WriteLine(e.Location);
        }
        private void button3_click(object sender, EventArgs e)
        {
            //Console.WriteLine("3 clicked");
            if (testing && buttonExp)
            {
                clickedButton = button3;
                clickedButtonIndex = 2;
                clicks++;
                validate();
                //sw.Stop();
                
                //writer.WriteLine(" " + "0 " + clicks + " " + sw.ElapsedMilliseconds);
                //nextTrial();
            }
            
            
            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(controlCenter);
            //Console.WriteLine(e.Location);
        }
        private void button4_click(object sender, EventArgs e)
        {
           // Console.WriteLine("4 clicked");
            if (testing && buttonExp)
            {
                clickedButton = button4;
                clickedButtonIndex = 3;
                clicks++;
                validate();
                //sw.Stop();
                
                //writer.WriteLine(" " + "0 " + clicks + " " + sw.ElapsedMilliseconds);
                //nextTrial();
            }
           
            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(controlCenter);
            //Console.WriteLine(e.Location);
        }
        private void button5_click(object sender, EventArgs e)
        {
            //Console.WriteLine("5 clicked");
            if ( testing && buttonExp)
            {
                clickedButton = button5;
                clickedButtonIndex = 4;
                clicks++;
                validate();
                //sw.Stop();
                
                //writer.WriteLine(" " + "0 " + clicks + " " + sw.ElapsedMilliseconds);
                //nextTrial();
            }
           
            
            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(controlCenter);
            //Console.WriteLine(e.Location);
        }
        private void button6_click(object sender, EventArgs e)
        {
            //Console.WriteLine("6 clicked");
            if (testing && buttonExp)
            {
                clickedButton = button6;
                clickedButtonIndex = 5;
                clicks++;
                validate();
                //sw.Stop();
                
                //writer.WriteLine(" " + "0 " + clicks + " " + sw.ElapsedMilliseconds);
                //nextTrial();
            }
            

            //Console.WriteLine(distance(controlCenter, e.Location));
            //Console.WriteLine(controlCenter);
            //Console.WriteLine(e.Location);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        public async void Start_Click(object sender, EventArgs e) {
            this.Invoke(new MethodInvoker(delegate { StartButton.Visible = false; }));
            startMsgs();
            
            foreach (Button button in button)
            {
                button.BackColor = Color.LightGray;
            }
            if (ButtonTest.Checked)
            {
                buttonExp = true;
                sliderExp = false;
                trialItem = new int[] { 3, 0, 5, 1, 3, 3, 5, 0, 4, 4, 0, 0, 1, 2, 2, 1, 4, 5, 2, 4, 3, 2, 1, 5 };
                Randomize(trialItem);
                foreach(int a in trialItem)
                {
                    //Console.Write(a + ", ");
                }
                
                
                foreach (TrackBar a in scroll)
                {
                    a.Visible = false;
                }
                foreach (Button a in button)
                {
                    a.Visible = true;
                }
                foreach (Label a in scrollLabel)
                {
                    a.Visible = false;
                }
                foreach (TextBox a in scrollValue)
                {
                    a.Visible = false;
                }

            }
            else if (SliderTest.Checked)
            {
                buttonExp = false;
                sliderExp = true;
                trialItem = new int[] { 0, 3, 4, 2, 1, 1, 0, 3, 4, 2 };
                sliderTrialVal = new int[] { 80, 30, 30, 80, 30, 80, 30, 80, 80, 30 };
                //Test
                if (testRun)
                {
                    sliderTrialVal = new int[] { 8, 10, 30, 60, 39, 48, 20, 5, 0, 80, 30 };
                    Randomize(trialItem);
                }




                foreach (TrackBar a in scroll)
                {
                    a.Visible = true;
                }
                foreach (Button a in button)
                {
                    a.Visible = false;
                }
                foreach (Label a in scrollLabel)
                {
                    a.Visible = true;
                }
                foreach (TextBox a in scrollValue)
                {
                    a.Visible = true;
                }
            }
            if (CursorTest.Checked)
            {
                Cursor.Position = new Point(this.Width / 2, this.Height / 2);
                currTrial = 0;
                cursorExp = true;
                posExp = false;
                tabExp = false;
                if (buttonExp)
                {
                    writer = new System.IO.StreamWriter(@".\Trials\" + "Button_LeapCursor.txt");
                    writer.WriteLine(DateTime.Now.ToString());
                    await Task.Delay(5000);
                    buttonTrial();
                }
                else
                {
                    writer = new System.IO.StreamWriter(@".\Trials\" + "Slider_LeapCursor.txt");
                    writer.WriteLine(DateTime.Now.ToString());
                    await Task.Delay(5000);
                    sliderTrial();
                }
                
                
                
            }
            else if (PosTest.Checked)
            {
                currTrial = 0;
                cursorExp = false;
                posExp = true;
                tabExp = false;
                Cursor.Hide();
                if (buttonExp)
                {
                    writer = new System.IO.StreamWriter(@".\Trials\" + "Button_LeapPosition.txt");
                    writer.WriteLine(DateTime.Now.ToString());
                    await Task.Delay(5000);
                    buttonTrial();
                }
                else
                {
                    writer = new System.IO.StreamWriter(@".\Trials\" + "Slider_LeapPosition.txt");
                    writer.WriteLine(DateTime.Now.ToString());
                    await Task.Delay(5000);
                    sliderTrial();
                }
                
                
            }
            else if (TabTest.Checked)
            {
                currTrial = 0;
                cursorExp = false;
                posExp = false;
                tabExp = true;
                Cursor.Hide();
                if (buttonExp)
                {
                    writer = new System.IO.StreamWriter(@".\Trials\" + "Button_LeapTab.txt");
                    writer.WriteLine(DateTime.Now.ToString());
                    await Task.Delay(5000);
                    buttonTrial();
                }
                else
                {
                    writer = new System.IO.StreamWriter(@".\Trials\" + "Slider_LeapTab.txt");
                    writer.WriteLine(DateTime.Now.ToString());
                    await Task.Delay(5000);
                    sliderTrial();
                }
                
            }

            
        }
        public async void nextTrial()
        {
            validated = false;
            buttonFocus(-1);
            if (buttonExp)
            {
                writer.WriteLine(" " + t_4);
                writer.Flush();
                if (currTrial < numButtonTrials - 1)
                {
                    buttonGesture = false;

                    nullButton.Select();
                    nullButton.Focus();
                    this.Invoke(new MethodInvoker(delegate { button[trialItem[currTrial]].BackColor = Color.LightGray; }));
                    currTrial++;
                    clicks = 0;
                    this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Please wait..."; }));
                    await Task.Delay(3000);
                    buttonTrial();
                }
                else
                {
                    endExp();
                }
            }
            if (sliderExp)
            {
                writer.WriteLine();
                writer.Flush();
                if (currTrial < numSliderTrials - 1)
                {
                    sliderGesture = false;
                    scrollSelected = false;
                    this.Invoke(new MethodInvoker(delegate { scroll[trialItem[currTrial]].BackColor = Color.LightGray; }));
                    this.Invoke(new MethodInvoker(delegate { scrollLabel[trialItem[currTrial]].BackColor = SystemColors.Control; }));
                    currTrial++;
                    //clicks = 0;
                    this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Please wait..."; }));
                    await Task.Delay(3000);
                    sliderTrial();
                }
                else
                {
                    endExp();
                }
            }
            
        }
        public async void buttonTrial()
        {

            nullButton.Focus();

            nullButton.Select();
            buttonFocus(-1);
            t_0 = 0;
            t_1 = 0;
            t_2 = 0;
            t_2_ = 0;
            t_3 = 0;
            t_4 = 0;
            //await Task.Delay(3000);
            sw.Reset();
            sw.Start();
            testing = true;
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Please select: " + button[trialItem[currTrial]].Name; }));
            this.Invoke(new MethodInvoker(delegate { button[trialItem[currTrial]].BackColor = Color.Thistle; }));
            writer.Write(currTrial + " " + trialItem[currTrial]);
        }
        public async void sliderTrial()
        {
            t_0 = 0;
            t_1 = 0;
            t_2 = 0;
            t_2_ = 0;
            t_3 = 0;
            t_4 = 0;
            sw.Reset();
            sw.Start();
            testing = true;
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Please set " + scroll[trialItem[currTrial]].Name + " to: " + sliderTrialVal[currTrial]; }));
            this.Invoke(new MethodInvoker(delegate { scrollLabel[trialItem[currTrial]].BackColor = Color.Thistle; }));
            writer.Write(currTrial + " " + trialItem[currTrial] + " " + sliderTrialVal[currTrial]);
        }
        public void endExp()
        {
            this.Invoke(new MethodInvoker(delegate { StartButton.Visible = true; }));
            foreach (Button a in button)
            {
                a.BackColor = Color.LightGray;
            }
            foreach (TrackBar a in scroll)
            {
                a.BackColor = Color.LightGray;
                a.Value = 0;
            }
            foreach (Label a in scrollLabel)
            {
                a.BackColor = SystemColors.Control;
            }
            
            writer.Close();
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "End"; }));
        }
        public void OnServiceConnect(object sender, ConnectionEventArgs args)
        {
            Console.WriteLine("Service Connected");
        }

        public void OnConnect(object sender, DeviceEventArgs args)
        {
            Console.WriteLine("Connected");
        }

        public void OnFrame(object sender, FrameEventArgs args)
        {
            //Console.WriteLine("Frame Available.");
            Frame frame = args.frame;

            // Console.WriteLine(
            //  "Frame id: {0}, timestamp: {1}, hands: {2}",
            //   frame.Id, frame.Timestamp, frame.Hands.Count
            //);

            //Console.WriteLine("  Hand id: {0}, palm position: {1}, fingers: {2}",
            //  hand.Id, hand.PalmPosition, hand.Fingers.Count);
            // Get the hand's normal vector and direction

            
            if (frame.Hands.Count == 1)
            {
                //twoHandTab = false;
                right_hand = frame.Hands[0];
                handPosition = right_hand.PalmPosition;
                Vector normal = right_hand.PalmNormal;
                Vector direction = right_hand.Direction;
                indexfinger = right_hand.Fingers[1];
                midfinger = right_hand.Fingers[2];
                Bone midmetacarpal = midfinger.Bone(Bone.BoneType.TYPE_METACARPAL);
                Bone midintermediate = midfinger.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
                float middot = midmetacarpal.Direction.Dot(midintermediate.Direction);
                midflexed = (1.0f - (1.0f + middot) / 2.0f);
                thumbfinger = right_hand.Fingers[0];
                thumbExtended = thumbfinger.IsExtended;
                Bone thumbproximal = thumbfinger.Bone(Bone.BoneType.TYPE_PROXIMAL);
                Bone thumbdistal = thumbfinger.Bone(Bone.BoneType.TYPE_DISTAL);
                float thumbdot = thumbproximal.Direction.Dot(thumbdistal.Direction);
                thumbflexed = (1.0f - (1.0f + thumbdot) / 2.0f);
                
            }

            else if (frame.Hands.Count > 1)
            {
                //twoHandTab = true;

                Hand hand_1 = frame.Hands[0];
                Hand hand_2 = frame.Hands[1];
                if (hand_1.IsRight)
                {
                    right_hand = hand_1;
                    left_hand = hand_2;
                }
                else
                {
                    left_hand = hand_1;
                    right_hand = hand_2;
                }
                Vector normal = right_hand.PalmNormal;
                Vector direction = right_hand.Direction;
                handPosition = right_hand.PalmPosition;
                indexfinger = right_hand.Fingers[1];
                midfinger = right_hand.Fingers[2];
                Bone midmetacarpal = midfinger.Bone(Bone.BoneType.TYPE_METACARPAL);
                Bone midintermediate = midfinger.Bone(Bone.BoneType.TYPE_INTERMEDIATE);
                float middot = midmetacarpal.Direction.Dot(midintermediate.Direction);
                midflexed = (1.0f - (1.0f + middot) / 2.0f);
                thumbfinger = right_hand.Fingers[0];
                thumbExtended = thumbfinger.IsExtended;
                Bone thumbproximal = thumbfinger.Bone(Bone.BoneType.TYPE_PROXIMAL);
                Bone thumbdistal = thumbfinger.Bone(Bone.BoneType.TYPE_DISTAL);
                float thumbdot = thumbproximal.Direction.Dot(thumbdistal.Direction);
                thumbflexed = (1.0f - (1.0f + thumbdot) / 2.0f);
                if (!left_hand.Fingers[0].IsExtended && !left_hand.Fingers[1].IsExtended && !left_hand.Fingers[2].IsExtended && !left_hand.Fingers[3].IsExtended && !left_hand.Fingers[4].IsExtended)
                {
                    left_hand_closed = true;
                }
                else
                {
                    left_hand_closed = false;
                }
            }
            else
            {
                //twoHandTab = false;
            }
            if (frame.Hands.Count > 0 && cursorExp)
            {
                
                if (LeapIsButtonCursor() && testing && !buttonGesture)
                {
                    t_1 = (int)sw.ElapsedMilliseconds;
                    buttonGesture = true;
                }
                if (buttonGesture && testing)
                {
                    xpos = (int)((handPosition.x * LEAP_SENS) + (1536 / 2));
                    ypos = (int)(-((handPosition.y - LEAP_HEIGHT_OFFSET) * LEAP_SENS) + (864 / 2));
                    Cursor.Position = new Point(xpos, ypos);
                    try
                    {
                        if (!indexfinger.IsExtended && !clicked && testing)
                        {
                            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
                            clicked = true;
                        }
                        else if (clicked && indexfinger.IsExtended && testing)
                        {
                            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
                            scrollClicks++;
                            clicked = false;
                        }
                    }
                    catch (System.NullReferenceException e)
                    {

                    }
                }
                if (LeapGestureRelease() && testing && buttonGesture && sliderExp)
                {
                    
                        buttonGesture = false;
                        validate();
                        nextTrial();
                    
                }
                else if (LeapGestureRelease() && validated && buttonExp)
                {
                    t_4 = (int)sw.ElapsedMilliseconds;
                    buttonGesture = false;
                    nextTrial();
                }
            }
            else if (frame.Hands.Count > 0 && posExp)
            {
                //textBox1.Text = handPosition.x.ToString();
                //textBox2.Text = handPosition.y.ToString();
                
                if (buttonExp)
                {
                    if (!buttonGesture && testing && LeapIsButton() && testing)
                    {
                        t_1 = (int)sw.ElapsedMilliseconds;
                        buttonGesture = true;
                    }
                    if (buttonGesture && testing)
                    {
                        //topleft button1
                        if (handPosition.x < 0 && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_THRESH)
                        {
                            button[0].Select();
                            button[0].Focus();
                            buttonFocus(0);
                            if (button[0].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            if (!indexfinger.IsExtended && !clicked)
                            {
                                button[0].PerformClick();
                                clicked = true;
                            }
                            else if (clicked && indexfinger.IsExtended)
                            {

                                clicked = false;
                            }
                        }
                        //midleft button2
                        if (handPosition.x < 0 && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_THRESH && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_THRESH)
                        {
                            button[1].Select();
                            button[1].Focus();
                            buttonFocus(1);
                            if (button[1].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            if (!indexfinger.IsExtended && !clicked)
                            {
                                button[1].PerformClick();
                                clicked = true;
                            }
                            else if (clicked && indexfinger.IsExtended)
                            {

                                clicked = false;
                            }
                        }
                        //botleft button3
                        if (handPosition.x < 0 && handPosition.y - LEAP_HEIGHT_OFFSET < BOT_THRESH)
                        {
                            button[2].Select();
                            button[2].Focus();
                            buttonFocus(2);
                            if (button[2].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            if (!indexfinger.IsExtended && !clicked)
                            {
                                button[2].PerformClick();
                                clicked = true;

                            }
                            else if (clicked && indexfinger.IsExtended)
                            {

                                clicked = false;
                            }
                        }
                        //topright button4
                        if (handPosition.x > 0 && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_THRESH)
                        {
                            button[3].Select();
                            button[3].Focus();
                            buttonFocus(3);
                            if (button[3].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            if (!indexfinger.IsExtended && !clicked)
                            {
                                button[3].PerformClick();
                                clicked = true;
                            }
                            else if (clicked && indexfinger.IsExtended)
                            {

                                clicked = false;
                            }
                        }
                        //midright button5
                        if (handPosition.x > 0 && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_THRESH && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_THRESH)
                        {
                            button[4].Select();
                            button[4].Focus();
                            buttonFocus(4);
                            if (button[4].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            if (!indexfinger.IsExtended && !clicked)
                            {
                                button[4].PerformClick();
                                clicked = true;
                            }
                            else if (clicked && indexfinger.IsExtended)
                            {

                                clicked = false;
                            }
                        }
                        //botright button6
                        if (handPosition.x > 0 && handPosition.y - LEAP_HEIGHT_OFFSET < BOT_THRESH)
                        {
                            button[5].Select();
                            button[5].Focus();
                            buttonFocus(5);
                            if (button[5].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            if (!indexfinger.IsExtended && !clicked)
                            {
                                button[5].PerformClick();
                                clicked = true;
                            }
                            else if (clicked && indexfinger.IsExtended)
                            {

                                clicked = false;
                            }
                        }
                        
                    }
                    if (LeapGestureRelease() && validated)
                    {
                        t_4 = (int)sw.ElapsedMilliseconds;
                        buttonGesture = false;
                        
                        nextTrial();
                    }
                }
                else if (sliderExp)
                { 
                    if (!sliderGesture && LeapIsButton() && testing)
                    {
                        t_1 = (int)sw.ElapsedMilliseconds;
                        sliderGesture = true;
                    }
                    if (sliderGesture)
                    {
                        //top scroll 1
                        if (handPosition.y - LEAP_HEIGHT_OFFSET > TOP_TOP_THRESH && !scrollSelected)
                        {
                            
                            scroll[0].Select();
                            scroll[0].Focus();
                            if (scroll[0].Equals(scroll[trialItem[currTrial]]) && !scrollSelected && !sliderReached)
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                                sliderReached = true;
                            }
                            else if (!scroll[0].Equals(scroll[trialItem[currTrial]]))
                            {
                                sliderReached = false;
                            }
                            
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightSteelBlue; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightGray; }));
                            if (isSliderHeld())
                            {
                                if (scroll[0].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_TOP_THRESH - 10)
                                {
                                    selectedScroll = scroll[0];
                                }
                                else if (scroll[1].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_TOP_THRESH + 10)
                                {
                                    selectedScroll = scroll[1];
                                }
                                else
                                {
                                    selectedScroll = scroll[0];
                                }
                                foreach (TrackBar a in scroll)
                                {
                                    if (!a.Equals(selectedScroll))
                                    {
                                        this.Invoke(new MethodInvoker(delegate { a.BackColor = Color.LightGray; }));
                                    }
                                }
                                scrollSelected = true;
                                if (selectedScroll.Equals(scroll[trialItem[currTrial]]))
                                {
                                    t_3 = (int)sw.ElapsedMilliseconds;
                                }
                            }

                        }
                        //topmid scroll 2
                        if (handPosition.y - LEAP_HEIGHT_OFFSET < TOP_TOP_THRESH && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_THRESH && !scrollSelected)
                        {
                            scroll[1].Select();
                            scroll[1].Focus();
                            if (scroll[1].Equals(scroll[trialItem[currTrial]]) && !scrollSelected && !sliderReached)
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                                sliderReached = true;
                            }
                            else if (!scroll[1].Equals(scroll[trialItem[currTrial]]))
                            {
                                sliderReached = false;
                            }
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightSteelBlue; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightGray; }));

                            if (isSliderHeld())
                            {
                                if (scroll[0].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_TOP_THRESH - 10)
                                {
                                    selectedScroll = scroll[0];
                                }
                                else if (scroll[1].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_TOP_THRESH + 10 && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_THRESH - 10)
                                {
                                    selectedScroll = scroll[1];
                                }
                                else if (scroll[2].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_THRESH + 10)
                                {
                                    selectedScroll = scroll[2];
                                }
                                else
                                {
                                    selectedScroll = scroll[1];
                                }
                                foreach (TrackBar a in scroll)
                                {
                                    if (!a.Equals(selectedScroll))
                                    {
                                        this.Invoke(new MethodInvoker(delegate { a.BackColor = Color.LightGray; }));
                                    }
                                }
                                scrollSelected = true;
                                if (selectedScroll.Equals(scroll[trialItem[currTrial]]))
                                {
                                    t_3 = (int)sw.ElapsedMilliseconds;
                                }
                            }
                        }
                        //mid scroll 3
                        if (handPosition.y - LEAP_HEIGHT_OFFSET > BOT_THRESH && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_THRESH && !scrollSelected)
                        {
                            scroll[2].Select();
                            scroll[2].Focus();
                            if (scroll[2].Equals(scroll[trialItem[currTrial]]) && !scrollSelected && !sliderReached)
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                                sliderReached = true;
                            }
                            else if (!scroll[2].Equals(scroll[trialItem[currTrial]]))
                            {
                                sliderReached = false;
                            }
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightSteelBlue; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightGray; }));
                            if (isSliderHeld())
                            {
                                if (scroll[1].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET > TOP_THRESH - 10)
                                {
                                    selectedScroll = scroll[1];
                                }
                                else if (scroll[2].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < TOP_THRESH + 10 && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_THRESH - 10)
                                {
                                    selectedScroll = scroll[2];
                                }
                                else if (scroll[3].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < BOT_THRESH + 10)
                                {
                                    selectedScroll = scroll[3];
                                }
                                else
                                {
                                    selectedScroll = scroll[2];
                                }
                                foreach (TrackBar a in scroll)
                                {
                                    if (!a.Equals(selectedScroll))
                                    {
                                        this.Invoke(new MethodInvoker(delegate { a.BackColor = Color.LightGray; }));
                                    }
                                }
                                scrollSelected = true;
                                if (selectedScroll.Equals(scroll[trialItem[currTrial]]))
                                {
                                    t_3 = (int)sw.ElapsedMilliseconds;
                                }
                            }
                        }
                        if (handPosition.y - LEAP_HEIGHT_OFFSET < BOT_THRESH && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_BOT_THRESH && !scrollSelected)
                        {
                            scroll[3].Select();
                            scroll[3].Focus();
                            if (scroll[3].Equals(scroll[trialItem[currTrial]]) && !scrollSelected && !sliderReached)
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                                sliderReached = true;
                            }
                            else if (!scroll[3].Equals(scroll[trialItem[currTrial]]))
                            {
                                sliderReached = false;
                            }
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightSteelBlue; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightGray; }));

                            if (isSliderHeld())
                            {
                                if (scroll[2].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_THRESH - 10)
                                {
                                    selectedScroll = scroll[2];
                                }
                                else if (scroll[3].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < BOT_THRESH + 10 && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_BOT_THRESH - 10)
                                {
                                    selectedScroll = scroll[3];
                                }
                                else if (scroll[4].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < BOT_BOT_THRESH + 10)
                                {
                                    selectedScroll = scroll[4];
                                }
                                else
                                {
                                    selectedScroll = scroll[3];
                                }
                                scrollSelected = true;
                                foreach (TrackBar a in scroll)
                                {
                                    if (!a.Equals(selectedScroll))
                                    {
                                        this.Invoke(new MethodInvoker(delegate { a.BackColor = Color.LightGray; }));
                                    }
                                }
                                if (selectedScroll.Equals(scroll[trialItem[currTrial]]))
                                {
                                    t_3 = (int)sw.ElapsedMilliseconds;
                                }
                            }
                        }
                        if (handPosition.y - LEAP_HEIGHT_OFFSET < BOT_BOT_THRESH && !scrollSelected)
                        {
                            scroll[4].Select();
                            scroll[4].Focus();
                            if (scroll[4].Equals(scroll[trialItem[currTrial]]) && !scrollSelected && !sliderReached)
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                                sliderReached = true;
                            }
                            else if (!scroll[4].Equals(scroll[trialItem[currTrial]]))
                            {
                                sliderReached = false;
                            }
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightSteelBlue; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightGray; }));
                            if (isSliderHeld())
                            {
                                if (scroll[3].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET > BOT_BOT_THRESH - 10)
                                {
                                    selectedScroll = scroll[3];
                                }
                                else if (scroll[4].Equals(scroll[trialItem[currTrial]]) && handPosition.y - LEAP_HEIGHT_OFFSET < BOT_BOT_THRESH + 10)
                                {
                                    selectedScroll = scroll[4];
                                }
                                else
                                {
                                    selectedScroll = scroll[4];
                                }
                                foreach (TrackBar a in scroll)
                                {
                                    if (!a.Equals(selectedScroll))
                                    {
                                        this.Invoke(new MethodInvoker(delegate { a.BackColor = Color.LightGray; }));
                                    }
                                }
                                scrollSelected = true;
                                if (selectedScroll.Equals(scroll[trialItem[currTrial]]))
                                {
                                    t_3 = (int)sw.ElapsedMilliseconds;
                                }
                            }
                        }

                        if (isSliderHeld() && scrollSelected)
                        {

                            this.Invoke(new MethodInvoker(delegate { selectedScroll.BackColor = Color.Thistle; }));
                            if (!initHand)
                            {
                                initHand = true;
                                initHandPos = handPosition;
                                sliderInit = selectedScroll.Value;
                            }
                            slider = sliderInit + (int)((handPosition.x - initHandPos.x) / 10);
                            // slider = (int)handPosition.x / 10;
                            if (slider >= SLIDER_MAX)
                            {
                                slider = SLIDER_MAX;
                            }
                            else if (slider <= SLIDER_MIN)
                            {
                                slider = SLIDER_MIN;
                            }
                            this.Invoke(new MethodInvoker(delegate { selectedScroll.Value = slider; }));

                        }
                        else
                        {
                            initHand = false;
                        }
                        if (LeapGestureRelease() && scrollSelected)
                        {
                            sliderGesture = false;
                            scrollSelected = false;
                            validate();
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightGray; }));
                            selectedScroll = new TrackBar();
                            testing = false;
                            nextTrial();
                        }
                    }
                }
                                
            }
            else if (frame.Hands.Count > 1 && tabExp)
            {
                //currTab = currTab % 6;
                if (buttonExp)
                {
                    if (!buttonGesture && LeapIsButton() && testing)
                    {
                        currTab = 0;
                        t_1 = (int)sw.ElapsedMilliseconds;
                                                    
                        buttonGesture = true;
                        button[currTab].Select();
                        button[currTab].Focus();
                        buttonFocus(currTab);
                    }
                    if (buttonGesture)
                    {
                        if (LeapIsButtonClick() && !clicked)
                        {
                            button[currTab].PerformClick();
                            clicked = true;
                        }
                        else if (!LeapIsButtonClick() && clicked)
                        {
                            clicked = false;
                        }
                        else if (!tabbed && left_hand_closed)
                        {
                            currTab = (currTab + 1) % 6;
                            button[currTab].Focus();
                            button[currTab].Select();
                            buttonFocus(currTab);
                            numTabs++;
                            if (button[currTab].Equals(button[trialItem[currTrial]]))
                            {
                                t_2 = (int) sw.ElapsedMilliseconds;
                            }
                            tabbed = true;
                        }
                        else if (tabbed && !left_hand_closed)
                        {
                            tabbed = false;
                        }
                        else if (LeapGestureRelease() && validated)
                        {
                            buttonGesture = false;
                            t_4 = (int)sw.ElapsedMilliseconds;
                            nextTrial();
                        }
                    }
                }
                else if (sliderExp)
                {
                    if (!sliderGesture && LeapIsButton() && testing)
                    {
                        t_1 = (int)sw.ElapsedMilliseconds;
                        currTab = 0;
                        sliderGesture = true;
                        scroll[currTab].Select();
                        scroll[currTab].Focus();
                        this.Invoke(new MethodInvoker(delegate { scroll[currTab].BackColor = Color.LightSteelBlue; }));
                    }
                    if (sliderGesture)
                    {
                        if (!scrollSelected && isSliderHeld())
                        {
                            scrollSelected = true;
                            selectedScroll = scroll[currTab];
                            if (selectedScroll.Equals(scroll[trialItem[currTrial]]))
                            {
                                t_3 = (int)sw.ElapsedMilliseconds;
                            }
                        }
                        else if (!tabbed && left_hand_closed && !scrollSelected)
                        {
                            this.Invoke(new MethodInvoker(delegate { scroll[currTab].BackColor = Color.LightGray; }));
                            currTab = (currTab + 1) % 5;
                            scroll[currTab].Focus();
                            scroll[currTab].Select();
                            if (scroll[currTab].Equals(scroll[trialItem[currTrial]]))
                            {
                                t_2 = (int)sw.ElapsedMilliseconds;
                            }
                            this.Invoke(new MethodInvoker(delegate { scroll[currTab].BackColor = Color.LightSteelBlue; }));
                            numTabs++;
                            tabbed = true;
                        }
                        else if (tabbed && !left_hand_closed && !scrollSelected)
                        {
                            tabbed = false;
                        }
                        if (isSliderHeld() && scrollSelected)
                        {

                            this.Invoke(new MethodInvoker(delegate { selectedScroll.BackColor = Color.Thistle; }));
                            if (!initHand)
                            {
                                initHand = true;
                                initHandPos = handPosition;
                                sliderInit = selectedScroll.Value;
                            }
                            slider = sliderInit + (int)((handPosition.x - initHandPos.x) / 10);
                            // slider = (int)handPosition.x / 10;
                            if (slider >= SLIDER_MAX)
                            {
                                slider = SLIDER_MAX;
                            }
                            else if (slider <= SLIDER_MIN)
                            {
                                slider = SLIDER_MIN;
                            }
                            this.Invoke(new MethodInvoker(delegate { selectedScroll.Value = slider; }));

                        }
                        else
                        {
                            initHand = false;
                        }
                        if (LeapGestureRelease() && scrollSelected)
                        {
                            sliderGesture = false;
                            scrollSelected = false;
                            validate();
                            this.Invoke(new MethodInvoker(delegate { scroll[0].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[1].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[2].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[3].BackColor = Color.LightGray; }));
                            this.Invoke(new MethodInvoker(delegate { scroll[4].BackColor = Color.LightGray; }));
                            selectedScroll = new TrackBar();
                            testing = false;
                            nextTrial();

                        }
                    }
                }
            }
            //Console.WriteLine(thumbfinger.TipPosition.DistanceTo(indexfinger.TipPosition));
            // Calculate the hand's pitch, roll, and yaw angles
            //Console.WriteLine(
            //  "  Hand pitch: {0} degrees, roll: {1} degrees, yaw: {2} degrees",
            //  direction.Pitch * 180.0f / (float)Math.PI,
            //  normal.Roll * 180.0f / (float)Math.PI,
            // direction.Yaw * 180.0f / (float)Math.PI
            // );
        }
        public async void startMsgs()
        {
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Hello!"; }));
            await Task.Delay(2000);
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Trials begin in: 3"; }));
            await Task.Delay(1000);
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Trials begin in: 2"; }));
            await Task.Delay(1000);
            this.Invoke(new MethodInvoker(delegate { CommandBox.Text = "Trials begin in: 1"; }));
            await Task.Delay(1000);
        }

        private void LeapPosition_Load(object sender, EventArgs e)
        {

        }
        public bool LeapIsButtonClick()
        {
            return !right_hand.Fingers[1].IsExtended && !right_hand.Fingers[2].IsExtended && !right_hand.Fingers[3].IsExtended && !right_hand.Fingers[4].IsExtended;
        }
        public bool isSliderHeld()
        {
            return !right_hand.Fingers[1].IsExtended && !right_hand.Fingers[0].IsExtended;
        }
        public bool LeapGestureRelease()
        {
            return right_hand.Fingers[0].IsExtended && right_hand.Fingers[1].IsExtended && right_hand.Fingers[2].IsExtended && right_hand.Fingers[3].IsExtended && right_hand.Fingers[4].IsExtended;
        }
        public bool LeapIsButtonCursor()
        {
            return !right_hand.Fingers[2].IsExtended && !right_hand.Fingers[3].IsExtended && !right_hand.Fingers[4].IsExtended;
        }
        public bool LeapIsButton()
        {
            return right_hand.Fingers[1].IsExtended && !right_hand.Fingers[2].IsExtended && !right_hand.Fingers[3].IsExtended && !right_hand.Fingers[4].IsExtended;
        }

        private void button1Label_Click(object sender, EventArgs e)
        {

        }
        public void buttonFocus(int a)
        {
            if (a == -1)
            {
                foreach(Label butt in buttonLabel)
                {
                    butt.Visible = false;
                }
            }
            for (int i = 0; i < buttonLabel.Length; i++)
            {
                if (i == a)
                {
                    buttonLabel[i].Visible = true;
                }
                else
                {
                    buttonLabel[i].Visible = false;
                }
            }
        }
    }
}
