using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

namespace NestManagerLibrary
{
    public class NestManager
    {
        public delegate void RefreshHandler(object sender, RefreshEventArguments e);
        public event RefreshHandler OnRefresh;
        public List<Nest> NestList = new List<Nest>();
        public string currenthickness = "PL12";
        static string DBPath = @"G:\Kevins Scripts, Lisps, Bats, AHK\Apps\PlateCuts\NestLog.sqlite";
        string OutputDirectory = "G:\\BURNING TABLE 2";
        bool FileChanged = true;
        bool FirstRun = true;
        public DataSet MainDataSet = new DataSet();
        DataView MainDataView = new DataView();
        SQLiteConnection DBConn = new SQLiteConnection($"Data Source={DBPath};Version=3;");
        SQLiteDataAdapter Adapter;
        FileSystemWatcher clientLogWatcher = new FileSystemWatcher()
        {
            Path = @"G:\Kevins Scripts, Lisps, Bats, AHK\Apps\PlateCuts",
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = "*.sqlite"
        };
        public DataGridView MainView;
        public ComboBox ThicknessComboBox;
        public Button btnPromoteNest;
        public Button btnCreateCustomNest;
        Timer RefreshTimer = new Timer()
        {
            Interval = 250,
        };
        int selectedIndex = 0;
        bool promoting = false;
        bool IsClient;


        public NestManager(DataGridView dgvMainView, ComboBox comboThickness, Button btnremovenest, Button btnpromotenest, Button btncreatecustomNest, bool isClient)
        {
            RefreshTimer.Tick += new EventHandler(RefreshMainDisplay);
            dgvMainView.CellEndEdit += new DataGridViewCellEventHandler(CommitDGVEdit);
            dgvMainView.CurrentCellDirtyStateChanged += new EventHandler(DirtyChanged);
            dgvMainView.SelectionChanged += new EventHandler(MainViewSelectionChanged);
            MainView = dgvMainView;
            MainView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            MainView.CellContentClick += new DataGridViewCellEventHandler(MainViewContentClick);
            ThicknessComboBox = comboThickness;
            ThicknessComboBox.SelectionChangeCommitted += new EventHandler(ThicknessComboChanged);
            btnremovenest.Click += (sender, args) => RemoveNest();
            btnpromotenest.Click += (sender, args) => PromoteNest();
            btnPromoteNest = btnpromotenest;
            btncreatecustomNest.Click += (sender, args) => CreateCustomNest();
            btnCreateCustomNest = btncreatecustomNest;
            clientLogWatcher.EnableRaisingEvents = true;
            clientLogWatcher.Changed += new FileSystemEventHandler(MarkFileChangeTrue);
            IsClient = isClient;
            RefreshTimer.Start();
        }

        private void RefreshEvent(List<Nest> nestlist)
        {
            if (OnRefresh == null) return;
            RefreshEventArguments args = new RefreshEventArguments(nestlist);
            OnRefresh(this, args);
        }

        private void MainViewContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex < 13)
                return;
            else
            {
                string partsList = MainView.Rows[e.RowIndex].Cells[11].FormattedValue.ToString();
                if (e.ColumnIndex == 13)
                {
                    if (MainView.Rows[e.RowIndex].Cells[0].FormattedValue.ToString() != "RECUT")
                    {
                        MessageBox.Show("Details not available for a non-custom nest");
                        return;
                    }
                    using (var form = new CustomNestDialog(partsList, MainView.Rows[e.RowIndex].Cells[4].FormattedValue.ToString(), false))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            int orderID = Convert.ToInt32(MainView.Rows[e.RowIndex].Cells[9].FormattedValue.ToString());
                            string newPartsList = form.partslist;
                            bool tryingToOpen = true;
                            string sql = $"UPDATE {currenthickness} SET 'partslist' = '{newPartsList}' WHERE OrderID = {orderID}";
                            RefreshTimer.Stop();
                            while (tryingToOpen)
                            {
                                try
                                {
                                    DBConn.Open();
                                    tryingToOpen = false;
                                }
                                catch (Exception)
                                {
                                    System.Threading.Thread.Sleep(30);
                                }
                            }
                            using (SQLiteCommand cmd = new SQLiteCommand())
                            {
                                cmd.Connection = DBConn;
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                            DBConn.Close();
                            RefreshTimer.Start();
                        }
                        else
                            return;
                    }
                }

