namespace AE_CommerceApi.Models.Entities
{
    public class Offer
    {
        public int Id { get; set; }

        public string title { get; set; } = string.Empty;

        //rabais(reduction) : Diminution faite sur un prix, un montant
        public int Discount { get; set; }
    }
}
