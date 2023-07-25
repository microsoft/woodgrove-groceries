
public class AccountData
{
    public string Error { get; set; }
    public string DisplayName { get; set; }
    public Payment Payment { get; set; }
}

public class Payment
{
    
    public string AccessTokenToCallThePaymentAPI { get; set; }
    public string CardNumber { get; set; }
    public string NameOnCard { get; set; }
    public string ExpirationDate { get; set; }
}



