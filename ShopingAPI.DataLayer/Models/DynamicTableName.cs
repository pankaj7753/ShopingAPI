namespace ShopingAPI.DataLayer.Models
{
    public class DynamicTableName: CommanProperty
    {
        public string TableName { get; set; }
        public int TotaolRecords { get; set; }
        public string TableType { get; set; }//U/V
    }
}