using Abp.MultiTenancy;
using System.Collections.Generic;
using static EC.Authorization.Roles.StaticRoleNames;

namespace EC.Authorization
{
    public static class PermissionNames
    {
        public const string Admin = "Admin";

        //User
        public const string Admin_User = "Admin.User";
        public const string Admin_User_View = "Admin.User.View";
        public const string Admin_User_Create = "Admin.User.Create";
        public const string Admin_User_Edit = "Admin.User.Edit";
        public const string Admin_User_EditUserRole = "Admin.User.EditUserRole";
        public const string Admin_User_Delete = "Admin.User.Delete";
        public const string Admin_User_ResetPassword = "Admin.User.ResetPassword";

        //Role
        public const string Admin_Role = "Admin.Role";
        public const string Admin_Role_View = "Admin.Role.View";
        public const string Admin_Role_Create = "Admin.Role.Create";
        public const string Admin_Role_Edit = "Admin.Role.Edit";
        public const string Admin_Role_Delete = "Admin.Role.Delete";

        //Tenant
        public const string Admin_Tenant = "Admin.Tenant";
        public const string Admin_Tenant_View = "Admin.Tenant.View";
        public const string Admin_Tenant_Create = "Admin.Tenant.Create";
        public const string Admin_Tenant_Edit = "Admin.Tenant.Edit";
        public const string Admin_Tenant_Delete = "Admin.Tenant.Delete";

        //Configuration
        public const string Admin_Configuration = "Admin.Configuration";
        public const string Admin_Configuration_View = "Admin.Configuration.View";
        public const string Admin_Configuration_Edit = "Admin.Configuration.Edit";

        //Contact
        public const string Contact = "Contact";
        public const string Contact_View = "Contact.View";
        public const string Contact_Create = "Contact.Create";
        public const string Contact_Edit = "Contact.Edit";
        public const string Contact_Delete = "Contact.Delete";

        //Email Template
        public const string Admin_EmailTemplate = "Admin.EmailTemplate";
        public const string Admin_EmailTemplate_View = "Admin.EmailTemplate.View";
        public const string Admin_EmailTemplate_Edit = "Admin.EmailTemplate.Edit";
        public const string Admin_EmailTemplate_PreviewTemplate = "Admin.EmailTemplate.PreviewTemplate";
        public const string Admin_EmailTemplate_PreviewTemplate_SendMail = "Admin.EmailTemplate.PreviewTemplate.SendMail";

        //SignServer
        public const string Admin_SignServer = "Admin.SignServer";

        // Contract template
        public const string Admin_ContractTemplate = "Admin.ContractTemplate";

        // home
        public const string Home = "Home";
        public const string Home_View_Statistic = "Home.ViewStatistic";
        public const string Home_View_Default_Signature = "Home.ViewDefaultSignature";
        public const string Home_Start_Pocess = "Home.StartPocess";

        //process
        public const string ProcessStep = "ProcessStep";

        //step upload
        public const string ProcessStep_StepUpload = "ProcessStep.StepUpload";
        public const string ProcessStep_StepUpload_View = "ProcessStep.StepUpload.View";

        //step Setting
        public const string ProcessStep_StepSetting = "ProcessStep.StepSetting";
        public const string ProcessStep_StepSetting_View = "ProcessStep.StepSetting.View";
        public const string ProcessStep_StepSetting_AddSigner = "ProcessStep.StepSetting.AddSigner";
        public const string ProcessStep_StepSetting_AddReviewer = "ProcessStep.StepSetting.AddReviewer";
        public const string ProcessStep_StepSetting_Delete = "ProcessStep.StepSetting.Delete";

        //step signature
        public const string ProcessStep_StepSignature = "ProcessStep.StepSignature";
        public const string ProcessStep_StepSignature_View = "ProcessStep.StepSignature.View";
        public const string ProcessStep_StepSignature_AddSignature = "ProcessStep.StepSignature.AddSignature";
        public const string ProcessStep_StepSignature_AddCustomeSignature = "ProcessStep.StepSignature.AddCustomeSignature";
        //public const string ProcessStep_StepSignature_DragSignature = "ProcessStep.StepSignature.DragSignature";
        public const string ProcessStep_StepSignature_EditSignature = "ProcessStep.StepSignature.EditSignature";
        public const string ProcessStep_StepSignature_RemoveSignature = "ProcessStep.StepSignature.RemoveSignature";
        //public const string ProcessStep_StepSignature_EditSignatureConfig = "ProcessStep.StepSignature.EditSignatureConfig";

