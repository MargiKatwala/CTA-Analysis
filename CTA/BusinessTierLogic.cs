//
// BusinessTier:  business logic, acting as interface between UI and data store.
//

using System;
using System.Collections.Generic;
using System.Data;
//Margi Katwala 
//Project 8
//12/6/2017

namespace BusinessTier
{

    //
    // Business:
    //
    public class Business
    {
        //
        // Fields:
        //
        private string _DBFile;
        private DataAccessTier.Data dataTier;


        ///
        /// <summary>
        /// Constructs a new instance of the business tier.  The format
        /// of the filename should be either |DataDirectory|\filename.mdf,
        /// or a complete Windows pathname.
        /// </summary>
        /// <param name="DatabaseFilename">Name of database file</param>
        /// 
        public Business(string DatabaseFilename)
        {
            _DBFile = DatabaseFilename;

            dataTier = new DataAccessTier.Data(DatabaseFilename);
        }


        ///
        /// <summary>
        ///  Opens and closes a connection to the database, e.g. to
        ///  startup the server and make sure all is well.
        /// </summary>
        /// <returns>true if successful, false if not</returns>
        /// 
        public bool TestConnection()
        {
            return dataTier.OpenCloseConnection();
        }


        ///
        /// <summary>
        /// Returns all the CTA Stations, ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStation objects</returns>
        /// 
        public IReadOnlyList<CTAStation> GetStations()
        {
            List<CTAStation> stations = new List<CTAStation>();

            try
            {


                string sql = "Select StationID , Name from Stations order by Name asc ";
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in result.Tables["TABLE"].Rows) {


                    stations.Add(new CTAStation(Convert.ToInt32(row["StationID"]), Convert.ToString(row["Name"])));

                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }

        public IReadOnlyList<string> getColor(string name)
        {
            List<string> color = new List<string>();

            try
            {


                string sql = string.Format(@"Select lines.color from lines, StopDetails, Stops
            Where Stops.Name ='{0}'and 
	        Lines.LineID = StopDetails.LineID and 
	        StopDetails.StopID = Stops.StopID
            Order by Name asc", name);

                DataSet result = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {


                    color.Add(Convert.ToString(row["color"]));

                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return color;
        }
        public int getdays(string name, string day)
        {
            int stations = new int();

            try
            {

                string sql = string.Format(@"
            SELECT Riderships.StationID, TypeOfDay, Sum(DailyTotal) AS Total
            FROM Stations
            INNER JOIN Riderships
            ON Stations.StationID = Riderships.StationID
            WHERE Name = '{0}'
            GROUP BY Riderships.TypeOfDay, Riderships.StationID
            ORDER BY Riderships.TypeOfDay;
            ", name);
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);

                System.Diagnostics.Debug.Assert(result.Tables["TABLE"].Rows.Count == 3);

                DataRow R1 = result.Tables["TABLE"].Rows[0];
                DataRow R2 = result.Tables["TABLE"].Rows[1];
                DataRow R3 = result.Tables["TABLE"].Rows[2];

                if (day == "A")
                {
                    System.Diagnostics.Debug.Assert(R1["TypeOfDay"].ToString() == "A");
                    return stations = Convert.ToInt32(R1["Total"]);
                }
                else if (day == "U")
                {
                    System.Diagnostics.Debug.Assert(R2["TypeOfDay"].ToString() == "U");
                    stations = Convert.ToInt32(R2["Total"]);
                }

                else
                {
                    System.Diagnostics.Debug.Assert(R3["TypeOfDay"].ToString() == "W");
                    stations = Convert.ToInt32(R3["Total"]);
                }


            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stations;
        }
        public long getRidershipT(string name)
        {
            long totalOverall;
            try
            {
                string sql = string.Format(@"Select Sum(convert(bigint, DailyTotal)) as Total
            FROM Riderships
            INNER JOIN Stations
            ON Riderships.StationID = Stations.StationID
            Where Stations.Name ='{0}'", name);
                Object result = dataTier.ExecuteScalarQuery(sql);

                totalOverall = Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return totalOverall;
        }
        public long getRidershipA(string name)
        {
            long totalOverall;
            try
            {
                string sql = string.Format(@"Select AVG(convert(bigint, DailyTotal)) as average
            FROM Riderships
            INNER JOIN Stations
            ON Riderships.StationID = Stations.StationID
            Where Stations.Name ='{0}'", name);

                Object result = dataTier.ExecuteScalarQuery(sql);

                totalOverall = Convert.ToInt64(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return totalOverall;
        }
        public int getID(string name)
        {
            int stationID;
            try
            {
                string sql = string.Format(@"Select StationID from stations 
                where Stations.Name = '{0}'", name);
                Object result = dataTier.ExecuteScalarQuery(sql);
                stationID = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return stationID;
        }
        public int getADA(string stopName) {
            int access;

            try
            {
                string sql = string.Format(@"
            select Stops.ADA as Total from Stops
	        Inner Join Stations on Stops.StationID = Stations.StationID
	        where Stops.Name = '{0}' and Stops.ADA = 'True';
            ", stopName);
                Object result = dataTier.ExecuteScalarQuery(sql);
                access = Convert.ToInt32(result);
                if (access == 1)
                {
                    return 1;
                }
                else
                    return 0;

            }


            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }


        }

        public int getADAreplace(string stopName, string text)
        {
            int num = 0;
            if (stopName == "Yes" ||  stopName=="YES")
            {
                num = 1;
            }
            else
            {
                num = 0;
            }

            try
            {
                string sql = string.Format(@"
             UPDATE Stops
             SET ADA = {0}
             WHERE Stops.Name = '{1}'
 
            ",num,stopName);
                int result = dataTier.ExecuteActionQuery(sql);
                
                if (result == 1)
                {
                    return 1;
                }
                else
                    return 0;

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

        }

        public string getDir(string stopName)
        {
            string dir="";

            try
            {
                string sql = string.Format(@"select Stops.Direction as dir from Stops
	        Inner Join Stations on Stops.StationID = Stations.StationID
	        where Stops.Name = '{0}' ", stopName);
                Object result = dataTier.ExecuteScalarQuery(sql);
                dir = Convert.ToString(result);
                return dir;
            }

            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }


        }
        public double getloc(string stopName)
        {
            double loc = new double();

            try
            {
                string sql = string.Format(@"Select Stops.Latitude from Stops
                  Inner Join Stations on
                    Stops.StationID = Stations.StationID
                 Where Stops.Name ='{0}'", stopName);
               
               
                 loc = Convert.ToDouble(dataTier.ExecuteScalarQuery(sql));
                return loc;
                
            }

            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }


        }
        public double getloc2(string stopName)
        {
            double loc = new double();

            try
            {
                string sql = string.Format(@"Select Stops.Longitude from Stops
                  Inner Join Stations on
                    Stops.StationID = Stations.StationID
                 Where Stops.Name ='{0}'", stopName);


                 loc = Convert.ToDouble(dataTier.ExecuteScalarQuery(sql));
                return loc;

            }

            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }


        }
        

        public Double getRidershipP(string name)
        {
            Double totalOverall;
            try
            {
                string sql2 = string.Format(@"Select sum(Convert(float,Riderships.DailyTotal)) from Riderships");
                string  sql = string.Format(@"
                SELECT SUM(CONVERT(Float, DailyTotal)) As TotalRiders
                FROM Riderships
                INNER JOIN Stations ON Riderships.StationID = Stations.StationID
                 WHERE Name = '{0}'
                ", name);

                Double result =(Double) dataTier.ExecuteScalarQuery(sql);
                Double result1 =(Double) dataTier.ExecuteScalarQuery(sql2);
                

                totalOverall =(result/result1) * 100;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetStations: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }

            return totalOverall;
        }

        ///
        /// <summary>
        /// Returns the CTA Stops associated with a given station,
        /// ordered by name.
        /// </summary>
        /// <returns>Read-only list of CTAStop objects</returns>
        ///
        public IReadOnlyList<CTAStop> GetStops(int stationID)
    {
      List<CTAStop> stops = new List<CTAStop>();

      try
      {
                string sql = String.Format(@" Select * from Stops 
                 where StationID = '{0}'
                order by Stops.Name asc", stationID);
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);
                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {
                    //stop id stop name 

                    stops.Add(new CTAStop(Convert.ToInt32(row["StopID"]),
                        Convert.ToString(row["Name"]),
                        stationID, Convert.ToString(row["Direction"]),
                        Convert.ToBoolean(row["ADA"]),
                        Convert.ToDouble(row["latitude"]),
                        Convert.ToDouble(row["Longitude"])));


                }



            }
            catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetStops: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stops;
    }


    ///
    /// <summary>
    /// Returns the top N CTA Stations by ridership, 
    /// ordered by name.
    /// </summary>
    /// <returns>Read-only list of CTAStation objects</returns>
    /// 
    public IReadOnlyList<String> GetTopStations(int N)
    {
      if (N < 1)
        throw new ArgumentException("GetTopStations: N must be positive");

      List<string> stations = new List<string>();

      try
      {

                string sql = string.Format(@"Select TOP {0} Riderships.DailyTotal as total, Stations.Name as Name from Riderships
            Inner Join Stations on Stations.StationID = Riderships.StationID
             ",N);
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {
                    stations.Add(Convert.ToString(row["Name"]));
                  
                }
       }
            catch (Exception ex)
      {
        string msg = string.Format("Error in Business.GetTopStations: '{0}'", ex.Message);
        throw new ApplicationException(msg);
      }

      return stations;
    }
        public IReadOnlyList<String> getLike(String stationName)
        {
            List<String> stations = new List<String>();

            try
            {


                string sql = String.Format(@"Select Stations.Name from Stations
                WHERE Stations.Name LIKE '%{0}%'
                Group by Stations.Name Order by Stations.Name asc ",stationName);
                DataSet result = dataTier.ExecuteNonScalarQuery(sql);

                foreach (DataRow row in result.Tables["TABLE"].Rows)
                {

                    
                    stations.Add(Convert.ToString(row["Name"]));

                }

            }
            catch (Exception ex)
            {
                string msg = string.Format("Error in Business.GetLike: '{0}'", ex.Message);
                throw new ApplicationException(msg);
            }


            return stations;
            }
    }//class
}//namespace
