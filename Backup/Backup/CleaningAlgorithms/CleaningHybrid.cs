using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningHybrid : ICleaningHybrid
    {
        public FileBackup backup { get; private set; }
        public Combo combo { get; private set; }
        public CleaningHybrid(FileBackup backup, Combo comboOption)
        {
            this.backup = backup;
            combo = comboOption;
        }
        public List<IPoints> Cleaning(List<ICleaningAlgorithm> hybrid)
        {
            List<IPoints> saveToDelbyDate = new List<IPoints>();
            List<List<IPoints>> listOfPoints = new List<List<IPoints>>();
            foreach (var algorithm in hybrid)
            {
                listOfPoints.Add(algorithm.Cleaning(backup));
            }

            
            switch (combo)
            {
                case Combo.AND:
                    IEnumerable<IPoints> maxPointsToRemove = listOfPoints.First();
                    for (int i = 1; i < listOfPoints.Count; ++i)
                    {
                        maxPointsToRemove = listOfPoints[i]
                            .Where(point => maxPointsToRemove.Contains(point));
                    }

                    saveToDelbyDate = (List<IPoints>) maxPointsToRemove;

                    break;
                case Combo.OR:
                    var minPointsToRemove = listOfPoints
                        .Select(list => new
                        {
                            Count = list.Count,
                            list
                        })
                        .OrderBy(list=> list.Count)
                        .Take(1);
                    saveToDelbyDate = (List<IPoints>) minPointsToRemove;

                    break;
            }

            return saveToDelbyDate;
        }
    }
}