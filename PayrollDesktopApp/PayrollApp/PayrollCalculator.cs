﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;

namespace PayrollApp
{
    public partial class PayrollCalculator : Form
    {
        //************************************   GLOBAL VARIABLES ***********************************************//
        private string fullName = String.Empty;

        //Variables for each day a week
        double Mon1 = 0.00, Tues1 = 0.00, Wen1 = 0.00, Thurs1 = 0.00, Fri1 = 0.00, Sat1 = 0.00, Sun1 = 0.00;
        double Mon2 = 0.00, Tues2 = 0.00, Wen2 = 0.00, Thurs2 = 0.00, Fri2 = 0.00, Sat2 = 0.00, Sun2 = 0.00;
        double Mon3 = 0.00, Tues3 = 0.00, Wen3 = 0.00, Thurs3 = 0.00, Fri3 = 0.00, Sat3 = 0.00, Sun3 = 0.00;
        double Mon4 = 0.00, Tues4 = 0.00, Wen4 = 0.00, Thurs4 = 0.00, Fri4 = 0.00, Sat4 = 0.00, Sun4 = 0.00;

        //Declare Variables for Hours
        double totalHoursWk1, totalHoursWk2, totalHoursWk3, totalHoursWk4;

        double contractualHoursWk1, contractualHoursWk2, contractualHoursWk3, contractualHoursWk4;

        double overtimeHoursWk1, overtimeHoursWk2, overtimeHoursWk3, overtimeHoursWk4;

        double totalContractualHours;
        double totalOvertimeHours;

        double totalHoursWorked;

        //Declare variables for Amount(Earnings)
        double contractualAmountWk1, contractualAmountWk2, contractualAmountWk3, contractualAmountWk4;

        double overtimeAmountWk1, overtimeAmountWk2, overtimeAmountWk3, overtimeAmountWk4;
        double totalContractualAmount;
        double totalOvertimeAmount;
        double totalAmountEarned;

        //Declare variables for Mandatory deduction
        double tax, NIContribution, taxRate, NIRate, SLC;
        double totalDeductions;

        //Declare & initialize Constant for Voluntary Deductions
        const double Union = 10.00;
        const double SLCRate = .05;

        //Declare variables for pay rates
        double hourlySalaryRate, overtimeSalaryRate;

        //Declare variables for Net pay
        double netPay;

        //*************************************************************************************************//
        public PayrollCalculator()
        {
            InitializeComponent();
        }

        private void PayrollCalculator_Load(object sender, EventArgs e)
        {
            ListOfMonths();
            ResetControls();
            LoadPayRecords();
            payrollTimer.Start();

        }

