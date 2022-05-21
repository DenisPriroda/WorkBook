using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WorkBook
{
    public partial class Form1 : Form
    {

        class WorkRecord
        {
            public string number { get; set; }
            public string date { get; set; }
            public string text { get; set; }
            public string nameDoc { get; set; }

            public WorkRecord(string _number, string _date, string _text, string _nameDoc)
            {

                number = _number;
                date = _date;
                text = _text;
                nameDoc = _nameDoc;
            }
        }

        BindingList<WorkRecord> records;
        int selected;

        public Form1()
        {
            InitializeComponent();
            button7.Visible = false;
            button8.Visible = false;
            records = new BindingList<WorkRecord>();
            openFileDialog1.Filter = "Text files(*.txt)|*.txt";
            saveFileDialog1.Filter = "Text files(*.txt)|*.txt";
            renew_grid();
        }

        public void renew_grid()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = records;
            dataGridView1.Columns["number"].HeaderText = "№ записи";
            dataGridView1.Columns["date"].HeaderText = "Дата";
            dataGridView1.Columns["text"].HeaderText = "Сведения о приеме на работу, квалификации, увольнении";
            dataGridView1.Columns["nameDoc"].HeaderText = "Наименование, дата и номер документа, на основании которого внесена запись";
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            textBox2.Text = "";
            textBox1.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти? Не забудьте сохранить изменения в файл.", "Выход", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (records.Count > 0)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                    return;
                string filename = saveFileDialog1.FileName;

                TextWriter wc = new StreamWriter(filename);
                foreach (WorkRecord rec in records)
                    wc.WriteLine(rec.number + ";" + rec.date + ";" + rec.text + ";" + rec.nameDoc);
                wc.Close();
                MessageBox.Show("Информация сохранена");
            }
            else
            {
                MessageBox.Show("Записи отсутствуют!", "Сохранение в файл",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;

            string filename = openFileDialog1.FileName;
            if (File.Exists(filename))
            {
                FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                records.Clear();
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    try
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            String[] words = line.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                            records.Add(new WorkRecord(words[0], words[1], words[2], words[3]));
                        }
                        MessageBox.Show("Информация загружена");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки файла");
                        records.Clear();                        
                    }
                }

            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (records.Count != 0)
            {
                if (dataGridView1.CurrentRow.Index > -1)
                {
                    records.RemoveAt(dataGridView1.CurrentRow.Index);
                    renew_grid();
                }
                else
                {
                    MessageBox.Show("Не выбрано ни единой записи", "Удаление",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Нет записей для удаления", "Удаление",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && (textBox2.Text != ""))
            {
                int number = 0;
                if (records.Count > 0)
                {
                    try
                    {
                        number = Int32.Parse(records[records.Count - 1].number);
                        number++;
                    }
                    catch (FormatException ex)
                    {
                       MessageBox.Show("Предыдущая запись имела не правильный порядковый номер", "Добавление",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                records.Add(new WorkRecord(number.ToString(), dateTimePicker1.Value.ToShortDateString(), textBox1.Text, textBox2.Text));
                renew_grid();
            }
            else
            {
                MessageBox.Show("Сведения или документ не заполнены.", "Добавление",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (records.Count != 0)
            {
                if (dataGridView1.CurrentRow.Index > -1)
                {
                    button1.Visible = false;
                    button2.Visible = false;
                    button3.Visible = false;
                    button4.Visible = false;
                    button5.Visible = false;
                    button6.Visible = false;
                    button7.Visible = true;
                    button8.Visible = true;
                    textBox1.Text = records[dataGridView1.CurrentRow.Index].text;
                    textBox2.Text = records[dataGridView1.CurrentRow.Index].nameDoc;
                    dateTimePicker1.Value = Convert.ToDateTime(records[dataGridView1.CurrentRow.Index].date);
                    selected = dataGridView1.CurrentRow.Index;
                }
                else
                {
                    MessageBox.Show("Не выбрано ни единой записи", "Редактирование",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }      

        private void button8_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            button2.Visible = true;
            button3.Visible = true;
            button4.Visible = true;
            button5.Visible = true;
            button6.Visible = true;
            button7.Visible = false;
            button8.Visible = false;
            renew_grid();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if ((textBox1.Text != "") && (textBox2.Text != ""))
            {
                records[selected].text = textBox1.Text;
                records[selected].nameDoc = textBox2.Text;
                records[selected].date = dateTimePicker1.Value.ToShortDateString();
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button4.Visible = true;
                button5.Visible = true;
                button6.Visible = true;
                button7.Visible = false;
                button8.Visible = false;
                renew_grid();
            }
            else
            {
                MessageBox.Show("Сведения или документ не заполнены.", "Редактирование",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
