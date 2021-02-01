using System;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using FastColoredTextBoxNS;

namespace TabularEditor.UI.Extensions.FCTB
{
    public partial class ReplaceForm : Form
    {
        FastColoredTextBox tb;
        bool firstSearch = true;
        Place startPlace;

        public ReplaceForm(FastColoredTextBox tb)
        {
            InitializeComponent();
            this.tb = tb;
            this.tb.TextChanged += Tb_TextChanged;
        }

        private bool suspendTextChanged = false;

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (suspendTextChanged) return;
            ResetSearch();
        }

        private void btClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btFindNext_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Find(tbFind.Text))
                {
                    MessageBox.Show(this, "Search reached end of text");
                    ResetSearch();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        public List<Range> FindAll(string pattern)
        {
            var opt = cbMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
            if (!cbRegex.Checked)
                pattern = Regex.Escape(pattern);
            if (cbWholeWord.Checked)
                pattern = "\\b" + pattern + "\\b";
            //
            var range = tb.Selection.IsEmpty ? tb.Range.Clone() : tb.Selection.Clone();
            //
            var list = new List<Range>();
            foreach (var r in range.GetRangesByLines(pattern, opt))
                list.Add(r);

            return list;
        }

        public bool Find(string pattern)
        {
            RegexOptions opt = cbMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase;
            if (!cbRegex.Checked)
                pattern = Regex.Escape(pattern);
            if (cbWholeWord.Checked)
                pattern = "\\b" + pattern + "\\b";
            //
            Range range = tb.Selection.Clone();
            range.Normalize();
            //
            if (firstSearch)
            {
                startPlace = range.Start;
                firstSearch = false;
            }
            //
            range.Start = range.End;
            if (range.Start >= startPlace)
                range.End = new Place(tb.GetLineLength(tb.LinesCount - 1), tb.LinesCount - 1);
            else
                range.End = startPlace;
            //
            foreach (var r in range.GetRangesByLines(pattern, opt))
            {
                tb.Selection.Start = r.Start;
                tb.Selection.End = r.End;
                tb.DoSelectionVisible();
                tb.Invalidate();
                return true;
            }
            if (range.Start >= startPlace && startPlace > Place.Empty)
            {
                tb.Selection.Start = new Place(0, 0);
                return Find(pattern);
            }
            return false;
        }

        private void tbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
                btFindNext_Click(sender, null);
            if (e.KeyChar == '\x1b')
                Hide();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) // David
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ReplaceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            this.tb.Focus();
        }

        private void btReplace_Click(object sender, EventArgs e)
        {
            suspendTextChanged = true;

            try
            {
                if (tb.SelectionLength != 0)
                    if (!tb.Selection.ReadOnly)
                    {
                        var start = tb.Selection.Start;
                        tb.SelectedText = "";
                        tb.Selection = tb.GetRange(start, start);
                        tb.SelectedText = tbReplace.Text;
                    }
                btFindNext_Click(sender, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }

            suspendTextChanged = false;
        }

        private void btReplaceAll_Click(object sender, EventArgs e)
        {
            suspendTextChanged = true;

            try
            {
                tb.Selection.BeginUpdate();

                //search
                var ranges = FindAll(tbFind.Text);
                //check readonly
                var ro = false;
                foreach (var r in ranges)
                    if (r.ReadOnly)
                    {
                        ro = true;
                        break;
                    }
                //replace
                if (!ro)
                    if (ranges.Count > 0)
                    {
                        tb.TextSource.Manager.ExecuteCommand(new ReplaceTextCommand(tb.TextSource, ranges, tbReplace.Text));
                        tb.Selection.Start = new Place(0, 0);
                    }
                //
                tb.Invalidate();
                MessageBox.Show(this, ranges.Count + " occurrence(s) replaced");
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
            tb.Selection.EndUpdate();

            suspendTextChanged = false;
        }

        protected override void OnActivated(EventArgs e)
        {
            tbFind.Focus();
            ResetSearch();
        }

        public void ResetSearch()
        {
            firstSearch = true;
        }

        private void cbMatchCase_CheckedChanged(object sender, EventArgs e)
        {
            ResetSearch();
        }
    }
}