                else if (e.ColumnIndex == 14) //open PDF button
                {
                    int numberOfMatches = 0;
                    int numberOfPiecemarks = 0;
                    string pdfPath;
                    string filename = MainView.Rows[e.RowIndex].Cells[5].FormattedValue.ToString();
                    List<string> pieceMarkList = new List<string>();
                    if (partsList != "na")
                    {
                        string[] splitstrings = partsList.Split(new[] { ',' });
                        foreach (string itemstring in splitstrings)
                        {
                            string[] splititems = itemstring.Split(new[] { '%' });
                            pieceMarkList.Add(splititems[1]);
                        }
                        numberOfPiecemarks = pieceMarkList.Count;
                        foreach (string piecemark in pieceMarkList)
                        {
                            if (Directory.GetFiles(OutputDirectory).Where(x => Path.GetFileName(x).Contains(piecemark) && (Path.GetExtension(x) == ".pdf" || Path.GetExtension(x) == ".PDF")).Any())
                            {
                                numberOfMatches += 1;
                            }
                        }
                    }
                    if ((numberOfMatches >= numberOfPiecemarks - 1 && numberOfPiecemarks > 1) || (numberOfPiecemarks == 1 && numberOfMatches == 1))
                    {
                        pdfPath = Directory.GetFiles(OutputDirectory).Where(x => Path.GetFileName(x).Contains(pieceMarkList.First()) && (Path.GetExtension(x) == ".pdf" || Path.GetExtension(x) == ".PDF")).First();
                    }
                    else
                        pdfPath = $"{OutputDirectory}\\{filename}.pdf";
                    if (!File.Exists(pdfPath))
                    {
                        MessageBox.Show($"Cannot find PDF in {OutputDirectory}");
                        return;
                    }
                    else
                    {
                        Process.Start(pdfPath);
                    }
                }
            } //end else
        }

        public void AddNest(Nest nest)
        {
            //format booleans to bit strings
            string partial;
            if (nest.Partial == true)
                partial = "1";
            else
                partial = "0";
            string completed;
            if (nest.Completed == true)
                completed = "1";
            else
                completed = "0";
            string nested;
            if (nest.Nested == true)
                nested = "1";
            else
                nested = "0";

            string tablename = nest.Thickness.Replace(" ", "_");
            RefreshTimer.Stop();
            currenthickness = tablename;

            //create thickness table if nonexistent
            if (!TableExists(tablename, DBConn))
                CreateThicknessTable(tablename);
            DBConn.Close();

            //determine orderid of row to be added
            string orderid = (GetTableRowCount(tablename) + 1).ToString();

            //add new row to database
            string sql = $"insert into {tablename} (CutNumber, JobNumber, Sequence, Batch, Thickness, FileName, Nested, Partial, Completed, OrderID, completedate, partslist, remarks) values ('{nest.CutNumber}','{nest.JobNumber}','{nest.Sequence}','{nest.BatchNumber}','{nest.Thickness}','{nest.FileName}',{nested},{partial},{completed},{orderid},'{nest.completedate}', '{nest.partslist}', 'remarks')";
            ExecuteNonQueryChange(sql, () => AddNest(nest));
        }

        public void RemoveNest()
        {
            // CAN BE MADE MORE EFFICIENT BY EXECUTING ALL AT ONCE BETWEEN A BEGIN AND COMMIT- MUST BUILD STATEMENT IN LOOP
            RefreshTimer.Stop();
            int orderID = 0;

            if (MainView.SelectedRows.Count != 1)
            {
                MessageBox.Show("Must select one row to remove from nest plan");
                RefreshTimer.Start();
                return;
            }
            else
                orderID = Convert.ToInt32(MainView.SelectedRows[0].Cells[9].FormattedValue.ToString());

            //currenthickness is tablename because row must be selected
            int rowcount = GetTableRowCount(currenthickness);
            ExecuteNonQueryChange($"DELETE FROM {currenthickness} WHERE OrderID = {orderID}", () => Return());

            //adjust the orderids of the remaining rows that came after the row remmoved
            try
            {
                DBConn.Open();
            }
            catch (Exception)
            {
                while (WaitForFile(DBPath) == false)
                    System.Threading.Thread.Sleep(10);
                DBConn.Open();
            }
            for (int i = orderID + 1; i < rowcount + 1; i++)
            {
                string sql = $"UPDATE '{currenthickness}' SET 'OrderID' = '{(i - 1).ToString()}' WHERE OrderID = '{i}';";
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = DBConn;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            DBConn.Close();
        }

        public void PromoteNest()
        {
            RefreshTimer.Stop();
            btnPromoteNest.Enabled = false;
            int orderID = 0;
            int rowcount = GetTableRowCount(currenthickness);
            if (MainView.SelectedRows.Count != 1)
            {
                MessageBox.Show("Must select one row to promote the order of.");
                RefreshTimer.Start();
                btnPromoteNest.Enabled = true;
                return;
            }
            else if (MainView.SelectedRows[0].Index == 0)
            {
                MessageBox.Show("Cannot promote first row");
                btnPromoteNest.Enabled = true;
            }
            else
                orderID = Convert.ToInt32(MainView.SelectedRows[0].Cells[9].FormattedValue.ToString());

            try
            {
                DBConn.Open();
            }
            catch (Exception)
            {
                while (WaitForFile(DBPath) == false)
                    System.Threading.Thread.Sleep(10);
                DBConn.Open();
            }
            
            string sql = $"BEGIN;UPDATE '{currenthickness}' SET 'orderid' = 'change' WHERE orderid = '{orderID}';UPDATE '{currenthickness}' SET 'orderid' = '{(orderID).ToString()}' WHERE orderid = '{orderID - 1}';UPDATE '{currenthickness}' SET 'orderid' = '{(orderID - 1).ToString()}' WHERE orderid = 'change';COMMIT;";
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Connection = DBConn;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            for (int i = orderID - 1; i < rowcount + 1; i++)
            {
                sql = $"UPDATE '{currenthickness}' SET 'nested' = '0' WHERE orderid = '{i.ToString()}';";
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = DBConn;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
            DBConn.Close();
            promoting = true;
            RefreshTimer.Start();
        }

        public void CreateCustomNest()
        {
            using (var form = new CustomNestDialog("na", currenthickness, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    List<string> joblist = new List<string>();
                    Nest customNest = new Nest();
                    foreach (var item in form.RecutItemList)
                    {
                        joblist.Add(item.JobNumber);
                    }
                    joblist = joblist.Distinct().ToList();
                    string jobs = "";
                    foreach (var job in joblist)
                    {
                        jobs = $"{jobs},{job}";
                    }
                    customNest.JobNumber = jobs.Trim(new[] { ',' });
                    customNest.CutNumber = "RECUT";
                    customNest.Sequence = " ";
                    customNest.BatchNumber = " ";
                    customNest.FileName = " ";
                    customNest.Completed = false;
                    customNest.Partial = false;
                    customNest.OrderID = 0;
                    customNest.completedate = " ";
                    customNest.partslist = form.partslist;
                    customNest.Thickness = form.thickness;

                    AddNest(customNest);
                }
            }
        }

        public void MarkNestedAs(Nest nest, bool state)
        {
            string batch = nest.BatchNumber;
            string thickness = nest.Thickness;
            string cutnumber = nest.CutNumber;
            string stringstate;
            bool tryingToOpen = true;
            if (state)
                stringstate = "1";
            else
                stringstate = "0";
            string sql = $"UPDATE {thickness} SET Nested = {stringstate} WHERE CutNumber = '{cutnumber}' AND Batch = '{batch}'";
            RefreshTimer.Stop();
            while (tryingToOpen)
            {
                try
                {
                    DBConn.Open();
                    tryingToOpen = false;
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
            if (!TableExistsNoOpen(thickness, DBConn))
            {
                DBConn.Close();
                return;
            }
            using (SQLiteCommand cmd = new SQLiteCommand())
            {
                cmd.Connection = DBConn;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            DBConn.Close();
            RefreshTimer.Start();
        }

        private void MainViewSelectionChanged(object sender, EventArgs e)
        {
            if (MainView.SelectedRows.Count > 0)
                selectedIndex = MainView.SelectedRows[0].Index;
        }

        private void ThicknessComboChanged(object sender, EventArgs e)
        {
            currenthickness = ThicknessComboBox.SelectedValue.ToString().Replace(" ", "_");
            MainDataView.RowFilter = $"Thickness = '{currenthickness}'";
            MainView.DataSource = MainDataView;
            LockNestedPropertyCells();
        }

        private void MarkFileChangeTrue(object sender, FileSystemEventArgs e)
        {
            FileChanged = true;
        }

        public void CreateThicknessTable(string tablename)
        {
            string sql = $"CREATE TABLE {tablename} (CutNumber VARCHAR(40), JobNumber VARCHAR(35), Sequence VARCHAR(40), Batch VARCHAR(6), Thickness VARCHAR(15), FileName VARCHAR(50), Nested BIT, Partial BIT, Completed BIT, OrderID INT, completedate VARCHAR(12), partslist VARCHAR(255), remarks VARCHAR(12))";
            ExecuteNonQueryChange(sql, () => CreateThicknessTable(tablename));
        }

        private void ExecuteNonQueryChange(string sql, Action actionOnError)
        {
            try
            {
                DBConn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = DBConn;
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                DBConn.Close();
                FileChanged = true;
                RefreshTimer.Start();
            }
            catch (Exception)
            {
                DBConn.Close();
                while (WaitForFile(DBPath) == false)
                    System.Threading.Thread.Sleep(10);
                actionOnError();
                return;
            }
        }

        private void DeleteAllEmptyTables()
        {
            foreach (string tablename in ThicknessComboBox.Items)
            {
                string sql = $"SELECT COUNT(*) FROM {tablename}";
                int count = 0;
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = DBConn;
                    cmd.CommandText = sql;
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                if (count == 0)
                {
                    string sqlnew = $"DROP Table '{tablename}'";
                    using (SQLiteCommand cmd = new SQLiteCommand())
                    {
                        cmd.Connection = DBConn;
                        cmd.CommandText = sqlnew;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void RefreshMainDisplay(object sender, EventArgs e)
        {
            RefreshTimer.Stop();
            if (FileChanged == false)
            {
                RefreshTimer.Start();
                return;
            }
            else
            {
                FileChanged = false;
                bool noneselected = false;
                if (MainView.SelectedRows.Count != 0)
                    selectedIndex = MainView.SelectedRows[0].Index;
                else
                    noneselected = true;
                if (promoting)
                    selectedIndex -= 1;
                MainView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

                try
                {
                    
                    //string sql = $"select * from {currenthickness} order by OrderID";
                    DBConn.Open();
                    MainView.CellEndEdit -= new DataGridViewCellEventHandler(CommitDGVEdit);

                    if (MainView.Columns["ViewDetails"] != null)
                        MainView.Columns.Remove("ViewDetails");
                    if (MainView.Columns["OpenPDF"] != null)
                        MainView.Columns.Remove("OpenPDF");
                    if (!FirstRun)
                    {
                        MainDataSet.Tables.Clear();
                    }
                    List<string> tableList = ListTables();
                    foreach (string table in tableList)
                    {
                        string sql = $"select * from {table} order by OrderID";
                        Adapter = new SQLiteDataAdapter(sql, DBConn);
                        Adapter.Fill(MainDataSet);
                    }
                    
                    MainDataView = MainDataSet.Tables[0].DefaultView;
                    MainDataView.RowFilter = $"Thickness = '{currenthickness}'";
                    MainView.DataSource = MainDataView;

                    for (int i = 0; i < MainView.Columns.Count; i++)
                    {
                        MainView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        if (i == 6 || i == 7 || i == 8)
                        {
                            MainView.Columns[i].ReadOnly = false;
                        }
                        else
                            MainView.Columns[i].ReadOnly = true;
                    }

                    MainView.Columns[9].Visible = false;
                    MainView.Columns[10].Visible = false;
                    MainView.Columns[11].Visible = false;
                    MainView.Columns[12].Visible = false;
                    if (IsClient)
                    {
                        MainView.Columns[6].ReadOnly = true;
                    }
                    LockNestedPropertyCells();

                    DataGridViewButtonColumn viewDetails = new DataGridViewButtonColumn();
                    viewDetails.Name = "ViewDetails";
                    viewDetails.Text = "View Details";
                    viewDetails.UseColumnTextForButtonValue = true;
                    if (MainView.Columns["ViewDetails"] == null)
                    {
                        MainView.Columns.Add(viewDetails);
                    }
                    DataGridViewButtonColumn openPDF = new DataGridViewButtonColumn();
                    openPDF.Name = "OpenPDF";
                    openPDF.Text = "Open PDF";
                    openPDF.UseColumnTextForButtonValue = true;
                    if (MainView.Columns["OpenPDF"] == null)
                    {
                        MainView.Columns.Add(openPDF);
                    }

                    DeleteAllEmptyTables();

                    RefreshThicknessComboBox();
                    
                    
                    DBConn.Close();
                    FirstRun = false;
                    
                    MainView.CellEndEdit += new DataGridViewCellEventHandler(CommitDGVEdit);
                    MainView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    MainView.ClearSelection();
                    if (!noneselected)
                        MainView.Rows[selectedIndex].Selected = true;
                    promoting = false;
                    if (btnPromoteNest.Enabled == false)
                        btnPromoteNest.Enabled = true;
                    NestList = MainDataSet.Tables[0].AsEnumerable().Select(dr => new Nest
                    {
                        JobNumber = dr.Field<string>("JobNumber"),
                        CutNumber = dr.Field<string>("CutNumber"),
                        Sequence = dr.Field<string>("Sequence"),
                        BatchNumber = dr.Field<string>("Batch"),
                        Thickness = dr.Field<string>("Thickness"),
                        FileName = dr.Field<string>("FileName"),
                        Nested = dr.Field<bool>("Nested"),
                        Partial = dr.Field<bool>("Partial"),
                        Completed = dr.Field<bool>("Completed"),
                        OrderID = dr.Field<int>("OrderID"),
                        completedate = dr.Field<string>("completedate"),
                        partslist = dr.Field<string>("partslist")
                    }).ToList();
                    RefreshEvent(NestList);
                    RefreshTimer.Start();
                    object o = new object();
                    if (ThicknessComboBox.SelectedIndex < 0)
                    {
                        ThicknessComboBox.SelectedIndex = 0;
                        ThicknessComboChanged(o, e);
                    }
                }
                catch (Exception)
                {
                    DBConn.Close();
                    while (WaitForFile(DBPath) == false)
                        System.Threading.Thread.Sleep(10);
                    FileChanged = true;
                    RefreshTimer.Start();
                    return;
                }
                
            }
        }

        private void LockNestedPropertyCells()
        {
            foreach (DataGridViewRow row in MainView.Rows)
            {
                if (row.Cells[0].FormattedValue.ToString() != "RECUT")
                {
                    row.Cells[6].ReadOnly = true;
                }
            }
        }

        private void CommitDGVEdit(object sender, DataGridViewCellEventArgs e)
        {
            RefreshTimer.Stop();
            string orderid = (e.RowIndex + 1).ToString();
            string col = MainView.Columns[e.ColumnIndex].Name;
            string data = MainView.Rows[e.RowIndex].Cells[e.ColumnIndex].FormattedValue.ToString();
            //string orderid = MainDataSet.Tables[0].Rows[e.RowIndex]["OrderID"] + "";
            //string col = MainDataSet.Tables[0].Columns[e.ColumnIndex].ColumnName;
            //string data = MainDataSet.Tables[0].Rows[e.RowIndex][e.ColumnIndex] + "";
            if (data == "True")
                data = "1";
            else
                data = "0";
            string sql = $"UPDATE `{currenthickness}` SET `{col}` = '{data}' WHERE OrderID = '{orderid}';";
            ExecuteNonQueryChange(sql, () => CommitDGVEdit(sender, e));
        }

        private void DirtyChanged(object sender, EventArgs e)
        {
            MainView.EndEdit();
        }

        private bool WaitForFile(string fullPath)
        {
            int numTries = 0;
            while (true)
            {
                ++numTries;
                try
                {
                    using (FileStream fs = new FileStream(fullPath,
                        FileMode.Open, FileAccess.ReadWrite,
                        FileShare.None, 100))
                    {
                        fs.ReadByte();
                        break;
                    }
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(10);
                }
            }
            return true;
        }

        private void Return()
        {
            return;
        }

        private bool TableExists(String tableName, SQLiteConnection connection)
        {
            DBConn.Open();
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = @name";
            cmd.Parameters.Add("@name", DbType.String).Value = tableName;
            return (cmd.ExecuteScalar() != null);
        }

        private bool TableExistsNoOpen(String tableName, SQLiteConnection connection)
        {
            SQLiteCommand cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT * FROM sqlite_master WHERE type = 'table' AND name = @name";
            cmd.Parameters.Add("@name", DbType.String).Value = tableName;
            return (cmd.ExecuteScalar() != null);
        }

        public List<string> ListTables()
        {
            List<string> tables = new List<string>();
            DataTable dt = DBConn.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            return tables;
        }

        public void RefreshThicknessComboBox()
        {
            ThicknessComboBox.DataSource = ListTables();
            int currentIndex = ThicknessComboBox.FindStringExact(currenthickness);
            ThicknessComboBox.SelectedIndex = currentIndex;
            ThicknessComboBox.Update();
        }

        public int GetTableRowCount(string tablename)
        {
            string stmt = $"SELECT COUNT(*) FROM {tablename}";
            int count = 0;
            try
            {
                DBConn.Open();
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    cmd.Connection = DBConn;
                    cmd.CommandText = stmt;
                    count = Convert.ToInt32(cmd.ExecuteScalar());
                }
                DBConn.Close();
            }
            catch (Exception)
            {
                DBConn.Close();
                while (WaitForFile(DBPath) == false)
                    System.Threading.Thread.Sleep(10);
                return GetTableRowCount(tablename);
            }
            return count;
        }

        private int LongestField(DataSet ds, int TableIndex, string ColumnName)
        {
            int maxlength = 0;
            int tot = ds.Tables[TableIndex].Rows.Count;
            string straux = "";
            int intaux = 0;

            Graphics g = MainView.CreateGraphics();

            int offset = Convert.ToInt32(Math.Ceiling(g.MeasureString(" ", MainView.Font).Width));

            for (int i = 0; i < tot; ++i)
            {
                straux = ds.Tables[TableIndex].Rows[i][ColumnName].ToString();

                intaux = Convert.ToInt32(Math.Ceiling(g.MeasureString(straux, MainView.Font).Width));
                if (intaux > maxlength)
                {
                    maxlength = intaux;
                }
            }

            return maxlength + offset;
        }
    }
}
