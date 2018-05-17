Select Stations.Name from Stations
                WHERE Stations.Name LIKE '%Clark%'
                Group by Stations.Name Order by Stations.Name asc
UPDATE Stops
   SET ADA = 0
   WHERE Stops.Name = 'Clark/Lake (Inner Loop)'
 