using Backend.Models.Enums;

namespace Backend.Utils
{
    public static class ErrorUtils
    {
        public static string GetMessage(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.ProductIdNotFound:
                    return "Product not registered on the database with this ID.";
                case ErrorType.RemoveItemFromEmptyCart:
                    return "There is no items in the chart in order to remove from it.";
                case ErrorType.ChangeItemFromEmptyCart:
                    return "There is no items in the chart in order to change its quantity.";
                case ErrorType.RemoveItemNotInCart:
                    return "This item doesn't exist in the chart. Unable to remove it.";
                case ErrorType.ChangeItemNotInCart:
                    return "This item doesn't exist in the chart. Unable to change quantity.";
                case ErrorType.CancelEmptyCart:
                    return "There is no open order to be cancelled.";
                case ErrorType.CheckoutEmptyCart:
                    return "The chart is empty. Unable to checkout.";
                case ErrorType.UniqueProductName:
                    return "Product already registered on the database with this name.";
                case ErrorType.InsufficientProductInStock:
                    return "The quantity ordered exceed the number of the product in stock.";
                case ErrorType.DeleteProductInRegisteredOrder:
                    return "Product belongs to one or more registered chart items. Deletion is forbidden.";
                case ErrorType.UniqueUserName:
                    return "User already registered on the database with this login.";
                case ErrorType.UniqueUserEmail:
                    return "User already registered on the database with this email.";
                case ErrorType.UniqueUserCpf:
                    return "User already registered on the database with this CPF.";
                case ErrorType.IncorrectLoginOrPassword:
                    return "Incorrect login and/or password.";
                case ErrorType.DeleteUserWithRegisteredOrder:
                    return "User has registered orders. Deletion is forbidden.";
                case ErrorType.UserIdNotFound:
                    return "No user registered on the database with this ID.";
                case ErrorType.OrderIdNotFound:
                    return "No order registered on the database with this ID.";
                case ErrorType.NotAuthorized:
                    return "Credentials not allowed for the operation.";
                case ErrorType.CredentialsNotFound:
                    return "No user registered on the database with this credentials.";
                case ErrorType.NoOpenOrders:
                    return "The user has no open orders.";
                case ErrorType.NoInProgressOrders:
                    return "The user has no orders in progress.";
                case ErrorType.ProductsNotFound:
                    return "No registered products matches on the database.";
                default:
                    return null;
            }
        }
    }
}


