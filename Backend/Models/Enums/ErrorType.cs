namespace Backend.Models.Enums
{
    public enum ErrorType
    {
        ProductIdNotFound,
        RemoveItemFromEmptyCart,
        ChangeItemFromEmptyCart,
        RemoveItemNotInCart,
        ChangeItemNotInCart,
        CancelEmptyCart,
        CheckoutEmptyCart,
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