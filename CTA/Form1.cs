using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Margi Katwala 
//Project 8
//12/6/2017
namespace CTA
{

    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            this.lstStations.Items.Add("");
            this.lstStations.Items.Add("[ Use File>>Load to display L stations... ]");
            this.lstStations.Items.Add("");
            this.lstStations.ClearSelected();
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            toolStripStatusLabel1.Text = string.Format("Number of stations:  0");
            string filename = this.txtDatabaseFilename.Text;
            BusinessTier.Business bizTier;
            bizTier = new BusinessTier.Business(filename);
            DataAccessTier.Data dataTier = new DataAccessTier.Data(this.txtDatabaseFilename.Text);
            bizTier.TestConnection();
         }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
      
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            //clear UI
            ClearStationUI(true /*clear stations*/);
            //load stations
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var stations = biztier.GetStations();
            foreach (var s in stations)
            {
                this.lstStations.Items.Add(s.Name);
            }
            toolStripStatusLabel1.Text = String.Format("Number of stations: {0}", stations.Count());



        }
        private void top10StationsByRidershipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearStationUI(true /*clear stations*/);
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var stations = biztier.GetTopStations(10);
          
            foreach (var s in stations)
            {
                this.lstStations.Items.Add(s);
            }
         
        }
        
        // User has clicked on a station for more info:
         private void lstStations_SelectedIndexChanged(object sender, EventArgs e)
        {
            // sometimes this event fires, but nothing is selected...
            if (this.lstStations.SelectedIndex < 0)   // so return now in this case:
                return;
            ClearStationUI();

            string stationName = this.lstStations.Text;
            stationName = stationName.Replace("'", "''");
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);

            try
            {
                this.txtTotalRidership.Text = string.Format("{0:#,##0}",biztier.getRidershipT(stationName));
                this.txtAvgDailyRidership.Text = string.Format("{0:#,##0}/day", biztier.getRidershipA(stationName));
                this.txtPercentRidership.Text = string.Format("{0:0.00}%", biztier.getRidershipP(stationName));
                int stationID = biztier.getID(stationName);  // all rows have same station ID:
                this.txtStationID.Text = string.Format(Convert.ToString(stationID));
                this.txtSaturdayRidership.Text = String.Format("{0:#,##0}", biztier.getdays(stationName, "A"));
                this.txtSundayHolidayRidership.Text = String.Format("{0:#,##0}", biztier.getdays(stationName, "U"));
                this.txtWeekdayRidership.Text = String.Format("{0:#,##0}", biztier.getdays(stationName, "W"));

                var stops = biztier.GetStops(stationID);
                foreach (var s in stops)
                {
                    this.lstStops.Items.Add(s.Name);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }

        }

        private void ClearStationUI(bool clearStatations = false)
        {
            ClearStopUI();

            this.txtTotalRidership.Clear();
            this.txtTotalRidership.Refresh();

            this.txtAvgDailyRidership.Clear();
            this.txtAvgDailyRidership.Refresh();

            this.txtPercentRidership.Clear();
            this.txtPercentRidership.Refresh();

            this.txtStationID.Clear();
            this.txtStationID.Refresh();

            this.txtWeekdayRidership.Clear();
            this.txtWeekdayRidership.Refresh();
            this.txtSaturdayRidership.Clear();
            this.txtSaturdayRidership.Refresh();
            this.txtSundayHolidayRidership.Clear();
            this.txtSundayHolidayRidership.Refresh();

            this.lstStops.Items.Clear();
            this.lstStops.Refresh();

            if (clearStatations)
            {
                this.lstStations.Items.Clear();
                this.lstStations.Refresh();
            }
        }
       
        // user has clicked on a stop for more info:
        private void lstStops_SelectedIndexChanged(object sender, EventArgs e)
        {
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            // sometimes this event fires, but nothing is selected...
            if (this.lstStops.SelectedIndex < 0)   // so return now in this case:
                return;
            ClearStopUI();
            string stopName = this.lstStops.Text;
            stopName = stopName.Replace("'", "''");
            try
            {
                int accessible = biztier.getADA(stopName);
               
                if (accessible == 1)
                    this.txtAccessible.Text = "Yes";
                else
                    this.txtAccessible.Text = "No";
                
                // direction of travel:
                this.txtDirection.Text = biztier.getDir(stopName);
                // lat/long position:
                this.txtLocation.Text = string.Format("({0:00.0000}, {1:00.0000})", biztier.getloc(stopName), biztier.getloc2(stopName));
                // display colors:
                var stops = biztier.getColor(stopName);
                foreach (var s in stops)
                {
                    lstLines.Items.Add(s);
                }
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error: '{0}'.", ex.Message);
                MessageBox.Show(msg);
            }

        }

        private void ClearStopUI()
        {
            this.txtAccessible.Clear();
            this.txtAccessible.Refresh();

            this.txtDirection.Clear();
            this.txtDirection.Refresh();

            this.txtLocation.Clear();
            this.txtLocation.Refresh();

            this.lstLines.Items.Clear();
            this.lstLines.Refresh();
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

            ClearStationUI(true /*clear stations*/);
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var stations = biztier.getLike(String.Format(textBox1.Text));

            foreach (var s in stations)
            {
                this.lstStations.Items.Add(s);
            }
        }

        private void txtAccessible_TextChanged(object sender, EventArgs e)
        {
            //ClearStationUI(true /*clear stations*/);
            var biztier = new BusinessTier.Business(this.txtDatabaseFilename.Text);
            var ada = biztier.getADAreplace(this.lstStops.Text,txtAccessible.Text);

        }
    }//class
}//namespace
