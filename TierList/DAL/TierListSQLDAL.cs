using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TierList.Models;

namespace TierList.DAL
{
    public class TierListSQLDAL : ITierListDAL
    {
        private string _connectionString;

        public TierListSQLDAL(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public TierListModel DeserializeTierList(string serializedTierList)
        {
            //The serialized tierlist starts with the name and then splits the rest of the list with a ']' so as followed (name):(restofTierList)
            //The rest of the serialized tierlist is delimeted by '|' and the patter goes as followed: (rowName)|(imagepath)|(rowName)|(imagepath)

            string[] nameTierListSplit = serializedTierList.Split(']');
            string[] seperatedTierList = nameTierListSplit[1].Split('|');
            TierListModel tierList = new TierListModel()
            {
                Name = nameTierListSplit[0]
            };

            for (int i = 0; i < seperatedTierList.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    //Creates a new row using the (rowName) as the key (it is known that all even indexes in the array of strings is the name)
                    string[] rowAndColor = seperatedTierList[i].Split('[');
                    tierList.CreateNewRow(rowAndColor[0], i+1, rowAndColor[1]);
                }
                else
                {
                    //Adds a new item to a row which already exists in the dictionary
                    tierList.AddItemToRow(seperatedTierList[i-1].Split('[')[0], seperatedTierList[i], i+1);
                }
            }
            return tierList;
        }

        /// <summary>
        /// Saves a new tierlist into the database.
        /// </summary>
        /// <param name="tierList"></param>
        /// <returns>Whether or not the saving Succeeded.</returns>
        public bool SaveTierList(TierListModel tierList)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    /* The database is structured to be able to hold a tier list. There is a table which holds the name of
                     * the tier list and the id of the tier list. Then the rows have a foreign key which refers to the tier list so a
                     * tier list can have many rows, and then from there the items in the tier list have their own table which have
                     * row ids as their foreign keys allowing rows to have many items but not vice versa.
                     * 
                     * To save it into the database I have to start with the thing with no foreign dependencies.
                     */ 
                    
                    //The OUTPUT INSERTED allows me to grab the id of the tier_list after it is inputted so that it can be used
                    //As a foreign key later
                    SqlCommand cmd = new SqlCommand("INSERT INTO tier_list OUTPUT INSERTED.id VALUES (@tierListName);", conn);
                    cmd.Parameters.AddWithValue("@tierListName", tierList.Name);
                    SqlDataReader reader = cmd.ExecuteReader();
                    reader.Read();
                    int tierId = Convert.ToInt32(reader["id"]);
                    reader.Close();

                    //I used kvps here so let me explain what it holds.
                    //the key, or the string is the name of each row within a givn tierlist
                    //The value reperesents all of the items in a list which the said row has
                    //I loop through each so that I can add every single row to the database
                    foreach (KeyValuePair<string, List<Image>> kvp in tierList.FullTierList)
                    {
                        cmd = new SqlCommand("INSERT INTO tier_rows (name, color, tier_list_id) OUTPUT INSERTED.id VALUES (@rowName, @rowColor, @tierId);", conn);
                        cmd.Parameters.AddWithValue("@rowName", kvp.Key);
                        cmd.Parameters.AddWithValue("@rowColor", tierList.RowColors[kvp.Key]);
                        cmd.Parameters.AddWithValue("@tierId", tierId);

                        reader = cmd.ExecuteReader();

                        reader.Read();
                        int rowId = Convert.ToInt32(reader["id"]);
                        reader.Close();

                        //I loop through this for the same reason as why I looped up above
                        foreach (var item in kvp.Value)
                        {
                            cmd = new SqlCommand("INSERT INTO items VALUES (@imagePath, @orderValue, @rowId);", conn);
                            cmd.Parameters.AddWithValue("@imagePath", item.ImagePath);
                            cmd.Parameters.AddWithValue("@orderValue", item.OrderValue);
                            cmd.Parameters.AddWithValue("@rowId", rowId);

                            cmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
                return true;

            }
            catch (SqlException)
            {
                throw;
                return false;
            }
        }

        public IList<TierListModel> GetTierLists()
        {
            IList<TierListModel> tierLists = new List<TierListModel>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    SqlCommand selectTierLists = new SqlCommand("SELECT * FROM tier_list;", conn);

                    SqlDataReader readTierLists = selectTierLists.ExecuteReader();

                    //Begin by selecting each individual tier list
                    while (readTierLists.Read())
                    {
                        string tierListName = Convert.ToString(readTierLists["name"]);
                        int tierListId = Convert.ToInt32(readTierLists["id"]);

                        //Create a whole new tierList Model
                        TierListModel currentTierList = new TierListModel()
                        {
                            Id = tierListId,
                            Name = tierListName
                        };

                        tierLists.Add(currentTierList);
                    }
                    readTierLists.Close();
                    


                    foreach (var tierList in tierLists)
                    {
                        IList<Row> rows = new List<Row>();

                        SqlCommand selectRows = new SqlCommand("SELECT * FROM tier_rows WHERE @tierId = tier_rows.tier_list_id;", conn);
                        selectRows.Parameters.AddWithValue("@tierId", tierList.Id);

                        SqlDataReader readRows = selectRows.ExecuteReader();

                        //read each of the rows to the corresponding tier list
                        while (readRows.Read())
                        {
                            Row row = new Row()
                            {
                                Name = Convert.ToString(readRows["name"]),
                                RowId = Convert.ToInt32(readRows["id"]),
                                Color = Convert.ToString(readRows["color"])
                            };

                            //****************************
                            //In Developement
                            int orderValue = 0;
                            //****************************

                            //Create the current row in the current tierlist model
                            tierList.CreateNewRow(row.Name, orderValue, row.Color);

                            rows.Add(row);
                            //select all the items that are currently stored in the table
                        }
                        readRows.Close();

                        foreach (var row in rows)
                        {
                            SqlCommand selectItems = new SqlCommand("SELECT * FROM items WHERE @rowId = row_id;", conn);
                            selectItems.Parameters.AddWithValue("@rowId", row.RowId);

                            SqlDataReader readItems = selectItems.ExecuteReader();

                            //Read each individual item and then add it into the tierList
                            while (readItems.Read())
                            {
                                //add all the new items into objects
                                string imagePath = Convert.ToString(readItems["image_path"]);
                                int orderValue = Convert.ToInt32(readItems["order_value"]);
                                tierList.AddItemToRow(row.Name, imagePath, orderValue);
                            }
                            readItems.Close();
                        }
                    }

                    conn.Close();
                }
                return tierLists;

            }
            catch (SqlException)
            {
                throw;
            }
        }
    }
}
