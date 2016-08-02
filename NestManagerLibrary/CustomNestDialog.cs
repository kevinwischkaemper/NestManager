using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NestManagerLibrary
{
    public partial class CustomNestDialog : Form
    {
        public string partslist;
        public BindingList<RecutItem> RecutItemList = new BindingList<RecutItem>();
        public string thickness = " ";
        List<string> ThicknessList = new List<string>()
        {
        #region thicknessList
            "PL116",
            "PL116_GR50",
            "PL18",
            "PL18_GR50",
            "PL316",
            "PL316_GR50",
            "PL14",
            "PL14_GR50",
            "PL516",
            "PL516_GR50",
            "PL38",
            "PL38_GR50",
            "PL716",
            "PL716_GR50",
            "PL12",
            "PL12_GR50",
            "PL916",
            "PL916_GR50",
            "PL58",
            "PL58_GR50",
            "PL1116",
            "PL1116_GR50",
            "PL34",
            "PL34_GR50",
            "PL1316",
            "PL1316_GR50",
            "PL78",
            "PL78_GR50",
            "PL1516",
            "PL1516_GR50",
            "PL1",
            "PL1_GR50",
            "PL1_18",
            "PL1_18_GR50",
            "PL1_14",
            "PL1_14_GR50",
            "PL1_38",
            "PL1_38_GR50",
            "PL1_12",
            "PL1_12_GR50",
            "PL1_34",
            "PL1_34_GR50",
            "PL2",
            "PL2_GR50"
        #endregion
        };
        public bool ThicknessChangeable;

        public CustomNestDialog(string startinglist, string thickness, bool thicknesschangeable)
        {
            partslist = startinglist;
            InitializeComponent();
            comboThicknessToAdd.DataSource = ThicknessList;
            if (ThicknessList.Contains(thickness))
                comboThicknessToAdd.SelectedIndex = comboThicknessToAdd.FindStringExact(thickness);
            comboThicknessToAdd.Update();
            dgvCustomCutlist.AllowUserToAddRows = false;
            ThicknessChangeable = thicknesschangeable;
        }


        private void AddRecutItem(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textJobNumber.Text))
            {
                MessageBox.Show("Must include a Job Number");
                return;
            }

            if (string.IsNullOrEmpty(textPieceMark.Text))
            {
                MessageBox.Show("Must include a PieceMark name");
                return;
            }
            if (string.IsNullOrEmpty(textQuantity.Text))
            {
                MessageBox.Show("Must include a Quantity");
                return;
            }

            if(textJobNumber.Text.Contains("%") || textPieceMark.Text.Contains("%") || textJobNumber.Text.Contains(",") || textPieceMark.Text.Contains(","))
            {
                MessageBox.Show("Cannot include a '%' or ',' in any values. Stop trying to bamboozle me.");
                return;
            }
            try
            {
                Convert.ToInt32(textQuantity.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Quantity must be an integer.");
                return;
            }

            RecutItem item = new RecutItem();
            item.JobNumber = textJobNumber.Text;
            item.PieceMark = textPieceMark.Text;
            item.Quantity = textQuantity.Text;
            RecutItemList.Add(item);
            ReCalculatePartslist();
            comboThicknessToAdd.Enabled = false;
        }

        private void RemoveSelectedItem(object sender, EventArgs e)
        {
            if (dgvCustomCutlist.SelectedRows.Count != 1)
            {
                MessageBox.Show("Must have a single row selected to remove");
                return;
            }
            else
            {
                var xtest = dgvCustomCutlist.SelectedRows[0].Index;
                RecutItemList.RemoveAt(xtest);
                ReCalculatePartslist();

                if (!RecutItemList.Any() && ThicknessChangeable)
                    comboThicknessToAdd.Enabled = true;
                return;
            }
        }

        private void ReCalculatePartslist()
        {
            partslist = "";
            foreach (var item in RecutItemList)
            {
                partslist = $"{partslist},{item.JobNumber}%{item.PieceMark}%{item.Quantity}";
            }
            partslist = partslist.Trim(new[] { ',' });
        }

        private void CustomNestDialogLoad(object sender, EventArgs e)
        {
            if (partslist != "na")
            {
                string[] splitstrings = partslist.Split(new[] { ',' });
                foreach (string itemstring in splitstrings)
                {
                    string[] splititems = itemstring.Split(new[] { '%' });
                    RecutItem item = new RecutItem();
                    item.JobNumber = splititems[0];
                    item.PieceMark = splititems[1];
                    item.Quantity = splititems[2];
                    RecutItemList.Add(item);
                }
            }
            if (!ThicknessChangeable)
                comboThicknessToAdd.Enabled = false;
            else
                comboThicknessToAdd.Enabled = true;
            var source = new BindingSource(RecutItemList, null);
            dgvCustomCutlist.DataSource = source; 
        }

        private void ThicknessValueChanged(object sender, EventArgs e)
        {
            thickness = comboThicknessToAdd.SelectedValue.ToString();
        }
    }
}
