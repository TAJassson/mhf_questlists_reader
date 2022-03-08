using System.Text;
using Dictionary;

namespace mhf_questlists_reader
{
    public partial class Form1 : Form
    {
        string fileHeader = "stored_header_data.txt";
        string fileQuest = "stored_quest_data.txt";

        public Form1()
        {
            InitializeComponent();
        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                listBox1.Items.Clear();

                string fileloc = openFileDialog1.FileName;
                byte[] bydata = File.ReadAllBytes(fileloc);

                int QuestCount = bydata[1];
                numQuestCount.Value = QuestCount;

                int prevPointer = 8;
                for (int i = 0; i < QuestCount; i++)
                {
                    byte[] questHeaderData0 = bydata.Skip(prevPointer).Take(16).ToArray();
                    int quest1Length = questHeaderData0[14] * 256 + questHeaderData0[15];
                    byte[] questData0 = bydata.Skip(prevPointer + 16).Take(quest1Length).ToArray();

                    lineChanger(BitConverter.ToString(questHeaderData0).Replace("-", string.Empty), fileHeader, i);
                    lineChanger(BitConverter.ToString(questData0).Replace("-", string.Empty), fileQuest, i);

                    int pTitleAndName = BitConverter.ToInt32(questData0, 320);
                    int pMainoObj = BitConverter.ToInt32(questData0, 324);

                    string tTitleAndName = Encoding.GetEncoding("Shift_JIS").GetString(questData0.Skip(pTitleAndName).Take(pMainoObj - pTitleAndName).ToArray()).Replace("\n", "\r\n");
                    listBox1.Items.Add(tTitleAndName);

                    //クエストの読み込みが一番最後のクエストまで達していないなら
                    //クエストデータの最後から一バイトずつ読み取り
                    //0が２回出てきたらforを中断
                    //末尾の最期をpointerとして返す　
                    //int prevNum = 0;
                    //bool zeroAppered = false;
                    //if (i == QuestCount - 1)
                    //{

                    //}
                    //else
                    //{
                    //    int t1 = 0;     //末尾から読み取った回数
                    //    for (int t = 0; t < 100; t++)
                    //    {
                    //        int singleByte = bydata[prevPointer + 16 + quest1Length + t];
                    //        if (singleByte == prevNum & singleByte == 0)
                    //        {
                    //            t1 = t + 1;
                    //            break;
                    //        }
                    //        prevNum = singleByte;
                    //    }
                    //    prevPointer = prevPointer + 16 + quest1Length + t1;
                    //}

                    int t1 = 0;
                    if (i == QuestCount - 1)
                    {

                    }
                    else
                    {
                        for (int t = 1; t < 250; t++)
                        {
                            int singleByte = bydata[prevPointer + 16 + quest1Length + t];           //末尾の最初から
                            if (singleByte == 64)
                            {
                                if (bydata[prevPointer + 16 + quest1Length + t+1] == 1)
                                {
                                    t1 = prevPointer + 16 + quest1Length + t - 56;
                                    break;
                                }
                            }
                        }
                        prevPointer = t1;
                    }
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strHeader = File.ReadLines(fileHeader).ElementAt(listBox1.SelectedIndex);
            var listHeader = new List<byte>();
            for (int i = 0; i < strHeader.Length / 2; i++)
            {
                listHeader.Add(Convert.ToByte(strHeader.Substring(i * 2, 2), 16));
            }

            string strData = File.ReadLines(fileQuest).ElementAt(listBox1.SelectedIndex);
            var listData = new List<byte>();
            for (int i = 0; i < strData.Length / 2; i++)
            {
                listData.Add(Convert.ToByte(strData.Substring(i * 2, 2), 16));
            }
            byte[] byteData = listData.ToArray();

            //Header
            comMark.SelectedIndex = listHeader[11];
            comQuestType.SelectedIndex = listHeader[4];
            numPlayers.Value = listHeader[3];

            //Target
            List.ObjectiveType.TryGetValue(BitConverter.ToInt32(byteData, 48), out string targetOM);
            List.ObjectiveType.TryGetValue(BitConverter.ToInt32(byteData, 56), out string targetOA);
            List.ObjectiveType.TryGetValue(BitConverter.ToInt32(byteData, 64), out string targetOB);
            textTypeM.Text = targetOM;
            textTypeA.Text = targetOA;
            textTypeB.Text = targetOB;

            int targetM = BitConverter.ToUInt16(byteData, 52);
            int targetA = BitConverter.ToInt16(byteData, 60);
            int targetB = BitConverter.ToInt16(byteData, 68);

            if (targetM > 176)
            {
                comTargetM.SelectedIndex = 177;
            }
            else
            {
                comTargetM.SelectedIndex = targetM;
            }

            if (targetA > 176)
            {
                comTargetA.SelectedIndex = 177;
            }
            else
            {
                comTargetA.SelectedIndex = targetA;
            }

            if (targetB > 176)
            {
                comTargetB.SelectedIndex = 177;
            }
            else
            {
                comTargetB.SelectedIndex = targetB;
            }

            numQuantityM.Value = BitConverter.ToInt16(byteData, 54);
            numQuantityA.Value = BitConverter.ToInt16(byteData, 62);
            numQuantityB.Value = BitConverter.ToInt16(byteData, 70);

            List.MonsterID.TryGetValue(byteData[185], out string Icon1);
            List.MonsterID.TryGetValue(byteData[186], out string Icon2);
            List.MonsterID.TryGetValue(byteData[187], out string Icon3);
            List.MonsterID.TryGetValue(byteData[188], out string Icon4);
            List.MonsterID.TryGetValue(byteData[189], out string Icon5);

            textMonsterIcon1.Text = Icon1;
            textMonsterIcon2.Text = Icon2;
            textMonsterIcon3.Text = Icon3;
            textMonsterIcon4.Text = Icon4;
            textMonsterIcon5.Text = Icon5;



            //Text
            int pTitleAndName = BitConverter.ToInt16(byteData, 320);
            int pMainoObj = BitConverter.ToInt16(byteData, 324);
            int pAObj = BitConverter.ToInt16(byteData, 328);
            int pBObj = BitConverter.ToInt16(byteData, 332);
            int pClearC = BitConverter.ToInt16(byteData, 336);
            int pFailC = BitConverter.ToInt16(byteData, 340);
            int pEmp = BitConverter.ToInt16(byteData, 344);
            int pText = BitConverter.ToInt16(byteData, 348);

            string tTitleAndName = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pTitleAndName).Take(pMainoObj - pTitleAndName).ToArray()).Replace("\n", "\r\n");
            textTitle.Text = tTitleAndName.ToString();