        private void ListOfMonths()
        {
            string[] months = { "Select a month...." ,"January" ,"February" ,"March" ,"April" ,"May" ,"June"
                              ,"July" ,"August" ,"September" ,"October" ,"November" ,"December" };

            foreach (var month in months)
            {
                cmbCurrentMonth.Items.Add(month);
                cmbCurrentMonth.SelectedIndex = 0;
            }

            foreach (var month in months)
            {
                cmbSearchPayMonth.Items.Add(month);
                cmbSearchPayMonth.SelectedIndex = 0;
            }
        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSavePay_Click(object sender, EventArgs e)
        {
            string cs = @"Data Source=DESKTOP-KGQRUI1;Initial Catalog=PayrollSystemDB;Integrated Security=True";
            SqlConnection objSqlConnection = new SqlConnection(cs);
            string insertCommand = "INSERT INTO tblPayRecords" +
                "(EmployeeID, FullName, NINumber, PayDate, PayPeriod, PayMonth, HourlyRate, ContractualHours, " +
                "OvertimeHours, TotalHours, ContractualEarnings, OvertimeEarnings, TotalEarnings, TaxCode, TaxAmount, NIContribution, " +
                "UnionFee, SLC, TotalDeductions, NetPay)" +
                "VALUES(@EmployeeID, @FullName, @NINumber, @PayDate, @PayPeriod, @PayMonth, @HourlyRate, @ContractualHours, " +
                "@OvertimeHours, @TotalHours, @ContractualEarnings, @OvertimeEarnings, @TotalEarnings, @TaxCode, @TaxAmount, @NIContribution, " +
                "@UnionFee, @SLC, @TotalDeductions, @NetPay)";

            SqlCommand objInsertCommand = new SqlCommand(insertCommand, objSqlConnection);
            objInsertCommand.Parameters.AddWithValue("@EmployeeID", txtEmployeeID.Text);
            objInsertCommand.Parameters.AddWithValue("@FullName", lblEmployeeFullName.Text);
            objInsertCommand.Parameters.AddWithValue("@NINumber", txtNINumber.Text);
            objInsertCommand.Parameters.AddWithValue("@PayDate", dtpCurrentDate.Value.ToString("MM/dd/yyyy"));
            objInsertCommand.Parameters.AddWithValue("@PayPeriod", listBoxPayPeriod.SelectedItem.ToString());
            objInsertCommand.Parameters.AddWithValue("@PayMonth", cmbCurrentMonth.SelectedItem.ToString());
            objInsertCommand.Parameters.AddWithValue("@HourlyRate", nudHourlyRate.Value.ToString());
            objInsertCommand.Parameters.AddWithValue("@ContractualHours", txtContractualHours.Text);
            objInsertCommand.Parameters.AddWithValue("@OvertimeHours", txtOvertimeHours.Text);
            objInsertCommand.Parameters.AddWithValue("@TotalHours", txtTotalHoursWorked.Text);
            objInsertCommand.Parameters.AddWithValue("@ContractualEarnings", txtContractualEarnings.Text);
            objInsertCommand.Parameters.AddWithValue("@OvertimeEarnings", txtOvertimeEarnings.Text);
            objInsertCommand.Parameters.AddWithValue("@TotalEarnings", txtTotalEarnings.Text);
            objInsertCommand.Parameters.AddWithValue("@TaxCode", txtTaxCode.Text);
            objInsertCommand.Parameters.AddWithValue("@TaxAmount", txtTaxAmount.Text);
            objInsertCommand.Parameters.AddWithValue("@NIContribution", txtNIContribution.Text);
            objInsertCommand.Parameters.AddWithValue("@UnionFee", txtunion.Text);
            objInsertCommand.Parameters.AddWithValue("@SLC", txtSLC.Text);
            objInsertCommand.Parameters.AddWithValue("@TotalDeductions", txtTotalDeductions.Text);
            objInsertCommand.Parameters.AddWithValue("@NetPay", txtNetPay.Text);

            try
            {
                objSqlConnection.Open();
                objInsertCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("The following error occured :" + ex.Message + ex.StackTrace, "Query Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                objSqlConnection.Close();
            }
            ResetControls();
            LoadPayRecords();

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetControls();
        }

       
        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = @"Data Source=DESKTOP-KGQRUI1;Initial Catalog=PayrollSystemDB;Integrated Security=True";
            SqlConnection sqlCon = new SqlConnection(connectionString);
            sqlCon.Open();

            string selectCommand = "SELECT FirstName, LastName, NINumber FROM tblEmployee Where EmployeeID = '" + txtEmployeeID.Text + "'";
            SqlCommand objSqlCommand = new SqlCommand(selectCommand, sqlCon);

            try
            {
                SqlDataReader dataReader = objSqlCommand.ExecuteReader();
                if(dataReader.Read())
                {
                    txtFirstName.Text = dataReader[0].ToString();
                    txtLastName.Text = dataReader[1].ToString();
                    txtNINumber.Text = dataReader[2].ToString();
                    lblEmployeeFullName.Text = txtLastName.Text + ", " + txtFirstName.Text;
                }
                else
                {
                    MessageBox.Show("No Records where found with " + txtEmployeeID.Text, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                dataReader.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error : " + ex.Message, "Error Message");
            }
            finally
            {
                sqlCon.Close();
            }
        }

        #region PAYROLL COMPUTATION
        private void btnComputePay_Click(object sender, EventArgs e)
        {
            if(ValidateControls())
            {
                GetWeek1Values();
                GetWeek2Values();
                GetWeek3Values();
                GetWeek4Values();

                //Sum weekly hours
                totalHoursWk1 = Mon1 + Tues1 + Wen1 + Fri1 + Sat1 + Sun1;
                totalHoursWk2 = Mon2 + Tues2 + Wen2 + Fri2 + Sat2 + Sun2;
                totalHoursWk3 = Mon3 + Tues3 + Wen3 + Fri3 + Sat3 + Sun3;
                totalHoursWk4 = Mon4 + Tues4 + Wen4 + Fri4 + Sat4 + Sun4;

                //Retrieve Hourly Rate
                try
                {
                    hourlySalaryRate = double.Parse(nudHourlyRate.Value.ToString());
                }
                catch (FormatException ex)
                {
                    MessageBox.Show("The following error has occured: " + ex.Message, "Error Message");
                }

                //Overtime Rate
                overtimeSalaryRate = hourlySalaryRate * 1.5;

                #region --------WEEK 1 COMPUTATION---------
                //Hours worked, No Overtime
                if (totalHoursWk1 <= 36)
                {
                    contractualHoursWk1 = totalHoursWk1;
                    contractualAmountWk1 = hourlySalaryRate * totalHoursWk1;
                    overtimeHoursWk1 = 0.00;
                    overtimeAmountWk1 = 0.00;
                }
                //Hours worked, with Overtime
                else if(totalHoursWk1 > 36)
                {
                    contractualHoursWk1 = 36;
                    contractualAmountWk1 = hourlySalaryRate * contractualHoursWk1;
                    overtimeHoursWk1 = totalHoursWk1 - contractualHoursWk1;
                    overtimeAmountWk1 = overtimeHoursWk1 * overtimeSalaryRate;
                }
                #endregion

                #region --------WEEK 2 COMPUTATION---------
                //Hours worked, No Overtime
                if (totalHoursWk2 <= 36)
                {
                    contractualHoursWk2 = totalHoursWk2;
                    contractualAmountWk2 = hourlySalaryRate * totalHoursWk2;
                    overtimeHoursWk2 = 0.00;
                    overtimeAmountWk2 = 0.00;
                }
                //Hours worked, with Overtime
                else if (totalHoursWk2 > 36)
                {
                    contractualHoursWk2 = 36;
                    contractualAmountWk2 = hourlySalaryRate * contractualHoursWk2;
                    overtimeHoursWk2 = totalHoursWk2 - contractualHoursWk2;
                    overtimeAmountWk2 = overtimeHoursWk2 * overtimeSalaryRate;
                }
                #endregion

                #region --------WEEK 3 COMPUTATION---------
                //Hours worked, No Overtime
                if (totalHoursWk3 <= 36)
                {
                    contractualHoursWk3 = totalHoursWk3;
                    contractualAmountWk3 = hourlySalaryRate * totalHoursWk3;
                    overtimeHoursWk3 = 0.00;
                    overtimeAmountWk3 = 0.00;
                }
                //Hours worked, with Overtime
                else if (totalHoursWk3 > 36)
                {
                    contractualHoursWk3 = 36;
                    contractualAmountWk3 = hourlySalaryRate * contractualHoursWk3;
                    overtimeHoursWk3 = totalHoursWk3 - contractualHoursWk3;
                    overtimeAmountWk3 = overtimeHoursWk3 * overtimeSalaryRate;
                }
                #endregion

                #region --------WEEK 4 COMPUTATION---------
                //Hours worked, No Overtime
                if (totalHoursWk4 <= 36)
                {
                    contractualHoursWk4 = totalHoursWk4;
                    contractualAmountWk4 = hourlySalaryRate * totalHoursWk4;
                    overtimeHoursWk4 = 0.00;
                    overtimeAmountWk4 = 0.00;
                }
                //Hours worked, with Overtime
                else if (totalHoursWk4 > 36)
                {
                    contractualHoursWk4 = 36;
                    contractualAmountWk4 = hourlySalaryRate * contractualHoursWk4;
                    overtimeHoursWk4 = totalHoursWk4 - contractualHoursWk4;
                    overtimeAmountWk4 = overtimeHoursWk4 * overtimeSalaryRate;
                }
                #endregion

                //Compute total hours and Amount
                totalContractualHours = contractualHoursWk1 + contractualHoursWk2 + contractualHoursWk3 + contractualHoursWk4;
                totalOvertimeHours = overtimeHoursWk1 + overtimeHoursWk2 + overtimeHoursWk3 + overtimeHoursWk4;
                totalContractualAmount = contractualAmountWk1 + contractualAmountWk2 + contractualAmountWk3 + contractualAmountWk4;
                totalOvertimeAmount = overtimeAmountWk1 + overtimeAmountWk2 + overtimeAmountWk3 + overtimeAmountWk4;
                totalHoursWorked = totalContractualHours + totalOvertimeHours;
                totalAmountEarned = totalContractualAmount + totalOvertimeAmount;

                //COMPUTE DEDUCTIONS
                #region ---Tax Computation----
                if(totalAmountEarned <= 916.67)
                {
                    //Tax free rate
                    taxRate = .0;
                    tax = totalAmountEarned * taxRate;
                }
                else if(totalAmountEarned > 916.67 && totalAmountEarned <= 3583.33)
                {
                    //Basic tax rate
                    taxRate = .20;
                    //Income tax
                    tax = ((916.67 * .0) + ((totalAmountEarned - 916.67) * taxRate));
                }
                else if(totalAmountEarned > 3583.33 && totalAmountEarned < 12500)
                {
                    //Higher tax rate
                    taxRate = .40;
                    //Income tax
                    tax = ((916.67 * .0) + ((3583.33 - 916.67) * .20) + ((totalAmountEarned - 3583.33) * taxRate));
                }
                else if (totalAmountEarned > 12500)
                {
                    //Additional tax rate
                    taxRate = .45;

                    //Income tax
                    tax = ((916.67 * 0) + ((3583.33 - 916.67) * .20) + ((12500 - 3583.33) * .40) + (totalAmountEarned - 12500));

                }
                #endregion

                #region ----NI Computation-----
                if(totalAmountEarned < 620)
                {
                    NIRate = .0;
                }
                else if(totalAmountEarned >= 620 && totalAmountEarned <= 3308)
                {
                    NIRate = .12;
                }
                else if(totalAmountEarned > 3308)
                {
                    NIContribution = .02;
                }
                #endregion

                NIContribution = totalAmountEarned * NIRate;
                SLC = totalAmountEarned * SLCRate;
                
                //Total Amount Deductable
                totalDeductions = tax + NIContribution + SLC + Union;

                //Compute Net Pay after deductions
                netPay = totalAmountEarned - totalDeductions;


                //Output to Controls
                txtContractualHours.Text = totalContractualHours.ToString("F");
                txtOvertimeHours.Text = totalOvertimeHours.ToString("F");
                txtTotalHoursWorked.Text = totalHoursWorked.ToString("F");
                txtContractualEarnings.Text = totalContractualAmount.ToString("C");
                txtOvertimeEarnings.Text = totalOvertimeAmount.ToString("C");
                txtTotalEarnings.Text = totalAmountEarned.ToString("C");
                txtOvertimeRate.Text = overtimeSalaryRate.ToString("F");
                txtTaxAmount.Text = tax.ToString("C");
                txtNIContribution.Text = NIContribution.ToString("C");
                txtSLC.Text = SLC.ToString("C");
                txtunion.Text = Union.ToString("C");
                txtTotalDeductions.Text = totalDeductions.ToString("C");
                txtNetPay.Text = netPay.ToString("C");
            }
        }
        #endregion

        #region CONVERT TIME TO DECIMAL

        private void btnConvert_Click(object sender, EventArgs e)
        {
            decimal hours, minutes, decimalHours = 0.00M;
            if (decimal.TryParse(txtHours.Text, out hours))
            {
                if (decimal.TryParse(txtMinutes.Text, out minutes))
                {
                    decimalHours = ConvertTimeToDecimal(hours, minutes);
                    txtDecimal.Text = decimalHours.ToString();
                }

                else
                {
                    MessageBox.Show("Please enter real number for minutes.");
                    txtMinutes.Focus();
                }
            }

            else
            {
                MessageBox.Show("Please enter real number for hours.");
                txtHours.Focus();
            }
        }

        private decimal ConvertTimeToDecimal(decimal hh, decimal mm)
        {
            return (hh + (mm / 60));
        }

        #endregion

        // SHOW CALCULATOR
        private void linkLabelWinCalc_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("calc.exe");
        }

        //SHOW TIME
        private void payrollTimer_Tick(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            btnTime.Text = dt.ToString("HH:mm:ss");
        }

        #region Get and Convert Week 1 values to double data type
        private void GetWeek1Values()
        {
            //Mon1
            try
            {
                Mon1 = double.Parse(nudMon1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudMon1.Focus();
            }

            //Tues1
            try
            {
                Tues1 = double.Parse(nudTues1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudTues1.Focus();
            }

            //Wen1
            try
            {
                Wen1 = double.Parse(nudWed1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudWed1.Focus();
            }

            //Thu1
            try
            {
                Thurs1 = double.Parse(nudThu1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudThu1.Focus();
            }

            //Fri1
            try
            {
                Fri1 = double.Parse(nudFri1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudFri1.Focus();
            }

            //Sat1
            try
            {
                Sat1 = double.Parse(nudSat1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSat1.Focus();
            }

            //Sun1
            try
            {
                Sun1 = double.Parse(nudSun1.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSun1.Focus();
            }
        }
        #endregion

        #region Get and Convert Week 2 values to double data type
        private void GetWeek2Values()
        {
            //Mon2
            try
            {
                Mon2 = double.Parse(nudMon2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudMon2.Focus();
            }

            //Tues2
            try
            {
                Tues2 = double.Parse(nudTues2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudTues2.Focus();
            }

            //Wen2
            try
            {
                Wen2 = double.Parse(nudWed2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudWed2.Focus();
            }

            //Thu2
            try
            {
                Thurs2 = double.Parse(nudThu2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudThu2.Focus();
            }

            //Fri2
            try
            {
                Fri2 = double.Parse(nudFri2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudFri2.Focus();
            }

            //Sat2
            try
            {
                Sat2 = double.Parse(nudSat2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSat2.Focus();
            }

            //Sun2
            try
            {
                Sun2 = double.Parse(nudSun2.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSun2.Focus();
            }
        }
        #endregion

        #region Get and Convert Week 3 values to double data type
        private void GetWeek3Values()
        {
            //Mon3
            try
            {
                Mon3 = double.Parse(nudMon3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudMon3.Focus();
            }

            //Tues3
            try
            {
                Tues3 = double.Parse(nudTues3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudTues3.Focus();
            }

            //Wen3
            try
            {
                Wen3 = double.Parse(nudWed3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudWed3.Focus();
            }

            //Thu3
            try
            {
                Thurs3 = double.Parse(nudThu3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudThu3.Focus();
            }

            //Fri3
            try
            {
                Fri3 = double.Parse(nudFri3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudFri3.Focus();
            }

            //Sat3
            try
            {
                Sat3 = double.Parse(nudSat3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSat3.Focus();
            }

            //Sun3
            try
            {
                Sun3 = double.Parse(nudSun3.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSun3.Focus();
            }
        }
        #endregion

        #region Get and Convert Week 4 values to double data type
        private void GetWeek4Values()
        {
            //Mon4
            try
            {
                Mon4 = double.Parse(nudMon4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudMon4.Focus();
            }

            //Tues4
            try
            {
                Tues4 = double.Parse(nudTues4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudTues4.Focus();
            }

            //Wen4
            try
            {
                Wen4 = double.Parse(nudWed4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudWed4.Focus();
            }

            //Thu4
            try
            {
                Thurs4 = double.Parse(nudThu4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudThu4.Focus();
            }

            //Fri4
            try
            {
                Fri4 = double.Parse(nudFri4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudFri4.Focus();
            }

            //Sat4
            try
            {
                Sat4 = double.Parse(nudSat4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSat4.Focus();
            }

            //Sun4
            try
            {
                Sun4 = double.Parse(nudSun4.Value.ToString());
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The following error occured: " + ex.Message, "Error Message");
                this.nudSun4.Focus();
            }
        }
        #endregion

        #region CONTROLS VALIDATION
        private bool ValidateControls()
        {
            if(txtEmployeeID.Text == "")
            {
                MessageBox.Show("Please, Enter Employee ID.", "Data Entry Error");
                txtEmployeeID.Focus();
                return false;
            }

            if (listBoxPayPeriod.SelectedIndex == 0)
            {
                MessageBox.Show("Please, Select Pay Period.", "Data Entry Error");
                txtEmployeeID.Focus();
                return false;
            }

            if (cmbCurrentMonth.SelectedIndex == 0)
            {
                MessageBox.Show("Please, Select Current Month.", "Data Entry Error");
                txtEmployeeID.Focus();
                return false;
            }
            return true;
        }
        #endregion

        #region Clear all Fields
        private void ResetControls()
        {
            txtEmployeeID.Text = "";
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtNINumber.Text = "";
            lblEmployeeFullName.Text = "";
            listBoxPayPeriod.SelectedIndex = 0;
            cmbCurrentMonth.SelectedIndex = 0;
            nudMon1.ResetText();
            nudTues1.ResetText();
            nudWed1.ResetText();
            nudThu1.ResetText();
            nudFri1.ResetText();
            nudSat1.ResetText();
            nudSun1.ResetText();

            nudMon2.ResetText();
            nudTues2.ResetText();
            nudWed2.ResetText();
            nudThu2.ResetText();
            nudFri2.ResetText();
            nudSat2.ResetText();
            nudSun2.ResetText();

            nudMon3.ResetText();
            nudTues3.ResetText();
            nudWed3.ResetText();
            nudThu3.ResetText();
            nudFri3.ResetText();
            nudSat3.ResetText();
            nudSun3.ResetText();

            nudMon4.ResetText();
            nudTues4.ResetText();
            nudWed4.ResetText();
            nudThu4.ResetText();
            nudFri4.ResetText();
            nudSat4.ResetText();
            nudSun4.ResetText();

            txtContractualEarnings.Text = "0.00";
            txtContractualHours.Text = "0.00";
            txtOvertimeEarnings.Text = "0.00";
            txtOvertimeHours.Text = "0.00";
            txtOvertimeRate.Text = "0.00";
            nudHourlyRate.ResetText();
            txtNIContribution.Text = "0.00";
            txtunion.Text = "0.00";
            txtTaxAmount.Text = "0.00";
            txtSLC.Text = "0.00";
            txtTotalDeductions.Text = "0.00";
            txtTotalEarnings.Text = "0.00";
            txtTotalHoursWorked.Text = "0.00";
            txtNetPay.Text = "0.00";
            txtHours.Text = "0.00";
            txtMinutes.Text = "0.00";
            txtDecimal.Text = "0.00";

        }
        #endregion

        #region LOAD PaymentRecords
        public void LoadPayRecords()
        {
            string connectionString = @"Data Source=DESKTOP-KGQRUI1;Initial Catalog=PayrollSystemDB;Integrated Security=True";
            SqlConnection sqlCon = new SqlConnection(connectionString);
            SqlDataAdapter dataAdapter = new SqlDataAdapter("select * from tblPayRecords", connectionString);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet); 
            dataGridViewPaymentRecords.DataSource = dataSet.Tables[0].DefaultView;

            sqlCon.Close();
        }
        #endregion

        #region SEARCH/FILTERING DATAGRIDVIEW
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder searchStatement = new StringBuilder();

                //PAYMENT ID
                if (txtSearchPaymentID.Text.Length > 0)
                {
                    searchStatement.Append(" Convert(PaymentID, 'System.String') like '%" + txtSearchPaymentID.Text + "%'");
                }

                //EMPLOYEE ID
                if (txtSearchEmploymentID.Text.Length > 0)
                {
                    if (searchStatement.Length > 0)
                    {
                        searchStatement.Append(" and");
                    }
                    searchStatement.Append(" Convert(EmployeeID, 'System.String') like '%" + txtEmployeeID.Text + "%'");
                }

                //FULLNAME
                if (txtSearchFullname.Text.Length > 0)
                {
                    if (searchStatement.Length > 0)
                    {
                        searchStatement.Append(" and");
                    }
                    searchStatement.Append(" FullName like '%" + txtSearchFullname.Text + "%'");
                }

                //NI Number
                if (txtSearchNINumber.Text.Length > 0)
                {
                    if (searchStatement.Length > 0)
                    {
                        searchStatement.Append(" and");
                    }
                    searchStatement.Append(" NINumber like '%" + txtSearchNINumber.Text + "%'");
                }

                //Pay Date
                if (txtSearchPayDate.Text.Length > 0)
                {
                    if (searchStatement.Length > 0)
                    {
                        searchStatement.Append(" and");
                    }
                    searchStatement.Append(" Convert(PayDate, 'System.String') like '%" + txtSearchPayDate.Text + "%'");
                }

                //Pay Month
                if (cmbSearchPayMonth.SelectedIndex > 0)
                {
                    if (searchStatement.Length > 0)
                    {
                        searchStatement.Append(" and");
                    }
                    searchStatement.Append(" PayMonth like '%" + cmbSearchPayMonth.Text + "%'");
                }

                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dataGridViewPaymentRecords.DataSource;
                bindingSource.Filter = searchStatement.ToString();
            }
            catch (Exception ex)
            {

                MessageBox.Show("The following error has ocurred: " + ex.Message, "Search error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearchPaymentID.Text = "";
            txtSearchEmploymentID.Text = "";
            txtSearchFullname.Text = "";
            txtSearchNINumber.Text = "";
            txtSearchPayDate.Text = "";
            cmbSearchPayMonth.SelectedIndex = 0;
        }
        #endregion

        #region PRINT
        private void btnGeneratePayslip_Click(object sender, EventArgs e)
        {
            printPreviewDialog1.Document = printDocument1;
            printPreviewDialog1.Show();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //e.Graphics.DrawString("Hello World", new Font("Times New Roman", 20, FontStyle.Bold), Brushes.Red, new Point(0,0));
            e.Graphics.DrawLine(new Pen(Color.Blue, 2), 60, 90, 750, 90); //x1, y1, x2, y2
            Image objImage = Image.FromFile(@"D:\Self Study\Projects\PayrollDeskApp\PayrollDesktopApp\PayrollApp\Image\google.jpg");
            e.Graphics.DrawImage(objImage, 10, 100);
            e.Graphics.DrawString("Google Payroll", new Font("Arial", 24, FontStyle.Bold), Brushes.DarkBlue, new Point(170, 130));
            e.Graphics.DrawLine(new Pen(Color.Blue, 2), 60, 250, 750, 250);
            e.Graphics.DrawString("Pay Date: " + dtpCurrentDate.Value.ToString("MM/dd/yyyy"), new Font("Times New Roman", 16, FontStyle.Regular), Brushes.DarkBlue, new Point(500, 180));

            //RECTANGULAR FOR RENTAL DETAILS
            e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), 60, 252, 700, 50); //x, y, width, height
            e.Graphics.DrawString("Employee ID: " + txtEmployeeID.Text + "    NAME: " + lblEmployeeFullName.Text+ "      NINO: " + txtNINumber.Text, new Font("Times New Roman", 16, FontStyle.Regular), Brushes.DarkBlue, new Point(60, 280));

            //HEADER FOR PAYMENT DETAILS
            e.Graphics.DrawLine(new Pen(Color.Blue, 3), 60, 350, 750, 350);
            e.Graphics.DrawString("EARNINGS", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(60, 320));
            e.Graphics.DrawString("HOURS", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(180, 320));
            e.Graphics.DrawString("RATES", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(260, 320));
            e.Graphics.DrawString("AMOUNTS", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(350, 320));
            e.Graphics.DrawString("DEDUCTIONS", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(500, 320));
            e.Graphics.DrawString("AMOUNTS", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(650, 320));

            //BASIC RATE
            e.Graphics.DrawString("BASIC: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(60, 360));
            e.Graphics.DrawString(txtContractualHours.Text, new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(180, 360));
            e.Graphics.DrawString(nudHourlyRate.Value.ToString("F"), new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(260, 360));
            e.Graphics.DrawString(txtContractualEarnings.Text, new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(350, 360));
            e.Graphics.DrawString("TAX: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(500, 360));
            e.Graphics.DrawString(txtTaxCode.Text, new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(650, 360));

            //OVERTIME
            e.Graphics.DrawString("OVERTIME: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(60, 400));
            e.Graphics.DrawString(txtOvertimeHours.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(180, 400));
            e.Graphics.DrawString(txtOvertimeRate.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(260, 400));
            e.Graphics.DrawString(txtOvertimeEarnings.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(350, 400));
            e.Graphics.DrawString("NIC: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(500, 400));
            e.Graphics.DrawString(txtNIContribution.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(650, 400));

            //UNION DEDUCTION ROW
            e.Graphics.DrawString("UNION: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(60, 440));
            e.Graphics.DrawString(txtunion.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(650, 440));

            //SLC DEDUCTION ROW
            e.Graphics.DrawString("SLC: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(60, 480));
            e.Graphics.DrawString(txtSLC.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(650, 480));

            //DRAWLINE
            e.Graphics.DrawLine(new Pen(Color.Blue, 1), 60, 520, 750, 520);
            //TOTAL EARNINGS ROW
            e.Graphics.DrawString("TOTAL EARNINGS: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(60, 550));
            e.Graphics.DrawString(txtTotalEarnings.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(260, 550));
            e.Graphics.DrawString("TOTAL DEDUCTIONS: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(400, 550));
            e.Graphics.DrawString(txtTotalDeductions.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(650, 550));

            //DRAWLINE
            e.Graphics.DrawLine(new Pen(Color.Blue, 1), 60, 580, 750, 580);
            //DRAW RECTANGLE
            e.Graphics.DrawRectangle(new Pen(Color.Blue, 2), 60, 610, 700, 50);
            e.Graphics.DrawString("NET PAY: ", new Font("Times New Roman", 14, FontStyle.Bold), Brushes.DarkBlue, new Point(500, 620));
            e.Graphics.DrawString(txtNetPay.Text, new Font("Times New Roman", 14, FontStyle.Italic), Brushes.DarkBlue, new Point(650, 620));

        }

        private void btnPrintPayslip_Click(object sender, EventArgs e)
        {
            printDocument1.Print();
        }

        #endregion

    }
}
