using Microsoft.AspNetCore.Mvc;

namespace Authenticator.API.UserEntry
{
    public class BusinessControleBase : ControllerBase
    {
        //Módulos controle de acesso
        public const string MODULE_ACCESS_CONTROL = "ACCESS_CONTROL";
        public const string MODULE_ACCESS_GROUP = "ACCESS_GROUP";

        //Módulos food
        public const string MODULE_BASIC_ACCESS = "BASIC_ACCESS";
        public const string MODULE_ADMIN = "ADMIN";
        public const string MODULE_USER = "USER";
        public const string MODULE_DASHBOARD = "DASHBOARD";
        public const string MODULE_CATEGORY = "CATEGORY";
        public const string MODULE_PRODUCT = "PRODUCT";
        public const string ADITIONAL_GROUP = "ADITIONAL_GROUP";
        public const string ADITIONAL = "ADITIONAL";
        public const string ORDER = "ORDER";

        //Operações
        public const string OPERATION_INSERT = "INSERT";
        public const string OPERATION_SELECT = "SELECT";
        public const string OPERATION_UPDATE = "UPDATE";
        public const string OPERATION_DELETE = "DELETE";
        public const string OPERATION_EXPORT = "EXPORT";
    }
}
