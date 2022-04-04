using System.Text;
using System.Linq;
using Dictionary;

namespace mhf_questlists_reader
{
    public partial class Form1 : Form
    {
        string fileHeader = "";
        string fileQuest = "";
        string fileEnd = "";
        string fileListHeader = "";
        string fileListEnd = "";
        int questCount0 = 0;
        int questCount1 = 0;
        int questCount2 = 0;
        int questCount3 = 0;
        int questCount4 = 0;
        int questCount5 = 0;
        int LoaddedFilesNum;
        bool isLoaded = false;
        bool isPending = true;
        int deleteQuestNo;

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
            listBox1.Items.Clear();
            listBox2.Items.Clear();
            listBox1.ClearSelected();
            listBox2.ClearSelected();

            string loca = File.ReadLines("Stored_Data/misc.txt").ElementAt(0);
            if (loca != null)
            {
                folderBrowserDialog1.SelectedPath = loca;
            }

            DialogResult drfolder = folderBrowserDialog1.ShowDialog();
            if (drfolder == DialogResult.OK)
            {
                string loc = folderBrowserDialog1.SelectedPath;
                lineChanger(loc, "Stored_Data/misc.txt", 0);

                string[] fileNames = Directory.GetFiles(folderBrowserDialog1.SelectedPath).Select(Path.GetFileName).ToArray();     //{"list_0.bin", "list_42.bin",}
                if (fileNames.Contains("list_0.bin"))       //0
                {
                    listBox2.Items.Add("list_0.bin");
                    byte[] byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/"+"list_0.bin");
                    CreateStoredData(byteData, 0);
                    LoaddedFilesNum = 1;

                    questCount0 = byteData[1];
                    string nextFileName = "list_" + questCount0.ToString() + ".bin";
                    if (fileNames.Contains(nextFileName))       //42
                    {
                        listBox2.Items.Add(nextFileName);
                        byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                        CreateStoredData(byteData, 1);
                        LoaddedFilesNum = 2;

                        questCount1 = byteData[1];
                        nextFileName = "list_" + (questCount0 + questCount1).ToString() + ".bin";
                        if (fileNames.Contains(nextFileName))       //84
                        {
                            listBox2.Items.Add(nextFileName);
                            byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                            CreateStoredData(byteData, 2);
                            LoaddedFilesNum = 3;

                            questCount2 = byteData[1];
                            nextFileName = "list_" + (questCount0 + questCount1 + questCount2).ToString() + ".bin";
                            if (fileNames.Contains(nextFileName))       //126
                            {
                                listBox2.Items.Add(nextFileName);
                                byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                                CreateStoredData(byteData, 3);
                                LoaddedFilesNum = 4;

                                questCount3 = byteData[1];
                                nextFileName = "list_" + (questCount0 + questCount1 + questCount2 + questCount3).ToString() + ".bin";
                                if (fileNames.Contains(nextFileName))       //168
                                {
                                    listBox2.Items.Add(nextFileName);
                                    byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                                    CreateStoredData(byteData, 4);
                                    LoaddedFilesNum = 5;

                                    questCount4 = byteData[1];
                                    nextFileName = "list_" + (questCount0 + questCount1 + questCount2 + questCount3 + questCount4).ToString() + ".bin";
                                    if (fileNames.Contains(nextFileName))       //No.6
                                    {
                                        listBox2.Items.Add(nextFileName);
                                        byteData = File.ReadAllBytes(folderBrowserDialog1.SelectedPath + "/" + nextFileName);
                                        CreateStoredData(byteData, 5);
                                        LoaddedFilesNum = 6;

                                        questCount5 = byteData[1];
                                    }
                                }
                            }
                        }
                    }
                    ManageLogs("Load completed.");
                    isLoaded = true;
                    listBox2.SelectedIndex = 0;
                    numTotal.Value = questCount0 + questCount1 + questCount2 + questCount3 + questCount4 + questCount5;
                }
                else
                {
                    MessageBox.Show("Could not find proper questlist files.");
                }

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
            fileListEnd = "Stored_Data/" + count.ToString() + "/stored_listend_data.txt";
            fileListHeader = "Stored_Data/" + count.ToString() + "/stored_listheader_data.txt";

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
                    questEndData = byteData.Skip(prevPointer + 16 + questLength).Take(endByteVal + 1).ToArray();
                    lineChanger(BitConverter.ToString(questEndData).Replace("-", string.Empty), fileEnd, i);

                    byte[] listEndData = byteData.Skip(prevPointer + 16 + questLength + 1 + endByteVal).Take(byteData.Length).ToArray();
                    lineChanger(BitConverter.ToString(listEndData).Replace("-", string.Empty), fileListEnd, 0);

                    byte[] listHeaderData = byteData.Skip(0).Take(8).ToArray();
                    lineChanger(BitConverter.ToString(listHeaderData).Replace("-", string.Empty), fileListHeader, 0);
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

        void SelectedQuestChanged()
        {
            if (isPending)
            {
                if (listBox1.Items.Count == 0)
                {
                    listBox1.ClearSelected();
                }
                else
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
                    textTitle.Text = tTitleAndName;

                    string tMainObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pMainoObj).Take(pAObj - pMainoObj).ToArray()).Replace("\n", "\r\n");
                    textMain.Text = tMainObj;


                    if (pAObj == pBObj)
                    {
                        string tAObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pAObj).Take(pClearC - pAObj).ToArray()).Replace("\n", "\r\n");
                        textA.Text = tAObj;
                        textB.Text = tAObj;
                    }
                    else
                    {
                        string tAObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pAObj).Take(pBObj - pAObj).ToArray()).Replace("\n", "\r\n");
                        textA.Text = tAObj;

                        string tBObj = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pBObj).Take(pClearC - pBObj).ToArray()).Replace("\n", "\r\n");
                        textB.Text = tBObj;
                    }


                    string tClearC = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pClearC).Take(pFailC - pClearC).ToArray()).Replace("\n", "\r\n");
                    textClear.Text = tClearC;

                    string tFailC = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pFailC).Take(pEmp - pFailC).ToArray()).Replace("\n", "\r\n");
                    textFail.Text = tFailC;

                    string tEmp = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pEmp).Take(pText - pEmp).ToArray()).Replace("\n", "\r\n");
                    textEmp.Text = tEmp;

                    string tText = Encoding.GetEncoding("Shift_JIS").GetString(listData.Skip(pText).Take(byteData.Length - pText).ToArray()).Replace("\n", "\r\n");
                    textText.Text = tText;

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
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isPending)
            {
                SelectedQuestChanged();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = new Point(265, 25);
            ManageLogs("Hit Open button to select and open questlist folder.");
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                listBox2.SelectedIndex = 0;
            }

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
            if(listBox1.Items.Count == 0)
            {
                listBox1.ClearSelected();
            }
            else
            {
                listBox1.SelectedIndex = 0;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveAllFiles();
        }

        void SaveAllFiles()
        {
            if (isLoaded)
            {
                string loca = File.ReadLines("Stored_Data/misc.txt").ElementAt(2);
                if (loca != null)
                {
                    folderBrowserDialog1.SelectedPath = loca;
                }

                DialogResult result = folderBrowserDialog1.ShowDialog();
                string savePath = "";
                string folderName = "";
                if (result == DialogResult.OK)
                {
                    string path = folderBrowserDialog1.SelectedPath;
                    lineChanger(path, "Stored_Data/misc.txt", 2);

                    int questNum = 0;
                    int file = 0;
                    for (int i = 0; i < LoaddedFilesNum; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                questNum = questCount0;
                                file = 0;
                                break;
                            case 1:
                                questNum = questCount1;
                                file = questCount0;
                                break;
                            case 2:
                                questNum = questCount2;
                                file = questCount1 + questCount0;
                                break;
                            case 3:
                                questNum = questCount3;
                                file = questCount2 + questCount1 + questCount0;
                                break;
                            case 4:
                                questNum = questCount4;
                                file = questCount3 + questCount2 + questCount1 + questCount0;
                                break;
                            case 5:
                                questNum = questCount5;
                                file = questCount4 + questCount3 + questCount2 + questCount1 + questCount0;
                                break;
                        }

                        folderName = folderBrowserDialog1.SelectedPath;
                        string fileName = "list_" + file.ToString() + ".bin";
                        savePath = folderName + "/" + fileName;

                        fileHeader = "Stored_Data/" + i.ToString() + "/stored_header_data.txt";
                        fileQuest = "Stored_Data/" + i.ToString() + "/stored_quest_data.txt";
                        fileEnd = "Stored_Data/" + i.ToString() + "/stored_end_data.txt";
                        fileListEnd = "Stored_Data/" + i.ToString() + "/stored_listend_data.txt";
                        fileListHeader = "Stored_Data/" + i.ToString() + "/stored_listheader_data.txt";

                        var data = new List<byte>();
                        string header = File.ReadLines(fileListHeader).ElementAt(0);
                        var header1 = new List<byte>();
                        for (int t = 0; t < header.Length / 2; t++)
                        {
                            header1.Add(Convert.ToByte(header.Substring(t * 2, 2), 16));
                        }
                        header1[1] = Convert.ToByte(questNum);
                        data.AddRange(header1);

                        if (questNum == 0)
                        {

                        }
                        else
                        {
                            for (int y = 0; y < questNum; y++)
                            {
                                string questHeader = File.ReadLines(fileHeader).ElementAt(y);
                                var questHeader1 = new List<byte>();
                                for (int t = 0; t < questHeader.Length / 2; t++)
                                {
                                    questHeader1.Add(Convert.ToByte(questHeader.Substring(t * 2, 2), 16));
                                }
                                data.AddRange(questHeader1);

                                string questData = File.ReadLines(fileQuest).ElementAt(y);
                                var questData1 = new List<byte>();
                                for (int t = 0; t < questData.Length / 2; t++)
                                {
                                    questData1.Add(Convert.ToByte(questData.Substring(t * 2, 2), 16));
                                }
                                data.AddRange(questData1);

                                var endData1 = new List<byte>();
                                if (y == questNum - 1)
                                {
                                    byte[] by = { 08, 151, 172, 139, 226, 142, 169, 141, 221 };
                                    endData1 = by.ToList();
                                }
                                else
                                {
                                    string endData = File.ReadLines(fileEnd).ElementAt(y);
                                    for (int t = 0; t < endData.Length / 2; t++)
                                    {
                                        endData1.Add(Convert.ToByte(endData.Substring(t * 2, 2), 16));
                                    }
                                }
                                data.AddRange(endData1);
                            }
                        }

                        string end = File.ReadLines(fileListEnd).ElementAt(0);
                        var end1 = new List<byte>();
                        for (int t = 0; t < end.Length / 2; t++)
                        {
                            end1.Add(Convert.ToByte(end.Substring(t * 2, 2), 16));
                        }
                        data.AddRange(end1);

                        File.WriteAllBytes(savePath, data.ToArray());
                    }
                    ManageLogs($"List files were successfully exported to {folderName}.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                isPending = false;
                deleteQuestNo = listBox1.SelectedIndex;

                string[] lines = File.ReadAllLines(fileHeader);
                List<string> lst = lines.ToList();
                lst.RemoveAt(deleteQuestNo);
                string[] newline = { "" };
                lst.AddRange(newline);
                File.WriteAllLines(fileHeader, lst);

                string[] lines1 = File.ReadAllLines(fileQuest);
                List<string> lst1 = lines1.ToList();
                lst1.RemoveAt(deleteQuestNo);
                lst1.AddRange(newline);
                File.WriteAllLines(fileQuest, lst1);

                string[] lines2 = File.ReadAllLines(fileEnd);
                List<string> lst2 = lines2.ToList();
                lst2.RemoveAt(deleteQuestNo);
                lst2.AddRange(newline);
                File.WriteAllLines(fileEnd, lst2);

                listBox1.Items.RemoveAt(deleteQuestNo);

                int count = 0;
                switch (listBox2.SelectedIndex)
                {
                    case 0:
                        questCount0 = questCount0 - 1;
                        count = questCount0;
                        break;
                    case 1:
                        questCount1 = questCount1 - 1;
                        count = questCount1;
                        break;
                    case 2:
                        questCount2 = questCount2 - 1;
                        count = questCount2;
                        break;
                    case 3:
                        questCount3 = questCount3 - 1;
                        count = questCount3;
                        break;
                    case 4:
                        questCount4 = questCount4 - 1;
                        count = questCount4;
                        break;
                    case 5:
                        questCount5 = questCount5 - 1;
                        count = questCount5;
                        break;
                }
                numQuestCount.Value = count;
                numTotal.Value = numTotal.Value - 1;
                //SelectedQuestChanged();
                ManageLogs($"Memoved a quest from list. Current count is {count}.");

                //when there's 0 quest
                if (listBox1.Items.Count == 0)
                {
                    listBox1.ClearSelected();
                }
                else
                {
                    if (deleteQuestNo == 0)
                    {
                        listBox1.SelectedIndex = 0;
                    }
                    else
                    {
                        listBox1.SelectedIndex = deleteQuestNo - 1;
                    }
                }

                isPending = true;
            }
            else
            {
                listBox1.ClearSelected();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int limit = LoaddedFilesNum * 42;
            int count = 0;
            switch (listBox2.SelectedIndex)
            {
                case 0:
                    count = questCount0;
                    questCount0 = questCount0 + 1;
                    break;
                case 1:
                    count = questCount1;
                    questCount1 = questCount1 + 1;
                    break;
                case 2:
                    count = questCount2;
                    questCount2 = questCount2 + 1;
                    break;
                case 3:
                    count = questCount3;
                    questCount3 = questCount3 + 1;
                    break;
                case 4:
                    count = questCount4;
                    questCount4 = questCount4 + 1;
                    break;
                case 5:
                    count = questCount5;
                    questCount5 = questCount5 + 1;
                    break;
            }

            if (isLoaded)
            {
                if (LoaddedFilesNum != limit & count != 42)
                {
                    string loca = File.ReadLines("Stored_Data/misc.txt").ElementAt(1);
                    if (loca != null)
                    {
                        openFileDialog1.InitialDirectory = loca;
                    }

                    DialogResult dr = openFileDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string path = openFileDialog1.FileName;
                        string directoryPath = Path.GetDirectoryName(path);
                        lineChanger(directoryPath, "Stored_Data/misc.txt", 1);
                        byte[] by = File.ReadAllBytes(openFileDialog1.FileName);

                        var by1 = new List<byte>();
                        if (by[0] == 192)
                        {
                            byte[] data = by.Skip(192).Take(320).ToArray();
                            by1.AddRange(data);     //140

                            int textPointer = BitConverter.ToInt16(by, 48);     //AE4
                            int textPointer1 = BitConverter.ToInt16(by, textPointer);
                            textPointer1 = textPointer1 - 32;
                            byte[] pointerData = by.Skip(textPointer1).Take(32).ToArray();
                            by1.AddRange(pointerData);  //160
                            int leng = by1.Count;

                            by1[40] = 64;
                            by1[41] = 1;

                            int pointer = BitConverter.ToInt16(by, textPointer + 4);
                            int val = by[pointer];
                            if (val == 0)
                            {
                                pointer = pointer + 1;
                            }

                            int pTitleAndName = BitConverter.ToInt16(by, textPointer1);
                            int pMainoObj = BitConverter.ToInt16(by, textPointer1 + 4);
                            int pAObj = BitConverter.ToInt16(by, textPointer1 + 8);
                            int pBObj = BitConverter.ToInt16(by, textPointer1 + 12);
                            int pClearC = BitConverter.ToInt16(by, textPointer1 + 16);
                            int pFailC = BitConverter.ToInt16(by, textPointer1 + 20);
                            int pEmp = BitConverter.ToInt16(by, textPointer1 + 24);
                            int pText = BitConverter.ToInt16(by, textPointer1 + 28);

                            int a1 = by.Skip(pTitleAndName).Take(pMainoObj - pTitleAndName).ToArray().Length;
                            int a2 = by.Skip(pMainoObj).Take(pBObj - pMainoObj).ToArray().Length;
                            int a3 = by.Skip(pAObj).Take(pBObj - pAObj).ToArray().Length;
                            int a4 = by.Skip(pBObj).Take(pClearC - pBObj).ToArray().Length;
                            int a5 = by.Skip(pClearC).Take(pFailC - pClearC).ToArray().Length;
                            int a6 = by.Skip(pFailC).Take(pEmp - pFailC).ToArray().Length;
                            int a7 = by.Skip(pEmp).Take(pText - pEmp).ToArray().Length;
                            int a8 = by.Skip(pText).Take(by.Length - pText).ToArray().Length;

                            byte[] b1 = by.Skip(pTitleAndName).Take(pMainoObj - pTitleAndName).ToArray();
                            byte[] b2 = by.Skip(pMainoObj).Take(pBObj - pMainoObj).ToArray();
                            byte[] b3 = by.Skip(pAObj).Take(pBObj - pAObj).ToArray();
                            byte[] b4 = by.Skip(pBObj).Take(pClearC - pBObj).ToArray();
                            byte[] b5 = by.Skip(pClearC).Take(pFailC - pClearC).ToArray();
                            byte[] b6 = by.Skip(pFailC).Take(pEmp - pFailC).ToArray();
                            byte[] b7 = by.Skip(pEmp).Take(pText - pEmp).ToArray();
                            byte[] b8 = by.Skip(pText).Take(by.Length - pText).ToArray();
                            by1.AddRange(b1);
                            by1.AddRange(b2);
                            by1.AddRange(b3);
                            by1.AddRange(b4);
                            by1.AddRange(b5);
                            by1.AddRange(b6);
                            by1.AddRange(b7);
                            by1.AddRange(b8);

                            leng = leng - 32;
                            int num = leng + 32;
                            byte[] c1 = BitConverter.GetBytes(num);    //01,60
                            by1[leng] = c1[0];
                            by1[leng + 1] = c1[1];

                            num = num + a1;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 4] = c1[0];
                            by1[leng + 5] = c1[1];

                            num = num + a2;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 8] = c1[0];
                            by1[leng + 9] = c1[1];

                            num = num + a3;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 12] = c1[0];
                            by1[leng + 13] = c1[1];

                            num = num + a4;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 16] = c1[0];
                            by1[leng + 17] = c1[1];

                            num = num + a5;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 20] = c1[0];
                            by1[leng + 21] = c1[1];

                            num = num + a6;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 24] = c1[0];
                            by1[leng + 25] = c1[1];

                            num = num + a7;
                            c1 = BitConverter.GetBytes(num);
                            by1[leng + 28] = c1[0];
                            by1[leng + 29] = c1[1];


                            fileHeader = "Stored_Data/" + listBox2.SelectedIndex.ToString() + "/stored_header_data.txt";
                            fileQuest = "Stored_Data/" + listBox2.SelectedIndex.ToString() + "/stored_quest_data.txt";
                            fileEnd = "Stored_Data/" + listBox2.SelectedIndex.ToString() + "/stored_end_data.txt";

                            //header
                            leng = by1.Count;       //478
                            byte[] b = BitConverter.GetBytes(leng); //2C8

                            byte[] header = { 00, 00, 15, 04, 18, 01, 00, 00, 00, 00, 00, 00, 00, 00, 255, 255 };
                            header[14] = b[1];
                            header[15] = b[0];
                            lineChanger(BitConverter.ToString(header).Replace("-", string.Empty), fileHeader, count);

                            //data
                            lineChanger(BitConverter.ToString(by1.ToArray()).Replace("-", string.Empty), fileQuest, count);

                            //end
                            byte[] end = { 10, 145, 229, 144, 72, 130, 162, 145, 206, 140, 88, 136, 179, 104, 00, 00, 00 };
                            lineChanger(BitConverter.ToString(end).Replace("-", string.Empty), fileEnd, count);

                            string tTitleAndName = Encoding.GetEncoding("Shift_JIS").GetString(by.Skip(pTitleAndName).Take(pMainoObj - pTitleAndName).ToArray()).Replace("\n", "\r\n");
                            listBox1.Items.Add(tTitleAndName);

                            numQuestCount.Value = count + 1;
                            numTotal.Value = numTotal.Value + 1;

                            ManageLogs($"Added a new quest to list. Current count is {count + 1}.");
                        }
                        else if (by[0] == 74)
                        {
                            MessageBox.Show("Encryption detected. Decrypt it with ReFrontier to load.");
                        }
                        else
                        {
                            MessageBox.Show("This is not a quest file.");
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Max limit reached.");
                }
            }
        }

        void ManageLogs(string text)
        { 
            labelLog1.Text = text;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {

        }

        private void btnDown_Click(object sender, EventArgs e)
        {

        }
    }
}