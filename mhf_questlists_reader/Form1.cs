using System.Text;
using Dictionary;

namespace mhf_questlists_reader
{
    public partial class Form1 : Form
    {
        string fileHeader = "";
        string fileQuest = "";
        string fileEnd = "";

        int questCount0;
        int questCount1;
        int questCount2;
        int questCount3;
        int questCount4;
        int questCount5;


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
            DialogResult drfolder = folderBrowserDialog1.ShowDialog();
            if (drfolder == DialogResult.OK)
            {
                label39.Text = "Loading questlist data...";
                listBox1.Items.Clear();
                listBox2.Items.Clear();

                string[] fileNames = Directory.GetFiles(folderBrowserDialog1.SelectedPath).Select(Path.GetFileName).ToArray();     //{"list_0.bin", "list_42.bin",}
                if (fileNames.Contains("list_0.bin"))       //0
                {
                    listBox2.Items.Add("list_0.bin");
                    byte[] byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/"+"list_0.bin");
                    CreateStoredData(byteData, 0);

                    questCount0 = byteData[1];
                    string nextFileName = "list_" + questCount0.ToString() + ".bin";
                    if (fileNames.Contains(nextFileName))       //42
                    {
                        listBox2.Items.Add(nextFileName);
                        byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                        CreateStoredData(byteData, 1);

                        questCount1 = byteData[1];
                        nextFileName = "list_" + (questCount0 + questCount1).ToString() + ".bin";
                        if (fileNames.Contains(nextFileName))       //84
                        {
                            listBox2.Items.Add(nextFileName);
                            byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                            CreateStoredData(byteData, 2);

                            questCount2 = byteData[1];
                            nextFileName = "list_" + (questCount0 + questCount1 + questCount2).ToString() + ".bin";
                            if (fileNames.Contains(nextFileName))       //126
                            {
                                listBox2.Items.Add(nextFileName);
                                byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                                CreateStoredData(byteData, 3);

                                questCount3 = byteData[1];
                                nextFileName = "list_" + (questCount0 + questCount1 + questCount2 + questCount3).ToString() + ".bin";
                                if (fileNames.Contains(nextFileName))       //168
                                {
                                    listBox2.Items.Add(nextFileName);
                                    byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                                    CreateStoredData(byteData, 4);

                                    questCount4 = byteData[1];
                                    nextFileName = "list_" + (questCount0 + questCount1 + questCount2 + questCount3 + questCount4).ToString() + ".bin";
                                    if (fileNames.Contains(nextFileName))       //No.6
                                    {
                                        listBox2.Items.Add(nextFileName);
                                        byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                                        CreateStoredData(byteData, 5);

                                        questCount5 = byteData[1];
                                    }
                                }
                            }
                        }
                    }

                }
                label39.Text = "Load completed";
            }
        }

        private  void CreateStoredData(byte[] byteData, int count)
        {
            int QuestCount = byteData[1];
            int prevPointer = 8;
            int prevPointerEndOfText = 0;
            byte[] questEndData;

            fileHeader = "Stored_Data/" + count.ToString() + "/stored_header_data.txt";
            fileQuest = "Stored_Data/" + count.ToString() + "/stored_quest_data.txt";
            fileEnd = "Stored_Data/" + count.ToString() + "/stored_end_data.txt";

            for (int i = 0; i < QuestCount; i++)
            {
                byte[] questHeaderData = byteData.Skip(prevPointer).Take(16).ToArray();
                int questLength = questHeaderData[14] * 256 + questHeaderData[15];
                byte[] questData = byteData.Skip(prevPointer + 16).Take(questLength).ToArray();

                lineChanger(BitConverter.ToString(questHeaderData).Replace("-", string.Empty), fileHeader, i);
                lineChanger(BitConverter.ToString(questData).Replace("-", string.Empty), fileQuest, i);

                int t1 = 0;
                int t2 = 0;
                if (i == QuestCount - 1)
                {
                    int endByteVal = byteData[prevPointer + 16 + questLength];
                    questEndData = byteData.Skip(prevPointer + 16 + questLength + 1).Take(endByteVal).ToArray();
                    lineChanger(BitConverter.ToString(questEndData).Replace("-", string.Empty), fileEnd, i);
                }
                else
                {
                    for (int t = 1; t < 250; t++)
                    {
                        int singleByte = byteData[prevPointer + 16 + questLength + t];
                        if (singleByte == 64)
                        {
                            if (byteData[prevPointer + 16 + questLength + t + 1] == 1)
                            {
                                t1 = prevPointer + 16 + questLength + t - 56;
                                t2 = prevPointer + 16 + questLength;
                                break;
                            }
                        }
                    }
                    prevPointer = t1;
                    prevPointerEndOfText = t2;
                    questEndData = byteData.Skip(prevPointerEndOfText).Take(prevPointer - prevPointerEndOfText).ToArray();
                    lineChanger(BitConverter.ToString(questEndData).Replace("-", string.Empty), fileEnd, i);
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            int amount = 1;

            int index = listBox2.SelectedIndex;
            fileHeader = "Stored_Data/" + index.ToString() + "/stored_header_data.txt";
            fileQuest = "Stored_Data/" + index.ToString() + "/stored_quest_data.txt";
            fileEnd = "Stored_Data/" + index.ToString() + "/stored_end_data.txt";

            switch (index)
            {
                case 0:
                    amount = questCount0;
                    break;
                case 1:
                    amount = questCount1;
                    break;
                case 2:
                    amount = questCount2;
                    break;
                case 3:
                    amount = questCount3;
                    break;
                case 4:
                    amount = questCount4;
                    break;
                case 5:
                    amount = questCount5;
                    break;
            }
            numQuestCount.Value = amount;

            for (int i = 0; i < amount; i++)
            {
                string strData = File.ReadLines(fileQuest).ElementAt(i);
                var listData = new List<byte>();
                for (int r = 0; r < strData.Length / 2; r++)
                {
                    listData.Add(Convert.ToByte(strData.Substring(r * 2, 2), 16));
                }
                byte[] data = listData.ToArray();

                int pTitleAndName = BitConverter.ToInt32(data, 320);
                int pMainoObj = BitConverter.ToInt32(data, 324);
                string tTitleAndName = Encoding.GetEncoding("Shift_JIS").GetString(data.Skip(pTitleAndName).Take(pMainoObj - pTitleAndName).ToArray()).Replace("\n", "\r\n");
                listBox1.Items.Add(tTitleAndName);
            }
        }
    }
}