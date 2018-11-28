using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TierList.Models;

namespace TierList.DAL
{
    public interface ITierListDAL
    {
        TierListModel DeserializeTierList(string serializedTierList);

        bool SaveTierList(TierListModel tierList);

        IList<TierListModel> GetTierLists();
    }
}