            string tMainObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pMainoObj).Take(pAObj - pMainoObj).ToArray()).Replace("\n", "\r\n");
            textMain.Text = tMainObj.ToString();

            string tAObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pAObj).Take(pBObj - pAObj).ToArray()).Replace("\n", "\r\n");
            textA.Text = tAObj.ToString();

            string tBObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pBObj).Take(pClearC - pBObj).ToArray()).Replace("\n", "\r\n");
            textB.Text = tBObj.ToString();

            string tClearC = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pClearC).Take(pFailC - pClearC).ToArray()).Replace("\n", "\r\n");
            textClear.Text = tClearC.ToString();

            string tFailC = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pFailC).Take(pEmp - pFailC).ToArray()).Replace("\n", "\r\n");
            textFail.Text = tFailC.ToString();

            string tEmp = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pEmp).Take(pText - pEmp).ToArray()).Replace("\n", "\r\n");
            textEmp.Text = tEmp.ToString();

            string tText = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pText).Take(byteData.Length - pText).ToArray()).Replace("\n", "\r\n");
            textText.Text = tText.ToString();

            //Misc
            numDifficulty.Value = listData[4];
            numQuestID.Value = BitConverter.ToUInt16(byteData, 46);
            numFee.Value = BitConverter.ToInt16(byteData, 12);
            numMR.Value = BitConverter.ToInt32(byteData, 16);
            numAR.Value = BitConverter.ToInt32(byteData, 24);
            numBR.Value = BitConverter.ToInt32(byteData, 28);
            numMP.Value = BitConverter.ToInt32(byteData, 164);
            numAP.Value = BitConverter.ToInt32(byteData, 168);
            numBP.Value = BitConverter.ToInt32(byteData, 172);
            numReqHR.Value = BitConverter.ToInt32(byteData, 74);
            numReqHR2.Value = BitConverter.ToInt32(byteData, 78);
            List.ItemID.TryGetValue(BitConverter.ToInt16(byteData, 176), out string Item1);
            List.ItemID.TryGetValue(BitConverter.ToInt16(byteData, 178), out string Item2);
            List.ItemID.TryGetValue(BitConverter.ToInt16(byteData, 180), out string Item3);
            textItem1.Text = Item1;
            textItem2.Text = Item2;
            textItem3.Text = Item3;
            comCourse.SelectedIndex = listData[6];
            numTime.Value = BitConverter.ToInt32(byteData, 32) / 30;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(265, 25);
        }
    }
}