        //step send mail
        public const string ProcessStep_StepSendMail = "ProcessStep.StepSendMail";
        public const string ProcessStep_StepSendMail_View = "ProcessStep.StepSendMail.View";
        public const string ProcessStep_StepSendMail_Send = "ProcessStep.StepSendMail.Send";

        //Signature
        public const string Signature = "Signature";
        public const string Signature_View = "Signature.View";
        public const string Signature_Create = "Signature.Create";
        public const string Signature_Edit = "Signature.Edit";
        public const string Signature_Delete = "Signature.Delete";
        public const string Signature_SetDefult = "Signature.SetDefult";

        //Contract
        public const string Contract = "Contract";
        public const string Contract_View = "Contract.View";
        public const string Contract_Create = "Contract.Create";
        public const string Contract_Edit = "Contract.Edit";
        public const string Contract_Delete = "Contract.Delete";
        public const string Contract_Cancel = "Contract.Cancel";
        public const string Contract_ViewDetail = "Contract.ViewDetail";
        public const string Contract_ViewHistory = "Contract.ViewHistory";
        public const string Contract_Sign = "Contract.Sign";
        public const string Contract_Download = "Contract.Download";

        //Contract detail
        public const string Contract_Detail = "Contract.Detail";
        public const string Contract_Detail_View = "Contract.Detail.View";
        public const string Contract_Detail_Edit = "Contract.Detail.Edit";
        public const string Contract_Detail_Delete = "Contract.Detail.Delete";
        public const string Contract_Detail_SendMail = "Contract.Detail.SendMail";
        public const string Contract_Detail_ViewHistory = "Contract.Detail.ViewHistory";
        public const string Contract_Detail_Download = "Contract.Detail.Download";
        public const string Contract_Detail_Sign = "Contract.Detail.Sign";
        public const string Contract_Detail_Cancel = "Contract.Detail.Cancel";
        public const string Contract_Detail_Print = "Contract.Detail.Print";

        public class GrantPermissionRoles
        {
            public static Dictionary<string, List<string>> PermissionRoles = new Dictionary<string, List<string>>() {
                {
                    Host.Admin,
                        new List < string > () {
                                PermissionNames.Home,
                                PermissionNames.Admin,

                                PermissionNames.Admin_User,
                                PermissionNames.Admin_User_View,
                                PermissionNames.Admin_User_Create,
                                PermissionNames.Admin_User_Edit,
                                PermissionNames.Admin_User_EditUserRole,
                                PermissionNames.Admin_User_Delete,
                                PermissionNames.Admin_User_ResetPassword,

                                PermissionNames.Admin_Role,
                                PermissionNames.Admin_Role_View,
                                PermissionNames.Admin_Role_Create,
                                PermissionNames.Admin_Role_Edit,
                                PermissionNames.Admin_Role_Delete,

                                PermissionNames.Admin_Tenant,
                                PermissionNames.Admin_Tenant_View,
                                PermissionNames.Admin_Tenant_Create,
                                PermissionNames.Admin_Tenant_Edit,
                                PermissionNames.Admin_Tenant_Delete,

                                PermissionNames.Admin_Configuration,
                                PermissionNames.Admin_Configuration_View,
                                PermissionNames.Admin_Configuration_Edit,

                                PermissionNames.Admin_EmailTemplate,
                                PermissionNames.Admin_EmailTemplate_View,
                                PermissionNames.Admin_EmailTemplate_Edit,
                                PermissionNames.Admin_EmailTemplate_PreviewTemplate,
                                PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail,

                                PermissionNames.Admin_SignServer,

                                PermissionNames.Admin_ContractTemplate,


                                PermissionNames.Contact,
                                PermissionNames.Contact_View,
                                PermissionNames.Contact_Create,
                                PermissionNames.Contact_Edit,
                                PermissionNames.Contact_Delete,

                                PermissionNames.Home,
                                PermissionNames.Home_View_Statistic,
                                PermissionNames.Home_View_Default_Signature,
                                PermissionNames.Home_Start_Pocess,

                                PermissionNames.ProcessStep,

                                PermissionNames.ProcessStep_StepUpload,
                                PermissionNames.ProcessStep_StepUpload_View,

                                PermissionNames.ProcessStep_StepSetting,
                                PermissionNames.ProcessStep_StepSetting_View,
                                PermissionNames.ProcessStep_StepSetting_AddSigner,
                                PermissionNames.ProcessStep_StepSetting_AddReviewer,
                                PermissionNames.ProcessStep_StepSetting_Delete,

                                PermissionNames.ProcessStep_StepSignature,
                                PermissionNames.ProcessStep_StepSignature_View,
                                PermissionNames.ProcessStep_StepSignature_AddSignature,
                                PermissionNames.ProcessStep_StepSignature_AddCustomeSignature,
                                //PermissionNames.ProcessStep_StepSignature_DragSignature,
                                PermissionNames.ProcessStep_StepSignature_EditSignature,
                                PermissionNames.ProcessStep_StepSignature_RemoveSignature,
                                //PermissionNames.ProcessStep_StepSignature_EditSignatureConfig,

                                PermissionNames.ProcessStep_StepSendMail,
                                PermissionNames.ProcessStep_StepSendMail_View,
                                PermissionNames.ProcessStep_StepSendMail_Send,

                                PermissionNames.Signature,
                                PermissionNames.Signature_View,
                                PermissionNames.Signature_Create,
                                PermissionNames.Signature_Edit,
                                PermissionNames.Signature_Delete,
                                PermissionNames.Signature_SetDefult,

                                PermissionNames.Contract,
                                PermissionNames.Contract_View,
                                PermissionNames.Contract_Create,
                                PermissionNames.Contract_Edit,
                                PermissionNames.Contract_Delete,
                                PermissionNames.Contract_Cancel,
                                PermissionNames.Contract_ViewDetail,
                                PermissionNames.Contract_ViewHistory,
                                PermissionNames.Contract_Sign,
                                PermissionNames.Contract_Download,

                                PermissionNames.Contract_Detail,
                                PermissionNames.Contract_Detail_View,
                                PermissionNames.Contract_Detail_Edit,
                                //PermissionNames.Contract_Detail_Delete,
                                PermissionNames.Contract_Detail_SendMail,
                                PermissionNames.Contract_Detail_ViewHistory,
                                PermissionNames.Contract_Detail_Download,
                                PermissionNames.Contract_Detail_Print,
                                PermissionNames.Contract_Detail_Sign,
                                PermissionNames.Contract_Detail_Cancel,
                        }
                },
            };
        }

