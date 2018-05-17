Select Station.Name 
				from Stations
                WHERE Stations.Name LIKE '%clark%'
                Group by Stations.Name Order by Stations.Name asc