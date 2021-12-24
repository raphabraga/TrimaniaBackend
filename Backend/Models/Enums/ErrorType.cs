namespace Backend.Models.Enums
{
    public enum ErrorType
    {
        ProductIdNotFound,
        RemoveItemFromEmptyChart,
        ChangeItemFromEmptyChart,
        RemoveItemNotInChart,
        ChangeItemNotInChart,
        CancelEmptyChart,
        CheckoutEmptyChart,
        UniqueProductName,
        InsufficientProductInStock,
        DeleteProductInRegisteredOrder,
        UniqueUserName,
        UniqueUserEmail,
        UniqueUserCpf,
        IncorrectLoginOrPassword,
        DeleteUserWithRegisteredOrder,
        UserIdNotFound,
        OrderIdNotFound,
        NotAuthorized,
        CredentialsNotFound,
        NoOpenOrders,
        NoInProgressOrders,
        ProductsNotFound,
    }
}