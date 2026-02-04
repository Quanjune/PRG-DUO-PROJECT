

// DONE BY KISHORE

//==========================================================
// Student Number : S10268576
// Student Name : Tang Quan Jun
// Partner Name : Rajakumar Kishore
//==========================================================

public class SpecialOffer
{
    public string OfferCode { get; set; }
    public string Description { get; set; }
    public double Discount { get; set; }

    public SpecialOffer(string offerCode, string description, double discount)
    {
        OfferCode = offerCode;
        Description = description;
        Discount = discount;
    }

    public override string ToString()
    {
        return $"{OfferCode} - {Description} ({Discount}%)";
    }
}