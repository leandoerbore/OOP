using System;
using System.Collections.Generic;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningByDate : ICleaningAlgorithm
    {
        
        
        public List<IPoints> Cleaning(FileBackup backup)
        {
            List<IPoints> restorePoints = backup.restorePoints;
            int dateTimeSpan = backup.dateTimeSpan;
            
            List<IPoints> saveToDelbyDate = new List<IPoints>();
            var dateNow = DateTime.Now;

            var firstStepPointsToDeletebyDate =
                from x in restorePoints
                where (dateNow - x._date).Days > dateTimeSpan
                select x;

            {
                var secondStepPointsToDeletebyDate =
                    from x in firstStepPointsToDeletebyDate
                    where x is FullPoint && x.IndexOfDeltas < 1
                    select x;

                foreach (var point in secondStepPointsToDeletebyDate)
                {
                    saveToDelbyDate.Add(point);
                }
            }

            var thirdStepPointsToDeletebyDate =
                from x in firstStepPointsToDeletebyDate
                where x is FullPoint && x.IndexOfDeltas > 0
                select x;

            int count = 0;
            foreach (var point in thirdStepPointsToDeletebyDate)
            {
                int indexOfFullPoint = restorePoints.IndexOf(point);
                for (int i = 1; i <= point.IndexOfDeltas; ++i)
                {
                    if ((dateNow - restorePoints[indexOfFullPoint + i]._date).Days > dateTimeSpan)
                    {
                        saveToDelbyDate.Add(restorePoints[indexOfFullPoint + i]);
                        count++;
                    }
                }

                point.IndexOfDeltas -= count;

                if (point.IndexOfDeltas == 0)
                {
                    saveToDelbyDate.Add(point);
                }
            }

            return saveToDelbyDate;
        }
    }
}