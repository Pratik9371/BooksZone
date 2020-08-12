using System;
using System.Collections.Generic;
using System.Text;

namespace BooksZone.Utility
{
    public static class SD
    {
        public const string Role_User_Indi = "Invidual Customer";
        public const string Role_User_Comp = "Company Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Emp = "Employee";


        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusInProcess = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled = "Cancelled";
        public const string OrderStatusRefunded = "Refunded";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "DelayedPayment";
        public const string PaymentStatusRejected = "Rejected";
    }
}
