using System.Collections.Generic;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningByLength : ICleaningAlgorithm
    {
        public List<IPoints> Cleaning(FileBackup backup)
        {
            List<IPoints> restorePoints = backup.restorePoints;
            int length = backup.length;
            
            
            List<IPoints> SaveToDelbyLen = new List<IPoints>();
            if (restorePoints.Count > length)
            {
                var FirstStepPointsToDeletebyLen =
                    from x in restorePoints
                    where restorePoints.IndexOf(x) < restorePoints.Count - length
                    select x;

                {
                    var SecondStepPointsToDeletebyLen =
                        from x in FirstStepPointsToDeletebyLen
                        where x is FullPoint && x.IndexOfDeltas == 0
                        select x;

                    foreach (var point in SecondStepPointsToDeletebyLen)
                    {
                        SaveToDelbyLen.Add(point);
                    }
                }

                var ThirdStepPointsToDeletebyLen =
                    from x in FirstStepPointsToDeletebyLen
                    where x is FullPoint && x.IndexOfDeltas > 0
                    select x;


                int count = 0;
                foreach (var point in ThirdStepPointsToDeletebyLen)
                {
                    for (int i = 1; i <= point.IndexOfDeltas; ++i)
                        if (restorePoints.IndexOf(point) + i < restorePoints.Count - length)
                        {
                            SaveToDelbyLen.Add(restorePoints[restorePoints.IndexOf(point) + i]);
                            count++;
                        }

                    point.IndexOfDeltas -= count;

                    if (point.IndexOfDeltas == 0)
                    {
                        SaveToDelbyLen.Add(point);
                    }
                }
            }
            return SaveToDelbyLen;
            
        }
    }
}