        public class SystemPermission
        {
            public string Name { get; set; }
            public MultiTenancySides MultiTenancySides { get; set; }
            public string DisplayName { get; set; }
            public bool IsConfiguration { get; set; }
            public List<SystemPermission> Childrens { get; set; }
            public static List<SystemPermission> ListPermissions = new List<SystemPermission>() { new SystemPermission {Name = PermissionNames.Admin, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Admin"},
                new SystemPermission {Name = PermissionNames.Admin_User, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "User"},
                new SystemPermission {Name = PermissionNames.Admin_User_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Admin_User_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                new SystemPermission {Name = PermissionNames.Admin_User_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Admin_User_EditUserRole, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ChangeRole"},
                new SystemPermission {Name = PermissionNames.Admin_User_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                new SystemPermission {Name = PermissionNames.Admin_User_ResetPassword, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ResetPassword"},
                new SystemPermission {Name = PermissionNames.Admin_Role, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Roles"},
                new SystemPermission {Name = PermissionNames.Admin_Role_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Admin_Role_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                new SystemPermission {Name = PermissionNames.Admin_Role_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Admin_Role_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                new SystemPermission {Name = PermissionNames.Admin_Tenant, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Company"},
                new SystemPermission {Name = PermissionNames.Admin_Tenant_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Admin_Tenant_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                new SystemPermission {Name = PermissionNames.Admin_Tenant_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Admin_Tenant_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                new SystemPermission {Name = PermissionNames.Admin_Configuration, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Configuration"},
                new SystemPermission {Name = PermissionNames.Admin_Configuration_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Admin_Configuration_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Admin_SignServer, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SignServer"},

                new SystemPermission {Name = PermissionNames.Admin_ContractTemplate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Contract Template"},


                //email template
                new SystemPermission {Name = PermissionNames.Admin_EmailTemplate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Email Template"},
                new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_PreviewTemplate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Preview"},
                new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sent"},

                //contact
                new SystemPermission {Name = PermissionNames.Contact, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Contact"},
                new SystemPermission {Name = PermissionNames.Contact_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Contact_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                new SystemPermission {Name = PermissionNames.Contact_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Contact_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                //home
                new SystemPermission {Name = PermissionNames.Home, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Home"},
                new SystemPermission {Name = PermissionNames.Home_View_Statistic, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewStats"},
                new SystemPermission {Name = PermissionNames.Home_View_Default_Signature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewDefaultSignature"},
                new SystemPermission {Name = PermissionNames.Home_Start_Pocess, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "CreateNewContract"},
                new SystemPermission {Name = PermissionNames.ProcessStep, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SetUpContract"},

                //step upload
                new SystemPermission {Name = PermissionNames.ProcessStep_StepUpload, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToUpload"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepUpload_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},

                //step setting
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToSetUpSigner"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_AddSigner, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNewSigner"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_AddReviewer, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNewCopyRecipients"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},

                //step signature
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToSetUpPosition"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_AddSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddSignBox"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_AddCustomeSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "MoreTypesOfBoxes"},
                //new SystemPermission{ Name =  PermissionNames.ProcessStep_StepSignature_DragSignature,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi vị trí ô ký"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_EditSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditSignBox"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_RemoveSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "DeleteSignBox"},
                //new SystemPermission{ Name =  PermissionNames.ProcessStep_StepSignature_EditSignatureConfig,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi thuộc tính"},

                //step send mail
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSendMail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToSendEmail"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSendMail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.ProcessStep_StepSendMail_Send, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sent"},

                //step signature
                new SystemPermission {Name = PermissionNames.Signature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SignatureManagement"},
                new SystemPermission {Name = PermissionNames.Signature_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Signature_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                new SystemPermission {Name = PermissionNames.Signature_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Signature_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                new SystemPermission {Name = PermissionNames.Signature_SetDefult, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SetAsDefaultSignature"},

                //contract
                new SystemPermission {Name = PermissionNames.Contract, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ContractManagement"},
                new SystemPermission {Name = PermissionNames.Contract_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Contract_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                new SystemPermission {Name = PermissionNames.Contract_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                new SystemPermission {Name = PermissionNames.Contract_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                new SystemPermission {Name = PermissionNames.Contract_Cancel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Cancel"},
                new SystemPermission {Name = PermissionNames.Contract_ViewDetail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewOrDetail"},
                new SystemPermission {Name = PermissionNames.Contract_ViewHistory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewHistory"},
                new SystemPermission {Name = PermissionNames.Contract_Sign, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sign"},
                new SystemPermission {Name = PermissionNames.Contract_Download, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download"},

                //Contract detail
                new SystemPermission {Name = PermissionNames.Contract_Detail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ContractDetails"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                //new SystemPermission {Name = PermissionNames.Contract_Detail_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_SendMail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SendEmail"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_ViewHistory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewHistory"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_Download, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_Print, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_Sign, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sign"},
                new SystemPermission {Name = PermissionNames.Contract_Detail_Cancel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Cancel"},
            };

            public static List<SystemPermission> TreePermissions = new List<SystemPermission>() {
                new SystemPermission {Name = PermissionNames.Admin, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Admin",
                        Childrens = new List < SystemPermission > () {new SystemPermission {Name = PermissionNames.Admin_User, MultiTenancySides = MultiTenancySides.Host, DisplayName = "Users",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.Admin_User_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.Admin_User_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                                        new SystemPermission {Name = PermissionNames.Admin_User_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                                        new SystemPermission {Name = PermissionNames.Admin_User_EditUserRole, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ChangeRole"},
                                        new SystemPermission {Name = PermissionNames.Admin_User_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                        new SystemPermission {Name = PermissionNames.Admin_User_ResetPassword, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ResetPassword"},
                                    }
                            },
                            new SystemPermission {Name = PermissionNames.Admin_Role, MultiTenancySides = MultiTenancySides.Host, DisplayName = "Roles",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.Admin_Role_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.Admin_Role_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                                        new SystemPermission {Name = PermissionNames.Admin_Role_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                                        new SystemPermission {Name = PermissionNames.Admin_Role_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                            },
                            new SystemPermission {Name = PermissionNames.Admin_Tenant, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Company",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.Admin_Tenant_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.Admin_Tenant_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                                        new SystemPermission {Name = PermissionNames.Admin_Tenant_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                                        new SystemPermission {Name = PermissionNames.Admin_Tenant_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                            },

                            new SystemPermission {Name = PermissionNames.Admin_Configuration, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Configuration",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.Admin_Configuration_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.Admin_Configuration_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                                    }
                            },
                            new SystemPermission {Name = PermissionNames.Admin_EmailTemplate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ConfigEmail",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                                        new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_PreviewTemplate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "PreviewTemplate"},
                                        new SystemPermission {Name = PermissionNames.Admin_EmailTemplate_PreviewTemplate_SendMail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "TestSendEmail"},
                                    }
                            },
                            new SystemPermission {Name = PermissionNames.Admin_SignServer, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SignServer" },

                            new SystemPermission {Name = PermissionNames.Admin_ContractTemplate, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Contract Template" }

                        }
                },

                new SystemPermission {Name = PermissionNames.Home, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Home",
                        Childrens = new List < SystemPermission > () {
                            new SystemPermission {Name = PermissionNames.Home_View_Statistic, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewStats"},
                            new SystemPermission {Name = PermissionNames.Home_View_Default_Signature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewDefaultSignature"},
                            new SystemPermission {Name = PermissionNames.Home_Start_Pocess, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "CreateNewContract"},
                        }
                },
                new SystemPermission {Name = PermissionNames.ProcessStep, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepsToSetUpContract",
                        Childrens = new List < SystemPermission > () {
                            new SystemPermission {Name = PermissionNames.ProcessStep_StepUpload, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToUpload",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepUpload_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"}
                                    }
                            },

                            new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToSetUpSigner",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_AddSigner, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNewSigner"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_AddReviewer, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNewCopyRecipients"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSetting_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                                    }
                            },

                            new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SetUpSignaturePosition",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_AddSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddSignBox"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_AddCustomeSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "MoreTypesOfBoxes"},
                                        //new SystemPermission{ Name =  PermissionNames.ProcessStep_StepSignature_DragSignature,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi vị trí ô ký"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_EditSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditSignBox"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSignature_RemoveSignature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "DeleteSignBox"},
                                        //new SystemPermission{ Name =  PermissionNames.ProcessStep_StepSignature_EditSignatureConfig,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Thay đổi thuộc tính ô ký"},
                                    }
                            },

                            new SystemPermission {Name = PermissionNames.ProcessStep_StepSendMail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "StepToSendEmail",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSendMail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.ProcessStep_StepSendMail_Send, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sent"},
                                    }
                            },
                        }
                },

                new SystemPermission {Name = PermissionNames.Signature, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SignatureManagement",
                        Childrens = new List < SystemPermission > () {
                            new SystemPermission {Name = PermissionNames.Signature_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                            new SystemPermission {Name = PermissionNames.Signature_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                            new SystemPermission {Name = PermissionNames.Signature_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                            new SystemPermission {Name = PermissionNames.Signature_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                            new SystemPermission {Name = PermissionNames.Signature_SetDefult, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SetupDefaultSignature"},
                        }
                },

                new SystemPermission {Name = PermissionNames.Contract, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ContractManagement",
                        Childrens = new List < SystemPermission > () {
                            new SystemPermission {Name = PermissionNames.Contract_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                            new SystemPermission {Name = PermissionNames.Contract_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "CreateNewContract"},
                            new SystemPermission {Name = PermissionNames.Contract_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                            new SystemPermission {Name = PermissionNames.Contract_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                            new SystemPermission {Name = PermissionNames.Contract_Cancel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "CancelContract"},
                            //new SystemPermission{ Name =  PermissionNames.Contract_ViewDetail,MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xem chi tiết"},
                            new SystemPermission {Name = PermissionNames.Contract_ViewHistory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewHistory"},
                            new SystemPermission {Name = PermissionNames.Contract_Sign, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sign"},
                            new SystemPermission {Name = PermissionNames.Contract_Download, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download"},

                            new SystemPermission {Name = PermissionNames.Contract_Detail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ContractDetails",
                                    Childrens = new List < SystemPermission > () {
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                                        //new SystemPermission {Name = PermissionNames.Contract_Detail_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Xóa"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_SendMail, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "SentMail"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_ViewHistory, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "ViewHistory"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_Download, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Download"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_Print, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Print"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_Sign, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Sign"},
                                        new SystemPermission {Name = PermissionNames.Contract_Detail_Cancel, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Cancel"},
                                    },
                            }
                        },
                },
                new SystemPermission {Name = PermissionNames.Contact, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Contact",
                        Childrens = new List < SystemPermission > () {
                            new SystemPermission {Name = PermissionNames.Contact_View, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "View"},
                            new SystemPermission {Name = PermissionNames.Contact_Create, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "AddNew"},
                            new SystemPermission {Name = PermissionNames.Contact_Edit, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "EditOrUpdate"},
                            new SystemPermission {Name = PermissionNames.Contact_Delete, MultiTenancySides = MultiTenancySides.Host | MultiTenancySides.Tenant, DisplayName = "Delete"},
                        }
                },
            };
        }
    };
}