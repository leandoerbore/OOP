using System;
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
                    var maxPointsToRemove = listOfPoints.First();
                    IEnumerable<IPoints> result = new List<IPoints>();

                    /*foreach (var listPoints in listOfPoints)
                    {
                        var result = listPoints.Ex
                    }*/
                    for (int i = 1; i < listOfPoints.Count; ++i)
                    {
                        result = maxPointsToRemove.Intersect(listOfPoints[i]);
                    }
                    foreach (var point in result)
                    {
                        saveToDelbyDate.Add(point);
                    }

                    break;
                case Combo.OR:
                    var minPointsToRemove = listOfPoints
                        .Select(list => new
                        {
                            Count = list.Count,
                            list
                        })
                        .OrderByDescending(list=> list.Count)
                        .Take(1);
                    if (minPointsToRemove.First() is null)
                        break;
                    
                    foreach (var point in minPointsToRemove.First().list)
                    {
                        saveToDelbyDate.Add(point);
                    }

                    break;
            }

            return saveToDelbyDate;
        }
    }
}