using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TierList.Models
{
    public class TierListModel
    {
        public Dictionary<string, List<Image>> FullTierList { get; private set; }

        public Dictionary<string, string> RowColors { get; private set; }

        public string Name { get; set; }

        public int Id { get; set; }

        public TierListModel()
        {
            FullTierList = new Dictionary<string, List<Image>>();
            RowColors = new Dictionary<string, string>();
        }

        public void CreateNewRow(string rowName, int orderValue, string color)
        {
            if (!FullTierList.ContainsKey(rowName))
            {
                FullTierList[rowName] = new List<Image>();
            }
            if (!RowColors.ContainsKey(rowName))
            {
                RowColors[rowName] = color;
            }
        }

        //public void RemoveRow(string rowName)
        //{
        //    FullTierList.Remove(rowName);
        //}

        public void AddItemToRow(string rowName, string imagePath, int orderValue)
        {
            Image item = new Image
            {
                OrderValue = orderValue,
                ImagePath = imagePath
            };

            FullTierList[rowName].Add(item);
        }
    }
}
